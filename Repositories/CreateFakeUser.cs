using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using uaParcer = UAParser.Parser;
using FakeUsersAPI.Models;
using Bogus;
using Microsoft.Extensions.Logging;


namespace FakeUsersAPI.Repositories
{
    public class CreateFakeUser
    {
        private AppSettingsConnection _conf;
        CallDapperDb _db;
        RabbitClient _rabbitClient;
        private readonly ILogger<CreateFakeUser> _logger;
        Faker _faker;
        uaParcer _ua;

        public CreateFakeUser(IOptions<AppSettingsConnection> conf, 
            CallDapperDb db, 
            RabbitClient rabbitClient,
            ILogger<CreateFakeUser> logger)
        {
            _conf = conf.Value;
            _db = db;
            _rabbitClient = rabbitClient;
            _logger = logger;
        }

        public void InitFakerParcer() 
        {
            _faker = new Faker("ru");
            _ua = uaParcer.GetDefault();
        }

        public async Task DeleteBlockAsync(string user)
        {
            await _db.RemoveBlockUser(user); 
        }

            public async Task CreateUserAsync()
        {
            
            Func<int, int, DateTime> dateOfBirth = (beginYear, endYear) => //создание даты рождения в интервале года от beginYear до endYear
            {
                int month = _faker.Random.Int(1, 12);
                int day = 1;
                if (month == 2)
                {
                    day = _faker.Random.Int(1, 28);
                }
                else
                {
                    day = _faker.Random.Int(1, 30);
                }
                return new DateTime 
                (
                    _faker.Random.Int(beginYear, endYear),
                    month,
                    day
                );
            };
            
            UserModelDB newUser = new UserModelDB();
            newUser.UserAgent = _faker.Internet.UserAgent();
            try
            {
                var ua = _ua.Parse(newUser.UserAgent);
                newUser.OS = ua.OS.Family + " " + ua.OS.Major;
                newUser.Browser = ua.UA.Family;
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Exception in UAParse: {0}", ex.Message);
            }
            
            newUser.IdUser = Guid.NewGuid();
            newUser.SecondNameUser = _faker.Person.LastName;
            newUser.FirstNameUser = _faker.Person.FirstName;
            newUser.DateBirth = dateOfBirth(1970, 2000);
            newUser.Login = _faker.Internet.UserName(newUser.SecondNameUser + newUser.FirstNameUser);
            newUser.Email = _faker.Internet.Email(newUser.Login);
            newUser.IPAddress = _faker.Internet.Ip();
            
            PassUserModelDB pu = new PassUserModelDB();
            pu.Series = _faker.Random.Int(1111, 5555).ToString();
            pu.Number = _faker.Random.Int(111111, 555555).ToString();
            pu.UnitName = await _db.RandomNameUnitFromUnitDbAsync();
                        
            Func<int, int, DateTime, DateTime> DateIssue = (firstMonth, secondMonth, date) =>
            {
                DateTime DateIssue = date;
                DateIssue = DateIssue.AddMonths(_faker.Random.Int(firstMonth, secondMonth));
                if (DateIssue > DateTime.Now) { return DateTime.Now; } else return DateIssue;
            };

            pu.DateIssue = DateIssue(168, 720, newUser.DateBirth);
            pu.UnitCode = $"{_faker.Random.Int(100, 500)}-{_faker.Random.Int(100, 500)}";
            pu.UserId = newUser.IdUser;
            newUser.Passport = pu;
            newUser.Lock = false;
            newUser.DateIn = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            //отправка в rabbit         
            _rabbitClient.SendToRabbit(newUser);
        }


        public async Task CreateUserWithOtherIpAsync(string login)
        {
            UserModelDB newUser = await _db.GetUserFromDbAsync(login);
            newUser.Passport = await _db.GetUserPassFromDbAsync(login);
            newUser.UserAgent = _faker.Internet.UserAgent();
            try
            {
                var ua = _ua.Parse(newUser.UserAgent);
                newUser.OS = ua.OS.Family + " " + ua.OS.Major;
                newUser.Browser = ua.UA.Family;
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Exception in UAParse: {0}", ex.Message);
            }
            
            newUser.IPAddress = _faker.Internet.Ip();
            newUser.DateIn = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            //отправка в rabbit         
            _rabbitClient.SendToRabbit(newUser);
        }

        public async Task CreateUserWithAnyParamsAsync(string login)
        {
            Func<int[]> NewPar = () => //генерация случайных изменений в пользователе
             {
                 int[] param = new int[_faker.Random.Int(1, 5)];
                 for (int i = 0; i < param.Length; i++)
                 {
                     param[i] = _faker.Random.Int(1, 5);
                 }

                 return param;
             };

            UserModelDB newUser = await _db.GetUserFromDbAsync(login);
            newUser.Passport = await _db.GetUserPassFromDbAsync(login);
            int[] newParameters = NewPar();
            foreach (var changePar in newParameters)
            {
                if (changePar == 1)
                {
                    newUser.UserAgent = _faker.Internet.UserAgent();
                    newUser.IPAddress = _faker.Internet.Ip();
                }
                if (changePar == 2)
                {
                    newUser.SecondNameUser = _faker.Person.LastName;
                }
                if (changePar == 3)
                {
                    newUser.FirstNameUser = _faker.Person.FirstName;
                }
                if (changePar == 4)
                {
                    newUser.Email = _faker.Internet.Email(newUser.Login);
                }
                if (changePar == 5)
                {
                    PassUserModelDB pu = new PassUserModelDB();
                    pu.Series = _faker.Random.Int(1111, 5555).ToString();
                    pu.Number = _faker.Random.Int(111111, 555555).ToString();
                    pu.UnitName = await _db.RandomNameUnitFromUnitDbAsync();
                    Func<int, int, DateTime, DateTime> DateIssue = (firstMonth, secondMonth, date) =>
                    {
                        DateTime DateIssue = date;
                        DateIssue = DateIssue.AddMonths(_faker.Random.Int(firstMonth, secondMonth));
                        if (DateIssue > DateTime.Now) { return DateTime.Now; } else return DateIssue;
                    };
                    pu.DateIssue = DateIssue(168, 720, newUser.DateBirth);
                    pu.UnitCode = $"{_faker.Random.Int(100, 500)}-{_faker.Random.Int(100, 500)}";
                    newUser.Passport = pu;
                }
            }
            try
            {
                var ua = _ua.Parse(newUser.UserAgent);
                newUser.OS = ua.OS.Family + " " + ua.OS.Major;
                newUser.Browser = ua.UA.Family;
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Exception in UAParse: {0}", ex.Message);
            }
            
            newUser.DateIn = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            //отправка в rabbit         
            _rabbitClient.SendToRabbit(newUser);
        }

    }
}
