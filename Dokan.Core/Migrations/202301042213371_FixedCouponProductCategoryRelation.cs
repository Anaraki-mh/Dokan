namespace Dokan.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixedCouponProductCategoryRelation : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ProductCategories", "Coupon_Id", "dbo.Coupons");
            DropIndex("dbo.ProductCategories", new[] { "Coupon_Id" });
            CreateTable(
                "dbo.CouponProductCategories",
                c => new
                    {
                        Coupon_Id = c.Int(nullable: false),
                        ProductCategory_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Coupon_Id, t.ProductCategory_Id })
                .ForeignKey("dbo.Coupons", t => t.Coupon_Id, cascadeDelete: true)
                .ForeignKey("dbo.ProductCategories", t => t.ProductCategory_Id, cascadeDelete: true)
                .Index(t => t.Coupon_Id)
                .Index(t => t.ProductCategory_Id);
            
            DropColumn("dbo.ProductCategories", "Coupon_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ProductCategories", "Coupon_Id", c => c.Int());
            DropForeignKey("dbo.CouponProductCategories", "ProductCategory_Id", "dbo.ProductCategories");
            DropForeignKey("dbo.CouponProductCategories", "Coupon_Id", "dbo.Coupons");
            DropIndex("dbo.CouponProductCategories", new[] { "ProductCategory_Id" });
            DropIndex("dbo.CouponProductCategories", new[] { "Coupon_Id" });
            DropTable("dbo.CouponProductCategories");
            CreateIndex("dbo.ProductCategories", "Coupon_Id");
            AddForeignKey("dbo.ProductCategories", "Coupon_Id", "dbo.Coupons", "Id");
        }
    }
}
