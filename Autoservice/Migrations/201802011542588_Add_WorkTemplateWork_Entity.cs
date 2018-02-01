namespace Autoservice.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_WorkTemplateWork_Entity : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Works", "WorkTemplate_Id", "dbo.WorkTemplates");
            DropIndex("dbo.Works", new[] { "WorkTemplate_Id" });
            CreateTable(
                "dbo.WorkTemplateWorks",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        TemplateId = c.Guid(nullable: false),
                        WorkId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Works", t => t.WorkId, cascadeDelete: false)
                .ForeignKey("dbo.WorkTemplates", t => t.TemplateId, cascadeDelete: false)
                .Index(t => t.TemplateId)
                .Index(t => t.WorkId);
            
            DropColumn("dbo.Works", "WorkTemplate_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Works", "WorkTemplate_Id", c => c.Guid());
            DropForeignKey("dbo.WorkTemplateWorks", "TemplateId", "dbo.WorkTemplates");
            DropForeignKey("dbo.WorkTemplateWorks", "WorkId", "dbo.Works");
            DropIndex("dbo.WorkTemplateWorks", new[] { "WorkId" });
            DropIndex("dbo.WorkTemplateWorks", new[] { "TemplateId" });
            DropTable("dbo.WorkTemplateWorks");
            CreateIndex("dbo.Works", "WorkTemplate_Id");
            AddForeignKey("dbo.Works", "WorkTemplate_Id", "dbo.WorkTemplates", "Id");
        }
    }
}
