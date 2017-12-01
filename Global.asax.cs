using AppPropertiesLibrary;
using log4net;
using System;
using System.Configuration;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace NetEasyPay
{
    public class MvcApplication : HttpApplication
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        protected void Application_Start()
        {
            //LoadApplicationProperties();

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }


        private void LoadApplicationProperties()
        {
            Log.Info("Loading application properties from controlCenter...");
            String appNames = ConfigurationManager.AppSettings["controlCenter.appNames"];

            Log.Info("ControlCenter AppName: " + appNames);
            Log.Info("ControlCenter Url: " + ConfigurationManager.AppSettings["controlCenter.url"]);

            Log.Info("Authentication Service ApiKey: " + ConfigurationManager.AppSettings["authenticationService.apiKey"]);
            Log.Info("Authentication Service Username: " + ConfigurationManager.AppSettings["authenticationService.username"]);
            Log.Info("AuthenticationService Url: " + ConfigurationManager.AppSettings["authenticationService.url"]);

            Log.Debug("invoking authenticate()...");
            AppProperty.Instance.Authenticate();
            Log.Debug("authentcate() successful");
            AppProperty.Instance.SetConfigurationManagerValues();
        }

    }
}
