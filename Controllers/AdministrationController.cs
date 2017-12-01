using Microsoft.Web.Http;
using netApi.Repositories.Authorization.Model;
using NetEasyPay.Interfaces;
using NetEasyPay.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Configuration;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using System.Data.Entity.Validation;

namespace NetEasyPay.Controllers
{
    [ApiVersion("1.0")]
    public class AdministrationController : ApiController
    {
        private readonly IAdministrationService _service;

        //log4Net being configured following the instructions on https://stackify.com/log4net-guide-dotnet-logging/

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public AdministrationController()
        {
            _service = new AdministrationService();
        }

        #region Add Methods

        /// <summary>
        /// Add new APP_ROLE to FOPS database
        /// </summary>
        /// <param name="value">(string) JSON version of APP_ROLE object to create</param>
        /// <returns>HTTPResult with JSON version of new APP_ROLE</returns>
        // TODO: [Authorize]
        [HttpPost]
        [Route("api/v{version:apiVersion}/Administration/AddRole")]
        public object AddRole([FromBody] string value)
        {
            APP_ROLE roleToAdd = null;

            try
            {
                roleToAdd = JsonConvert.DeserializeObject<APP_ROLE>(value);

                var executingUser = "";
                var str = string.Format("User {0} created a new role with the following data: \n\n{1}", executingUser, value);

                APP_ROLE result = _service.AddRole(roleToAdd);
                log.Info(str);
                return Ok(result);

            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    StringBuilder errmsg = new StringBuilder();

                    errmsg.Append(string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State.ToString()));
                    foreach (var ve in eve.ValidationErrors)
                    {
                        errmsg.Append(string.Format("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage));
                    }
                    log.Error(e);
                }
                return BadRequest("netApi.Controller.AddUser(string) caused DbEntityValidationException!");
            }
            catch (Exception e)
            {
                log.Error(e);

                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Adds a new USER_ROLE record to associate a role with a user
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="RoleId"></param>
        /// <param name="APP_ID"></param>
        /// <returns></returns>
        // TODO: [Authorize]
        [HttpPost]
        [Route("api/v{version:apiVersion}/Administration/AddRoleToUser")]
        public object AddRoleToUser(long UserId, int RoleId, string APP_ID)
        {

            if (UserId == 0 || RoleId == 0) { return BadRequest("Values for UserId and RoleId are required.  Please verify your data."); }

            try
            {
                var executingUser = "";

                USER_ROLE urToAdd = new USER_ROLE() { USER_ID = UserId, ROLE_ID = RoleId, IS_ACTIVE = "1", AUDIT_REC_CREATE_APPL_ID = APP_ID, AUDIT_REC_CREATE_DTS = DateTime.Now, AUDIT_REC_CREATE_APPL_USER_ID = "1", AUDIT_REC_CREATE_DB_USER_ID = "Not supposed to be done by API!" };
                USER_ROLE result = _service.AddRoleToUser(urToAdd);
                log.Info(string.Format("User {2} has added New RoleId: {0} association to UserId: {1}", RoleId, UserId, executingUser));
                return Ok(result);
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    StringBuilder errmsg = new StringBuilder();

                    errmsg.Append(string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State.ToString()));
                    foreach (var ve in eve.ValidationErrors)
                    {
                        errmsg.Append(string.Format("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage));
                    }
                    log.Error(e);
                }
                return BadRequest("netApi.Controller.AddRoleToUser(int, int) caused DbEntityValidationException!");
            }
            catch (Exception e)
            {
                var msg = string.Format("Error creating new User/Role association.\n\nUserId:\n{0}, RoleId: {1}.\n\n{2}", UserId, RoleId, e.Message);
                log.Error(e);

                return BadRequest(msg);
            }
        }

        /// <summary>
        /// Add a new APP_USER to the FOPS database
        /// </summary>
        /// <param name="value">(string) JSON version of new APP_USER</param>
        /// <returns>HTTPResponse with JSON representation of new APP_USER object</returns>
        // TODO: [Authorize]
        [HttpPost]
        [Route("api/v{version:apiVersion}/Administration/AddUser")]
        public object AddUser([FromBody] JToken value)
        {
            APP_USER newUser = null;

            try
            {
                newUser = JsonConvert.DeserializeObject<APP_USER>(value.ToString());

                string executingUser = "";

                APP_USER result = _service.AddUser(newUser);
                log.Info(string.Format("User {0} created New user with the following data: \n\n{1}", executingUser, value));
                return Ok(result);
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    StringBuilder errmsg = new StringBuilder();

                    errmsg.Append(string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State.ToString()));
                    foreach (var ve in eve.ValidationErrors)
                    {
                        errmsg.Append(string.Format("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage));
                    }
                    log.Error(errmsg);
                }
                return BadRequest("netApi.Controller.AddUser(string) caused DbEntityValidationException!");
            }
            catch (Exception e)
            {
                var msg = string.Format("Error Saving User Record.\n\nData:\n{0}.\n\n{1}", value, e.Message);
                log.Error(e);

                return BadRequest(msg);
            }
        }

        /// <summary>
        /// Adds a new APP_PERMISSION record to the database.
        /// </summary>
        /// <param name="value">(string) JSON serialized APP_PERMISSION object fully hydrated.</param>
        /// <returns>(APP_PERMISSION) object in a JsonResult</returns>
        // TODO: [Authorize]
        [HttpPost]
        [Route("api/v{version:apiVersion}/Administration/AddPermission")]
        public object AddPermission([FromBody] string value)
        {
            APP_PERMISSION newPermission = null;

            try
            {
                newPermission = JsonConvert.DeserializeObject<APP_PERMISSION>(value);

                string executingUser = "";

                APP_PERMISSION result = _service.AddPermission(newPermission);
                log.Info(string.Format("User {0} created New permission with the following data: \n\n{1}", executingUser, value));
                return Ok(result);
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    StringBuilder errmsg = new StringBuilder();

                    errmsg.Append(string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State.ToString()));
                    foreach (var ve in eve.ValidationErrors)
                    {
                        errmsg.Append(string.Format("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage));
                    }
                    log.Error(errmsg);
                }
                return BadRequest("netApi.Controller.AddPermission(string) caused DbEntityValidationException!");
            }
            catch (Exception e)
            {
                var msg = string.Format("Error Saving Permission Record.\n\nData:\n{0}.\n\n{1}", value, e.Message);
                log.Error(e);

                return BadRequest(msg);
            }
        }

        /// <summary>
        /// Adds an app permission to a specified role
        /// </summary>
        /// <param name="RoleId"></param>
        /// <param name="PermissionId"></param>
        /// <returns></returns>
        // TODO: [Authorize]
        [HttpPost]
        [Route("api/v{version:apiVersion}/Administration/AddPermissionToRole")]
        public object AddPermissionToRole(int RoleId, int PermissionId)
        {
            if (RoleId == 0 || PermissionId == 0) { return BadRequest("Values for PermissionId and RoleId are required.  Please verify your data."); }

            try
            {
                ROLE_PERMISSION result = _service.AddPermissionToRole(RoleId, PermissionId);
                log.Info(string.Format("User {2} has added New RoleId: {0} association to PermissionId: {1}", RoleId, PermissionId, "FOPS"));
                return Ok(result);
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    StringBuilder errmsg = new StringBuilder();

                    errmsg.Append(string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State.ToString()));
                    foreach (var ve in eve.ValidationErrors)
                    {
                        errmsg.Append(string.Format("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage));
                    }
                    log.Error(e);
                }
                return BadRequest("netApi.Controller.AddRoleToUser(int, int) caused DbEntityValidationException!");
            }
            catch (Exception e)
            {
                var msg = string.Format("Error creating new User/Role association.\n\nPermissionId:\n{0}, RoleId: {1}.\n\n{2}", PermissionId, RoleId, e.Message);
                log.Error(e);

                return BadRequest(msg);
            }
        }

        /// <summary>
        /// Saves the user's token to the database.
        /// </summary>
        /// <param name="userId">(long) USER_SYS_ID to associate the token with</param>
        /// <param name="token">(long) Token to be stored</param>
        /// <returns></returns>
        // TODO: [Authorize]
        [HttpPut]
        [Route("api/v{version:apiVersion}/Administration/StoreUserToken")]
        public object SaveUserToken(long userId, long token)
        {
            try
            {
                return Json(_service.SaveUserToken(userId, token));
            }
            catch (Exception e)
            {
                var msg = string.Format("Error Saving User Token.\n\n{0}", e.Message);
                log.Error(e);

                return BadRequest(msg);
            }
        }

        #endregion

        #region Remove/Delete Methods

        /// <summary>
        /// Soft delete a role in the FOPS application
        /// </summary>
        /// <param name="RoleId">(int) ROLE_SYS_ID of the role to deactivate</param>
        /// <returns>HTTPResult with JSON representation of the deactivated APP_ROLE</returns>
        // TODO: [Authorize]
        [HttpPut]
        [Route("api/v{version:apiVersion}/Administration/DeleteRole")]
        public object DeleteRole(int RoleId)
        {

            if (RoleId == 0) { return BadRequest("Value for RoleId is required.  Please verify your data."); }

            try
            {
                var executingUser = "";

                var result = _service.DeleteRole(RoleId);
                log.Info(string.Format("User {0} marked RoleId: {1} as IsActive=False", executingUser, RoleId));
                return Ok(result);
            }
            catch (Exception e)
            {
                var msg = string.Format("Error Deleting RoleId:{0}.\n\n{1}", RoleId, e.Message);
                log.Error(e);

                return BadRequest(msg);
            }
        }

        /// <summary>
        /// Soft delete a user in the FOPS application
        /// </summary>
        /// <param name="UserId">(int) USER_SYS_ID of the user to deactivate</param>
        /// <returns>HTTPResult with JSON serialized APP_USER object</returns>
        // TODO: [Authorize]
        [HttpPut]
        [Route("api/v{version:apiVersion}/Administration/DeleteUser")]
        public object DeleteUser(long UserId)
        {
            if (UserId == 0) { return BadRequest("Value for UserId is required.  Please verify your data."); }

            try
            {
                var executingUser = "";

                var result = _service.DeleteUser(UserId);
                log.Info(string.Format("User {0} marked UserId: {1} as IsActive=False", executingUser, UserId));
                return Ok(result);
            }
            catch (Exception e)
            {
                var msg = string.Format("Error Deleting UserId:{0}.\n\n{1}", UserId, e.Message);
                log.Error(e);

                return BadRequest(msg);
            }
        }

        /// <summary>
        /// Remove the association between an APP_USER and APP_ROLE
        /// </summary>
        /// <param name="UserId">(int) USER_SYS_ID of the user to remove role from</param>
        /// <param name="RoleId">(int) ROLE_SYS_ID of the role to remove from the user</param>
        /// <returns>HTTPResult of the database remove</returns>
        // TODO: [Authorize]
        [HttpPost]
        [Route("api/v{version:apiVersion}/Administration/RemoveRoleFromUser")]
        public object RemoveRoleFromUser(long UserId, int RoleId)
        {
            if (UserId == 0 || RoleId == 0) { return BadRequest("Values for UserId and RoleId are required.  Please verify your data."); }

            try
            {
                var executingUser = "";

                var result = _service.RemoveRoleFromUser(UserId, RoleId);
                log.Info(string.Format("User {0} removed RoleId: {1} from UserId: {2}", executingUser, RoleId, UserId));
                return Ok(result);
            }
            catch (Exception e)
            {
                var msg = string.Format("Error removing RoleId:{0} from UserId: {1}.\n\n{2}", RoleId, UserId, e.Message);
                log.Error(e);

                return BadRequest(msg);
            }
        }

        /// <summary>
        /// Delete a user's token from the database.  This is a HARD delete and the record is not backed up anywhere.
        /// </summary>
        /// <param name="userId">(long) USER_SYS_ID of the user that owns the token</param>
        /// <param name="token">(long) Token to delete</param>
        /// <returns>(APIController) Response indicating success (OK or BadRequest)</returns>
        // TODO: [Authorize]
        [HttpPost]
        [Route("api/v{version:apiVersion}/Administration/DeleteUserToken")]
        public object DeleteUserToken(long userId, long token)
        {
            try
            {
                if (_service.DeleteUserToken(userId, token) == true)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest("Unable to delete token.");
                }
            }
            catch (Exception e)
            {
                var msg = string.Format("Error Deleting User Token.\n\n{0}", e.Message);
                log.Error(e);

                return BadRequest(msg);
            }
        }

