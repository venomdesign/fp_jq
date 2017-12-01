using System.Web.Http;
using WebActivatorEx;
using NetEasyPay;
using Swashbuckle.Application;
using System;
using System.Linq;

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

                        var schemes = new string[] { "http", "https" };
                        c.Schemes(schemes);
                        c.IgnoreObsoleteActions();
                        c.IncludeXmlComments(string.Format(@"{0}Models\EasyPayApi.xml", AppDomain.CurrentDomain.BaseDirectory));
                    })
                .EnableSwaggerUi(c =>
                    {
                        c.DocumentTitle("FNF EasyPay API");
                    });
        }
    }
}
