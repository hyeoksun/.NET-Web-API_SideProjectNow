namespace SideProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class projects : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Projects",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProjectName = c.String(nullable: false, maxLength: 100),
                        ProjectContext = c.String(nullable: false, maxLength: 1000),
                        GroupPhoto = c.String(maxLength: 30),
                        InitDate = c.DateTime(nullable: false),
                        GroupDeadline = c.DateTime(nullable: false),
                        FinishedDeadline = c.DateTime(nullable: false),
                        GroupNum = c.Int(nullable: false),
                        PartnerCondition = c.String(nullable: false, maxLength: 1000),
                        PartnerSkills = c.String(nullable: false, maxLength: 1000),
                        ProjectType = c.String(nullable: false, maxLength: 10),
                        ProjectState = c.String(nullable: false, maxLength: 5),
                        ProjectWebsite = c.String(maxLength: 100),
                        ProjectBanner = c.String(maxLength: 30),
                        ProjectPhotos = c.String(maxLength: 100),
                        ProjectExperience = c.String(maxLength: 1000),
                        MembersId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Members", t => t.MembersId, cascadeDelete: true)
                .Index(t => t.MembersId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Projects", "MembersId", "dbo.Members");
            DropIndex("dbo.Projects", new[] { "MembersId" });
            DropTable("dbo.Projects");
        }
    }
}
