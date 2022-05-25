using FakeUsersAPI.Models;
using FakeUsersAPI.Repositories;

namespace FakeUsersAPI.Mappers
{
    public class EnterDataMapper
    {
        private CallDapperDb _db;

        public EnterDataMapper(CallDapperDb db) 
        {
            _db = db;
        }
        
        public EntersModelDB ParseUserModelToEnterModel(UserModelDB u)
        {
            EntersModelDB EnterRow = new EntersModelDB();
            EnterRow.IdUser = u.IdUser;
            EnterRow.DateIn = u.DateIn;
            EnterRow.SecondNameUser = u.SecondNameUser;
            EnterRow.FirstNameUser = u.FirstNameUser;
            EnterRow.OS = u.OS;
            EnterRow.Browser = u.Browser;
            EnterRow.Email = u.Email;
            EnterRow.IPAddress = u.IPAddress;
            EnterRow.UserAgent = u.UserAgent;
            EnterRow.Series = u.Passport?.Series;
            EnterRow.Number = u.Passport?.Number;
            EnterRow.UnitCode = u.Passport?.UnitCode;
            EnterRow.UnitName = u.Passport?.UnitName;
            EnterRow.DateIssue = u.Passport?.DateIssue;
            return EnterRow;
        }
    }
}
