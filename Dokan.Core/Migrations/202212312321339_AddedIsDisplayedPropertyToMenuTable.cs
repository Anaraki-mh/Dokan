namespace Dokan.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedIsDisplayedPropertyToMenuTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Menus", "IsDisplayed", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Menus", "IsDisplayed");
        }
    }
}
