using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BusinessLayer.Interface;
using ModelLayer.Registration;
using ModelLayer.ResponseModel;

namespace Fundo_Notes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private readonly IRegistrationBusinessLayer _registrationbl;

        public RegistrationController(IRegistrationBusinessLayer registrationbusinessLayer)
        {
            _registrationbl = registrationbusinessLayer;
        }

        [HttpPost("UserRegister")]
        public async Task<IActionResult> UserRegistration(UserRegistrationModel user)
        {
            try
            {
                await _registrationbl.RegisterUser(user);
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
