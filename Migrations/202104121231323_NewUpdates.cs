namespace MovieShop.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NewUpdates : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderRows", "NoofCopies", c => c.Int(nullable: false));
            AddColumn("dbo.Orders", "TotalPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "TotalPrice");
            DropColumn("dbo.OrderRows", "NoofCopies");
        }
    }
}
