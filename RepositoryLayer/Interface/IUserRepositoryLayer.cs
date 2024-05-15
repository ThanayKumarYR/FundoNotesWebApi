using ModelLayer.User;
using Repository.Entity;

namespace RepositoryLayer.Interface
{
    public interface IRegistrationRepositoryLayer
    {
        public Task<bool> RegisterUser(UserRegistrationModel userRegistrationModel);
        public Task<string> UserLogin(UserLoginModel userLogin);
        public Task UpdatePassword(string email, string oldPassword, string newPassword);
        //public Task<string> ForgotPassword(string Email);
        //public Task<bool> ResetPassword(string token, string newPassword);
        public Task<UserEntity> GetByEmailAsync(string email);
        public Task<int> UpdatePasswordByOtp(string mailid, string password);
    }
}
