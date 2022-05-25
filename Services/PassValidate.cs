using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using FakeUsersAPI.Models;
using FakeUsersAPI.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FakeUsersAPI.Services
{
    public class PassValidate
    {
        
        private PassUserModelDB? _passport;
        private DateTime _userDateBirth;
        AnalyzePassModelDB? _passValidParams;
        private bool _passValid;
        private readonly ILogger<PassValidate> _logger;
        private AppSettingsChecks _conf;
        private CallDapperDb _db;
        private string[] level = { "0", "1", "2", "3" };

        public PassValidate(ILogger<PassValidate> logger, CallDapperDb db, IOptions<AppSettingsChecks> conf) 
        {
            _logger = logger;
            _db = db;
            _conf = conf.Value;
        }
        public async Task SerieValidateAsync()
        {
            var resultDistrict = await _db.SerieIsDistrictAsync(_passport.Series.Substring(0, 2));
            Func<int, int, bool> ResultYear = (yearSerie, yearIssue) =>
            {
                bool valid = true; 
                int currYear = Convert.ToInt32(DateTime.Now.Year.ToString().Substring(2, 2));
                valid &= yearSerie - 5 < yearIssue && yearIssue < yearSerie + 3;
                valid &= 97 <= yearIssue && yearIssue <= 99;
                valid &= 0 <= yearIssue && yearIssue <= currYear + 1;
                return valid;
            };
            var resultYear = ResultYear(Convert.ToInt32(_passport.Series.Substring(2, 2)), Convert.ToInt32(_passport.DateIssue.Year.ToString().Substring(2, 2)));
            if (resultDistrict == false && resultYear == false) { _passValidParams.Serie = "fail"; _passValid = false; }
        }

        public async Task CodeNameValidateAsync()
        {
            var codeNameList = new List<Task>();
            var codeRegion = _db.GetCodeRegionFromDbAsync(_passport.Series.Substring(0, 2));
            var unitNames = _db.GetUnitNamesFromDbAsync(_passport.UnitCode);
            codeNameList.Add(codeRegion);
            codeNameList.Add(unitNames);
            await Task.WhenAll(codeNameList);

            if (codeRegion.Result != _passport.UnitCode.Substring(0, 2))
            {
                _passValidParams.SerieEqualRegion = "fail";
                _passValid = false;
            }

            var j = 0;
            foreach (string str in level)
            {
                if (str == _passport.UnitCode.Substring(2, 1))
                {
                    break;
                }
                else j++;
            }
            if (j == 4)
            {
                _passValidParams.Code = "fail";
                _passValid = false;
            }

            Func<List<string>, string, bool> UnitNameEqualsCatalogue = (catalogue, unitName) =>
            {
                bool equal = false;
                foreach(var name in catalogue)
                {
                    int val = 0;
                    var minLen = Math.Min(name.Length, unitName.Length);
                    for (var i = 0; i < minLen; i++)
                    {
                        if (name[i] == unitName[i]) val++;

                    }
                    int percentMistake = Convert.ToInt32(0.5 * Convert.ToDouble(name.Length));
                    if (val >= percentMistake) { equal = true; break; }
                }
                return equal;
            };

            if (!UnitNameEqualsCatalogue(unitNames.Result, _passport.UnitName)) { _passValidParams.Code = "fail"; _passValid = false; }

        }

        public void PassIsOutOfDate()
        {
            double[][] ageIssueInDays = { new double[] { 16*365, 19*365+30 }, new double[] { 20*365, 44*365+30 }, new double[] { 45*365, 120*365 } };
            Func<double[][], double, int> PassIsOut = (array, date) =>
            {
                int rank = 0;
                foreach (var d in array) 
                {
                    rank++;
                    if (date > d[0] && date < d[1]) { return rank; }
                }
                return 0;
            };
            double dateBirthToDays = DateTime.Now.Subtract(_userDateBirth).TotalDays;
            double dateIssueToDays = _passport.DateIssue.Subtract(_userDateBirth).TotalDays;
            //проверка если паспорт не мог быть выдан
            if (PassIsOut(ageIssueInDays, dateBirthToDays) == 0 || PassIsOut(ageIssueInDays, dateIssueToDays) == 0)
            {
                _passValidParams.PassIsOut = "fail"; _passValid = false; 
            }
            //проверка если паспорт просрочен или выдан раньше срока
            else
            if (PassIsOut(ageIssueInDays, dateBirthToDays) != PassIsOut(ageIssueInDays, dateIssueToDays))
            {
                _passValidParams.PassIsOut = "fail"; _passValid = false;
            }


        }

        public async Task<AnalyzePassModelDB> ValidateAsync(string login)
        {
            _passValid = true;
            var passListTask = new List<Task>();
            var passTask = _db.GetUserPassFromDbAsync(login);
            var idTask = _db.GetIdUserByLoginAsync(login);
            var dateBirthTask = _db.GetUserDateBirthFromDbAsync(login);
            passListTask.Add(passTask);
            passListTask.Add(idTask);
            passListTask.Add(dateBirthTask);
            await Task.WhenAll(passListTask);
            _passport = passTask.Result;
            _userDateBirth = dateBirthTask.Result;
            _passValidParams = new AnalyzePassModelDB();
            // check num
            if (_conf.PassCheckNum) 
            {
                if (!(Convert.ToInt32(_passport.Number) >= 101 && Convert.ToInt32(_passport.Number) <= 999999))
                {
                    _passValidParams.Number = "fail";
                    _passValid = false;
                }
            }
            if (_conf.PassCheckCodeName)
            {
                await CodeNameValidateAsync();
            }
            if (_conf.PassCheckOutOfDate)
            {
                PassIsOutOfDate();
            }
            _passValidParams.IdUser = idTask.Result;
            _passValidParams.Date = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            _passValidParams.Valid = _passValid;
            if (_passValid != true) 
            {
                _logger.LogInformation("passport number {0} is incorrect", _passport.Number);
            }
            return _passValidParams;
        }

        
    }
}
