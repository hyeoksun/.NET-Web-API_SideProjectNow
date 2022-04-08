namespace SideProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PasswordLength : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Members", "Password", c => c.String(nullable: false, maxLength: 100));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Members", "Password", c => c.String(nullable: false, maxLength: 10));
        }
    }
}