        /// <summary>
        /// Remove the ROLE_PERMISSION association record from the database.  This is a HARD delete.
        /// </summary>
        /// <param name="RoleId">(int) ROLE_ID of the ROLE_PERMISSION to remove</param>
        /// <param name="PermissionId">(int) PERMISSION_ID of the ROLE_PERMISSION to remove</param>
        /// <returns>(bool) True if deleted, False if error</returns>
        // TODO: [Authorize]
        [HttpPost]
        [Route("api/v{version:apiVersion}/Administration/DeleteRolePermission")]
        public object DeletePermissionFromRole(int RoleId, int PermissionId)
        {
            try
            {
                if (_service.DeletePermissionFromRole(RoleId, PermissionId) == true)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest("Unable to delete ROLE_PERMISSION.");
                }
            }
            catch (Exception e)
            {
                var msg = string.Format("Error Deleting Role_Permission association.\n\n{0}", e.Message);
                log.Error(e);

                return BadRequest(msg);
            }
        }

        /// <summary>
        /// Removes a permission from the database.  This is a HARD delete.
        /// </summary>
        /// <param name="PermissionId">(int) PERMISSION_SYS_ID to delte</param>
        /// <returns>(bool) True if deleted, False if error</returns>
        // TODO: [Authorize]
        [HttpPost]
        [Route("api/v{version:apiVersion}/Administration/DeletePermission")]
        public object DeletePermission(int PermissionId)
        {
            try
            {
                if (_service.DeletePermission(PermissionId) == true)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest("Unable to delete APP_PERMISSION.");
                }
            }
            catch (Exception e)
            {
                var msg = string.Format("Error Deleting APP_PERMISSION.\n\n{0}", e.Message);
                log.Error(e);

                return BadRequest(msg);
            }
        }
        #endregion

