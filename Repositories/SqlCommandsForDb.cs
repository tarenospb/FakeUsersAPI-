namespace FakeUsersAPI.Repositories
{
    public class SqlCommandsForDb
    {
        //добавление нового клиента
        private readonly string _addUser = @"EXEC AddUser_v1 @json";
        private readonly string _addEnter = @"EXEC AddEnter_v1 @json";
        private readonly string _addPassport = @"EXEC AddPass_v1 @json";
        //обновление текущего пользователя
        private readonly string _editUser = @"EXEC UpdateUserPass_v1 @json";
        //получение юзера по логину 
        private readonly string _getUser = @"SELECT IdUser, SecondNameUser, FirstNameUser, OS, Browser, DateBirth, Login, Email, IPAddress, UserAgent, Lock, DateIn FROM Users WHERE Login = @login";
                //выставление блока
        private readonly string _addLock = @"UPDATE Users SET [Lock] = 1 WHERE IdUser = @id";
        //удаление блока
        private readonly string _remLock = @"UPDATE Users SET [Lock] = 0 WHERE Login = @login";
        //проверка блока у юзера
        private readonly string _userHaveBlock = @"SELECT [Lock] FROM Users WHERE Login = @login";
        //получение паспорта по логину
        private readonly string _getPass = @"SELECT Series, Number, UnitCode, UnitName, DateIssue, UserId FROM Passports WHERE UserId = (SELECT IdUser FROM Users WHERE Login = @login)";
        //получение ДР по логину
        private readonly string _getDateBirth = @"SELECT DateBirth FROM Users WHERE Login = @login";
        //проверка наличия юзера в системе
        private readonly string _userIsExist = @"SELECT COUNT(*) FROM Users WHERE Login = @login";
        //возврат количества пользователей
        private readonly string _countUsers = @"SELECT COUNT(*) FROM Users";
        //получение кода региона по ОКАТО
        private readonly string _getCodeRegion = @"SELECT CodeRegion FROM OKATO WHERE CodeOkato = @okato";
        //получение названий подразделений по коду подразделения
        private readonly string _getUnitNames = @"SELECT Unit FROM UnitCode WHERE UnitCode = @unitCode";
        //проверка соответствия серии паспорта коду подразделения
        private readonly string _getSerieIsDistrict = @"SELECT CodeOkato, CodeRegion, NameDistrict FROM OKATO WHERE CodeOkato = @serie";
        //получение случайной записи названия подразделения при создании пользователя
        private readonly string _getRandomNameUnit = @"SELECT Unit FROM UnitCode ORDER BY IdUnitCode OFFSET @randomRecord ROWS FETCH NEXT 1 ROWS ONLY";
        //получение случайного пользователя из Users
        private readonly string _getRandomLoginUser = @"SELECT Login FROM Users ORDER BY IdUser OFFSET @randomRecord ROWS FETCH NEXT 1 ROWS ONLY";
        //добавление анализа по паспорту
        private readonly string _addPassAnalyze = @"INSERT INTO AnalyzePass VALUES (@Date, @IdUser, @Serie, @Number, @SerieEqualRegion, @Code, @PassIsOut, @Valid)";
        //добавление анализа по входу
        private readonly string _addEnterAnalyze = @"INSERT INTO AnalyzeEnter VALUES (@Date, @IdUser, @IPLocate, @IPReason, @PassChange, @PassReason, @InCorrectLogin, @BrowserFail, @OSFail, @Valid)";
        //получение анализа по входу
        private readonly string _getEnterAnalyze = @"SELECT TOP(1) Date, IdUser, IPLocate, IPReason, PassChange, PassReason, InCorrectLogin, BrowserFail, OSFail, Valid FROM AnalyzeEnter WHERE IdUser = (SELECT IdUser FROM Users WHERE Login = @login) ORDER BY Date DESC";
        //получение idклиента по логину
        private readonly string _getIdUserByLogin = @"SELECT IdUser FROM Users WHERE Login = @login";
        //получение строки с координатами из датасета по ip-стринге
        private readonly string _getCoordsString = @"SELECT country_name, region_name, city_name, latitude, longitude FROM GetCoordsByIpInRow(@ip)";
        //получение данные предыдущей авторизации
        private readonly string _getOldAuthorization = @"SELECT * FROM GetUserWithPassByLogin(@login)";
        //получение количества авторизаций в день разными браузерами
        private readonly string _getCountOtherBrowserInDay = @"SELECT COUNT(DISTINCT Browser) AS Expr FROM EnterLogs 
            WHERE (IdUser = @id) AND (DAY(DateIn) = DAY({ fn NOW() })) 
            AND (MONTH(DateIn) = MONTH({ fn NOW() })) AND (YEAR(DateIn) = YEAR({ fn NOW() }))";
        //получение количества авторизаций в день разными осями
        private readonly string _getCountOtherOSInDay = @"SELECT COUNT(DISTINCT OS) AS Expr FROM EnterLogs 
            WHERE (IdUser = @id) AND (DAY(DateIn) = DAY({ fn NOW() })) 
            AND (MONTH(DateIn) = MONTH({ fn NOW() })) AND (YEAR(DateIn) = YEAR({ fn NOW() }))";
        //получение всех времен авторизаций юзера для подсчета количества входов
        private readonly string _getTimesEnterOfUserInThisDay = @"SELECT TOP (100) PERCENT DateIn, Email FROM dbo.EnterLogs WHERE (IdUser = @id) 
            AND(CONVERT(date, DateIn) = CONVERT(date, { fn NOW() })) ORDER BY DateIn DESC";
        //получение даты рождения по id юзера
        private readonly string _getDateBirthById = @"SELECT DateBirth FROM Users WHERE (IdUser = @id)";

        public string AddUserToDbUser()
        {
            return _addUser;
        }
        public string AddEnterToDb()
        {
            return _addEnter;
        }
        public string AddUserToDbPassport()
        {
            return _addPassport;
        }
        public string GetUserPassFromDb()
        {
            return _getPass;
        }
        public string GetUserFromDb()
        {
            return _getUser;
        }

        public string GetUserDateBirthFromDb()
        {
            return _getDateBirth;
        }
        public string GetCodeRegionFromDb()
        {
            return _getCodeRegion;
        }

        public string GetUnitNamesFromDb()
        {
            return _getUnitNames;
        }
        public string GetSerieIsDistrict()
        {
            return _getSerieIsDistrict;
        }
        public string GetRandomNameUnit()
        {
            return _getRandomNameUnit;
        }
        public string GetRandomLoginUser()
        {
            return _getRandomLoginUser;
        }
        
        public string UserIsExist()
        {
            return _userIsExist;
        }
        public string CountUsers()
        {
            return _countUsers;
        }
        public string EditUserInDb()
        {
            return _editUser;
        }
        public string AddBlockToUser()
        {
            return _addLock;
        }
        public string RemoveBlockUser()
        {
            return _remLock;
        }
        public string UserHaveBlock()
        {
            return _userHaveBlock;
        }
        public string AddPassAnalyze()
        {
            return _addPassAnalyze;
        }
        public string AddEnterAnalyze()
        {
            return _addEnterAnalyze;
        }
        public string GetEnterAnalyze()
        {
            return _getEnterAnalyze;
        }
        public string GetIdUserByLogin()
        {
            return _getIdUserByLogin;
        }
        public string GetCoordsStringByIp()
        {
            return _getCoordsString;
        }
        public string GetOldAuthorizationByLogin()
        {
            return _getOldAuthorization;
        }
        public string GetCountOtherBrowserInDay()
        {
            return _getCountOtherBrowserInDay;
        }
        public string GetCountOtherOSInDay()
        {
            return _getCountOtherOSInDay;
        }
        public string GetTimesEnterOfUserInThisDayWithEmail()
        {
            return _getTimesEnterOfUserInThisDay;
        }
        public string GetDateBirthById()
        {
            return _getDateBirthById;
        }
        
    }
}
