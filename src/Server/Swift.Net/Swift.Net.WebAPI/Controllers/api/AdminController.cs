using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using DocumentDB.AspNet.Identity;
using Swift.Net.WebAPI.Models;
using Swift.Net.WebAPI.Helpers;

namespace Swift.Net.WebAPI.Controllers.api
{
    [Authorize(Roles = "Admin")]
    [RoutePrefix("api/Admin")]
    public class AdminController : ApiController
    {
        #region ctors
        private ApplicationUserManager _userManager;
        private static HttpClient _client = new HttpClient();

        public AdminController()
        {
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }
        #endregion

        #region User CRUD

        ///<summary>
        ///Gets a flattened list of all users currently in the system.
        ///</summary>
        [HttpGet]
        [Route("GetUsers")]
        public async Task<IEnumerable<FlatUserViewModel>> GetUsers()
        {

            List<FlatUserViewModel> userList = new List<FlatUserViewModel>();
            var users = UserManager.Users.ToList();

            foreach (ApplicationUser user in users)
            {

                userList.Add(new FlatUserViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    EmailConfirmed = user.EmailConfirmed,
                    Roles = user.Roles
                });

            }

            return userList;

        }

        ///<summary>
        ///Gets a user in the database based on their username.
        ///</summary>
        /// <param name="model"></param>
        /// <returns>An HTTP Status code - 200 (OK) or 400 (Bad Request)</returns>
        [HttpPost]
        [Route("GetUser")]
        public async Task<IHttpActionResult> GetUser(GetUserAdminBindingModel model)
        {
            if (!ModelState.IsValid)
            {

                return BadRequest(ModelState);

            }

            try
            {
                // Get User in DB To Confirm
                var user = await UserManager.FindByEmailAsync(model.Email);

                if (user == null)
                {
                    return BadRequest("User not found.");

                }

                // Mask Password Hash and Security Stamp (Do Not Send Over Wire)
                user.PasswordHash = "XXX";
                user.SecurityStamp = "XXX";

                return Ok(user);

            }

            catch (Exception)
            {

                throw;

            }

        }

        ///<summary>
        ///Gets a user token based for the input user based on their username.
        ///</summary>
        /// <param name="model"></param>
        /// <returns>An HTTP Status code - 200 (OK) or 400 (Bad Request)</returns>
        [HttpPost]
        [Route("GetUserToken")]
        public async Task<IHttpActionResult> GetUserToken(GetUserAdminBindingModel model)
        {
            if (ModelState.IsValid)
            {
                // find user by username
                var user = await UserManager.FindByNameAsync(model.Email);

                if (user != null)
                {
                    ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(UserManager, OAuthDefaults.AuthenticationType);
                    ClaimsIdentity cookiesIdentity = await user.GenerateUserIdentityAsync(UserManager, CookieAuthenticationDefaults.AuthenticationType);

                    AuthenticationProperties properties = CreateProperties(user.UserName);
                    AuthenticationTicket ticket = new AuthenticationTicket(oAuthIdentity, properties);

                    return Ok(new GetUserTokenViewModel()
                    {
                        ticket = ticket
                    });

                }

            }

            // If we got this far, something failed
            return BadRequest("Failed");
        }

        ///<summary>
        ///Adds the input roles to the user in the database based on their username.
        ///</summary>
        /// <param name="model"></param>
        /// <returns>An HTTP Status code - 200 (OK) or 400 (Bad Request)</returns>
        [HttpPost]
        [Route("AddUserRoles")]
        public async Task<IHttpActionResult> AddUserRoles(AddUserRolesAdminBindingModel model)
        {
            if (!ModelState.IsValid)
            {

                return BadRequest(ModelState);

            }

            try
            {
                // Get User in DB To Confirm
                var user = await UserManager.FindByEmailAsync(model.Email);

                if (user == null)
                {
                    return BadRequest("User not found.");

                }

                var roleResult = await UserManager.AddToRolesAsync(user.Id, model.Roles);

                return Ok(roleResult);

            }

            catch (Exception)
            {

                throw;

            }

        }

