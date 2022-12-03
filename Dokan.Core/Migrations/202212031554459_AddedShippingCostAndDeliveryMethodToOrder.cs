namespace Dokan.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedShippingCostAndDeliveryMethodToOrder : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "ShippingCost", c => c.Double(nullable: false));
            AddColumn("dbo.Orders", "DeliveryMethod", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "DeliveryMethod");
            DropColumn("dbo.Orders", "ShippingCost");
        }
    }
}
