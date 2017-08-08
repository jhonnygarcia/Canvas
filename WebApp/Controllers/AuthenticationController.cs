using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using LtiLibrary.AspNet.Extensions;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using WebApp.Models.Model;
using Microsoft.Owin.Host.SystemWeb;

namespace WebApp.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly CanvasExtendContenxt _context;
        public AuthenticationController(CanvasExtendContenxt context)
        {
            _context = context;
        }
        private enum CodigoErrorLti
        {
            AuthenticationPublic = -1, //Error de autenticaicion con llave publica
            AuthenticationPrivate = -2, //Error de autenticaicion con llave privada
            NoSendAccontId = 101,  //No se envio el Id de la cuenta
        }
        [HttpPost]
        public ActionResult LoginExternal()
        {
            /*
             * Llave publica = 12345678900
             * Llave privada = 98765432100
             * 
             * Para la verificación de códigos generados se recibe en [Request.Params["oauth_signature"]] el codigo generado por el LMS 
             * Se obtiene un nuevo código a partir de la request que debe ser igual al recibido para pasar la validación  
             * Request.GenerateOAuthSignature("98765432100")
             */
            const string messageGeneric = "Codigo({0}): Ha ocurrido un error inesperado, por favor contacte con Soporte de Aplicaciones aplicaciones@unir.net enviando el codigo del error";

            var keyPublic = ConfigurationManager.AppSettings["KEY_LTI_PUBLIC"];
            var keyPrivate = ConfigurationManager.AppSettings["KEY_LTI_PRIVATE"];
            if (!Request.Params["oauth_consumer_key"].Equals(keyPublic))
            {
                return View("Error", null, string.Format(messageGeneric, (int)CodigoErrorLti.AuthenticationPublic));
            }
            var codigoRecibido = Request.Params["oauth_signature"];
            var codigoGenerado = Request.GenerateOAuthSignature(keyPrivate);

            if (!codigoRecibido.Equals(codigoGenerado))
            {
                return View("Error", null, string.Format(messageGeneric, (int)CodigoErrorLti.AuthenticationPrivate) + " - " + codigoGenerado);
            }

            if (Request.Params["custom_canvas_account_id"] == "" || Request.Params["custom_canvas_account_id"] == null)
            {
                return View("Error", null, string.Format(messageGeneric, (int)CodigoErrorLti.NoSendAccontId));
            }

            var values = new RouteValueDictionary
            {
                {"accountId", Request.Params["custom_canvas_account_id"]}
            };

            var account = new WebAppAccount { UserName = "lti-migrate-canvas" };
            HttpContext.GetOwinContext()
                       .Authentication
                       .SignIn(new AuthenticationProperties
                       {
                           ExpiresUtc = DateTime.UtcNow.AddMinutes(120)
                       }, account.GetUserIdentity(DefaultAuthenticationTypes.ApplicationCookie));

            return RedirectToAction("Index", "Account", values);
        }
        public ActionResult Error(string message)
        {
            return View("Error", null, message);
        }
    }
}