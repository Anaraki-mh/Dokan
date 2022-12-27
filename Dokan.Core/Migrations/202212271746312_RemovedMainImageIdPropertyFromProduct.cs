namespace Dokan.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemovedMainImageIdPropertyFromProduct : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Products", "MainImageId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Products", "MainImageId", c => c.Int(nullable: false));
        }
    }
}
