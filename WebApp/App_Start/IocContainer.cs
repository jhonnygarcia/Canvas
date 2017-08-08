using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Autofac;

namespace WebApp.App_Start
{
    public class IocContainer
    {
        public IContainer Container { get; set; }
        public IocContainer(Action<ContainerBuilder> builderAction)
        {
            var containerBuilder = new ContainerBuilder();
            builderAction.Invoke(containerBuilder);
            Container = containerBuilder.Build();
        }
    }
}