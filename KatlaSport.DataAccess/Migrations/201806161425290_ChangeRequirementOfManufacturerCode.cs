namespace KatlaSport.DataAccess.Migrations
{
    using System.Data.Entity.Migrations;

    /// <summary>
    /// Change requirement of manufacturer code migration.
    /// </summary>
    public partial class ChangeRequirementOfManufacturerCode : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.catalogue_products", "product_manufacturer_code", c => c.String(nullable: false, maxLength: 10));
        }

        public override void Down()
        {
            AlterColumn("dbo.catalogue_products", "product_manufacturer_code", c => c.String(maxLength: 10));
        }
    }
}
