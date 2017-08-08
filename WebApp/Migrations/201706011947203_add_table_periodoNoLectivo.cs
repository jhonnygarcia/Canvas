namespace WebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_table_periodoNoLectivo : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "extend.PeriodosNoLectivos",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Inicio = c.DateTime(nullable: false),
                        Fin = c.DateTime(nullable: false),
                        AccountGenerateId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("extend.AccountGenerates", t => t.AccountGenerateId, cascadeDelete: true)
                .Index(t => t.AccountGenerateId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("extend.PeriodosNoLectivos", "AccountGenerateId", "extend.AccountGenerates");
            DropIndex("extend.PeriodosNoLectivos", new[] { "AccountGenerateId" });
            DropTable("extend.PeriodosNoLectivos");
        }
    }
}
