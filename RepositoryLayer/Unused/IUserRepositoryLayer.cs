using ModelLayer.User;
using Repository.Entity;

namespace RepositoryLayer.Unused
{
    public interface IRegistrationRepositoryLayer
    {
        public Task<string> ForgotPassword(string Email);
        public Task<bool> ResetPassword(string token, string newPassword);
    }
}