        #region Get Methods

        /// <summary>
        /// Get All Active Roles in the FOPS database
        /// </summary>
        /// <returns>HTTPResponse with JSON of Active Roles</returns>
        // TODO: [Authorize]
        [HttpGet]
        [Route("api/v{version:apiVersion}/Administration/GetActiveRoles")]
        public object GetActiveRoles()
        {
            try
            {
                return Ok(_service.GetActiveRoles());
            }
            catch (Exception e)
            {
                var msg = string.Format("Error Retrieving Active Roles from database.\n\n{0}", e.Message);
                log.Error(e);

                return BadRequest(msg);
            }
        }

        /// <summary>
        /// Get specific role from FOPS database
        /// </summary>
        /// <param name="Id">(int) ROLE_SYS_ID of the role to retreive</param>
        /// <returns>HttpResponse with JSON of requested role</returns>
        // TODO: [Authorize]
        [HttpGet]
        [Route("api/v{version:apiVersion}/Administration/GetRole")]
        public object GetRole(int Id)
        {
            if (Id == 0) { return BadRequest("Value for Id is required.  Please verify your data."); }

            try
            {
                return Ok(_service.GetRole(Id));
            }
            catch (Exception e)
            {
                var msg = string.Format("Error Retrieving RoleID: {0}.\n\n{1}", Id, e.Message);
                log.Error(e);

                return BadRequest(msg);
            }
        }

