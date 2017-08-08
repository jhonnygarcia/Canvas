namespace WebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_table_log : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "extend.Log",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        Level = c.String(maxLength: 100),
                        Loger = c.String(),
                        Message = c.String(),
                        Exception = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("extend.Log");
        }
    }
}
