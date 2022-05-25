using System;
using System.Threading.Tasks;
using FakeUsersAPI.Models;
using Newtonsoft.Json;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using FakeUsersAPI.Repositories;
using Microsoft.Extensions.Logging;

namespace FakeUsersAPI.Mappers
{
    public class UserDataMapper
    {
        private ValidateUserAndWriteResult _validateUser;
        private readonly ILogger<UserDataMapper> _logger;
        public UserDataMapper(ValidateUserAndWriteResult validateUser, ILogger<UserDataMapper> logger) 
        {
            _validateUser = validateUser;
            _logger = logger;
        }
        public async Task ParseBodyToUserModel(string s)
        {
            UserModelDB user = new UserModelDB();
            
            if (s != null)
            {
                try
                {
                    user = JsonConvert.DeserializeObject<UserModelDB>(s);
                    await _validateUser.SendToValidate(user);
                }
                catch (Exception ex)
                {
                    _logger.LogInformation("exception in UserDataMapper: {0}", ex);
                }
            }
        }
    }
}
