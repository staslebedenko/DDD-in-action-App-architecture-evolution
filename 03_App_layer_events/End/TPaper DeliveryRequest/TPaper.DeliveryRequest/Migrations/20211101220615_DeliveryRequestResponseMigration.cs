using Microsoft.EntityFrameworkCore.Migrations;

namespace TPaper.DeliveryRequest
{
    public partial class DeliveryRequestResponseMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Response_Status",
                schema: "dbo",
                table: "DeliveryRequest",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Response_Status",
                schema: "dbo",
                table: "DeliveryRequest");
        }
    }
}
