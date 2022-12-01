namespace Dokan.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedBasicTables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BlogCategories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(maxLength: 40),
                        Priority = c.Int(nullable: false),
                        ParentId = c.Int(),
                        IsRemoved = c.Boolean(nullable: false),
                        CreateDateTime = c.DateTime(nullable: false),
                        UpdateDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BlogCategories", t => t.ParentId)
                .Index(t => t.ParentId);
            
            CreateTable(
                "dbo.BlogPosts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(maxLength: 75),
                        ShortDescription = c.String(maxLength: 75),
                        Content = c.String(),
                        BlogCategoryId = c.Int(nullable: false),
                        ImageId = c.Int(nullable: false),
                        IsRemoved = c.Boolean(nullable: false),
                        CreateDateTime = c.DateTime(nullable: false),
                        UpdateDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BlogCategories", t => t.BlogCategoryId, cascadeDelete: true)
                .ForeignKey("dbo.Files", t => t.ImageId, cascadeDelete: true)
                .Index(t => t.BlogCategoryId)
                .Index(t => t.ImageId);
            
            CreateTable(
                "dbo.BlogComments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(maxLength: 128),
                        BlogPostId = c.Int(nullable: false),
                        ParentId = c.Int(),
                        Title = c.String(maxLength: 40),
                        Body = c.String(maxLength: 300),
                        IsApproved = c.Boolean(nullable: false),
                        IsRemoved = c.Boolean(nullable: false),
                        CreateDateTime = c.DateTime(nullable: false),
                        UpdateDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BlogPosts", t => t.BlogPostId, cascadeDelete: true)
                .ForeignKey("dbo.BlogComments", t => t.ParentId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.BlogPostId)
                .Index(t => t.ParentId);
            
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CreateDateTime = c.DateTime(nullable: false),
                        TrackingCode = c.String(maxLength: 50),
                        OrderState = c.Int(nullable: false),
                        PaymentState = c.Int(nullable: false),
                        UserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.OrderItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Price = c.Double(nullable: false),
                        Discount = c.Int(nullable: false),
                        Tax = c.Int(nullable: false),
                        Quantity = c.Int(nullable: false),
                        Total = c.Double(nullable: false),
                        ProductId = c.Int(nullable: false),
                        OrderId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Orders", t => t.OrderId, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .Index(t => t.ProductId)
                .Index(t => t.OrderId);
            
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(maxLength: 40),
                        MainImageId = c.Int(nullable: false),
                        ShortDescription = c.String(maxLength: 40),
                        Description = c.String(maxLength: 3000),
                        Price = c.Double(nullable: false),
                        Stock = c.Int(nullable: false),
                        ProductCategoryId = c.Int(nullable: false),
                        PricingRuleId = c.Int(),
                        IsRemoved = c.Boolean(nullable: false),
                        CreateDateTime = c.DateTime(nullable: false),
                        UpdateDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ProductPricingRules", t => t.PricingRuleId)
                .ForeignKey("dbo.ProductCategories", t => t.ProductCategoryId, cascadeDelete: true)
                .Index(t => t.ProductCategoryId)
                .Index(t => t.PricingRuleId);
            
            CreateTable(
                "dbo.Files",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(maxLength: 50),
                        CreateDateTime = c.DateTime(nullable: false),
                        FileType = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Testimonials",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FullName = c.String(maxLength: 50),
                        Position = c.String(maxLength: 35),
                        Content = c.String(maxLength: 300),
                        ImageId = c.Int(nullable: false),
                        IsRemoved = c.Boolean(nullable: false),
                        CreateDateTime = c.DateTime(nullable: false),
                        UpdateDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Files", t => t.ImageId, cascadeDelete: true)
                .Index(t => t.ImageId);
            
            CreateTable(
                "dbo.ProductPricingRules",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(maxLength: 75),
                        IsActive = c.Boolean(nullable: false),
                        ExpiryDateTime = c.DateTime(nullable: false),
                        Tax = c.Int(nullable: false),
                        Discount = c.Int(nullable: false),
                        MultiplyPriceBy = c.Int(nullable: false),
                        IsRemoved = c.Boolean(nullable: false),
                        CreateDateTime = c.DateTime(nullable: false),
                        UpdateDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ProductCategories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(maxLength: 40),
                        Priority = c.Int(nullable: false),
                        ParentId = c.Int(),
                        PricingRuleId = c.Int(),
                        IsRemoved = c.Boolean(nullable: false),
                        CreateDateTime = c.DateTime(nullable: false),
                        UpdateDateTime = c.DateTime(nullable: false),
                        Coupon_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ProductCategories", t => t.ParentId)
                .ForeignKey("dbo.ProductCategoryPricingRules", t => t.PricingRuleId)
                .ForeignKey("dbo.Coupons", t => t.Coupon_Id)
                .Index(t => t.ParentId)
                .Index(t => t.PricingRuleId)
                .Index(t => t.Coupon_Id);
            
            CreateTable(
                "dbo.ProductCategoryPricingRules",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(maxLength: 75),
                        IsActive = c.Boolean(nullable: false),
                        ExpiryDateTime = c.DateTime(nullable: false),
                        Tax = c.Int(nullable: false),
                        Discount = c.Int(nullable: false),
                        MultiplyPriceBy = c.Int(nullable: false),
                        IsRemoved = c.Boolean(nullable: false),
                        CreateDateTime = c.DateTime(nullable: false),
                        UpdateDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ProductComments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(maxLength: 128),
                        ProductId = c.Int(nullable: false),
                        ParentId = c.Int(),
                        Title = c.String(maxLength: 40),
                        Body = c.String(maxLength: 300),
                        IsApproved = c.Boolean(nullable: false),
                        IsRemoved = c.Boolean(nullable: false),
                        CreateDateTime = c.DateTime(nullable: false),
                        UpdateDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ProductComments", t => t.ParentId)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.ProductId)
                .Index(t => t.ParentId);
            
            CreateTable(
                "dbo.Coupons",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(maxLength: 75),
                        Code = c.String(maxLength: 20),
                        IsActive = c.Boolean(nullable: false),
                        ExpiryDateTime = c.DateTime(),
                        Discount = c.Int(nullable: false),
                        UsageLimit = c.Int(nullable: false),
                        UsageCount = c.Int(nullable: false),
                        IsRemoved = c.Boolean(nullable: false),
                        CreateDateTime = c.DateTime(nullable: false),
                        UpdateDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Carousels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Display = c.Boolean(nullable: false),
                        Title = c.String(maxLength: 75),
                        Description = c.String(maxLength: 80),
                        ButtonOne = c.String(maxLength: 30),
                        LinkOne = c.String(maxLength: 40),
                        ButtonTwo = c.String(maxLength: 30),
                        LinkTwo = c.String(maxLength: 40),
                        TitleColor = c.String(maxLength: 8),
                        DescriptionColor = c.String(maxLength: 8),
                        ButtonOneBgColor = c.String(maxLength: 8),
                        ButtonOneFgColor = c.String(maxLength: 8),
                        ButtonTwoBgColor = c.String(maxLength: 8),
                        ButtonTwoFgColor = c.String(maxLength: 8),
                        ImageId = c.Int(nullable: false),
                        IsRemoved = c.Boolean(nullable: false),
                        CreateDateTime = c.DateTime(nullable: false),
                        UpdateDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Files", t => t.ImageId, cascadeDelete: true)
                .Index(t => t.ImageId);
            
            CreateTable(
                "dbo.KeyValueContents",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ContentKey = c.String(maxLength: 50),
                        ContentValue = c.String(maxLength: 1000),
                        Description = c.String(maxLength: 75),
                        ContentType = c.Int(nullable: false),
                        IsRemoved = c.Boolean(nullable: false),
                        CreateDateTime = c.DateTime(nullable: false),
                        UpdateDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Logs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LogType = c.Int(nullable: false),
                        Controller = c.String(maxLength: 100),
                        Method = c.String(maxLength: 100),
                        Description = c.String(maxLength: 500),
                        Code = c.String(maxLength: 50),
                        AdditionalInfo = c.String(maxLength: 500),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Menus",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(maxLength: 45),
                        Link = c.String(maxLength: 75),
                        ParentId = c.Int(),
                        IsRemoved = c.Boolean(nullable: false),
                        CreateDateTime = c.DateTime(nullable: false),
                        UpdateDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Menus", t => t.ParentId)
                .Index(t => t.ParentId);
            
            CreateTable(
                "dbo.FileProducts",
                c => new
                    {
                        File_Id = c.Int(nullable: false),
                        Product_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.File_Id, t.Product_Id })
                .ForeignKey("dbo.Files", t => t.File_Id, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.Product_Id, cascadeDelete: true)
                .Index(t => t.File_Id)
                .Index(t => t.Product_Id);
            
            CreateTable(
                "dbo.CouponUsers",
                c => new
                    {
                        Coupon_Id = c.Int(nullable: false),
                        User_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.Coupon_Id, t.User_Id })
                .ForeignKey("dbo.Coupons", t => t.Coupon_Id, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id, cascadeDelete: true)
                .Index(t => t.Coupon_Id)
                .Index(t => t.User_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Menus", "ParentId", "dbo.Menus");
            DropForeignKey("dbo.Carousels", "ImageId", "dbo.Files");
            DropForeignKey("dbo.BlogCategories", "ParentId", "dbo.BlogCategories");
            DropForeignKey("dbo.CouponUsers", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.CouponUsers", "Coupon_Id", "dbo.Coupons");
            DropForeignKey("dbo.ProductCategories", "Coupon_Id", "dbo.Coupons");
            DropForeignKey("dbo.Orders", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ProductComments", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ProductComments", "ProductId", "dbo.Products");
            DropForeignKey("dbo.ProductComments", "ParentId", "dbo.ProductComments");
            DropForeignKey("dbo.Products", "ProductCategoryId", "dbo.ProductCategories");
            DropForeignKey("dbo.ProductCategories", "PricingRuleId", "dbo.ProductCategoryPricingRules");
            DropForeignKey("dbo.ProductCategories", "ParentId", "dbo.ProductCategories");
            DropForeignKey("dbo.Products", "PricingRuleId", "dbo.ProductPricingRules");
            DropForeignKey("dbo.OrderItems", "ProductId", "dbo.Products");
            DropForeignKey("dbo.Testimonials", "ImageId", "dbo.Files");
            DropForeignKey("dbo.FileProducts", "Product_Id", "dbo.Products");
            DropForeignKey("dbo.FileProducts", "File_Id", "dbo.Files");
            DropForeignKey("dbo.BlogPosts", "ImageId", "dbo.Files");
            DropForeignKey("dbo.OrderItems", "OrderId", "dbo.Orders");
            DropForeignKey("dbo.BlogComments", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.BlogComments", "ParentId", "dbo.BlogComments");
            DropForeignKey("dbo.BlogComments", "BlogPostId", "dbo.BlogPosts");
            DropForeignKey("dbo.BlogPosts", "BlogCategoryId", "dbo.BlogCategories");
            DropIndex("dbo.CouponUsers", new[] { "User_Id" });
            DropIndex("dbo.CouponUsers", new[] { "Coupon_Id" });
            DropIndex("dbo.FileProducts", new[] { "Product_Id" });
            DropIndex("dbo.FileProducts", new[] { "File_Id" });
            DropIndex("dbo.Menus", new[] { "ParentId" });
            DropIndex("dbo.Carousels", new[] { "ImageId" });
            DropIndex("dbo.ProductComments", new[] { "ParentId" });
            DropIndex("dbo.ProductComments", new[] { "ProductId" });
            DropIndex("dbo.ProductComments", new[] { "UserId" });
            DropIndex("dbo.ProductCategories", new[] { "Coupon_Id" });
            DropIndex("dbo.ProductCategories", new[] { "PricingRuleId" });
            DropIndex("dbo.ProductCategories", new[] { "ParentId" });
            DropIndex("dbo.Testimonials", new[] { "ImageId" });
            DropIndex("dbo.Products", new[] { "PricingRuleId" });
            DropIndex("dbo.Products", new[] { "ProductCategoryId" });
            DropIndex("dbo.OrderItems", new[] { "OrderId" });
            DropIndex("dbo.OrderItems", new[] { "ProductId" });
            DropIndex("dbo.Orders", new[] { "UserId" });
            DropIndex("dbo.BlogComments", new[] { "ParentId" });
            DropIndex("dbo.BlogComments", new[] { "BlogPostId" });
            DropIndex("dbo.BlogComments", new[] { "UserId" });
            DropIndex("dbo.BlogPosts", new[] { "ImageId" });
            DropIndex("dbo.BlogPosts", new[] { "BlogCategoryId" });
            DropIndex("dbo.BlogCategories", new[] { "ParentId" });
            DropTable("dbo.CouponUsers");
            DropTable("dbo.FileProducts");
            DropTable("dbo.Menus");
            DropTable("dbo.Logs");
            DropTable("dbo.KeyValueContents");
            DropTable("dbo.Carousels");
            DropTable("dbo.Coupons");
            DropTable("dbo.ProductComments");
            DropTable("dbo.ProductCategoryPricingRules");
            DropTable("dbo.ProductCategories");
            DropTable("dbo.ProductPricingRules");
            DropTable("dbo.Testimonials");
            DropTable("dbo.Files");
            DropTable("dbo.Products");
            DropTable("dbo.OrderItems");
            DropTable("dbo.Orders");
            DropTable("dbo.BlogComments");
            DropTable("dbo.BlogPosts");
            DropTable("dbo.BlogCategories");
        }
    }
}
