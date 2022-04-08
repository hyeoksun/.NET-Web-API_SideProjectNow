namespace SideProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProjectAndClassCol : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Projects", "ProjectTypeId", c => c.Int(nullable: false));
            CreateIndex("dbo.Projects", "ProjectTypeId");
            AddForeignKey("dbo.Projects", "ProjectTypeId", "dbo.ProjectClasses", "Id", cascadeDelete: true);
            DropColumn("dbo.Projects", "ProjectType");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Projects", "ProjectType", c => c.String(nullable: false, maxLength: 10));
            DropForeignKey("dbo.Projects", "ProjectTypeId", "dbo.ProjectClasses");
            DropIndex("dbo.Projects", new[] { "ProjectTypeId" });
            DropColumn("dbo.Projects", "ProjectTypeId");
        }
    }
}
