using Microsoft.Web.Http;
using netApi.Repositories.Authorization.Model;
using NetEasyPay.Interfaces;
using NetEasyPay.Services;
using Newtonsoft.Json;
using System;
using System.Web.Http;
using System.Web.Http.Results;
using static netApi.Common.Helpers;

namespace NetEasyPay.Controllers
{
    [ApiVersion("1.0")]
    public class AuthorizationController : ApiController
    {
        private readonly IAuthorizationService _authService;
        private readonly IAdministrationService _adminService;

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public AuthorizationController()
        {
            _authService = new AuthorizationService();
            _adminService = new AdministrationService();
        }

        /// <summary>
        /// Returns a fully populated APP_USER object with roles AND permissions.  
        /// </summary>
        /// <param name="UserId">(long) USER_SYS_ID of the user to load</param>
        /// <returns>(APP_USER) Fully hydrated APP_USER object with ROLES and PERMISSIONS</returns>
        // TODO: [Authorize]
        [HttpGet]
        [Route("api/v{version:apiVersion}/Authorization/LoadUser")]
        public object LoadUser(long UserId)
        {
            try
            {
                return Ok(_authService.LoadUser(UserId));
            }
            catch (Exception e)
            {
                var msg = $"Error Retrieving User Record.\n\n{e.Message}";
                log.Error(e);

                return BadRequest(msg);
            }
        }

        /// <summary>
        /// Returns each claim contained in the JWT token
        /// </summary>
        /// <remarks>
        /// This method will return an individual object containing the 
        /// subject, claim type and claim value.
        /// </remarks>
        /// <returns>JSON</returns>
        // TODO: [Authorize]
        [Route("api/v{version:apiVersion}/Authorization/GetClaims")]
        public IHttpActionResult GetClaims()
        {
            try
            {
                return Ok(_authService.GetClaims(this.User));
            }
            catch (Exception e)
            {
                var msg = string.Format("Error Retrieving User Claims.\n\n{0}", e.Message);
                log.Error(e);

                return BadRequest(msg);
            }
        }

        /// <summary>
        /// This method takes the user's email address and validates it against the FOPS database
        /// to determine if they already have a user account
        /// </summary>
        /// <param name="email">(string) Email address to validate</param>
        /// <param name="SSO">(bool) Does the user already have an SSO token?</param>
        /// <returns>(HTTPResult) Ok if user has account, Unauthorized/NotFound if not</returns>
        // TODO: [Authorize]
        [HttpPost]
        [Route("api/v{version:apiVersion}/Authorization/ValidateEmail")]
        public object ValidateEmail(bool SSO, string email)
        {
            try
            {
                return Ok((Response)_authService.ValidateEmail(email.ToLower()));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException.ToString());
            }
        }

        [HttpPost]
        [Route("api/v{version:apiVersion}/Authorization/AuthZeroCallback")]
        public string Auth0Callback(string er)
        {
            // attempt to load the user.  If a user exists, they were logging in.  If there is no user, they were registering.
            try
            {
                var o = _adminService.GetUser(er);

                if (o.ACCOUNT_STATUS == "COMPLETE")
                {
                    //they were logging in, otherwise we'd get an exception telling us that there were no items in the sequence
                    return "/home/invoices";
                }
                else
                {
                    return "/home/incomplete";
                }
            }
            catch (Exception e)
            {
                // TODO: Don't use exceptions as Status Switching...
                //there was no user, go back to the registration page
                return "/home/register?q1=true&q2=" + er;
                //return $"/home/register2/?email={er}&sso=true";
            }

        }

        [HttpPost]
        [Route("api/v{version:apiVersion}/Authorization/AuthenticateUser")]
        public object AuthenticateUser([FromBody] string credentials)
        {
            return Ok(credentials);
            //var creds = JsonConvert.DeserializeObject<dynamic>(credentials);
            //try
            //{
            //    return Ok(_service.AuthenticateUser(creds.emailAddress, creds.password));
            //}
            //catch (Exception ex)
            //{
            //    return BadRequest(ex.InnerException.ToString());
            //}
        }

        [HttpGet]
        [Route("api/v{version:apiVersion}/Authorization/GetProfile")]
        public object GetProfile(string emailAddress)
        {
            return new Auth0Profile() { firstName = "Test", lastName = "User", companyName = "TestCo, Inc.", emailAddress = emailAddress, mfaEnabled = true, mfaPhoneNumber = "8885551212", phoneNumber = "8005551212", password = "testtest" };
        }

        [HttpPost]
        [Route("api/v{version:apiVersion}/Authorization/CreateProfile")]
        public object CreateProfile(string profile)
        {
            try
            {
                var o = JsonConvert.DeserializeObject<Auth0Profile>(profile);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.InnerException.ToString());
            }
        }

        [HttpPut]
        [Route("api/v{version:apiVersion}/Authorization/UpdateProfile")]
        public object UpdateProfile(string profile)
        {
            try
            {
                var o = JsonConvert.DeserializeObject<Auth0Profile>(profile);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.InnerException.ToString());
            }
        }

        [HttpPut]
        [Route("api/v{version:apiVersion}/Authorization/ResetPW")]
        public object ResetPassword(string emailAddress)
        {
            if (emailAddress != null) { return new Uri("http://www.ChangeYourPassword.com"); } else { return BadRequest(); }
        }

        [HttpPut]
        [Route("api/v{version:apiVersion}/Authorization/ChangePassword")]
        public object ChangePassword(string emailAddress, string password)
        {
            if (emailAddress != null && password != null) { return Ok(); } else { return NotFound(); }
        }
    }
}
