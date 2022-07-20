namespace SideProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class projectMaxEdit : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Projects", "GroupPhoto", c => c.String());
            AlterColumn("dbo.Projects", "PartnerCondition", c => c.String(nullable: false));
            AlterColumn("dbo.Projects", "PartnerSkills", c => c.String(nullable: false));
            AlterColumn("dbo.Projects", "ProjectState", c => c.String(nullable: false, maxLength: 10));
            AlterColumn("dbo.Projects", "ProjectWebsite", c => c.String());
            AlterColumn("dbo.Projects", "ProjectBanner", c => c.String());
            AlterColumn("dbo.Projects", "ProjectPhotos", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Projects", "ProjectPhotos", c => c.String(maxLength: 100));
            AlterColumn("dbo.Projects", "ProjectBanner", c => c.String(maxLength: 30));
            AlterColumn("dbo.Projects", "ProjectWebsite", c => c.String(maxLength: 100));
            AlterColumn("dbo.Projects", "ProjectState", c => c.String(nullable: false, maxLength: 5));
            AlterColumn("dbo.Projects", "PartnerSkills", c => c.String(nullable: false, maxLength: 1000));
            AlterColumn("dbo.Projects", "PartnerCondition", c => c.String(nullable: false, maxLength: 1000));
            AlterColumn("dbo.Projects", "GroupPhoto", c => c.String(maxLength: 30));
        }
    }
}
