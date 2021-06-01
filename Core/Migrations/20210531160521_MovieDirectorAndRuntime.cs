using Microsoft.EntityFrameworkCore.Migrations;

namespace Core.Migrations
{
    public partial class MovieDirectorAndRuntime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Director",
                table: "Movie",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Runtime",
                table: "Movie",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Director",
                table: "Movie");

            migrationBuilder.DropColumn(
                name: "Runtime",
                table: "Movie");
        }
    }
}
