namespace BussinesLayer.Interface
{
    public interface IEmailBusinessLayer
    {
        Task<bool> SendEmailAsync(string to, string subject, string body);
    }
}