        /// <summary>
        /// Get All Roles in system regardless of IsActive flag
        /// </summary>
        /// <returns></returns>
        // TODO: [Authorize]
        [HttpGet]
        [Route("api/v{version:apiVersion}/Administration/GetRoles")]
        public object GetRoles()
        {
            try
            {
                return Ok(_service.GetRoles());
            }
            catch (Exception e)
            {
                var msg = string.Format("Error Retrieving Roles.");
                log.Error(e);

                return BadRequest(msg);
            }
        }

        /// <summary>
        /// Gets all APP_ROLEs for specified user
        /// </summary>
        /// <param name="UserId">(int) USER_SYS_ID to get roles for</param>
        /// <returns>HTTPResponse with JSON of APP_ROLE array</returns>
        // TODO: [Authorize]
        [HttpGet]
        [Route("api/v{version:apiVersion}/Administration/GetRolesByUser")]
        public object GetRolesByUser(long UserId)
        {
            if (UserId == 0) { return BadRequest("Value for UserId is required.  Please verify your data."); }

            try
            {
                return Ok(_service.GetRolesByUser(UserId));
            }
            catch (Exception e)
            {
                var msg = string.Format("Error Retrieving User Record for UserId {0}.\n\n{1}", UserId, e.Message);
                log.Error(e);

                return BadRequest(msg);
            }
        }

