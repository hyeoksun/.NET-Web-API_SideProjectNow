namespace SideProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateUpdateState : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UpdateProjectStates",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ApplicantState = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.UpdateProjectStates");
        }
    }
}
