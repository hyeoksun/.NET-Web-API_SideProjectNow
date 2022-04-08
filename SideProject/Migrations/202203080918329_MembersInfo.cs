namespace SideProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MembersInfo : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Members",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Account = c.String(nullable: false, maxLength: 30),
                        Password = c.String(nullable: false, maxLength: 10),
                        NickName = c.String(nullable: false, maxLength: 20),
                        Gender = c.String(maxLength: 5),
                        ProfilePicture = c.String(maxLength: 30),
                        Ig = c.String(maxLength: 100),
                        Fb = c.String(maxLength: 100),
                        ProfileWebsite = c.String(maxLength: 100),
                        ContactTime = c.String(maxLength: 20),
                        SelfIntroduction = c.String(maxLength: 1000),
                        WorkState = c.String(maxLength: 10),
                        Language = c.String(maxLength: 50),
                        Company = c.String(maxLength: 30),
                        Industry = c.String(maxLength: 20),
                        Position = c.String(maxLength: 10),
                        Skills = c.String(nullable: false, maxLength: 30),
                        JobDescription = c.String(nullable: false, maxLength: 1000),
                        InitDate = c.DateTime(nullable: false, defaultValueSql: "GETDATE()"),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Members");
        }
    }
}
