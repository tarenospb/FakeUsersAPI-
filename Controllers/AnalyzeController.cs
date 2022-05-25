using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using FakeUsersAPI.Services;
using FakeUsersAPI.Repositories;
using System.Collections.Generic;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;

namespace FakeUsersAPI.Controllers
{
    [Route("api/analyze")]
    [ApiController]
    public class AnalyzeController : ControllerBase
    {
        PassValidate _passValidate;
        EnterValidate _enterValidate;
        private readonly ILogger<AnalyzeController> _logger;

        public AnalyzeController(PassValidate passValidate, EnterValidate enterValidate, ILogger<AnalyzeController> logger)
        {
            _passValidate = passValidate;
            _enterValidate = enterValidate;
            _logger = logger;
        }

        
        [HttpGet]
        public async Task<IActionResult> PassEnterValid([FromBody] string userLogin)
        {
            var pasCheck = await _passValidate.ValidateAsync(userLogin);
            var enterCheck = await _enterValidate.ReadAnalyzeAsync(userLogin);
            _logger.LogInformation("--------DBlog for user {0}:--------", userLogin);
            _logger.LogInformation(pasCheck.ToString());
            _logger.LogInformation(enterCheck.ToString());
            _logger.LogInformation("-----------------------------------");
            return Ok();
        }





    }
}
