using System;
using FakeUsersAPI.Repositories;
using FakeUsersAPI.Models;
using FakeUsersAPI.Mappers;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FakeUsersAPI.Services
{
    public class EnterValidate
    {
        
        private EntersModelDB? _enter;
        private EntersModelDB? _enterOld;
        public bool _enterValid;
        private IpLocateMapper _ipMapper;
        private CoordLocate _coordLocate;
        AnalyzeEnterModelDB? _enterValidParams;
        private readonly ILogger<EnterValidate> _logger;
        private CallDapperDb _db;
        private AppSettingsChecks _conf;
        private AppSettingsFreq _freqConf;

        public EnterValidate(IpLocateMapper ipMapper, 
            CoordLocate coordLocate, 
            ILogger<EnterValidate> logger, 
            CallDapperDb db, 
            IOptions<AppSettingsChecks> conf,
            IOptions<AppSettingsFreq> freqConf)
        {
            _ipMapper = ipMapper;
            _coordLocate = coordLocate;
            _logger = logger;
            _db = db;
            _conf = conf.Value;
            _freqConf = freqConf.Value;
        }

        public async Task BrowserOsValidateAsync()
        {
            var browserOsList = new List<Task>();
            var countOS = _db.GetCountOtherOSInDayAsync(_enter.IdUser);
            var countBrowser = _db.GetCountOtherBrowserInDayAsync(_enter.IdUser);
            browserOsList.Add(countOS);
            browserOsList.Add(countBrowser);
            await Task.WhenAll(browserOsList);
            if (countOS.Result >= _freqConf.FreqOsPerDay)
            {
                _enterValid = false;
                _logger.LogInformation("change OS {0} times in day", countOS.Result);
                _enterValidParams.OSFail = "fail";
            }

            if (countBrowser.Result >= _freqConf.FreqBrowserPerDay)
            {
                _enterValid = false;
                _logger.LogInformation("change browser {0} times in day", countBrowser.Result);
                _enterValidParams.BrowserFail = "fail";
            }
        }

        public async Task EmailChangeValidateAsync()
        {
            int countChange = 0;
            var currEmail = "";
            var currTime = 0;
            var timesEmailList = await _db.GetTimesEnterOfUserInThisDayWithEmailAsync(_enter.IdUser);
            foreach (var rec in timesEmailList)
            {
                if (rec.DateIn.Hour == DateTime.Now.Hour)
                {

                    var interval = (int)Math.Abs(currTime - rec.DateIn.Minute);
                    currTime = rec.DateIn.Minute;
                    if (interval <= _freqConf.EmailInterval)
                    {
                        if (currEmail != rec.Email) countChange++;
                        currEmail = rec.Email;
                    }
                }
            }
            if (countChange >= _freqConf.FreqEmailPerInterval)
            {
                _enterValid = false;
                _enterValidParams.InCorrectLogin = "fail";
                _logger.LogInformation("email change too much");
            }

        }

        public async Task PassportChangeValidateAsync()
        {
            bool valid = false;
            var dateBirth = await _db.GetDateBirthByIdAsync(_enter.IdUser);
            var datePassChange = new DateTime[2]
            {
             dateBirth.AddYears(20),
             dateBirth.AddYears(45)
            };
            //проверка на смену ПД по возрасту
            if (_enter.DateIssue > _enterOld.DateIssue)
            {
                foreach (var date in datePassChange)
                {
                    if (_enter.DateIssue > date)
                    {
                        _enterValidParams.PassReason += "by age; ";
                        valid = true; 
                    }
                }
                if (!valid)
                {
                    //проверка на смену ПД по смене фамилии
                    if (_enter.SecondNameUser != _enterOld.SecondNameUser)
                    {
                        _enterValidParams.PassReason += "by second name change; ";
                        valid = true;
                    }
                    //проверка на смену ПД по смене имени
                    if (_enter.FirstNameUser != _enterOld.FirstNameUser)
                    {
                        _enterValidParams.PassReason += "by first name change; ";
                        valid = true;
                    }
                }
            }
                
            //смена по потере не учитывается и подлежит ручной проверке
            if (!valid)
            {
                _enterValid = false;
                _enterValidParams.PassChange = "fail";
                _enterValidParams.PassReason += "change of passport for no reason";

            }

        }

        public async Task<bool> ValidateAsync(EntersModelDB enter, EntersModelDB enterOld) 
        {
            _enterValid = true; 
            _enter = enter;
            _enterOld = enterOld;
            _enterValidParams = new AnalyzeEnterModelDB();

            if (_conf.EnterIPLocate)
            { 
                var locationTask = new List<Task>();
                var ipLocate = _db.GetCoordsStringByIpFromDbAsync(_enter.IPAddress ?? "0.0.0.0");
                var ipLocateOld = _db.GetCoordsStringByIpFromDbAsync(_enterOld.IPAddress ?? "0.0.0.0");
                locationTask.Add(ipLocate);
                locationTask.Add(ipLocateOld);
                await Task.WhenAll(locationTask);

                //проверка на то, что добраться до ПН реально
                var autorityIsRealForFly = _coordLocate.DistanceIsRealForFly(
                    _ipMapper.GetLongitudeLatitude(ipLocateOld.Result),
                    _ipMapper.GetLongitudeLatitude(ipLocate.Result),
                    _enterOld.DateIn,
                    _enter.DateIn
                    );
                _enterValid = autorityIsRealForFly;
            
                if (autorityIsRealForFly == false)
                {
                    _enterValidParams.IPReason = _ipMapper.GetReason(ipLocateOld.Result);
                    _enterValidParams.IPLocate = "fail";
                    _logger.LogInformation("Enter in servise is false by IP");
                }
            }
            //проверка на авторизацию с разными браузерами и ОС
            if (_conf.EnterBrowserOs)
            {
                await BrowserOsValidateAsync();
            }
            //проверка на изменение Email за последние 10 мин не более 2 раз
            if (_conf.EnterEmailChange)
            {
                await EmailChangeValidateAsync();
            }
            //проверка на изменение ПД
            if (_conf.EnterPassportChange)
            {
                if (_enter.Number != _enterOld.Number)
                {
                    await PassportChangeValidateAsync();
                }
            }
            _enterValidParams.IdUser = _enter.IdUser;
            _enterValidParams.Date = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            _enterValidParams.Valid = _enterValid;
            //добавление анализа в таблицу
            await _db.AddEnterAnalyzeAsync(_enterValidParams);
            return _enterValid;

        }
        public async Task<AnalyzeEnterModelDB> ReadAnalyzeAsync(string login)
        {
            return await _db.GetEnterAnalyzeAsync(login);
        }
    }
}
