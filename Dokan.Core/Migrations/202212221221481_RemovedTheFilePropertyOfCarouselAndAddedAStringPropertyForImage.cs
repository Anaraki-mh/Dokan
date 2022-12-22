namespace Dokan.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemovedTheFilePropertyOfCarouselAndAddedAStringPropertyForImage : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Carousels", "ImageId", "dbo.Files");
            DropIndex("dbo.Carousels", new[] { "ImageId" });
            AddColumn("dbo.Carousels", "IsDisplayed", c => c.Boolean(nullable: false));
            AddColumn("dbo.Carousels", "Image", c => c.String(maxLength: 30));
            DropColumn("dbo.Carousels", "Display");
            DropColumn("dbo.Carousels", "ImageId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Carousels", "ImageId", c => c.Int(nullable: false));
            AddColumn("dbo.Carousels", "Display", c => c.Boolean(nullable: false));
            DropColumn("dbo.Carousels", "Image");
            DropColumn("dbo.Carousels", "IsDisplayed");
            CreateIndex("dbo.Carousels", "ImageId");
            AddForeignKey("dbo.Carousels", "ImageId", "dbo.Files", "Id", cascadeDelete: true);
        }
    }
}
