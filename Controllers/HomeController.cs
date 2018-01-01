using netApi.Repositories.Administration.Model;
using NetEasyPay.Interfaces;
using NetEasyPay.Models;
using NetEasyPay.Services;
using Newtonsoft.Json;
using System;
using System.Data.Entity.Validation;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Web.Mvc;

namespace NetEasyPay.Controllers
{
    public class HomeController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IAdministrationService _service;


        public HomeController()
        {
            _service = new AdministrationService();
        }

        public ActionResult Index(string status)
        {
            if (!string.IsNullOrEmpty(status))
            {
                if (status.ToLower() == "inactive")
                    ViewBag.UserInactive = true;
                else if (status.ToLower() == "locked")
                    ViewBag.UserLocked = true;
            }

            return View();
        }

        public ActionResult Incomplete()
        {
            return View();
        }

        public ActionResult PaymentHistory()
        {
            return View();
        }

        public ActionResult Invoices()
        {
            return View();
        }

        public ActionResult Logout()
        {
            return View();
        }

        public ActionResult ForgotPW()
        {
            return View();
        }

        public ActionResult Register(bool? newWay)
        {
            return View("EnterEmail");
        }

        public ActionResult EnterEmail()
        {
            return View();
        }


        public ActionResult Register2(string email, bool? sso)
        {
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("EnterEmail");
            }

            RegistrationModel model = new RegistrationModel
            {
                EmailAddress = email,
                HasSSO = sso ?? false
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult Register2(RegistrationModel model)
        {
            // TODO: Refactor into a service class

            // HACK MASTER!!! Need to convert the RegistrationModel into the EasyPayAuth0User model
            var seralizedModel = JsonConvert.SerializeObject(model);
            var newUser = JsonConvert.DeserializeObject<EasyPayAuth0User>(seralizedModel);
            newUser.AddToAuth0 = !model.HasSSO;

            if (model.HasSSO == true)
            {
                ModelState.Remove("Password");
                ModelState.Remove("ConfirmPassword");
            }
            else
            {
                // Validate the password agains the email address
                // - The password cannot be the same as the username and cannot contain 3 or more consecutive characters from the username
                var isPasswordValid = IsPasswordValid(model.EmailAddress, model.Password);
                if (!isPasswordValid)
                {
                    ModelState.AddModelError("Password", "Passwords should meet the displayed requirments.");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    string executingUser = "";

                    var result = _service.AddUser(newUser);
                    log.Info(string.Format("User {0} created New user with the following data: \n\n{1}", executingUser, seralizedModel));

                    ViewBag.IsSuccessful = true;
                    return Redirect("/home/ThankYou/?emailAddress=" + model.EmailAddress);
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
                    ViewBag.IsSuccessful = false;
                    ViewBag.Message = "The system experienced a problem submitting your information.  Please contact the system administrator.";
                }
                catch (Exception e)
                {
                    var msg = string.Format("Error Saving User Record.\n\nData:\n{0}.\n\n{1}", seralizedModel, e.Message);
                    log.Error(e);

                    ViewBag.IsSuccessful = false;
                    ViewBag.Message = "The system experienced a problem submitting your information.  Please contact the system administrator.";
                }

            }

            
            return View(model);
        }

        public ActionResult ThankYou()
        {
            return View();
        }

        public ActionResult SsoCallback()
        {
            return View();
        }

        private bool IsPasswordValid(string username, string password)
        {
            var testUserName = username.ToLower();
            var testPassword = password.ToLower();

            for (int i = 0; (i + 3) <= testUserName.Length; i++)
            {
                if (testPassword.IndexOf(testUserName.Substring(i, 3)) != -1)
                {
                    return false;
                }
            }

            return true;
        }

    }
}