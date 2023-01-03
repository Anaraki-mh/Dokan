namespace Dokan.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemovedContentTypePropertyFromKeyValueContent : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.KeyValueContents", "ContentType");
        }
        
        public override void Down()
        {
            AddColumn("dbo.KeyValueContents", "ContentType", c => c.Int(nullable: false));
        }
    }
}
