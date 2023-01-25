namespace Dokan.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemovedProfilePictureFromOrderEntity : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Orders", "ProfilePicture");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Orders", "ProfilePicture", c => c.String(maxLength: 50));
        }
    }
}
