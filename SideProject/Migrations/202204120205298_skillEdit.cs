namespace SideProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class skillEdit : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Skills", "ProjectSkills_Id", "dbo.ProjectSkills");
            DropIndex("dbo.Skills", new[] { "ProjectSkills_Id" });
            DropColumn("dbo.Skills", "ProjectSkills_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Skills", "ProjectSkills_Id", c => c.Int());
            CreateIndex("dbo.Skills", "ProjectSkills_Id");
            AddForeignKey("dbo.Skills", "ProjectSkills_Id", "dbo.ProjectSkills", "Id");
        }
    }
}
