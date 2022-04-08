namespace SideProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProjectClass : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProjectClasses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProjectType = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ProjectClasses");
        }
    }
}
