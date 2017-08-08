namespace WebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_table_courseGenerates : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "extend.CourseGenerates",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Name = c.String(),
                        AccountId = c.Int(nullable: false),
                        SisId = c.String(maxLength: 100),
                        Inicio = c.DateTime(nullable: false),
                        Fin = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("extend.AccountGenerates", t => t.AccountId, cascadeDelete: true)
                .Index(t => t.AccountId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("extend.CourseGenerates", "AccountId", "extend.AccountGenerates");
            DropIndex("extend.CourseGenerates", new[] { "AccountId" });
            DropTable("extend.CourseGenerates");
        }
    }
}
