namespace RepositoryLayer.Interface
{
    public interface IEmailRepositoryLayer
    {
        public Task<bool> SendEmailAsync(string to, string subject, string body);

    }
}