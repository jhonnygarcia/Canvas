namespace WebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class recreate_tables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "extend.Accounts",
                c => new
                    {
                        AccountId = c.Int(nullable: false),
                        Name = c.String(),
                        IdEstudio = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.AccountId)
                .ForeignKey("extend.Estudios", t => t.IdEstudio, cascadeDelete: true)
                .Index(t => t.IdEstudio);
            
            CreateTable(
                "extend.Asignaturas",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Nombre = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "extend.Courses",
                c => new
                    {
                        CourseId = c.Int(nullable: false),
                        Name = c.String(),
                        AccountId = c.Int(nullable: false),
                        IdAsignatura = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CourseId)
                .ForeignKey("extend.Asignaturas", t => t.IdAsignatura, cascadeDelete: true)
                .ForeignKey("extend.Accounts", t => t.AccountId, cascadeDelete: true)
                .Index(t => t.AccountId)
                .Index(t => t.IdAsignatura);
            
            CreateTable(
                "extend.Estudios",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Nombre = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "extend.PeriodoActivos",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Nombre = c.String(),
                        FechaInicio = c.DateTime(nullable: false),
                        FechaFin = c.DateTime(nullable: false),
                        AnioAcademico = c.String(),
                        NroPeriodo = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "extend.AccountAsignaturas",
                c => new
                    {
                        AccountId = c.Int(nullable: false),
                        IdAsignatura = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.AccountId, t.IdAsignatura })
                .ForeignKey("extend.Accounts", t => t.AccountId, cascadeDelete: true)
                .ForeignKey("extend.Asignaturas", t => t.IdAsignatura, cascadeDelete: true)
                .Index(t => t.AccountId)
                .Index(t => t.IdAsignatura);
            
            CreateTable(
                "extend.AccountPeriodoActivos",
                c => new
                    {
                        AccountId = c.Int(nullable: false),
                        IdPeriodoActivo = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.AccountId, t.IdPeriodoActivo })
                .ForeignKey("extend.Accounts", t => t.AccountId, cascadeDelete: true)
                .ForeignKey("extend.PeriodoActivos", t => t.IdPeriodoActivo, cascadeDelete: true)
                .Index(t => t.AccountId)
                .Index(t => t.IdPeriodoActivo);
            
        }
        
        public override void Down()
        {
            DropForeignKey("extend.AccountPeriodoActivos", "IdPeriodoActivo", "extend.PeriodoActivos");
            DropForeignKey("extend.AccountPeriodoActivos", "AccountId", "extend.Accounts");
            DropForeignKey("extend.Accounts", "IdEstudio", "extend.Estudios");
            DropForeignKey("extend.Courses", "AccountId", "extend.Accounts");
            DropForeignKey("extend.Courses", "IdAsignatura", "extend.Asignaturas");
            DropForeignKey("extend.AccountAsignaturas", "IdAsignatura", "extend.Asignaturas");
            DropForeignKey("extend.AccountAsignaturas", "AccountId", "extend.Accounts");
            DropIndex("extend.AccountPeriodoActivos", new[] { "IdPeriodoActivo" });
            DropIndex("extend.AccountPeriodoActivos", new[] { "AccountId" });
            DropIndex("extend.AccountAsignaturas", new[] { "IdAsignatura" });
            DropIndex("extend.AccountAsignaturas", new[] { "AccountId" });
            DropIndex("extend.Courses", new[] { "IdAsignatura" });
            DropIndex("extend.Courses", new[] { "AccountId" });
            DropIndex("extend.Accounts", new[] { "IdEstudio" });
            DropTable("extend.AccountPeriodoActivos");
            DropTable("extend.AccountAsignaturas");
            DropTable("extend.PeriodoActivos");
            DropTable("extend.Estudios");
            DropTable("extend.Courses");
            DropTable("extend.Asignaturas");
            DropTable("extend.Accounts");
        }
    }
}