        /// <summary>
        /// Gets list of APP_PERMISSIONS for the specified role
        /// </summary>
        /// <param name="RoleId">(int) ROLE_SYS_ID of the role to load permissions for</param>
        /// <returns>(List(APP_PERMISSION)) A list of the APP_PERMISSIONS available for specified role</returns>
        // TODO: [Authorize]
        [HttpGet]
        [Route("api/v{version:apiVersion}/Administration/GetPermissionsByRole")]
        public object GetPermissionsByRole(int RoleId)
        {
            if (RoleId == 0) { return BadRequest("Value for RoleId is required.  Please verify your data."); }
            try
            {
                return Ok(_service.GetPermissionsByRole(RoleId));
            }
            catch (Exception e)
            {
                var msg = string.Format("Error Retrieving permissions for RoleId {0}.\n\n{1}", RoleId, e.Message);
                log.Error(e);

                return BadRequest(msg);
            }
        }

        /// <summary>
        /// Loads all permissions in the database
        /// </summary>
        /// <returns>(List(APP_PERMISSION)) Full list of permissions, regardless of IS_ACTIVE</returns>
        // TODO: [Authorize]
        [HttpGet]
        [Route("api/v{version:apiVersion}/Administration/GetPermissions")]
        public object GetPermissions()
        {
            try
            {
                return Ok(_service.GetPermissions());
            }
            catch (Exception e)
            {
                var msg = string.Format("Error Retrieving all permissions.\n\n{0}", e.Message);
                log.Error(e);

                return BadRequest(msg);
            }

        }

        /// <summary>
        /// Get user by USER_SYS_ID
        /// </summary>
        /// <param name="UserId">(int) USER_SYS_ID of user to retreieve</param>
        /// <returns>HTTPResult with JSON of APP_USER</returns>
        // TODO: [Authorize]
        [HttpGet]
        [Route("api/v{version:apiVersion}/Administration/GetUser")]
        public object GetUser(long UserId)
        {
            if (UserId == 0) { return BadRequest("Value for UserId is required.  Please verify your data."); }

            try
            {
                return Ok(_service.GetUser(UserId));
            }
            catch (Exception e)
            {
                var msg = string.Format("Error Retrieving User Record for username {0}.\n\n{1}", UserId, e.Message);
                log.Error(e);

                return BadRequest(msg);
            }
        }

