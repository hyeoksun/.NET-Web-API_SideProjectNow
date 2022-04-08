namespace SideProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EditMessage : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Messages", "MessageTitle", c => c.String(nullable: false));
            CreateIndex("dbo.Messages", "MembersId");
            AddForeignKey("dbo.Messages", "MembersId", "dbo.Members", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Messages", "MembersId", "dbo.Members");
            DropIndex("dbo.Messages", new[] { "MembersId" });
            DropColumn("dbo.Messages", "MessageTitle");
        }
    }
}
