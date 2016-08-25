using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using SwiftDotNet.WebAPI.Models;

namespace SwiftDotNet.WebAPI.Providers
{
    public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
    {
        private readonly string _publicClientId;

        public ApplicationOAuthProvider(string publicClientId)
        {
            if (publicClientId == null)
            {
                throw new ArgumentNullException("publicClientId");
            }

            _publicClientId = publicClientId;
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            // For best practices, you should use always use a dynamic access-control-allow-origin response.

            // Get the Allowed Origins from Helper
            string origins = AppSettingsConfig.CorsPolicyOrigins;

            // Get the Origin of the Request
            string requestOrigin = context.OwinContext.Request.Headers.Get("origin");

            // If the Origin of the Request is contained in the Allowed Origins Set Access-Control-Allow-Origin for that Origin only.
            if (origins.Contains(requestOrigin))
            {
                context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new string[] { requestOrigin });
                //context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", origins.Split(',') );
            }

            // NOTE :: Only works when Allowed Origins is a single URI (not a comma separated list).
            //string origins = AppSettingsConfig.CorsPolicyOrigins;
            //context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new string[] { origins });

            // Allow All Sample - Not recommended unless you are intentionally accepting requests from unknown origins.
            //context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new string[] { "*" });

            var userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();

            // Find User By UserName First
            var user = await userManager.FindByNameAsync(context.UserName);

            if (user != null)
            {
                var validCredentials = await userManager.FindAsync(context.UserName, context.Password);

                // When a user is lockedout, this check is done to ensure that even if the credentials are valid
                // the user can not login until the lockout duration has passed
                if (await userManager.IsLockedOutAsync(user.Id))
                {
                    context.SetError("lockout_enabled", string.Format("Your account has been locked out for {0} minutes due to multiple failed login attempts.", TimeSpan.FromMinutes(10)));
                    return;
                }
                // if user is subject to lockouts and the credentials are invalid
                // record the failure and check if user is lockedout and display message, otherwise, 
                // display the number of attempts remaining before lockout
                else if (await userManager.GetLockoutEnabledAsync(user.Id) && validCredentials == null)
                {
                    // Record the failure which also may cause the user to be locked out
                    await userManager.AccessFailedAsync(user.Id);

                    string message;

                    if (await userManager.IsLockedOutAsync(user.Id))
                    {
                        message = string.Format("Your account has been locked out for {0} minutes due to multiple failed login attempts.", TimeSpan.FromMinutes(10));
                    }
                    else
                    {
                        int accessFailedCount = await userManager.GetAccessFailedCountAsync(user.Id);

                        int attemptsLeft = 5 - accessFailedCount;

                        message = string.Format(
                        "Invalid credentials. You have {0} more attempt(s) before your account gets locked out.", attemptsLeft);
                    }

                    context.SetError("lockout_enabled", message);
                    return;
                }
                // I needed to add this in order to check if the email was confirmed when a user log on.
                else if (!user.EmailConfirmed)
                {
                    context.SetError("email_not_confirmed", "User did not confirm email.");
                    return;
                }

                await userManager.ResetAccessFailedCountAsync(user.Id);

                ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(userManager,
                   OAuthDefaults.AuthenticationType);
                ClaimsIdentity cookiesIdentity = await user.GenerateUserIdentityAsync(userManager,
                    CookieAuthenticationDefaults.AuthenticationType);

                AuthenticationProperties properties = CreateProperties(user.UserName);
                AuthenticationTicket ticket = new AuthenticationTicket(oAuthIdentity, properties);
                context.Validated(ticket);
                context.Request.Context.Authentication.SignIn(cookiesIdentity);
            }

            else
            {
                context.SetError("invalid_grant", "The user name or password is incorrect.");
                return;
            }

        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            // Resource owner password credentials does not provide a client ID.
            if (context.ClientId == null)
            {
                context.Validated();
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        {
            if (context.ClientId == _publicClientId)
            {
                Uri expectedRootUri = new Uri(context.Request.Uri, "/");

                if (expectedRootUri.AbsoluteUri == context.RedirectUri)
                {
                    context.Validated();
                }
            }

            return Task.FromResult<object>(null);
        }

        public static AuthenticationProperties CreateProperties(string userName)
        {
            IDictionary<string, string> data = new Dictionary<string, string>
            {
                { "userName", userName }
            };
            return new AuthenticationProperties(data);
        }
    }
}