        /// <summary>
        /// Get user by USER_NAME (email address)
        /// </summary>
        /// <param name="userName">(string) Email address of user to retreieve</param>
        /// <returns>HTTPResult with JSON of APP_USER</returns>
        // TODO: [Authorize]
        [HttpGet]
        [Route("api/v{version:apiVersion}/Administration/GetUserByEmail")]
        public object GetUser(string userName)
        {
            if (!HttpContext.Current.User.Identity.IsAuthenticated)
            {
                if (HttpContext.Current.Request.Headers["SystemRequest"] == null
                    || HttpContext.Current.Request.Headers["SystemRequest"] != ConfigurationManager.AppSettings["as:AudienceSecret"])
                {
                    return Unauthorized();
                }
            }

            if (userName == string.Empty) { return BadRequest("Value for username is required.  Please verify your data."); }

            try
            {
                return Ok(_service.GetUser(userName));
            }
            catch (Exception e)
            {
                var msg = string.Format("Error Retrieving User Record for username {0}.\n\n{1}", userName, e.Message);
                log.Error(e);

                return BadRequest(msg);
            }
        }

        /// <summary>
        /// Returns all users who are pending approval in EasyPay.  This function will be used
        /// to allow the users access to the application.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/v{version:apiVersion}/Administration/GetUsersPendingApproval")]
        public object GetPendingUsers(string status = "")
        {
            try
            {
                if (status.ToUpper() != "IN PROGRESS" &&
                    status.ToUpper() != "PENDING USER VALIDATION" &&
                    status.ToUpper() != "COMPLETE" &&
                    status.ToUpper() != "CANCELLED"
                    ) { return BadRequest("Invalid Status"); }

                return Ok(_service.GetPendingUsers(status));
            }
            catch (Exception e)
            {
                var msg = string.Format("Error Retrieving Pending User Records.\n\n{0}", e.Message);
                log.Error(e);

                return BadRequest(msg);
            }

        }

        /// <summary>
        /// Gets all users in FOPS database
        /// </summary>
        /// <returns>HTTPResponse with JSON array of APP_USER</returns>
        // TODO: [Authorize]
        [HttpGet]
        [Route("api/v{version:apiVersion}/Administration/GetUsers")]
        public object GetUsers()
        {
            try
            {
                return Ok(_service.GetUsers());
            }
            catch (Exception e)
            {
                var msg = string.Format("Error Retrieving User Records.\n\n{0}", e.Message);
                log.Error(e);

                return BadRequest(msg);
            }
        }

        #endregion

        #region Various Methods

        /// <summary>
        /// Update the existing APP_USER in the FOPS database with the one sent in
        /// </summary>
        /// <param name="value">(string) Serialized JSON APP_USER object</param>
        /// <returns>HTTPResult with JSON representation of updated APP_USER</returns>
        // TODO: [Authorize]
        [HttpPut]
        [Route("api/v{version:apiVersion}/Administration/UpdateUser")]
        public object UpdateUser([FromBody] JToken value)
        {
            APP_USER userToUpdate = null;

            try
            {
                //Deserialize the string back into an APP_USER object
                userToUpdate = JsonConvert.DeserializeObject<APP_USER>(value.ToString());

                var executingUser = "";

                var result = _service.UpdateUser(userToUpdate);
                log.Info(string.Format("User {0} updated UserID: {1} updated with the following data: \n\n{1}", executingUser, userToUpdate.USER_SYS_ID, userToUpdate));
                return Ok(result);
            }
            catch (Exception e)
            {
                var msg = string.Format("Error Updating User Record.\n\nData:\n{0}.\n\n{1}", value, e.Message);
                log.Error(e);

                return BadRequest(string.Format("Error updating user with passed in data.\n\nData:\n{0}", value));
            }
        }

        /// <summary>
        /// Takes the user's registration information, validates that there isn't an account with the information already in the database,
        /// and creates a new registration.
        /// </summary>
        /// <param name="registration">(Json) Object with all of the registration properties</param>
        /// <returns>(HTTPResponse) Result of the registration</returns>
        // TODO: [Authorize]
        [HttpPost]
        [Route("api/v{version:apiVersion}/Administration/Register")]
        public object SubmitRegistration([FromBody]string registration)
        {
            APP_USER reg = null;

