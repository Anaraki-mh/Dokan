namespace Dokan.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedUserInformationTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserInformations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProfilePicture = c.String(maxLength: 50),
                        FirstName = c.String(maxLength: 50),
                        LastName = c.String(maxLength: 50),
                        Country = c.String(maxLength: 50),
                        State = c.String(maxLength: 50),
                        City = c.String(maxLength: 50),
                        Address = c.String(maxLength: 120),
                        ZipCode = c.String(maxLength: 12),
                        PhoneNumber = c.String(maxLength: 15),
                        UserId = c.String(),
                        User_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .Index(t => t.User_Id);
            
            AddColumn("dbo.AspNetUsers", "UserInformationId", c => c.Int(nullable: false));
            AddColumn("dbo.Orders", "ProfilePicture", c => c.String(maxLength: 50));
            AddColumn("dbo.Orders", "FirstName", c => c.String(maxLength: 50));
            AddColumn("dbo.Orders", "LastName", c => c.String(maxLength: 50));
            AddColumn("dbo.Orders", "Country", c => c.String(maxLength: 50));
            AddColumn("dbo.Orders", "State", c => c.String(maxLength: 50));
            AddColumn("dbo.Orders", "City", c => c.String(maxLength: 50));
            AddColumn("dbo.Orders", "Address", c => c.String(maxLength: 120));
            AddColumn("dbo.Orders", "ZipCode", c => c.String(maxLength: 12));
            AddColumn("dbo.Orders", "PhoneNumber", c => c.String(maxLength: 15));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserInformations", "User_Id", "dbo.AspNetUsers");
            DropIndex("dbo.UserInformations", new[] { "User_Id" });
            DropColumn("dbo.Orders", "PhoneNumber");
            DropColumn("dbo.Orders", "ZipCode");
            DropColumn("dbo.Orders", "Address");
            DropColumn("dbo.Orders", "City");
            DropColumn("dbo.Orders", "State");
            DropColumn("dbo.Orders", "Country");
            DropColumn("dbo.Orders", "LastName");
            DropColumn("dbo.Orders", "FirstName");
            DropColumn("dbo.Orders", "ProfilePicture");
            DropColumn("dbo.AspNetUsers", "UserInformationId");
            DropTable("dbo.UserInformations");
        }
    }
}
