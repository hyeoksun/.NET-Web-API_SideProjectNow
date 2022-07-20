namespace SideProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class projectSkillTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProjectSkills",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProjectId = c.Int(nullable: false),
                        SkillId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Projects", t => t.ProjectId, cascadeDelete: true)
                .Index(t => t.ProjectId);
            
            AddColumn("dbo.Skills", "ProjectSkills_Id", c => c.Int());
            CreateIndex("dbo.Skills", "ProjectSkills_Id");
            AddForeignKey("dbo.Skills", "ProjectSkills_Id", "dbo.ProjectSkills", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Skills", "ProjectSkills_Id", "dbo.ProjectSkills");
            DropForeignKey("dbo.ProjectSkills", "ProjectId", "dbo.Projects");
            DropIndex("dbo.Skills", new[] { "ProjectSkills_Id" });
            DropIndex("dbo.ProjectSkills", new[] { "ProjectId" });
            DropColumn("dbo.Skills", "ProjectSkills_Id");
            DropTable("dbo.ProjectSkills");
        }
    }
}
