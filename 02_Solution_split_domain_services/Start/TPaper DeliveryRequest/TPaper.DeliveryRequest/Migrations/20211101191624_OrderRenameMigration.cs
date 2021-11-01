using Microsoft.EntityFrameworkCore.Migrations;

namespace TPaper.DeliveryRequest
{
    public partial class OrderRenameMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable("EdiOrder", "dbo", "DeliveryRequest");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable("DeliveryRequest", "dbo", "EdiOrder");
        }
    }
}
