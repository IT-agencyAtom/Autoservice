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
                        PreOrderDateTime = c.DateTime(),
                        StartDate = c.DateTime(nullable: false),
                        RepairZone = c.String(),
                        ClientCarId = c.Guid(nullable: false),
                        Notes = c.String(),
                        TotalPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PaymentMethod = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ClientCars", t => t.ClientCarId, cascadeDelete: true)
                .Index(t => t.ClientCarId);
            
            CreateTable(
                "dbo.ClientCars",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        RegistrationNumber = c.String(),
                        Mileage = c.Int(nullable: false),
                        CarId = c.Guid(nullable: false),
                        ClientId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Cars", t => t.CarId, cascadeDelete: true)
                .ForeignKey("dbo.Clients", t => t.ClientId, cascadeDelete: true)
                .Index(t => t.CarId)
                .Index(t => t.ClientId);
            
            CreateTable(
                "dbo.Cars",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Brand = c.String(),
                        Model = c.String(),
                        Type = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Clients",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        Phone = c.String(),
                        Discount = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.OrderSpareParts",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        OrderId = c.Guid(nullable: false),
                        SparePartId = c.Guid(nullable: false),
                        Number = c.Int(nullable: false),
                        Source = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Orders", t => t.OrderId, cascadeDelete: true)
                .ForeignKey("dbo.SpareParts", t => t.SparePartId, cascadeDelete: true)
                .Index(t => t.OrderId)
                .Index(t => t.SparePartId);
            
            CreateTable(
                "dbo.SpareParts",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        Cargo = c.String(),
                        Number = c.Int(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Manufacturer = c.String(),
                        ParentId = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SparePartsFolders", t => t.ParentId)
                .Index(t => t.ParentId);
            
            CreateTable(
                "dbo.SparePartsFolders",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        ParentId = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SparePartsFolders", t => t.ParentId)
                .Index(t => t.ParentId);
            
            CreateTable(
                "dbo.OrderWorks",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        WorkId = c.Guid(nullable: false),
                        MasterId = c.Guid(nullable: false),
                        OrderId = c.Guid(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
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
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WorkTemplate_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.WorkTemplates", t => t.WorkTemplate_Id)
                .Index(t => t.WorkTemplate_Id);
            
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
            
            CreateTable(
                "dbo.WorkTemplates",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Works", "WorkTemplate_Id", "dbo.WorkTemplates");
            DropForeignKey("dbo.Activities", "UserId", "dbo.Users");
            DropForeignKey("dbo.OrderWorks", "WorkId", "dbo.Works");
            DropForeignKey("dbo.OrderWorks", "OrderId", "dbo.Orders");
            DropForeignKey("dbo.OrderWorks", "MasterId", "dbo.Masters");
            DropForeignKey("dbo.OrderSpareParts", "SparePartId", "dbo.SpareParts");
            DropForeignKey("dbo.SpareParts", "ParentId", "dbo.SparePartsFolders");
            DropForeignKey("dbo.SparePartsFolders", "ParentId", "dbo.SparePartsFolders");
            DropForeignKey("dbo.OrderSpareParts", "OrderId", "dbo.Orders");
            DropForeignKey("dbo.Orders", "ClientCarId", "dbo.ClientCars");
            DropForeignKey("dbo.ClientCars", "ClientId", "dbo.Clients");
            DropForeignKey("dbo.ClientCars", "CarId", "dbo.Cars");
            DropForeignKey("dbo.Activities", "OrderId", "dbo.Orders");
            DropIndex("dbo.Works", new[] { "WorkTemplate_Id" });
            DropIndex("dbo.OrderWorks", new[] { "OrderId" });
            DropIndex("dbo.OrderWorks", new[] { "MasterId" });
            DropIndex("dbo.OrderWorks", new[] { "WorkId" });
            DropIndex("dbo.SparePartsFolders", new[] { "ParentId" });
            DropIndex("dbo.SpareParts", new[] { "ParentId" });
            DropIndex("dbo.OrderSpareParts", new[] { "SparePartId" });
            DropIndex("dbo.OrderSpareParts", new[] { "OrderId" });
            DropIndex("dbo.ClientCars", new[] { "ClientId" });
            DropIndex("dbo.ClientCars", new[] { "CarId" });
            DropIndex("dbo.Orders", new[] { "ClientCarId" });
            DropIndex("dbo.Activities", new[] { "UserId" });
            DropIndex("dbo.Activities", new[] { "OrderId" });
            DropTable("dbo.WorkTemplates");
            DropTable("dbo.Users");
            DropTable("dbo.Works");
            DropTable("dbo.Masters");
            DropTable("dbo.OrderWorks");
            DropTable("dbo.SparePartsFolders");
            DropTable("dbo.SpareParts");
            DropTable("dbo.OrderSpareParts");
            DropTable("dbo.Clients");
            DropTable("dbo.Cars");
            DropTable("dbo.ClientCars");
            DropTable("dbo.Orders");
            DropTable("dbo.Activities");
        }
    }
}
