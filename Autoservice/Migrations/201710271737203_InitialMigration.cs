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
                        StartDate = c.DateTime(nullable: false),
                        PersonalNumber = c.String(),
                        ClientId = c.Guid(nullable: false),
                        RepairZone = c.String(),
                        CarId = c.Guid(nullable: false),
                        Notes = c.String(),
                        TotalPrice = c.Int(nullable: false),
                        PaymentMethod = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Cars", t => t.CarId, cascadeDelete: false)
                .ForeignKey("dbo.Clients", t => t.ClientId, cascadeDelete: false)
                .Index(t => t.ClientId)
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
                        Phone = c.Long(nullable: false),
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
                "dbo.Works",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        Price = c.Single(nullable: false),
                        MasterId = c.Guid(nullable: false),
                        OrderId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Masters", t => t.MasterId, cascadeDelete: true)
                .ForeignKey("dbo.Orders", t => t.OrderId, cascadeDelete: false)
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
            DropForeignKey("dbo.Works", "OrderId", "dbo.Orders");
            DropForeignKey("dbo.Works", "MasterId", "dbo.Masters");
            DropForeignKey("dbo.SpareParts", "Order_Id", "dbo.Orders");
            DropForeignKey("dbo.Orders", "ClientId", "dbo.Clients");
            DropForeignKey("dbo.Orders", "CarId", "dbo.Cars");
            DropForeignKey("dbo.Cars", "ClientId", "dbo.Clients");
            DropForeignKey("dbo.Activities", "OrderId", "dbo.Orders");
            DropIndex("dbo.Works", new[] { "OrderId" });
            DropIndex("dbo.Works", new[] { "MasterId" });
            DropIndex("dbo.SpareParts", new[] { "Order_Id" });
            DropIndex("dbo.Cars", new[] { "ClientId" });
            DropIndex("dbo.Orders", new[] { "CarId" });
            DropIndex("dbo.Orders", new[] { "ClientId" });
            DropIndex("dbo.Activities", new[] { "UserId" });
            DropIndex("dbo.Activities", new[] { "OrderId" });
            DropTable("dbo.Users");
            DropTable("dbo.Masters");
            DropTable("dbo.Works");
            DropTable("dbo.SpareParts");
            DropTable("dbo.Clients");
            DropTable("dbo.Cars");
            DropTable("dbo.Orders");
            DropTable("dbo.Activities");
        }
    }
}
