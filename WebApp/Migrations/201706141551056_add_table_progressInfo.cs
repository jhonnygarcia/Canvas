namespace WebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_table_progressInfo : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "extend.ProgressInfo",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        Completion = c.Int(nullable: false),
                        Message = c.String(),
                        State = c.String(),
                        Exception = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("extend.ProgressInfo");
        }
    }
}
