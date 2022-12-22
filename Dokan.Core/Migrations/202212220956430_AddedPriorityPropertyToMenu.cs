namespace Dokan.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedPriorityPropertyToMenu : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Menus", "Priority", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Menus", "Priority");
        }
    }
}
