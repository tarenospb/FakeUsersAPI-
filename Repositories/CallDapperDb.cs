using System;
using System.Threading.Tasks;
using Dapper;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using FakeUsersAPI.Models;
using Microsoft.Extensions.Logging;

namespace FakeUsersAPI.Repositories
{
    public class CallDapperDb
    {
        private AppSettingsConnection _conf;
        private SqlCommandsForDb _sql;
        
        private readonly ILogger<CallDapperDb> _logger;

        public CallDapperDb(
            IOptions<AppSettingsConnection> conf, 
            SqlCommandsForDb sql, 
            ILogger<CallDapperDb> logger) 
        {
            _conf = conf.Value;
            _sql = sql;
            _logger = logger;
        }
        
        

        public async Task AddUserPassEnterTransactionToDbAsync(UserModelDB u, EntersModelDB enterRow)
        {
            using (var conn = new SqlConnection(_conf.DBConnection))
            {
                var parametersUser = new DynamicParameters();
                parametersUser.Add("json", JsonConvert.SerializeObject(u), DbType.String);
                var parametersPassport = new DynamicParameters();
                parametersPassport.Add("json", JsonConvert.SerializeObject(u.Passport), DbType.String);
                var parametersEnter = new DynamicParameters();
                parametersEnter.Add("json", JsonConvert.SerializeObject(enterRow), DbType.String);
                await conn.OpenAsync();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        var affectedUserRows = await conn.ExecuteAsync(_sql.AddUserToDbUser(), param: parametersUser, transaction: transaction);
                        var affectedPassRows = await conn.ExecuteAsync(_sql.AddUserToDbPassport(), param: parametersPassport, transaction: transaction);
                        var affectedEnterRows = await conn.ExecuteAsync(_sql.AddEnterToDb(), param: parametersEnter, transaction: transaction);
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogInformation("Exception in AddUserToDbUser: {0}", ex.Message);
                        transaction.Rollback();
                    }
                }
            }
        }

        public async Task EditUserInDbAsync(EntersModelDB enterRow)
        {
            var parametersEditUser = new DynamicParameters();
            parametersEditUser.Add("json", JsonConvert.SerializeObject(enterRow), DbType.String);
            try
            {
                using (var conn = new SqlConnection(_conf.DBConnection))
                {
                    var affectedEditUserRows = await conn.ExecuteAsync(_sql.EditUserInDb(), param: parametersEditUser);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Exception in EditUserInDb: {0}", ex.Message);
            }
        }

        public async Task AddBlockToUserAsync(Guid id)
        {
            var lockParam = new DynamicParameters();
            lockParam.Add("id", id, DbType.Guid);
            try
            {
                using (var conn = new SqlConnection(_conf.DBConnection))
                {
                    var affectedBlockUser = await conn.ExecuteAsync(_sql.AddBlockToUser(), param: lockParam);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Exception in AddBlockToUser: {0}", ex.Message);
            }
        }


        public async Task AddEnterToDbAsync(EntersModelDB enterRow)
        {
            var parametersEnter = new DynamicParameters();
            parametersEnter.Add("json", JsonConvert.SerializeObject(enterRow), DbType.String);
            try
            {
                using (var conn = new SqlConnection(_conf.DBConnection))
                {
                    var affectedEnterRows = await conn.ExecuteAsync(_sql.AddEnterToDb(), param: parametersEnter);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Exception in AddEnterToDb: {0}", ex.Message);
            }
        }

        public async Task<EntersModelDB> GetOldAuthorizationByLoginAsync(string login)
        {
            var oldUser = new EntersModelDB();
            var parameter = new DynamicParameters();
            parameter.Add("login", login, DbType.String);
            try
            {
                using (var conn = new SqlConnection(_conf.DBConnection))
                {
                    oldUser = await conn.QueryFirstOrDefaultAsync<EntersModelDB>(_sql.GetOldAuthorizationByLogin(), param: parameter);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Exception in GetUserFromDb: {0}", ex.Message);
            }
            return oldUser ?? new EntersModelDB();

        }

        public async Task<UserModelDB> GetUserFromDbAsync(string login)
        {
            var affectedUserRow = new UserModelDB();
            var parameter = new DynamicParameters();
            parameter.Add("login", login, DbType.String);
            try 
            {
                using (var conn = new SqlConnection(_conf.DBConnection))
                {
                    affectedUserRow = await conn.QueryFirstOrDefaultAsync<UserModelDB>(_sql.GetUserFromDb(), param: parameter);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Exception in GetUserFromDb: {0}", ex.Message);
            }
            return affectedUserRow ?? new UserModelDB();
            
        }


        public async Task<PassUserModelDB> GetUserPassFromDbAsync(string login)
        {
            var affectedPassportRow = new PassUserModelDB();
            try
            {
                using (var conn = new SqlConnection(_conf.DBConnection))
                {
                    var parameter = new DynamicParameters();
                    parameter.Add("login", login, DbType.String);
                    affectedPassportRow = await conn.QueryFirstOrDefaultAsync<PassUserModelDB>(_sql.GetUserPassFromDb(), param: parameter);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Exception in GetUserPassFromDb: {0}", ex.Message);
            }

            return affectedPassportRow ?? new PassUserModelDB();
            
        }

        public async Task RemoveBlockUser(string login)
        {
            try
            {
                using (var conn = new SqlConnection(_conf.DBConnection))
                {
                    var parameter = new DynamicParameters();
                    parameter.Add("login", login, DbType.String);
                    var removeBlockRow = await conn.ExecuteAsync(_sql.RemoveBlockUser(), param: parameter);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Exception in RemoveBlockUser: {0}", ex.Message);
            }


        }

        public async Task<Ip2LocationModelDB> GetCoordsStringByIpFromDbAsync(string ip)
        {
            var affectedIpRow = new Ip2LocationModelDB();
            try
            {
                using (var conn = new SqlConnection(_conf.DBConnection))
                {
                    var parameter = new DynamicParameters();
                    parameter.Add("ip", ip, DbType.String);
                    affectedIpRow = await conn.QueryFirstOrDefaultAsync<Ip2LocationModelDB>(_sql.GetCoordsStringByIp(), param: parameter);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Exception in GetCoordsStringByIp: {0}", ex.Message);
            }
            return affectedIpRow;

        }

        public async Task<bool> IsUserHaveBlockAsync(string login)
        {
            using (var conn = new SqlConnection(_conf.DBConnection))
            {
                var parameter = new DynamicParameters();
                parameter.Add("login", login, DbType.String);
                var userBlock = await conn.QueryFirstOrDefaultAsync<bool>(_sql.UserHaveBlock(), param: parameter);
                return userBlock;
            }
            
        }

        public async Task<DateTime> GetUserDateBirthFromDbAsync(string login)
        {
            var affectedDateBirthRow = new DateTime();
            try
            {
                using (var conn = new SqlConnection(_conf.DBConnection))
                {
                    var parameter = new DynamicParameters();
                    parameter.Add("login", login, DbType.String);
                    affectedDateBirthRow = await conn.QueryFirstOrDefaultAsync<DateTime>(_sql.GetUserDateBirthFromDb(), param: parameter);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Exception in GetUserDateBirthFromDb: {0}", ex.Message);
            }
            return affectedDateBirthRow;
            
        }

        public async Task<string> GetCodeRegionFromDbAsync(string okato)
        {
            var affectedOkatoRow = new String("");
            try
            {
                using (var conn = new SqlConnection(_conf.DBConnection))
                {
                    var parameter = new DynamicParameters();
                    parameter.Add("okato", okato, DbType.String);
                    affectedOkatoRow = await conn.QueryFirstOrDefaultAsync<string>(_sql.GetCodeRegionFromDb(), param: parameter);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Exception in GetCodeRegionFromDb: {0}", ex.Message);
            }

            return affectedOkatoRow ?? new String("");
   
        }

        public async Task<List<string>> GetUnitNamesFromDbAsync(string unitCode)
        {
            List<string> result = new List<string>();
            try
            {
                using (var conn = new SqlConnection(_conf.DBConnection))
                {
                    var parameter = new DynamicParameters();
                    parameter.Add("unitCode", unitCode, DbType.String);
                    var affectedUnitNameRow = await conn.QueryAsync<string>(_sql.GetUnitNamesFromDb(), param: parameter);
                    result = affectedUnitNameRow.AsList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Exception in GetUnitNamesFromDb: {0}", ex.Message);
            }

            return result ?? new List<string>();
          
        }

        public async Task<bool> SerieIsDistrictAsync(string serie)
        {
            List<OKATODB> result = new List<OKATODB>();
            try
            {
                using (var conn = new SqlConnection(_conf.DBConnection))
                {
                    var parameter = new DynamicParameters();
                    parameter.Add("serie", serie, DbType.String);
                    var affectedOkatoRows = await conn.QueryAsync<OKATODB>(_sql.GetSerieIsDistrict(), param: parameter);
                    result = affectedOkatoRows.AsList<OKATODB>();
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Exception in GetSerieIsDistrict: {0}", ex.Message);
            }
            return result?.Count > 0;

        }
        public async Task<bool> UserIsExistAsync(string login)
        {
            var affectedUserRows = new Int32();
            try
            {
                using (var conn = new SqlConnection(_conf.DBConnection))
                {
                    var parameter = new DynamicParameters();
                    parameter.Add("login", login, DbType.String);
                    affectedUserRows = await conn.QueryFirstOrDefaultAsync<int>(_sql.UserIsExist(), param: parameter);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Exception in UserIsExist: {0}", ex.Message);
            }
            return affectedUserRows > 0;

        }
        public async Task<string> RandomNameUnitFromUnitDbAsync()
        {
            var affectedRandomRow = new String("");
            try
            {
                using (var conn = new SqlConnection(_conf.DBConnection))
                {
                    var parameter = new DynamicParameters();
                    parameter.Add("randomRecord", new Random().Next(1, 16461), DbType.Int32);
                    affectedRandomRow = await conn.QueryFirstOrDefaultAsync<string>(_sql.GetRandomNameUnit(), param: parameter);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Exception in GetRandomNameUnit: {0}", ex.Message);
            }

            return affectedRandomRow ?? new String("");
        }
        public async Task<string> GetRandomLoginUserFromDbAsync()
        {
            var affectedRandomRow = new String("");
            var countRows = 1;
            try
            {
                using (var conn = new SqlConnection(_conf.DBConnection))
                {
                    countRows = await conn.QueryFirstOrDefaultAsync<int>(_sql.CountUsers()); 
                    
                    var parameter = new DynamicParameters();
                    parameter.Add("randomRecord", new Random().Next(1, countRows+1), DbType.Int32);
                    affectedRandomRow = await conn.QueryFirstOrDefaultAsync<string>(_sql.GetRandomLoginUser(), param: parameter);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Exception in GetRandomLoginUser: {0}", ex.Message);
            }

            return affectedRandomRow ?? new String("");
        }

        public async Task<int> GetCountUsersFromDbAsync()
        {
            var countRows = 0;
            try
            {
                using (var conn = new SqlConnection(_conf.DBConnection))
                {

                    countRows = await conn.QueryFirstOrDefaultAsync<int>(_sql.CountUsers());
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Exception in CountUsers: {0}", ex.Message);
            }

            return countRows;
        }

        public async Task<Guid> GetIdUserByLoginAsync(string login)
        {
            var affectedIdRow = new Guid();
            try
            {
                using (var conn = new SqlConnection(_conf.DBConnection))
                {
                    var parameter = new DynamicParameters();
                    parameter.Add("login", login, DbType.String);
                    affectedIdRow = await conn.QueryFirstOrDefaultAsync<Guid>(_sql.GetIdUserByLogin(), param: parameter);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Exception in GetIdUserByLogin: {0}", ex.Message);
            }
            return affectedIdRow;
            
        }

        public async Task AddPassAnalyzeAsync(AnalyzePassModelDB param)
        {
            var parameter = new DynamicParameters();
            parameter.Add("Date", param.Date, DbType.DateTime);
            parameter.Add("IdUser", param.IdUser, DbType.Guid);
            parameter.Add("Serie", param.Serie, DbType.String);
            parameter.Add("Number", param.Number, DbType.String);
            parameter.Add("SerieEqualRegion", param.SerieEqualRegion, DbType.String);
            parameter.Add("Code", param.Code, DbType.String);
            parameter.Add("PassIsOut", param.PassIsOut, DbType.String);
            parameter.Add("Valid", param.Valid, DbType.Boolean);
            try
            {
                using (var conn = new SqlConnection(_conf.DBConnection))
                {
                    var affectedAnalyzeRow = await conn.ExecuteAsync(_sql.AddPassAnalyze(), param: parameter);
                }
              
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Exception in AddPassAnalyze: {0}", ex.Message);
            }

        }
        public async Task AddEnterAnalyzeAsync(AnalyzeEnterModelDB param)
        {
            var parameterEnter = new DynamicParameters();
            parameterEnter.Add("Date", param.Date, DbType.DateTime);
            parameterEnter.Add("IdUser", param.IdUser, DbType.Guid);
            parameterEnter.Add("IPLocate", param.IPLocate, DbType.String);
            parameterEnter.Add("IPReason", param.IPReason, DbType.String);
            parameterEnter.Add("PassChange", param.PassChange, DbType.String);
            parameterEnter.Add("PassReason", param.PassReason, DbType.String);
            parameterEnter.Add("InCorrectLogin", param.InCorrectLogin, DbType.String);
            parameterEnter.Add("BrowserFail", param.BrowserFail, DbType.String);
            parameterEnter.Add("OSFail", param.OSFail, DbType.String);
            parameterEnter.Add("Valid", param.Valid, DbType.Boolean);
            try
            {
                using (var conn = new SqlConnection(_conf.DBConnection))
                {
                    var affectedAnalyzeRow = await conn.ExecuteAsync(_sql.AddEnterAnalyze(), param: parameterEnter);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Exception in AddEnterAnalyze: {0}", ex.Message);
            }

        }
        public async Task<int> GetCountOtherBrowserInDayAsync(Guid id)
        {
            var affectedCountRow = 0;
            try
            {
                using (var conn = new SqlConnection(_conf.DBConnection))
                {
                    var parameter = new DynamicParameters();
                parameter.Add("id", id, DbType.Guid);
                affectedCountRow = await conn.QueryFirstOrDefaultAsync<int>(_sql.GetCountOtherBrowserInDay(), param: parameter);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Exception in GetCountOtherBrowserInDay: {0}", ex.Message);
            }
            return affectedCountRow;

        }
        public async Task<int> GetCountOtherOSInDayAsync(Guid id)
        {
            var affectedCountRow = 0;
            try
            {
                using (var conn = new SqlConnection(_conf.DBConnection))
                {
                    var parameter = new DynamicParameters();
                    parameter.Add("id", id, DbType.Guid);
                    affectedCountRow = await conn.QueryFirstOrDefaultAsync<int>(_sql.GetCountOtherOSInDay(), param: parameter);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Exception in GetCountOtherOSInDay: {0}", ex.Message);
            }
            return affectedCountRow;

        }
        public async Task<List<TimesEmailModel>> GetTimesEnterOfUserInThisDayWithEmailAsync(Guid id)
        {
            List<TimesEmailModel> result = new List<TimesEmailModel>();
            try
            {
                using (var conn = new SqlConnection(_conf.DBConnection))
                {
                    var parameter = new DynamicParameters();
                    parameter.Add("id", id, DbType.Guid);
                    var affectedTimesEmailRows = await conn.QueryAsync<TimesEmailModel>(_sql.GetTimesEnterOfUserInThisDayWithEmail(), param: parameter);
                    result = affectedTimesEmailRows.AsList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Exception in GetTimesEnterOfUserInThisDayWithEmail: {0}", ex.Message);
            }

            return result ?? new List<TimesEmailModel>();

        }

        public async Task<DateTime> GetDateBirthByIdAsync(Guid id)
        {
            var affectedDateBirthRow = new DateTime();
            try
            {
                using (var conn = new SqlConnection(_conf.DBConnection))
                {
                    var parameter = new DynamicParameters();
                    parameter.Add("id", id, DbType.Guid);
                    affectedDateBirthRow = await conn.QueryFirstOrDefaultAsync<DateTime>(_sql.GetDateBirthById(), param: parameter);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Exception in GetDateBirthById: {0}", ex.Message);
            }
            return affectedDateBirthRow;

        }
        public async Task<AnalyzeEnterModelDB> GetEnterAnalyzeAsync(string login)
        {
            var affectedAnalyzeRow = new AnalyzeEnterModelDB();
            try
            {
                using (var conn = new SqlConnection(_conf.DBConnection))
                {
                    var parameter = new DynamicParameters();
                    parameter.Add("login", login, DbType.String);
                    affectedAnalyzeRow = await conn.QueryFirstOrDefaultAsync<AnalyzeEnterModelDB>(_sql.GetEnterAnalyze(), param: parameter);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Exception in GetEnterAnalyze: {0}", ex.Message);
            }
            return affectedAnalyzeRow ?? new AnalyzeEnterModelDB();

        }
    }
}
