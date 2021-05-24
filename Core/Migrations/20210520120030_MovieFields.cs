using Microsoft.EntityFrameworkCore.Migrations;

namespace Core.Migrations
{
    public partial class MovieFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Movie",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Year",
                table: "Movie",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Movie");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "Movie");
        }
    }
}
