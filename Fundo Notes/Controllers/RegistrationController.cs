using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BusinessLayer.Interface;
using ModelLayer.Registration;
using ModelLayer.ResponseModel;
using Microsoft.Extensions.Logging;

namespace Fundo_Notes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private readonly IRegistrationBusinessLayer _registrationbl;
        private readonly ILogger<RegistrationController> _logger;
        public RegistrationController(IRegistrationBusinessLayer registrationbusinessLayer, ILogger<RegistrationController> logger)
        {
            _registrationbl = registrationbusinessLayer;
            _logger = logger;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> UserRegistration(UserRegistrationModel user)
        {
            try
            {
                await _registrationbl.RegisterUser(user);
                _logger.LogInformation("Registration successful");
                var response = new ResponseModelLayer<UserRegistrationModel>
                {
                    Success = true,
                    Message = "User Registered Successfully",
                    Data = new UserRegistrationModel {
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        Password = "*******",
                    }
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Invalid Request");
                var response = new ResponseModelLayer<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                };
                return Ok(response);
            }
        }
    }
}
