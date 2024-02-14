using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarberShop.Infrastructure.Migrations
{
    public partial class ImageUrlBarber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Barbers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Barbers");
        }
    }
}
