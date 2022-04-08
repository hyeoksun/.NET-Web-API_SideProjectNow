namespace SideProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TestProfile : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Members", "ProfilePicture", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Members", "ProfilePicture", c => c.String(nullable: false));
        }
    }
}
