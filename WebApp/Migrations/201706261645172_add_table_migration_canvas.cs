namespace WebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_table_migration_canvas : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "extend.MigrationToCanvas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        GenerateId = c.Int(nullable: false),
                        Inicio = c.DateTime(nullable: false),
                        Fin = c.DateTime(nullable: false),
                        Data = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("extend.AccountGenerates", t => t.GenerateId, cascadeDelete: true)
                .Index(t => t.GenerateId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("extend.MigrationToCanvas", "GenerateId", "extend.AccountGenerates");
            DropIndex("extend.MigrationToCanvas", new[] { "GenerateId" });
            DropTable("extend.MigrationToCanvas");
        }
    }
}