            //Create a new AppUser object with the information passed in.
            try
            {
                reg = JsonConvert.DeserializeObject<APP_USER>(registration);
            }
            catch (JsonReaderException jex)
            {
                return BadRequest("Registration does not map to a user object.  Please check JSON and resubmit." + jex.StackTrace.ToString());
            }
            var tmp = _service.GetUser(reg.USER_NAME);

            //Does the email address already exist in the database?
            if (tmp != null)
            {
                //Someone in the database 
                log.Error(string.Format("{0}\n\n{1}", "Username already exists.", reg.USER_SYS_ID));
                return Conflict();
            }

            var resp = AddUser(registration);

            log.Info(string.Format("New registration created from new user: \n\n{0}", registration));

            return resp;
        }

        /// <summary>
        /// Approves a user and activates their account.  This will allow the user to begin making payments.
        /// </summary>
        /// <param name="UserIdToApprove">(long) USER_SYS_ID of the user to activate</param>
        /// <returns>(APP_USER) Populated APP_USER object with the settings updated for activation.</returns>
        // TODO: [Authorize]
        [HttpPut]
        [Route("api/v{version:apiVersion}/Administration/ApproveRegistration")]
        public object ApproveRegistration(long UserIdToApprove)
        {
            //Try to get the user
            var tmp = GetUser(UserIdToApprove);

            //Did we get a good result from the GetUser() call or an error?
            if (tmp.GetType() != typeof(OkNegotiatedContentResult<APP_USER>))
            {

                var msg = string.Format("Registration approval failed.\n\nUserId:\n{0}.", UserIdToApprove);
                log.Error(new Exception("Response was " + tmp.GetType().ToString()));

                return BadRequest(string.Format("Unable to approve registration for userId: {0}.  Check logs for details.", UserIdToApprove));
            }

            //Convert the response of GetUser() to something that we can work with
            var user = (OkNegotiatedContentResult<APP_USER>)tmp;

            //Get a reference to the actual user
            var UserToUpdate = user.Content;

            //Activate the account
            UserToUpdate.IS_ACTIVE = "0";
            UserToUpdate.IS_LOCKED = "0";
            UserToUpdate.ACCOUNT_STATUS = "";

            //Update the user in the database and return whatever result that we're passed!
            var resp = UpdateUser(JsonConvert.SerializeObject(UserToUpdate));

            log.Info(string.Format("Newly registered user has been approved: \n\n{0}", UserIdToApprove));

            return resp;
        }

        /// <summary>
        /// Creates an association between the USER_SYS_ID and the CRRAR Contact and Attention Ids
        /// </summary>
        /// <param name="UserSysID">(long) USER_SYS_ID of user to create association for</param>
        /// <param name="ContactId">(long) CRRAR_CONTACT_ID from CRRAR system</param>
        /// <param name="attnId">(long) OPTIONAL: CRRAR_ATTENTION_ID from the CRRAR system</param>
        /// <returns>(USER_CONTACT) Object with association created and new index</returns>
        [HttpPost]
        [Route("api/v{version:apiVersion}/Administration/AssociateCrrarContact")]
        public object AssociateCRRARContactToUser(long UserSysID, long ContactId, long attnId = 0)
        {
            try
            {


                if (UserSysID == 0 || ContactId == 0) { return BadRequest("UserId and ContactId are required.  Attn is optional, leave as 0 for ALL attentionTo records."); }

                var newUC = new USER_CONTACT() { USER_SYS_ID = UserSysID, CRRAR_CONTACT_ID = ContactId, CRRAR_ATTN_ID = attnId, AUDIT_REC_CREATE_APPL_USER_ID = "FOPS", AUDIT_REC_CREATE_DTS = DateTime.Now };

                _service.AssociateCRRARContactToUser(newUC);

                return Ok(newUC);

            }
            catch (Exception e)
            {
                var msg = string.Format("CRRAR Contact/Attn association {1}/{2} failed.\n\nUserId:\n{0}.", UserSysID, ContactId, attnId);
                log.Error(e);

                return BadRequest(msg);
            }
        }

        #endregion
    }
}
