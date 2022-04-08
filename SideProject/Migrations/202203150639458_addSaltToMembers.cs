namespace SideProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addSaltToMembers : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Members", "PasswordSalt", c => c.String(nullable: false));
            AlterColumn("dbo.Members", "Skills", c => c.String(maxLength: 30));
            AlterColumn("dbo.Members", "JobDescription", c => c.String(maxLength: 1000));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Members", "JobDescription", c => c.String(nullable: false, maxLength: 1000));
            AlterColumn("dbo.Members", "Skills", c => c.String(nullable: false, maxLength: 30));
            DropColumn("dbo.Members", "PasswordSalt");
        }
    }
}
