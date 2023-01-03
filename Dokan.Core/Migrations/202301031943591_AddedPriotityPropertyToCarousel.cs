namespace Dokan.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedPriotityPropertyToCarousel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Carousels", "Priority", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Carousels", "Priority");
        }
    }
}
