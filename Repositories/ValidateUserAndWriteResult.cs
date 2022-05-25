using FakeUsersAPI.Mappers;
using FakeUsersAPI.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using FakeUsersAPI.Models;

namespace FakeUsersAPI.Repositories
{
    public class ValidateUserAndWriteResult
    {
        private PassValidate? _passValidate;
        private EnterValidate? _enterValidate;
        private CallDapperDb? _db;
        private readonly ILogger<CallDapperDb> _logger;
        private EnterDataMapper _enterMapper;

        public ValidateUserAndWriteResult(PassValidate passValidate,
            EnterValidate enterValidate,
            CallDapperDb db,
            EnterDataMapper enterMapper,
            ILogger<CallDapperDb> logger) 
        {
            _passValidate = passValidate;
            _logger = logger;
            _enterValidate = enterValidate;
            _db = db;
            _enterMapper = enterMapper;
    }
        public async Task SendToValidate(UserModelDB u) 
        {
            _logger.LogInformation("__________start verify enter of user {0}__________", u.Login);
            
            var blockUser = _db.IsUserHaveBlockAsync(u.Login);
            var existUser = _db.UserIsExistAsync(u.Login);
            var userStatusTask = new Task<bool>[] { blockUser, existUser };
            await Task.WhenAll(userStatusTask);
            
            if (userStatusTask[0].Result == false && u != default)
            {
                var enterData = _enterMapper.ParseUserModelToEnterModel(u);
                var id = await _db.GetIdUserByLoginAsync(u.Login);

                if (userStatusTask[1].Result == false)
                {
                        //добавление юзера, паспорта и входа в систему в базу
                        await _db.AddUserPassEnterTransactionToDbAsync(u, enterData);

                    // анализ паспорта добавленного пользователя
                    var pasCheck = await _passValidate.ValidateAsync(u.Login);
                    await _db.AddPassAnalyzeAsync(pasCheck);
                }
                else
                {
                    await _db.AddEnterToDbAsync(enterData);
                    var oldUser = _db.GetOldAuthorizationByLoginAsync(u.Login);
                    //тут будет проверка входной модели на корректность входных данных
                    if (await _enterValidate.ValidateAsync(enterData, oldUser.Result) == true) 
                    {
                        await _db.EditUserInDbAsync(enterData);
                        // анализ паспорта обновленного пользователя
                        var pasCheck = await _passValidate.ValidateAsync(u.Login);
                        await _db.AddPassAnalyzeAsync(pasCheck);
                    }
                    else
                    {
                        await _db.AddBlockToUserAsync(enterData.IdUser);
                    }

                }
            }
            else { _logger.LogInformation("User {0} is blocked", u.Login); }
        }
    }
}
