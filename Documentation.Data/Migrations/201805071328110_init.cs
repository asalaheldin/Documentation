namespace Documentation.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Document",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Subject = c.String(nullable: false),
                        Date = c.DateTime(nullable: false),
                        SerialNumber = c.String(nullable: false),
                        Remarks = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        Creator = c.Guid(nullable: false),
                        Updator = c.Guid(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        Type_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Type", t => t.Type_Id, cascadeDelete: true)
                .Index(t => t.Type_Id);
            
            CreateTable(
                "dbo.Type",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.User",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        UserName = c.String(),
                        Password = c.String(),
                        FirstName = c.String(),
                        LastName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Document", "Type_Id", "dbo.Type");
            DropIndex("dbo.Document", new[] { "Type_Id" });
            DropTable("dbo.User");
            DropTable("dbo.Type");
            DropTable("dbo.Document");
        }
    }
}
