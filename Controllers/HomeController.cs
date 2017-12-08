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

        public ActionResult Index()
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
            if (newWay.HasValue && newWay.Value)
                return RedirectToAction("EnterEmail");

            return View();
        }

        public ActionResult EnterEmail()
        {
            return View();
        }

        //[HttpPost]
        //public ActionResult EnterEmail(EnterEmailModel model)
        //{
        //    // Is Internal Email?
        //    //    Yes > Display message to reach out ot FOPS Admin to be added as a FOPS Support User
        //    if (false)
        //    {
        //        ViewBag.IsInternalEmail = true;
        //        return View();
        //    }
            

        //    // Is Existing SSO User?
        //    //    Yes > Redirect to Registration Page
        //    // TODO: encrypt email address in the url so it cant be modified
        //    return Redirect("/Home/Register2/?email=" + model.EmailAddress); 

        //    // Is SSO User account blocked (Inactive)?
        //    //    Yes > Display message that there is an issue with their account and they need to contact Help Desk

        //    // Is Existing FOPs User?
        //    //    Yes > Redirect to Login Page


        //    return View();
        //}

        public ActionResult Register2(string email, bool? hasSSO)
        {
            RegistrationModel model = new RegistrationModel
            {
                EmailAddress = email,
                HasSSO = hasSSO ?? false
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

            try
            {
                string executingUser = "";

                var result = _service.AddUser(newUser);
                log.Info(string.Format("User {0} created New user with the following data: \n\n{1}", executingUser, seralizedModel));

                return Json(result, JsonRequestBehavior.AllowGet);
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
                return Json("netApi.Controller.AddUser(string) caused DbEntityValidationException!");
            }
            catch (Exception e)
            {
                var msg = string.Format("Error Saving User Record.\n\nData:\n{0}.\n\n{1}", seralizedModel, e.Message);
                log.Error(e);

                return Json(msg);
            }
        }

        public ActionResult ThankYou()
        {
            return View();
        }

        public ActionResult SsoCallback()
        {
            return View();
        }

    }
}