using System;
using System.Collections.Generic;
using System.Configuration;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Owin;
using WebApp;

[assembly: OwinStartup(typeof(Startup))]

namespace WebApp
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureOAuth(app);
        }

        public void ConfigureOAuth(IAppBuilder app)
        {
            // Permitir que la aplicación use una cookie para almacenar información para el usuario que inicia sesión
            // y una cookie para almacenar temporalmente información sobre un usuario que inicia sesión con un proveedor de inicio de sesión de terceros
            var options = new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie
            };

            app.UseCookieAuthentication(options);
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Configure la aplicación para el flujo basado en OAuth
            var oAuthOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/token"),
                Provider = new WebAppOAuthProvider(),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(14),
                AllowInsecureHttp = true
            };

            // Permitir que la aplicación use tokens portadores para autenticar usuarios
            app.UseOAuthBearerTokens(oAuthOptions);
        }
    }

    internal class WebAppOAuthProvider : OAuthAuthorizationServerProvider
    {
        private readonly string _publicClientId;

        public WebAppOAuthProvider()
        {
            _publicClientId = "self";
        }

        /// <summary>
        /// Sobreescritura de: <see cref="OAuthAuthorizationServerProvider.GrantResourceOwnerCredentials"/>
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });

            if (!WebAppAccount.ValidatePassword(context.UserName, context.Password))
            {
                context.SetError("invalid_grant", "El Usuario ó la contraseña es incorrecta");
                return;
            }

            var account = new WebAppAccount { UserName = context.UserName };
            var ticket = new AuthenticationTicket(account.GetUserIdentity(OAuthDefaults.AuthenticationType), new AuthenticationProperties());

            context.Validated(ticket);
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (var property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            // La credenciales de la contraseña del propietario del recurso no proporcionan un id. de cliente.
            if (context.ClientId == null)
            {
                context.Validated();
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        {
            if (context.ClientId == _publicClientId)
            {
                var expectedRootUri = new Uri(context.Request.Uri, "/");

                if (expectedRootUri.AbsoluteUri == context.RedirectUri)
                {
                    context.Validated();
                }
            }

            return Task.FromResult<object>(null);
        }

        public static AuthenticationProperties CreateProperties(string sessionId)
        {
            IDictionary<string, string> data = new Dictionary<string, string>
            {
                { "sessionId", sessionId }
            };
            return new AuthenticationProperties(data);
        }
    }

    internal class WebAppAccount : IUser<string>
    {
        public static bool ValidatePassword(string username, string password)
        {
            var authentication = ConfigurationManager.AppSettings["authentication"];
            var configUserName = authentication.Split(';')[0].Split(':')[1];
            var configPassword = authentication.Split(';')[1].Split(':')[1];

            var bytes = Convert.FromBase64String(configPassword);
            configPassword = System.Text.Encoding.UTF8.GetString(bytes);

            return configUserName.Trim().ToLower() == username.Trim().ToLower()
                    && configPassword == password;
        }

        public string Id { get; set; }

        public string UserName { get; set; }

        public ClaimsIdentity GetUserIdentity(string authenticationType)
        {
            var identity = new ClaimsIdentity(authenticationType);
            identity.AddClaim(new Claim("sub", UserName));
            identity.AddClaim(new Claim("role", "user"));

            return identity;
        }
    }
}