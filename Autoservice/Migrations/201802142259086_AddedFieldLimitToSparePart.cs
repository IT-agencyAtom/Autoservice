namespace Autoservice.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedFieldLimitToSparePart : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SpareParts", "Limit", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SpareParts", "Limit");
        }
    }
}
