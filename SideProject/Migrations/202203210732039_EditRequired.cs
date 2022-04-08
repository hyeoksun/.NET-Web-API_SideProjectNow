namespace SideProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EditRequired : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Members", "Gender", c => c.String(nullable: false, maxLength: 20));
            AlterColumn("dbo.Members", "ProfilePicture", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Members", "ProfilePicture", c => c.String());
            AlterColumn("dbo.Members", "Gender", c => c.String(maxLength: 20));
        }
    }
}
