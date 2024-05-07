using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BusinessLayer.Interface;
using ModelLayer.Registration;
using ModelLayer.ResponseModel;
using Microsoft.Extensions.Logging;

namespace Fundo_Notes.Controllers
{
    [Route("api/")]
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

        [HttpPost("login")]
        public async Task<IActionResult> UserLogin(UserLoginModel userLogin)
        {
            try
            {

                // Authenticate the user and generate JWT token
                var Token = await _registrationbl.UserLogin(userLogin);
                var response = new ResponseModelLayer<string>
                {
                    Success = true,
                    Message = "User Login Successfully",
                    Data = Token

                };
                return Ok(response);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to login {ex.Message}");
                var response = new ResponseModelLayer<string>
                {
                    Success = false,
                    Message = ex.Message,
                };
                return Ok(response);
            }
        }
    }
}
