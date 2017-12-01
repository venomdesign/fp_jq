using Microsoft.Web.Http;
using NetEasyPay.Interfaces;
using NetEasyPay.Services;
using Newtonsoft.Json;
using System;
using System.Web.Http;
using static netApi.Common.Helpers;

namespace NetEasyPay.Controllers
{
    [ApiVersion("1.0")]
    public class AuthorizationController : ApiController
    {
        private readonly IAuthorizationService _service;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public AuthorizationController()
        {
            _service = new AuthorizationService();
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
                return Ok(_service.LoadUser(UserId));
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
                return Ok(_service.GetClaims(this.User));
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
            var emailUpper = email.ToUpper();

            Response response = null;

            if (SSO)
            {
                switch (emailUpper)
                {
                    case "VALIDSSONOFOPS@TEST.COM":
                        response = new Response
                        {
                            Success = true,
                            RowCount = 1,
                            ElapsedTime = 0,
                            //Results = "VALID WITH SSO"
                            Results = "{\"userName\": \"validssonofops@test.com\", \"firstName\": \"ValidSso\", \"lastName\": \"NoFops\", \"companyId\": \"Fidelity National & Financial\", \"status\": \"VALID SSO, NO FOPS\" }"
                        };
                        break;
                    case "VALIDWITHOUTSSO@TEST.COM":
                        response = new Response
                        {
                            Success = true,
                            RowCount = 1,
                            ElapsedTime = 0,
                            Results = "VALID WITHOUT SSO"
                        };
                        break;
                    case "INVALIDWITHSSO@TEST.COM":
                        response = new Response
                        {
                            Success = true,
                            RowCount = 1,
                            ElapsedTime = 0,
                            Results = "INVALID WITH SSO"
                        };
                        break;
                    case "INVALIDWITHOUTSSO@TEST.COM":
                        response = new Response
                        {
                            Success = true,
                            RowCount = 1,
                            ElapsedTime = 0,
                            Results = "INVALID WITHOUT SSO"
                        };
                        break;
                    //case "RIC.CASTAGNA@GMAIL.COM":
                    //    response = new Response
                    //    {
                    //        Success = true,
                    //        RowCount = 1,
                    //        ElapsedTime = 0,
                    //        Results = "FOPS USER NOT SIGNED IN"
                    //    };
                    //    break;
                    default:
                        if (emailUpper.Contains("@FNF.COM"))
                        {
                            response = new Response
                            {
                                Success = true,
                                RowCount = 1,
                                ElapsedTime = 0,
                                Results = "INTERNAL EMAIL DETECTED"
                            };
                        }
                        else
                        {
                            response = (Response)_service.ValidateEmail(email);
                        }
                        break;
                }
            }

            return response;
        }

        [HttpGet]
        [Route("api/v{version:apiVersion}/Authorization/GetProfile")]
        public object GetProfile(string emailAddress)
        {
            return new Profile() { FirstName = "Test", LastName = "User", CompanyName = "TestCo, Inc.", EmailAddress = emailAddress, MFAEnabled = true, MFAPhoneNumber = "888-555-1212", PhoneNumber = "800-555-1212", Password = "testtest", Status = "I have no idea what this field is for!" };
        }

        [HttpPost]
        [Route("api/v{version:apiVersion}/Authorization/CreateProfile")]
        public object CreateProfile(string profile)
        {
            try
            {
                var o = JsonConvert.DeserializeObject<Profile>(profile);
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
                var o = JsonConvert.DeserializeObject<Profile>(profile);
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

    internal class Profile
    {
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CompanyName { get; set; }
        public string PhoneNumber { get; set; }
        public bool MFAEnabled { get; set; }
        public string MFAPhoneNumber { get; set; }
        public string Status { get; set; }
    }
}
