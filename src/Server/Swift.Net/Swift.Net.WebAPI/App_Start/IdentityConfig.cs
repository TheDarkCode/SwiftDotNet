using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using DocumentDB.AspNet.Identity;
using Microsoft.Owin;
using Swift.Net.WebAPI.Models;
using System.Security.Claims;
using Swift.Net.WebAPI.Helpers;
using System;
using Twilio;
using Microsoft.Owin.Security;
using System.Diagnostics;

namespace Swift.Net.WebAPI
{

    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Credentials:
            var sentFrom = "support@swiftdotnet.com";

            // Configure the client:
            System.Net.Mail.SmtpClient client =
                new System.Net.Mail.SmtpClient();

            // Create the message:
            var mail =
                new System.Net.Mail.MailMessage();

            mail.From = new System.Net.Mail.MailAddress(sentFrom);
            mail.To.Add(message.Destination);
            mail.Subject = message.Subject;
            mail.Body = message.Body;
            mail.IsBodyHtml = true;

            // Send:
            return client.SendMailAsync(mail);
        }
    }

    public class SmsService : IIdentityMessageService
    {
        /// <summary>
        /// Twilio SMS Settings
        /// </summary>
        public static string TwilioSid = AppSettingsConfig.TwilioSid;
        public static string TwilioToken = AppSettingsConfig.TwilioToken;
        public static string TwilioFromPhone = AppSettingsConfig.TwilioFromPhone;

        public Task SendAsync(IdentityMessage message)
        {
            var Twilio = new TwilioRestClient(
               TwilioSid,
               TwilioToken
           );
            var result = Twilio.SendMessage(
                TwilioFromPhone,
               message.Destination, message.Body);

            // Status is one of Queued, Sending, Sent, Failed or null if the number is not valid
            Trace.TraceInformation(result.Status);

            // Twilio doesn't currently have an async API, so return success.
            return Task.FromResult(0);
        }
    }


    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.

    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            string endpoint = AppSettingsConfig.EndPoint;
            string authkey = AppSettingsConfig.AuthKey;
            string db = AppSettingsConfig.Db;
            string collection = AppSettingsConfig.UserCollection;

            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(new Uri(endpoint), authkey, db, collection));
            // Configure validation logic for usernames

            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };
            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true

            };

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;


            // Register two factor authentication providers. This application uses Phone and Emails as a step of
            // receiving a code for verifying the user.
            // You can write your own provider and plug it in here.
            //manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<ApplicationUser>
            //{
            //    MessageFormat = "Your Dryverless Ads Offer Portal security code is {0}"
            //});
            //manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<ApplicationUser>
            //{
            //    Subject = "SwitDotNet Security Code",
            //    BodyFormat = "Your Swift.Net security code is {0}"
            //});

            manager.EmailService = new EmailService();

            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }

    // Configure the application sign-in manager which is used in this application.
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }
}
