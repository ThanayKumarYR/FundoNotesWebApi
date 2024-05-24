using ModelLayer.User;
namespace RepositoryLayer.Helper
{
    public class RegistrationDetailsForPublishing
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        // Add any other fields you want to include for publishing

        public RegistrationDetailsForPublishing(UserRegistrationModel userRegModel)
        {
            FirstName = userRegModel.FirstName;
            LastName = userRegModel.LastName;
            Email = userRegModel.Email;

        }
    }
}
