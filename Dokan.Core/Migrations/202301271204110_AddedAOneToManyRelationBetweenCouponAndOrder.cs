namespace Dokan.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedAOneToManyRelationBetweenCouponAndOrder : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.CouponProductCategories", newName: "ProductCategoryCoupons");
            DropPrimaryKey("dbo.ProductCategoryCoupons");
            AddColumn("dbo.Orders", "CouponId", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.ProductCategoryCoupons", new[] { "ProductCategory_Id", "Coupon_Id" });
            CreateIndex("dbo.Orders", "CouponId");
            AddForeignKey("dbo.Orders", "CouponId", "dbo.Coupons", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Orders", "CouponId", "dbo.Coupons");
            DropIndex("dbo.Orders", new[] { "CouponId" });
            DropPrimaryKey("dbo.ProductCategoryCoupons");
            DropColumn("dbo.Orders", "CouponId");
            AddPrimaryKey("dbo.ProductCategoryCoupons", new[] { "Coupon_Id", "ProductCategory_Id" });
            RenameTable(name: "dbo.ProductCategoryCoupons", newName: "CouponProductCategories");
        }
    }
}
