namespace Dokan.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemovedRelationsOfFileTable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.BlogPosts", "ImageId", "dbo.Files");
            DropForeignKey("dbo.FileProducts", "File_Id", "dbo.Files");
            DropForeignKey("dbo.FileProducts", "Product_Id", "dbo.Products");
            DropForeignKey("dbo.Testimonials", "ImageId", "dbo.Files");
            DropIndex("dbo.BlogPosts", new[] { "ImageId" });
            DropIndex("dbo.Testimonials", new[] { "ImageId" });
            DropIndex("dbo.FileProducts", new[] { "File_Id" });
            DropIndex("dbo.FileProducts", new[] { "Product_Id" });
            AddColumn("dbo.BlogPosts", "Image", c => c.String(maxLength: 30));
            AddColumn("dbo.Products", "Image1", c => c.String(maxLength: 30));
            AddColumn("dbo.Products", "Image2", c => c.String(maxLength: 30));
            AddColumn("dbo.Products", "Image3", c => c.String(maxLength: 30));
            AddColumn("dbo.Products", "Image4", c => c.String(maxLength: 30));
            AddColumn("dbo.Products", "Image5", c => c.String(maxLength: 30));
            AddColumn("dbo.Testimonials", "Image", c => c.String(maxLength: 30));
            DropColumn("dbo.BlogPosts", "ImageId");
            DropColumn("dbo.Testimonials", "ImageId");
            DropTable("dbo.FileProducts");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.FileProducts",
                c => new
                    {
                        File_Id = c.Int(nullable: false),
                        Product_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.File_Id, t.Product_Id });
            
            AddColumn("dbo.Testimonials", "ImageId", c => c.Int(nullable: false));
            AddColumn("dbo.BlogPosts", "ImageId", c => c.Int(nullable: false));
            DropColumn("dbo.Testimonials", "Image");
            DropColumn("dbo.Products", "Image5");
            DropColumn("dbo.Products", "Image4");
            DropColumn("dbo.Products", "Image3");
            DropColumn("dbo.Products", "Image2");
            DropColumn("dbo.Products", "Image1");
            DropColumn("dbo.BlogPosts", "Image");
            CreateIndex("dbo.FileProducts", "Product_Id");
            CreateIndex("dbo.FileProducts", "File_Id");
            CreateIndex("dbo.Testimonials", "ImageId");
            CreateIndex("dbo.BlogPosts", "ImageId");
            AddForeignKey("dbo.Testimonials", "ImageId", "dbo.Files", "Id", cascadeDelete: true);
            AddForeignKey("dbo.FileProducts", "Product_Id", "dbo.Products", "Id", cascadeDelete: true);
            AddForeignKey("dbo.FileProducts", "File_Id", "dbo.Files", "Id", cascadeDelete: true);
            AddForeignKey("dbo.BlogPosts", "ImageId", "dbo.Files", "Id", cascadeDelete: true);
        }
    }
}
