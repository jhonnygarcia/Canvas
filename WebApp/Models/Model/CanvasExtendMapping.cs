using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using WebApp.Models.Model.Entity;

namespace WebApp.Models.Model
{
    public class AccountExtendMapping : EntityTypeConfiguration<AccountExtend>
    {
        public AccountExtendMapping()
        {
            HasKey(p => p.AccountId);

            HasRequired(p => p.Estudio)
                .WithMany()
                .HasForeignKey(p => p.IdEstudio);

            HasMany(p => p.Asignaturas)
                .WithMany(p => p.AccountExtends)
                .Map(cs =>
                {
                    cs.MapLeftKey("AccountId");
                    cs.MapRightKey("IdAsignatura");
                    cs.ToTable("AccountAsignaturas", "extend");
                });

            HasMany(p => p.PeriodoActivos)
                .WithMany(p => p.AccountExtends)
                .Map(cs =>
                {
                    cs.MapLeftKey("AccountId");
                    cs.MapRightKey("IdPeriodoActivo");
                    cs.ToTable("AccountPeriodoActivos", "extend");
                });

            HasMany(p => p.Courses)
                .WithRequired(p => p.Account)
                .HasForeignKey(p => p.AccountId);

            ToTable("extend.Accounts");
            Property(p => p.AccountId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
        }
    }
    public class AsignaturaMapping : EntityTypeConfiguration<Asignatura>
    {
        public AsignaturaMapping()
        {
            HasKey(p => p.Id);
            HasMany(p => p.AccountExtends)
                .WithMany(p => p.Asignaturas)
                .Map(cs =>
                {
                    cs.MapLeftKey("IdAsignatura");
                    cs.MapRightKey("AccountId");
                    cs.ToTable("AccountAsignaturas", "extend");
                });
            ToTable("extend.Asignaturas");
            Property(p => p.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
        }
    }
    public class PeriodoActivoMapping : EntityTypeConfiguration<PeriodoActivo>
    {
        public PeriodoActivoMapping()
        {
            HasKey(p => p.Id);
            HasMany(p => p.AccountExtends)
                .WithMany(p => p.PeriodoActivos)
                .Map(cs =>
                {
                    cs.MapLeftKey("IdPeriodoActivo");
                    cs.MapRightKey("AccountId");
                    cs.ToTable("AccountPeriodoActivos", "extend");
                });

            ToTable("extend.PeriodoActivos");
            Property(p => p.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
        }
    }
    public class CourseExtendMapping : EntityTypeConfiguration<CourseExtend>
    {
        public CourseExtendMapping()
        {
            HasKey(p => p.CourseId);

            HasRequired(p => p.Asignatura)
                .WithMany()
                .HasForeignKey(p => p.IdAsignatura);

            HasRequired(p => p.Account)
                .WithMany(p => p.Courses)
                .HasForeignKey(p => p.AccountId);

            ToTable("extend.Courses");
            Property(p => p.CourseId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
        }
    }
    public class EstudioMapping : EntityTypeConfiguration<Estudio>
    {
        public EstudioMapping()
        {
            HasKey(p => p.Id);
            ToTable("extend.Estudios");
            Property(p => p.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
        }
    }
    public class AccountGenerateMapping : EntityTypeConfiguration<AccountGenerate>
    {
        public AccountGenerateMapping()
        {
            HasKey(p => p.Id);
            ToTable("extend.AccountGenerates");

            HasRequired(p => p.Account)
                .WithMany()
                .HasForeignKey(p => p.AccountId)
                .WillCascadeOnDelete(false);

            HasRequired(p => p.Estudio)
                .WithMany()
                .HasForeignKey(p => p.IdEstudio)
                .WillCascadeOnDelete(false);

            HasRequired(p => p.PeriodoActivo)
                .WithMany()
                .HasForeignKey(p => p.IdPeriodoActivo)
                .WillCascadeOnDelete(false);

            HasMany(p => p.PeriodosNoLectivos)
                .WithRequired(p => p.AccountGenerate)
                .HasForeignKey(p => p.AccountGenerateId);

            HasMany(p => p.CoursesGenerates)
                .WithRequired(p => p.Account)
                .HasForeignKey(p => p.AccountId);

            HasMany(p => p.Migrations)
                .WithRequired(p => p.Generate)
                .HasForeignKey(p => p.GenerateId);

            Property(p => p.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
        }
    }
    public class LogMapping : EntityTypeConfiguration<Log>
    {
        public LogMapping()
        {
            HasKey(p => p.Id);
            ToTable("extend.Log");
            Property(p => p.Level).HasMaxLength(100);
            Property(p => p.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
        }
    }
    public class PeriodoNoLectivoMapping : EntityTypeConfiguration<PeriodoNoLectivo>
    {
        public PeriodoNoLectivoMapping()
        {
            HasKey(p => p.Id);

            ToTable("extend.PeriodosNoLectivos");

            HasRequired(p => p.AccountGenerate)
                .WithMany(p => p.PeriodosNoLectivos)
                .HasForeignKey(p => p.AccountGenerateId);

            Property(p => p.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
        }
    }
    public class CourseGenerateMapping: EntityTypeConfiguration<CourseGenerate>
    {
        public CourseGenerateMapping()
        {
            HasKey(p => p.Id);
            Property(p => p.SisId).HasMaxLength(100);
            HasRequired(p => p.Account)
                .WithMany(p => p.CoursesGenerates)
                .HasForeignKey(p => p.AccountId);

            ToTable("extend.CourseGenerates");
            Property(p => p.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
        }
    }

    public class ProgressInfoMapping : EntityTypeConfiguration<ProgressInfo>
    {
        public ProgressInfoMapping()
        {
            HasKey(p => p.Id);
            ToTable("extend.ProgressInfo");
            Property(p => p.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
        }
    }
    public class MigrationToCanvasMapping : EntityTypeConfiguration<MigrationToCanvas>
    {
        public MigrationToCanvasMapping()
        {
            HasKey(p => p.Id);

            HasRequired(p => p.Generate)
                .WithMany(p => p.Migrations)
                .HasForeignKey(p => p.GenerateId);

            ToTable("extend.MigrationToCanvas");
            Property(p => p.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
        }
    }
}