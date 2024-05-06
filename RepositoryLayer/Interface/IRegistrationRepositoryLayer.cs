using ModelLayer.Registration;

namespace RepositoryLayer.Interface
{
    public interface IRegistrationRepositoryLayer
    {
        public Task<bool> RegisterUser(UserRegistrationModel userRegistrationModel);
    }
}
