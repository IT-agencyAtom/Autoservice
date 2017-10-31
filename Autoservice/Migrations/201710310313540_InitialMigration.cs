namespace Autoservice.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Activities",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        UniqueString = c.String(),
                        StartTime = c.DateTime(nullable: false),
                        EndTime = c.DateTime(),
                        OrderId = c.Guid(nullable: false),
                        Status = c.Int(nullable: false),
                        UserId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Orders", t => t.OrderId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.OrderId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Number = c.Int(nullable: false, identity: true),
                        StartDate = c.DateTime(nullable: false),
                        RepairZone = c.String(),
                        CarId = c.Guid(nullable: false),
                        Notes = c.String(),
                        TotalPrice = c.Int(nullable: false),
                        PaymentMethod = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Cars", t => t.CarId, cascadeDelete: true)
                .Index(t => t.CarId);
            
            CreateTable(
                "dbo.Cars",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        RegistrationNumber = c.String(),
                        Brand = c.String(),
                        Model = c.String(),
                        Type = c.Int(nullable: false),
                        Mileage = c.Int(nullable: false),
                        ClientId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Clients", t => t.ClientId, cascadeDelete: true)
                .Index(t => t.ClientId);
            
            CreateTable(
                "dbo.Clients",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        Phone = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SpareParts",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        Order_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Orders", t => t.Order_Id)
                .Index(t => t.Order_Id);
            
            CreateTable(
                "dbo.OrderWorks",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        WorkId = c.Guid(nullable: false),
                        MasterId = c.Guid(nullable: false),
                        OrderId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Masters", t => t.MasterId, cascadeDelete: true)
                .ForeignKey("dbo.Orders", t => t.OrderId, cascadeDelete: true)
                .ForeignKey("dbo.Works", t => t.WorkId, cascadeDelete: true)
                .Index(t => t.WorkId)
                .Index(t => t.MasterId)
                .Index(t => t.OrderId);
            
            CreateTable(
                "dbo.Masters",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        Position = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Works",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        Price = c.Single(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Login = c.String(),
                        Password = c.String(),
                        Role = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Activities", "UserId", "dbo.Users");
            DropForeignKey("dbo.OrderWorks", "WorkId", "dbo.Works");
            DropForeignKey("dbo.OrderWorks", "OrderId", "dbo.Orders");
            DropForeignKey("dbo.OrderWorks", "MasterId", "dbo.Masters");
            DropForeignKey("dbo.SpareParts", "Order_Id", "dbo.Orders");
            DropForeignKey("dbo.Orders", "CarId", "dbo.Cars");
            DropForeignKey("dbo.Cars", "ClientId", "dbo.Clients");
            DropForeignKey("dbo.Activities", "OrderId", "dbo.Orders");
            DropIndex("dbo.OrderWorks", new[] { "OrderId" });
            DropIndex("dbo.OrderWorks", new[] { "MasterId" });
            DropIndex("dbo.OrderWorks", new[] { "WorkId" });
            DropIndex("dbo.SpareParts", new[] { "Order_Id" });
            DropIndex("dbo.Cars", new[] { "ClientId" });
            DropIndex("dbo.Orders", new[] { "CarId" });
            DropIndex("dbo.Activities", new[] { "UserId" });
            DropIndex("dbo.Activities", new[] { "OrderId" });
            DropTable("dbo.Users");
            DropTable("dbo.Works");
            DropTable("dbo.Masters");
            DropTable("dbo.OrderWorks");
            DropTable("dbo.SpareParts");
            DropTable("dbo.Clients");
            DropTable("dbo.Cars");
            DropTable("dbo.Orders");
            DropTable("dbo.Activities");
        }
    }
}
