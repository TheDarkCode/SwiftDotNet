using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Cors;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Swift.Net.WebAPI.Models;
using Swift.Net.WebAPI.Extensions;
using Swift.Net.WebAPI.Helpers;

namespace Swift.Net.WebAPI.Controllers.api
{
    [AllowAnonymous]
    [RoutePrefix("api/Mail")]
    public class MailController : ApiController
    {

        #region ctors

        public MailController()
        {
        }

        #endregion

        #region Send Emails

        /// <summary>
        /// Sends an email from the contact form.
        /// </summary>
        /// <param name="model"></param>
        /// <returns>An HTTP Status code - 200 (OK) or 400 (Bad Request)</returns>
        [HttpPost]
        [Authorize]
        //[AntiForgeryValidate]
        public async Task<IHttpActionResult> Post(ContactFormModel model)
        {
            if (ModelState.IsValid)
            {

                string userIP = Request.GetIPWithCloudflare();

                if (model.IP != userIP)
                {
                    return BadRequest("IP address did not match request.");
                }

                if (model.Destination != "info@swiftdotnet.com")
                {
                    return BadRequest("Invalid Destination.");
                }

                var task = SendAsync(model);

                if (task.IsCompleted)
                {
                    return Ok(model);

                }

                else
                {
                    if (task.IsFaulted)
                    {
                        return BadRequest(task.Exception.ToString());
                    }
                    return BadRequest("Unable to send mail.");
                }

            }
            else
            {
                return BadRequest("Model is invalid.");
            }

        }

        #endregion

        #region Private Methods
        private async Task SendAsync(ContactFormModel model)
        {
            // Credentials:
            var sentFrom = "noreply@swiftdotnet.com";

            // Configure the client:
            System.Net.Mail.SmtpClient client =
                new System.Net.Mail.SmtpClient();

            // Create the message:
            var mail =
                new System.Net.Mail.MailMessage();

            mail.From = new System.Net.Mail.MailAddress(sentFrom);
            mail.To.Add(model.Destination);
            mail.Subject = "New Contact Form Message";

            var emailBody = MessageTemplates.ContactForm(model);

            mail.Body = emailBody;
            mail.IsBodyHtml = true;

            // Send:
            await client.SendMailAsync(mail);
        }
        #endregion

    }
}