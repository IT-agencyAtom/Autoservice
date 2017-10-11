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
                        StartTime = c.DateTime(nullable: false),
                        EndTime = c.DateTime(nullable: false),
                        Status = c.Int(nullable: false),
                        Order_Id = c.Guid(),
                        User_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Orders", t => t.Order_Id)
                .ForeignKey("dbo.Users", t => t.User_Id)
                .Index(t => t.Order_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        PersonalNumber = c.String(),
                        RepairZone = c.String(),
                        Status = c.Int(nullable: false),
                        Car_Id = c.Guid(),
                        Client_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Cars", t => t.Car_Id)
                .ForeignKey("dbo.Clients", t => t.Client_Id)
                .Index(t => t.Car_Id)
                .Index(t => t.Client_Id);
            
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
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Clients",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Masters",
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
                        Order_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Orders", t => t.Order_Id)
                .Index(t => t.Order_Id);
            
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
            DropForeignKey("dbo.Activities", "User_Id", "dbo.Users");
            DropForeignKey("dbo.Works", "Order_Id", "dbo.Orders");
            DropForeignKey("dbo.SpareParts", "Order_Id", "dbo.Orders");
            DropForeignKey("dbo.Masters", "Order_Id", "dbo.Orders");
            DropForeignKey("dbo.Orders", "Client_Id", "dbo.Clients");
            DropForeignKey("dbo.Orders", "Car_Id", "dbo.Cars");
            DropForeignKey("dbo.Activities", "Order_Id", "dbo.Orders");
            DropIndex("dbo.Works", new[] { "Order_Id" });
            DropIndex("dbo.SpareParts", new[] { "Order_Id" });
            DropIndex("dbo.Masters", new[] { "Order_Id" });
            DropIndex("dbo.Orders", new[] { "Client_Id" });
            DropIndex("dbo.Orders", new[] { "Car_Id" });
            DropIndex("dbo.Activities", new[] { "User_Id" });
            DropIndex("dbo.Activities", new[] { "Order_Id" });
            DropTable("dbo.Users");
            DropTable("dbo.Works");
            DropTable("dbo.SpareParts");
            DropTable("dbo.Masters");
            DropTable("dbo.Clients");
            DropTable("dbo.Cars");
            DropTable("dbo.Orders");
            DropTable("dbo.Activities");
        }
    }
}
