namespace SideProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddApliTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Applicants", "ApplicantSelfIntro", c => c.String(nullable: false));
            AddColumn("dbo.Applicants", "ApplicantMessage", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Applicants", "ApplicantMessage");
            DropColumn("dbo.Applicants", "ApplicantSelfIntro");
        }
    }
}
