namespace Autoservice.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMasterPercentageFieldMigration : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderWorks", "MasterPercentage", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderWorks", "MasterPercentage");
        }
    }
}
