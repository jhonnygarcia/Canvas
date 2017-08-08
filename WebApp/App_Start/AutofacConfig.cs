using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Routing;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.WebApi;
using Newtonsoft.Json;
using WebApp.Architecture.ApiControllers;
using Autofac.Integration.Mvc;
using DataCacheServices.AppDataCache;
using DataCacheServices.AppDataCache.ConfigSection;
using WebApp.App_Start;
using WebApp.Architecture;
using WebApp.Common.WepApi;
using WebApp.Models.Model;
using WebApp.Models.Services;

namespace WebApp
{
    public static class AutofacConfig
    {
        public static IContainer Container { get; set; }
        public static void Configure()
        {
            var builder = BuildGlobalContainer();
            Container = builder.Container;
            // Configurar WebApi
            GlobalConfiguration.Configure(config => WebApiConfig(config, Container));
            // Configurar Mvc
            MvcConfig(Container);
        }

        private static void RegisterExtraReferences(ContainerBuilder builder)
        {
            builder.RegisterType<CanvasExtendContenxt>().AsSelf().InstancePerLifetimeScope();

            //-> Servicios
            builder.RegisterAssemblyTypes(typeof(ICanvasAppServices).Assembly)
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope()
                .Where(t => t.Name.EndsWith("AppServices"))
                .PropertiesAutowired();

            builder.RegisterAssemblyTypes(typeof(ICanvasAppServices).Assembly)
                .InstancePerLifetimeScope()
                .Where(t => t.Name.EndsWith("AppServices"))
                .PropertiesAutowired();


            builder.RegisterAssemblyTypes(typeof(IAccountsApiCanvas).Assembly)
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope()
                .Where(t => t.Name.EndsWith("ApiCanvas"))
                .PropertiesAutowired();

            builder.RegisterAssemblyTypes(typeof(IAccountsApiCanvas).Assembly)
                .InstancePerLifetimeScope()
                .Where(t => t.Name.EndsWith("ApiCanvas"))
                .PropertiesAutowired();

            builder.RegisterType<EstudiosClient>().As<IEstudios>()
                   .InstancePerLifetimeScope().PropertiesAutowired();

            builder.RegisterType<JobService>().As<IJobService>()
                .SingleInstance();

            // Gestión almacenamiento Global por Aplicación de Datos Serializables
            var dataCacheConfig = ConfigurationManager.GetSection("dataCacheManager") as DataCacheConfigSection;
            if (dataCacheConfig != null)
            {
                if (string.IsNullOrEmpty(dataCacheConfig.Impl))
                {
                    throw new ConfigurationErrorsException("Debe prover una implementación para el Componente de Almacenamiento en Caché. Especifique un Nombre Calificado en el atributo: 'impl'");
                }
                var dataCacheType = Type.GetType(dataCacheConfig.Impl);
                if (dataCacheType == null)
                {
                    throw new ConfigurationErrorsException("No se puede contrar un tipo con el nombre calificado: " + dataCacheConfig.Impl + ". Revise el valor del atributo 'impl' del elemento <dataCacheManager>");
                }
                builder.RegisterType(dataCacheType).As<IDataCacheService>().SingleInstance();
            }
            else
            {
                throw new ConfigurationErrorsException("Falta la section: 'dataCacheManager'");
            }

        }
        private static IocContainer BuildGlobalContainer()
        {
            // Registrar Referencias de Container
            Action<ContainerBuilder> registerAction = builder =>
            {
                // 1.WebAPI
                builder.RegisterApiControllers(AppDomain.CurrentDomain.GetAssemblies())
                    .AssignableTo<ApiControllerBase>()
                    .PropertiesAutowired();

                builder.RegisterWebApiFilterProvider(GlobalConfiguration.Configuration);

                // 2.MVC5
                builder.RegisterSource(new ViewRegistrationSource());
                builder.RegisterFilterProvider();

                builder.RegisterControllers(AppDomain.CurrentDomain.GetAssemblies())
                    .AssignableTo<Controller>()
                    .PropertiesAutowired();

                // 3.Extras
                RegisterExtraReferences(builder);
            };
            // Construir Container
            return Activator.CreateInstance(typeof(IocContainer), registerAction) as IocContainer;
        }
        private static void MvcConfig(IContainer container)
        {
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
            AreaRegistration.RegisterAllAreas();
        }
        private static void WebApiConfig(HttpConfiguration config, IContainer container)
        {
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
            config.Formatters.JsonFormatter.SerializerSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            };
            config.MapHttpAttributeRoutes(new CustomDirectRouteProvider());
            config.Filters.Add(new ExceptionHandlingAttribute());
        }
        public class CustomDirectRouteProvider : DefaultDirectRouteProvider
        {
            protected override IReadOnlyList<IDirectRouteFactory>
            GetActionRouteFactories(HttpActionDescriptor actionDescriptor)
            {
                // inherit route attributes decorated on base class controller's actions
                return actionDescriptor.GetCustomAttributes<IDirectRouteFactory>(true);
            }
        }
    }
}