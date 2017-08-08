using System.Data.Entity.Migrations;
using WebApp.Models.Model;

namespace WebApp.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<CanvasExtendContenxt>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            CommandTimeout = 600;
        }

        protected override void Seed(CanvasExtendContenxt context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }
    }
}
