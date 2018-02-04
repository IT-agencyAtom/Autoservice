namespace Autoservice.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ClientFieldIsLegalEntityAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Clients", "IsLegalEntity", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Clients", "IsLegalEntity");
        }
    }
}
