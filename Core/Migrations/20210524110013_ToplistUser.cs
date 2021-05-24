using Microsoft.EntityFrameworkCore.Migrations;

namespace Core.Migrations
{
    public partial class ToplistUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Toplist_UserId",
                table: "Toplist",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Toplist_User_UserId",
                table: "Toplist",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Toplist_User_UserId",
                table: "Toplist");

            migrationBuilder.DropIndex(
                name: "IX_Toplist_UserId",
                table: "Toplist");
        }
    }
}
