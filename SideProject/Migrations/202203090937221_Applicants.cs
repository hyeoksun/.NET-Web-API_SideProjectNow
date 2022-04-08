namespace SideProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Applicants : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Applicants",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ApplicantState = c.String(nullable: false, maxLength: 5),
                        MembersId = c.Int(nullable: false),
                        ProjectsId = c.Int(nullable: false),
                        InitDate = c.DateTime(nullable: false, defaultValueSql: "GETDATE()"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Projects", t => t.ProjectsId, cascadeDelete: true)
                .Index(t => t.ProjectsId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Applicants", "ProjectsId", "dbo.Projects");
            DropIndex("dbo.Applicants", new[] { "ProjectsId" });
            DropTable("dbo.Applicants");
        }
    }
}
