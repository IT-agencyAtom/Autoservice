namespace Autoservice.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemovedFieldPriceInOrderWork : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.OrderWorks", "Price");
        }
        
        public override void Down()
        {
            AddColumn("dbo.OrderWorks", "Price", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}
