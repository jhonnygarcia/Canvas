using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;
using WebApp.Models.Model.Entity;

namespace WebApp.Models.Model
{
    public class CanvasExtendContenxt : DbContext
    {
        public CanvasExtendContenxt()
            : base("canvas")
        {
            
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Conventions.Remove(new PluralizingEntitySetNameConvention());
            modelBuilder.Conventions.Remove(new PluralizingTableNameConvention());

            modelBuilder.Configurations.Add(new AccountExtendMapping());
            modelBuilder.Configurations.Add(new CourseExtendMapping());
            modelBuilder.Configurations.Add(new AsignaturaMapping());
            modelBuilder.Configurations.Add(new PeriodoActivoMapping());
            modelBuilder.Configurations.Add(new EstudioMapping());
            modelBuilder.Configurations.Add(new AccountGenerateMapping());
            modelBuilder.Configurations.Add(new LogMapping());
            modelBuilder.Configurations.Add(new PeriodoNoLectivoMapping());
            modelBuilder.Configurations.Add(new CourseGenerateMapping());
            modelBuilder.Configurations.Add(new ProgressInfoMapping());
            modelBuilder.Configurations.Add(new MigrationToCanvasMapping());
        }
        public DbSet<AccountExtend> AccountExtends { get; set; }
        public DbSet<CourseExtend> CourseExtends { get; set; }
        public DbSet<Asignatura> Asignaturas { get; set; }
        public DbSet<PeriodoActivo> PeriodosActivos { get; set; }
        public DbSet<Estudio> Estudios { get; set; }
        public DbSet<AccountGenerate> AccountGenerates { get; set; }
        public DbSet<Log> Log { get; set; }
        public DbSet<PeriodoNoLectivo> PeriodosNoLectivos { get; set; }
        public DbSet<CourseGenerate> CourseGenerates { get; set; }
        public DbSet<ProgressInfo> ProgressInfo { get; set; }
        public DbSet<MigrationToCanvas> MigrationToCanvas { get; set; }
        public void SaveChangeDetach()
        {
            var objectContext = ((IObjectContextAdapter)this).ObjectContext;
            var objectStateEntries = objectContext
                .ObjectStateManager
                .GetObjectStateEntries(EntityState.Added);

            SaveChanges();

            foreach (var objectStateEntry in objectStateEntries)
            {
                objectContext.Detach(objectStateEntry.Entity);
            }
        }
    }
}