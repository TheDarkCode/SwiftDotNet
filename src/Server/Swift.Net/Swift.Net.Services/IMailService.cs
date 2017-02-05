using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Swift.Net.Services
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
        Task<bool> SendMail(string from, string fromEmail, string to, string toEmail, string subject, string body, bool isHtml, string username, string password, string domain, string host, int port = 25);
    }
}
