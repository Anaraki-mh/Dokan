namespace Dokan.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedMessagesTableAndModifiedPricingRulesTables : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.ProductPricingRules", newName: "DiscountCategories");
            DropForeignKey("dbo.ProductCategories", "PricingRuleId", "dbo.ProductCategoryPricingRules");
            DropIndex("dbo.ProductCategories", new[] { "PricingRuleId" });
            RenameColumn(table: "dbo.Products", name: "PricingRuleId", newName: "DiscountCategoryId");
            RenameIndex(table: "dbo.Products", name: "IX_PricingRuleId", newName: "IX_DiscountCategoryId");
            CreateTable(
                "dbo.TaxCategories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(maxLength: 75),
                        Tax = c.Int(nullable: false),
                        IsRemoved = c.Boolean(nullable: false),
                        CreateDateTime = c.DateTime(nullable: false),
                        UpdateDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Messages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 75),
                        Email = c.String(maxLength: 75),
                        Subject = c.String(maxLength: 75),
                        MessageBody = c.String(maxLength: 400),
                        IsRemoved = c.Boolean(nullable: false),
                        CreateDateTime = c.DateTime(nullable: false),
                        UpdateDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.BlogComments", "Rating", c => c.Int(nullable: false));
            AddColumn("dbo.ProductCategories", "TaxCategoryId", c => c.Int());
            AddColumn("dbo.ProductComments", "Rating", c => c.Int(nullable: false));
            CreateIndex("dbo.ProductCategories", "TaxCategoryId");
            AddForeignKey("dbo.ProductCategories", "TaxCategoryId", "dbo.TaxCategories", "Id");
            DropColumn("dbo.DiscountCategories", "Tax");
            DropColumn("dbo.DiscountCategories", "MultiplyPriceBy");
            DropColumn("dbo.ProductCategories", "PricingRuleId");
            DropTable("dbo.ProductCategoryPricingRules");
        }
        
        public override void Down()
        {
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
            
            AddColumn("dbo.ProductCategories", "PricingRuleId", c => c.Int());
            AddColumn("dbo.DiscountCategories", "MultiplyPriceBy", c => c.Int(nullable: false));
            AddColumn("dbo.DiscountCategories", "Tax", c => c.Int(nullable: false));
            DropForeignKey("dbo.ProductCategories", "TaxCategoryId", "dbo.TaxCategories");
            DropIndex("dbo.ProductCategories", new[] { "TaxCategoryId" });
            DropColumn("dbo.ProductComments", "Rating");
            DropColumn("dbo.ProductCategories", "TaxCategoryId");
            DropColumn("dbo.BlogComments", "Rating");
            DropTable("dbo.Messages");
            DropTable("dbo.TaxCategories");
            RenameIndex(table: "dbo.Products", name: "IX_DiscountCategoryId", newName: "IX_PricingRuleId");
            RenameColumn(table: "dbo.Products", name: "DiscountCategoryId", newName: "PricingRuleId");
            CreateIndex("dbo.ProductCategories", "PricingRuleId");
            AddForeignKey("dbo.ProductCategories", "PricingRuleId", "dbo.ProductCategoryPricingRules", "Id");
            RenameTable(name: "dbo.DiscountCategories", newName: "ProductPricingRules");
        }
    }
}
