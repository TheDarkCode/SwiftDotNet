using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Swift.Net.WebAPI.Services
{
    public interface IMailService
    {
        /// <summary>
        /// The method used to send mail.
        /// </summary>
        /// <param name="from">a String that contains the address of the sender</param>
        /// <param name="to">a string that contains the address of the recipient</param>
        /// <param name="subject">a String that contains the title text</param>
        /// <param name="body">a String that contains the message body</param>
        /// <param name="isHtml">Indicate wether the mail message body is HTML</param>
        /// <returns>sucess</returns>
        bool SendMail(string from, string to, string subject, string body, bool isHtml);
    }

    public class MailService : IMailService
    {

        public bool SendMail(string from, string to, string subject, string body, bool isHtml)
        {
            try
            {
                var msg = new MailMessage(from, to, subject, body);
                msg.IsBodyHtml = isHtml;
                var client = new SmtpClient();
                client.Send(msg);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
    public class MockMailService : IMailService
    {
        public bool SendMail(string from, string to, string subject, string body, bool isHtml)
        {
            Debug.WriteLine(string.Concat("SendMail: ", subject));
            return true;
        }
    }
}
