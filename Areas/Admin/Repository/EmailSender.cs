
using System.Net;
using System.Net.Mail;

namespace ShoppingDemo.Areas.Admin.Repository
{
    public class EmailSender : IEmailSender
    {
        public Task SenEmailAsync(string email, string subject, string message)
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true, //bật bảo mật
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("bghia112@gmail.com", "jtfdksghgktmhaif")
            };

            return client.SendMailAsync(
                new MailMessage(from: "bghia112@gmail.com",
                                to: email,
                                subject,
                                message
                                ));
        }
    }
}
