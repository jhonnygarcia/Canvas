namespace WebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_columns_AccountsGenerate : DbMigration
    {
        public override void Up()
        {
            AddColumn("extend.AccountGenerates", "NombrePeriodoMatriculacion", c => c.String());
            AddColumn("extend.AccountGenerates", "AnioAcademico", c => c.String());
            AddColumn("extend.AccountGenerates", "FechaInicio", c => c.DateTime(nullable: false));
            AddColumn("extend.AccountGenerates", "FechaFin", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("extend.AccountGenerates", "FechaFin");
            DropColumn("extend.AccountGenerates", "FechaInicio");
            DropColumn("extend.AccountGenerates", "AnioAcademico");
            DropColumn("extend.AccountGenerates", "NombrePeriodoMatriculacion");
        }
    }
}
