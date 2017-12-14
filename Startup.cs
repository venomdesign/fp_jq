using Microsoft.Owin;
using Owin;
using System.Configuration;
using System.Web.Cors;
using Microsoft.Owin.Cors;
using System.Threading.Tasks;
using netApi.Repositories.Authorization.Model;
using Microsoft.Owin.Security.OAuth;
using System;
using netApi.Repositories.Authorization.Providers;
using Microsoft.Owin.Security.DataHandler.Encoder;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Jwt;
using System.Web.Http;

[assembly: OwinStartup(typeof(NetEasyPay.Startup))]

namespace NetEasyPay
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var corsPolicy = new CorsPolicy();
            var allowedOrigins = ConfigurationManager.AppSettings["as:AllowedOrigins"].Split(';');
            foreach (var item in allowedOrigins)
            {
                corsPolicy.Origins.Add(item);
            }

            corsPolicy.AllowAnyHeader = true;
            corsPolicy.AllowAnyMethod = true;

            app.UseCors(
                new CorsOptions
                {
                    PolicyProvider = new CorsPolicyProvider
                    {
                        PolicyResolver = context => Task.FromResult(corsPolicy)
                    }
                }
                );

            var httpConfig = new HttpConfiguration();
            ConfigureOAuthTokenGeneration(app);
            ConfigureOAuthTokenConsumption(app);
        }


        private void ConfigureOAuthTokenGeneration(IAppBuilder app)
        {
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationRoleManager>(ApplicationRoleManager.Create);

            OAuthAuthorizationServerOptions OAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
                //TODO: For Dev enviroment only (on production should be AllowInsecureHttp = false)
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString(ConfigurationManager.AppSettings["as:oauthTokenEndpoint"]),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(1),
                Provider = new FopsOAuthProvider(),
                AccessTokenFormat = new FopsJwtFormat(ConfigurationManager.AppSettings["as:IssuerUrl"])
            };

            app.UseOAuthAuthorizationServer(OAuthServerOptions);
        }

        private void ConfigureOAuthTokenConsumption(IAppBuilder app)
        {
            var issuer = ConfigurationManager.AppSettings["as:IssuerUrl"];
            string audienceId = ConfigurationManager.AppSettings["as:AudienceId"];
            byte[] audienceSecret = TextEncodings.Base64Url.Decode(ConfigurationManager.AppSettings["as:AudienceSecret"]);

            app.UseJwtBearerAuthentication(
                new JwtBearerAuthenticationOptions
                {
                    AuthenticationMode = AuthenticationMode.Active,
                    AllowedAudiences = new[] { audienceId },
                    IssuerSecurityTokenProviders = new IIssuerSecurityTokenProvider[]
                    {
                        new SymmetricKeyIssuerSecurityTokenProvider(issuer, audienceSecret)
                    }
                });
        }
    }
}