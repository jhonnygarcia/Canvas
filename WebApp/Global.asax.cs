using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using WebApp.Bundles;
using WebApp.Models.Model;

namespace WebApp
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            BundlesConfig.RegisterBundles(BundleTable.Bundles);
        
            AutofacConfig.Configure();

            RouteTable.Routes.MapMvcAttributeRoutes();
            RouteTable.Routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Cliente", action = "Create", id = UrlParameter.Optional }
            );

            Database.SetInitializer<CanvasExtendContenxt>(null);

            log4net.Config.XmlConfigurator.Configure();
        }
    }
}
