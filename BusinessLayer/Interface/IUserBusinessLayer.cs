using ModelLayer.User;

namespace BusinessLayer.Interface
{
    public interface IUserBusinessLayer
    {
        public Task<bool> RegisterUser(UserRegistrationModel userRegistrationModel);
        public Task<string> UserLogin(UserLoginModel userLogin);
        public Task UpdatePassword(string email, string oldPassword, string newPassword);
        public Task<string> ForgotPassword(string Email);
        public Task<string> ResetPassword(string otp, string password);
    }
}
