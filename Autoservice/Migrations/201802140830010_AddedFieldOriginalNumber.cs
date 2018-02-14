namespace Autoservice.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedFieldOriginalNumber : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SpareParts", "OriginalNumber", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.SpareParts", "OriginalNumber");
        }
    }
}
