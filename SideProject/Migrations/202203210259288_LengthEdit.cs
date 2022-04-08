namespace SideProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LengthEdit : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Members", "Account", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.Members", "NickName", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.Members", "Gender", c => c.String(maxLength: 20));
            AlterColumn("dbo.Members", "ProfilePicture", c => c.String(nullable: false));
            AlterColumn("dbo.Members", "Ig", c => c.String(maxLength: 200));
            AlterColumn("dbo.Members", "Fb", c => c.String(maxLength: 200));
            AlterColumn("dbo.Members", "ProfileWebsite", c => c.String(maxLength: 200));
            AlterColumn("dbo.Members", "ContactTime", c => c.String(maxLength: 50));
            AlterColumn("dbo.Members", "WorkState", c => c.String(maxLength: 20));
            AlterColumn("dbo.Members", "Language", c => c.String(maxLength: 100));
            AlterColumn("dbo.Members", "Company", c => c.String(maxLength: 100));
            AlterColumn("dbo.Members", "Industry", c => c.String(maxLength: 100));
            AlterColumn("dbo.Members", "Position", c => c.String(maxLength: 100));
            AlterColumn("dbo.Members", "Skills", c => c.String());
            AlterColumn("dbo.Members", "InitDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Members", "InitDate", c => c.DateTime(nullable: false, defaultValueSql: "GETDATE()"));
            AlterColumn("dbo.Members", "Skills", c => c.String(maxLength: 30));
            AlterColumn("dbo.Members", "Position", c => c.String(maxLength: 10));
            AlterColumn("dbo.Members", "Industry", c => c.String(maxLength: 20));
            AlterColumn("dbo.Members", "Company", c => c.String(maxLength: 30));
            AlterColumn("dbo.Members", "Language", c => c.String(maxLength: 50));
            AlterColumn("dbo.Members", "WorkState", c => c.String(maxLength: 10));
            AlterColumn("dbo.Members", "ContactTime", c => c.String(maxLength: 20));
            AlterColumn("dbo.Members", "ProfileWebsite", c => c.String(maxLength: 100));
            AlterColumn("dbo.Members", "Fb", c => c.String(maxLength: 100));
            AlterColumn("dbo.Members", "Ig", c => c.String(maxLength: 100));
            AlterColumn("dbo.Members", "ProfilePicture", c => c.String(maxLength: 30));
            AlterColumn("dbo.Members", "Gender", c => c.String(maxLength: 5));
            AlterColumn("dbo.Members", "NickName", c => c.String(nullable: false, maxLength: 20));
            AlterColumn("dbo.Members", "Account", c => c.String(nullable: false, maxLength: 30));
        }
    }
}
