using System.Web.Http;
using WebActivatorEx;
using NetEasyPay;
using Swashbuckle.Application;
using System;
using System.Linq;
using System.Configuration;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace NetEasyPay
{
    public class SwaggerConfig
    {
        public static void Register()
        {
            var thisAssembly = typeof(SwaggerConfig).Assembly;

            GlobalConfiguration.Configuration
                .EnableSwagger(c =>
                    {
                        c.SingleApiVersion("v1", "NetEasyPay");
                        c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
                        c.PrettyPrint();

                        //var schemes = new string[] { "http", "https" };
                        //c.Schemes(schemes);
                        c.Schemes(new[] { "http", "https" });
                        c.RootUrl(r => GetRootUrlFromAppConfig());


                        c.IgnoreObsoleteActions();
                        c.IncludeXmlComments(string.Format(@"{0}Models\EasyPayApi.xml", AppDomain.CurrentDomain.BaseDirectory));
                    })
                .EnableSwaggerUi(c =>
                    {
                        c.DocumentTitle("FNF EasyPay API");
                    });
        }

        private static string GetRootUrlFromAppConfig()
        {
            var s = "";
#if DEBUG
            s = "http://localhost:50753";
#else
            s = ConfigurationManager.AppSettings["SwaggerRootURL"].ToString();
#endif

            return s;

        }
    }
}