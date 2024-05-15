using Dapper;
using ModelLayer.User;
using Repository.Context;
using Repository.GlobalExceptions;
using System.Data;
using RepositoryLayer.RegularExpressions;
using Repository.Entity;
using Repository.Interface;
using RepositoryLayer.Service;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Data.SqlClient;
using RepositoryLayer.Interface;

namespace RepositoryLayer.Unused
{
    public class UserServiceRepositoryLayer : IRegistrationRepositoryLayer
    {
        private readonly DapperContext _context;
        private readonly IAuthServiceRepositoryLayer _authService;
        private readonly RegexValidation _regexValidation;
        private readonly IEmailRepositoryLayer _emailService;
        public UserServiceRepositoryLayer(DapperContext context, IAuthServiceRepositoryLayer authService, IEmailRepositoryLayer email)
        {
            _context = context;
            _authService = authService;
            _regexValidation = new RegexValidation();
            _emailService = email;
        }
        public async Task<string> ForgotPassword(string email)
        {
            var parameters = new DynamicParameters();
            parameters.Add("Email", email);

            string query = "SELECT UserId,Email from Users where Email = @Email;";

            using (var connection = _context.CreateConnection())
            {
                var user = await connection.QueryFirstOrDefaultAsync<UserEntity>(query, parameters);
                if (user == null)
                {
                    throw new NotFoundException($"User Email {email} not found ");
                }

                var token = _authService.GenerateJwtTokenForgetPassword(user);

                var resetpasswordurl = $"https://localhost:8080/api/User/ResetPassword?token={token}";
                var emailbody = $"Reset your password using the following link: {resetpasswordurl}";

                await _emailService.SendEmailAsync(email, "Reset password", emailbody);
                return token;
            }
        }
        public async Task<bool> ResetPassword(string token, string newPassword)
        {
            try
            {
                var jwtHandler = new JwtSecurityTokenHandler();
                var jwtToken = jwtHandler.ReadToken(token) as JwtSecurityToken;

                if (jwtToken == null)
                {
                    // Invalid token
                    return false;
                }

                // Extract user ID from the token
                var userId = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "userId")?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    // userId claim not found in the token
                    return false;
                }

                // Retrieve user from the database
                var user = await GetUserByIdAsync(int.Parse(userId));

                if (user == null)
                {
                    // User not found in the database
                    return false;
                }

                // Update user's password
                await UpdatePasswordAsync(user.UserId, newPassword);

                return true;
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                Console.WriteLine($"An error occurred during password reset: {ex}");
                return false;
            }
        }
        private async Task<UserEntity> GetUserByIdAsync(int userId)
        {
            var query = "SELECT * FROM Users WHERE UserId = @UserId;";
            var parameters = new { UserId = userId };

            using (var connection = _context.CreateConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<UserEntity>(query, parameters);
            }
        }
        private async Task UpdatePasswordAsync(int userId, string newPassword)
        {
            string updatePasswordQuery = @"UPDATE Users SET Password = @Password WHERE Email = @Email;";
            var parameters = new { UserId = userId };

            using (var connection = _context.CreateConnection())
            {
                if (!_regexValidation.IsValidPassword(newPassword))
                {
                    throw new InvalidFormatException("Format is invalid in the new Password !");
                }
                else
                {
                    await connection.ExecuteAsync(updatePasswordQuery, parameters);
                }
            }
        }

    }
}
