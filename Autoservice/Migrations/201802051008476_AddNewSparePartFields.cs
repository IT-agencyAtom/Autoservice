namespace Autoservice.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddNewSparePartFields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SpareParts", "ReceiptDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.SpareParts", "PurchasePrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SpareParts", "PurchasePrice");
            DropColumn("dbo.SpareParts", "ReceiptDate");
        }
    }
}
