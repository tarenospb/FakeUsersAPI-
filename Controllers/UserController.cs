using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using FakeUsersAPI.Repositories;
using FakeUsersAPI.Services;

namespace FakeUsersAPI.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private CreateFakeUser _userClass;
        private PassValidate _passValidate;
        private Sheduler _sheduler;

        public UserController(CreateFakeUser userClass, PassValidate passValidate, Sheduler sheduler)
        {
            _userClass = userClass;
            _passValidate = passValidate;
            _sheduler = sheduler;
            _userClass.InitFakerParcer();
        }

        
        [HttpPost]
        public async Task<IActionResult> SendUser()
        {
            await _userClass.CreateUserAsync();
            return Ok();
        }

        [HttpPost("ip")]
        public async Task<IActionResult> SendUserIp([FromBody] string userLogin)
        {
            await _userClass.CreateUserWithOtherIpAsync(userLogin);
            return Ok();
        }

        [HttpPost("any")]
        public async Task<IActionResult> SendUserAny([FromBody] string userLogin)
        {
            await _userClass.CreateUserWithAnyParamsAsync(userLogin);
            return Ok();
        }
        [HttpPost("block")]
        public async Task<IActionResult> DeleteBlock([FromBody] string userLogin)
        {
            await _userClass.DeleteBlockAsync(userLogin);
            return Ok();
        }
        [HttpPost("timer")]
        public IActionResult EnableTimer([FromBody] double interval) //interval in sec
        {
            _sheduler.SetTimer(interval);
            return Ok();
        }
    }
}
