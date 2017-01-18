using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Net;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace SwiftDotNet.Services
{
    public class MailService : IMailService
    {

        public async Task<bool> SendMail(string from, string fromEmail, string to, string toEmail, string subject, string body, bool isHtml, string username, string password, string domain, string host, int port = 25)
        {
            try
            {
                var msg = new MimeMessage();

                msg.From.Add(new MailboxAddress(from, fromEmail));
                msg.To.Add(new MailboxAddress(to, toEmail));
                msg.Subject = subject;

                msg.Body = isHtml ? new TextPart("html") { Text = body } : new TextPart("plain") { Text = body };

                using (var smtpClient = new SmtpClient())
                {
                    var credentials = new NetworkCredential
                    {
                        UserName = username,
                        Password = password
                    };

                    smtpClient.LocalDomain = domain;
                    await smtpClient.ConnectAsync(host, port, SecureSocketOptions.Auto).ConfigureAwait(false);
                    await smtpClient.AuthenticateAsync(credentials);
                    await smtpClient.SendAsync(msg).ConfigureAwait(false);
                    await smtpClient.DisconnectAsync(true).ConfigureAwait(false);
                }

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
        public async Task<bool> SendMail(string from, string fromEmail, string to, string toEmail, string subject, string body, bool isHtml, string username, string password, string domain, string host, int port = 25)
        {
            Debug.WriteLine(string.Concat("SendMail: ", subject));
            return true;
        }
    }
}
