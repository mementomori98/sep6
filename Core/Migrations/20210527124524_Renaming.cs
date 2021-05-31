using Microsoft.EntityFrameworkCore.Migrations;

namespace Core.Migrations
{
    public partial class Renaming : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserDiscussionItemInteraction");

            migrationBuilder.CreateTable(
                name: "Interaction",
                columns: table => new
                {
                    DiscussionItemId = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Interaction", x => new { x.DiscussionItemId, x.UserId });
                    table.ForeignKey(
                        name: "FK_Interaction_DiscussionItem_DiscussionItemId",
                        column: x => x.DiscussionItemId,
                        principalTable: "DiscussionItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Interaction_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Interaction_UserId",
                table: "Interaction",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Interaction");

            migrationBuilder.CreateTable(
                name: "UserDiscussionItemInteraction",
                columns: table => new
                {
                    DiscussionItemId = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    InteractionType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDiscussionItemInteraction", x => new { x.DiscussionItemId, x.UserId });
                    table.ForeignKey(
                        name: "FK_UserDiscussionItemInteraction_DiscussionItem_DiscussionItemId",
                        column: x => x.DiscussionItemId,
                        principalTable: "DiscussionItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserDiscussionItemInteraction_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserDiscussionItemInteraction_UserId",
                table: "UserDiscussionItemInteraction",
                column: "UserId");
        }
    }
}
