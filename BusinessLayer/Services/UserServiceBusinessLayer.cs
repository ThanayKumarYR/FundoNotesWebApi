using BusinessLayer.Interface;
using ModelLayer.User;
using Repository.GlobalExceptions;
using RepositoryLayer.Interface;
using System.Text.RegularExpressions;
using Repository.Entity;
using RepositoryLayer;

namespace BusinessLayer.Services
{
    public class RegistrationServiceBusinessLayer : IUserBusinessLayer
    {
        private readonly IRegistrationRepositoryLayer _registration;
        private static string otp;
        private static string mailid;
        private static UserEntity entity;


        public RegistrationServiceBusinessLayer(IRegistrationRepositoryLayer registration)
        {
            _registration = registration;
        }
        public Task<bool> RegisterUser(UserRegistrationModel userRegistrationModel)
        {

            return _registration.RegisterUser(userRegistrationModel);
        }

        public Task<string> UserLogin(UserLoginModel userLogin)
        {
            return _registration.UserLogin(userLogin);
        }

        public Task UpdatePassword(string email, string oldPassword, string newPassword)
        {
            return _registration.UpdatePassword(email, oldPassword, newPassword);
        }
        public Task<string> ForgotPassword(string Email)
        {
            try
            {
                entity = _registration.GetByEmailAsync(Email).Result;
            }
            catch (Exception e)
            {
                throw new UserNotFoundException("UserNotFoundByEmailId" + e.Message);
            }

            Random r = new Random();
            otp = r.Next(100000, 999999).ToString();
            mailid = Email;

            try
            {
                string subject = "Change password for Fundoo Notes";
                string message = "This is your otp please enter to change password " + otp;
                MailSender.sendMail(Email,subject, message);
                Console.WriteLine(otp);
                return Task.FromResult("OTP sent");
            }
            catch (EmailSendingException ex)
            {
                Console.WriteLine("Failed to send email: " + ex.Message);
                return Task.FromResult(ex.Message);
            }
        }

        public async Task<string> ResetPassword(string otp, string userpassword)
        {
            if (otp == null)
            {
                return "Generate Otp First";
            }

            // Hash the user's provided password
            string hashedUserPassword = HashPassword(userpassword);

            // Check if the hashed password matches the stored password
            if (entity != null && VerifyPassword(hashedUserPassword, entity.Password))
            {
                throw new PasswordMismatchException("Don't give the existing password");
            }

            // Check password complexity using regex
            if (!Regex.IsMatch(userpassword, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*])[a-zA-Z\d!@#$%^&*]{8,16}$"))
            {
                return "Password does not meet complexity requirements";
            }

            // Verify OTP
            if (RegistrationServiceBusinessLayer.otp != otp)
            {
                return "OTP mismatch";
            }

            // Update the password in the database with the hashed password
            int result = await _registration.UpdatePasswordByOtp(mailid, hashedUserPassword);

            if (result == 1)
            {
                // Clear sensitive data
                entity = null;
                otp = null;
                mailid = null;

                return "Password changed successfully";
            }
            else
            {
                return "Password not changed";
            }
        }

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt());
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}
