using BussinesLayer.Interface;
using RepositoryLayer.Interface;

namespace BussinesLayer.Service
{
    public class EmailServiceBusinessLayer : IEmailBusinessLayer
    {
        private readonly IEmailRepositoryLayer _emailrepo;

        public EmailServiceBusinessLayer(IEmailRepositoryLayer emailrepo)
        {
            _emailrepo = emailrepo;
        }

        public Task<bool> SendEmailAsync(string to, string subject, string body)
        {
            return _emailrepo.SendEmailAsync(to, subject, body);
        }
    }
}