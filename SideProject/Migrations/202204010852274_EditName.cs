namespace SideProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EditName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UpdateProjectStates", "UpdateTime", c => c.DateTime(nullable: false));
            DropColumn("dbo.UpdateProjectStates", "ApplicantState");
        }
        
        public override void Down()
        {
            AddColumn("dbo.UpdateProjectStates", "ApplicantState", c => c.DateTime(nullable: false));
            DropColumn("dbo.UpdateProjectStates", "UpdateTime");
        }
    }
}
