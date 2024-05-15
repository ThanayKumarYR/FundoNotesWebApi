using System.Net.Mail;
using System.Net;
using ModelLayer.Email;
using Repository.GlobalExceptions;
using RepositoryLayer.Interface;

namespace RepositoryLayer.Service
{
    public class EmailServiceRepositoryLayer : IEmailRepositoryLayer
    {
        private readonly EmailSettings emailsetting;

        public EmailServiceRepositoryLayer(EmailSettings emailsetting1)
        {
            emailsetting = emailsetting1;
        }

        public async Task<bool> SendEmailAsync(string to, string subject, string body)
        {
            try
            {
                using (var client = new SmtpClient(emailsetting.SmtpServer, emailsetting.SmtpPort))
                {
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential(emailsetting.SmtpUsername, emailsetting.SmtpPassword);

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(emailsetting.FromEmail),
                        Subject = subject,
                        Body = body


                    };
                    mailMessage.To.Add(to);

                    await client.SendMailAsync(mailMessage);
                    return true;
                }
            }
            catch (SmtpException ex)
            {
                Console.WriteLine("failed to send email: SMTP error", ex);
                throw new EmailSendingException(ex.Message);
            }
            catch (InvalidOperationException invalidOpEx)
            {
                throw new EmailSendingException("Invalid operation occurred while sending email", invalidOpEx);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

    }
}