        ///<summary>
        ///Removes the input roles from the user in the database based on their username.
        ///</summary>
        /// <param name="model"></param>
        /// <returns>An HTTP Status code - 200 (OK) or 400 (Bad Request)</returns>
        [HttpPost]
        [Route("RemoveUserRoles")]
        public async Task<IHttpActionResult> RemovesUserRoles(AddUserRolesAdminBindingModel model)
        {
            if (!ModelState.IsValid)
            {

                return BadRequest(ModelState);

            }

            try
            {
                // Get User in DB To Confirm
                var user = await UserManager.FindByEmailAsync(model.Email);

                if (user == null)
                {
                    return BadRequest("User not found.");

                }

                var roleResult = await UserManager.RemoveFromRolesAsync(user.Id, model.Roles);

                return Ok(roleResult);

            }

            catch (Exception)
            {

                throw;

            }

        }

        ///<summary>
        ///Resets the password for the selected user and sends them an email.
        ///</summary>
        /// <param name="model"></param>
        /// <returns>An HTTP Status code - 200 (OK) or 400 (Bad Request)</returns>
        [HttpPost]
        [Route("ResetUserPassword")]
        public async Task<IHttpActionResult> ResetUserPassword(ResetUserPasswordAdminBindingModel model)
        {
            if (!ModelState.IsValid)
            {

                return BadRequest(ModelState);

            }

            try
            {
                var user = await UserManager.FindByEmailAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    ModelState.AddModelError("", "Email is not confirmed.");
                    return BadRequest(ModelState);
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                string clientSite = AppSettingsConfig.ClientSite;

                var callbackUrl = clientSite + "/#/resetpassword?userId=" + user.Id + "&code=" + code;

                //var callbackEmail = MessageTemplates.ResetPassword(callbackUrl);

                //await UserManager.SendEmailAsync(user.Id, "Reset Password", callbackEmail);
                return Ok("Password Reset");

            }

            catch (Exception)
            {

                throw;

            }

        }

        /// <summary>
        /// Removes the current application cookie and signs the user out everywhere.
        /// </summary>
        /// <param name="model"></param>
        /// <returns>An HTTP Status code - 200 (OK) or 400 (Bad Request)</returns>
        [Route("SignOutUserEverywhere")]
        public async Task<IHttpActionResult> SignOutUserEverywhere(GetUserAdminBindingModel model)
        {
            var result = await UserManager.UpdateSecurityStampAsync(model.Email);
            return Ok(result.Succeeded);
        }


        /// <summary>
        /// Deletes the selected user from the database. If not an admin, it will only delete your individual account.
        /// </summary>
        /// <param name="model"></param>
        /// <returns>An HTTP Status code - 200 (OK) or 400 (Bad Request)</returns>
        [Route("DeleteUser")]
        public async Task<IHttpActionResult> DeleteUser(DeleteUserBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                if (User.Identity.GetUserId() != model.Email || User.IsInRole("Admin"))
                {
                    return BadRequest("Unauthorized request.");
                }

                var user = await UserManager.FindByNameAsync(model.Email);

                if (user == null)
                {
                    return BadRequest("No user with input Id.");
                }

                var result = await UserManager.DeleteAsync(user);

                return Ok(result.Succeeded);

            }

            catch (Exception)
            {

                throw;
            }
        }

        ///<summary>
        ///Resends the confirm account email to the selected user.
        ///</summary>
        /// <param name="model"></param>
        /// <returns>An HTTP Status code - 200 (OK) or 400 (Bad Request)</returns>
        [HttpPost]
        [Route("ResendConfirmEmail")]
        public async Task<IHttpActionResult> ResendConfirmEmail(ResendConfirmEmailAdminBindingModel model)
        {
            if (!ModelState.IsValid)
            {

                return BadRequest(ModelState);

            }

            try
            {
                var user = await UserManager.FindByNameAsync(model.Email);

                await UserManager.UpdateSecurityStampAsync(user.Id);
                string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                code = HttpUtility.UrlEncode(code);
                string clientSite = AppSettingsConfig.ClientSite;
                var callbackUrl = clientSite + "/#/confirmemail?userId=" + user.Id + "&code=" + code;

                //var callbackEmail = MessageTemplates.ConfirmEmail(callbackUrl);

                //await UserManager.SendEmailAsync(user.Id, "Confirm your account", callbackEmail);

                return Ok("Confirmation Email Sent");
            }

            catch (Exception)
            {

                throw;

            }

        }

        #endregion

        #region Private Methods
        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }

        private static AuthenticationProperties CreateProperties(string userName)
        {
            IDictionary<string, string> data = new Dictionary<string, string>
            {
                { "userName", userName }
            };
            return new AuthenticationProperties(data);
        }
        #endregion
    }
}
