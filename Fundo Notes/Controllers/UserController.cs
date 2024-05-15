using Microsoft.AspNetCore.Mvc;
using BusinessLayer.Interface;
using ModelLayer.User;
using ModelLayer.ResponseModel;
using System.ComponentModel.DataAnnotations;

namespace Fundo_Notes.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserBusinessLayer _registrationbl;
        private readonly ILogger<UserController> _logger;
        public UserController(IUserBusinessLayer registrationbusinessLayer, ILogger<UserController> logger)
        {
            _registrationbl = registrationbusinessLayer;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> UserRegistration(UserRegistrationModel user)
        {
            try
            {
                await _registrationbl.RegisterUser(user);
                _logger.LogInformation("Registration successful");
                var response = new ResponseModel<UserRegistrationModel>
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
                var response = new ResponseModel<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                };
                return Ok(response);
            }
        }
        [HttpPost("login")]
        public async Task<IActionResult> UserLogin([Required] string email, [Required] string password)
        {
            try
            {
                // Authenticate the user and generate JWT token
                var Token = await _registrationbl.UserLogin(
                    new UserLoginModel() 
                    { 
                        Email = email, 
                        Password = password
                    });
                _logger.LogInformation("User Login Successfully");
                var response = new ResponseModel<string>
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
                var response = new ResponseModel<string>
                {
                    Success = false,
                    Message = ex.Message,
                };
                return Ok(response);
            }
        }

        [HttpPatch("updatePassword")]
        public async Task<IActionResult> UpdatePassword(string email, string oldPassword, string newPassword)
        {
            try
            {

                // Authenticate the user and generate JWT token
                await _registrationbl.UpdatePassword(email,oldPassword,newPassword);
                _logger.LogInformation("Successfully Updated Password");
                var response = new ResponseModel<string>
                {
                    Success = true,
                    Message = "Successfully Updated Password",
                    Data = null

                };
                return Ok(response);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to login {ex.Message}");
                var response = new ResponseModel<string>
                {
                    Success = false,
                    Message = ex.Message,
                };
                return Ok(response);
            }
        }

        [HttpPost("forgotpassword")]
        public async Task<IActionResult> ForgotPassword(string Email)
        {
            try
            {
                _logger.LogInformation("Email Sent");
                await _registrationbl.ForgotPassword(Email);
                var response = new ResponseModel<string>
                {
                    Success = true,
                    Message = "Email Sent Successfully"

                };
                return Ok(response);

            }
            catch(Exception ex)

            {
                _logger.LogError($"error occured while sending mail {ex.Message}");
                var response = new ResponseModel<string>
                {
                    Success = false,
                    Message = ex.Message,
                    
                };
                return Ok(response);
            }
        }

        [HttpPost("resetPassword")]
        public async Task<IActionResult> ResetPassword(string otp, string Newpassword)
        {
            try
            {
                _logger.LogInformation("Password reset successful");
                await _registrationbl.ResetPassword(otp, Newpassword);
                var response = new ResponseModel<string>
                {
                    Success = true,
                    Message = "Password Reset done"

                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new ResponseModel<string>
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
