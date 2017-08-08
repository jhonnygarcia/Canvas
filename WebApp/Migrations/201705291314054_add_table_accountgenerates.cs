namespace WebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_table_accountgenerates : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "extend.AccountGenerates",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Name = c.String(),
                        AccountId = c.Int(nullable: false),
                        IdEstudio = c.Int(nullable: false),
                        IdPeriodoActivo = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("extend.Accounts", t => t.AccountId)
                .ForeignKey("extend.Estudios", t => t.IdEstudio)
                .ForeignKey("extend.PeriodoActivos", t => t.IdPeriodoActivo)
                .Index(t => t.AccountId)
                .Index(t => t.IdEstudio)
                .Index(t => t.IdPeriodoActivo);
            
        }
        
        public override void Down()
        {
            DropForeignKey("extend.AccountGenerates", "IdPeriodoActivo", "extend.PeriodoActivos");
            DropForeignKey("extend.AccountGenerates", "IdEstudio", "extend.Estudios");
            DropForeignKey("extend.AccountGenerates", "AccountId", "extend.Accounts");
            DropIndex("extend.AccountGenerates", new[] { "IdPeriodoActivo" });
            DropIndex("extend.AccountGenerates", new[] { "IdEstudio" });
            DropIndex("extend.AccountGenerates", new[] { "AccountId" });
            DropTable("extend.AccountGenerates");
        }
    }
}
