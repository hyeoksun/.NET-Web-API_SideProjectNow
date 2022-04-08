namespace SideProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EditCollection : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Collections", "InitDate", c => c.DateTime(nullable: false));
            CreateIndex("dbo.Collections", "MembersId");
            AddForeignKey("dbo.Collections", "MembersId", "dbo.Members", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Collections", "MembersId", "dbo.Members");
            DropIndex("dbo.Collections", new[] { "MembersId" });
            DropColumn("dbo.Collections", "InitDate");
        }
    }
}
