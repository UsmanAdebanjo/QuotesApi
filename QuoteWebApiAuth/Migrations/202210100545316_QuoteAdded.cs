namespace QuoteWebApiAuth.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class QuoteAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Quotes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 20),
                        Author = c.String(nullable: false, maxLength: 30),
                        Description = c.String(nullable: false),
                        Type = c.String(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Quotes");
        }
    }
}
