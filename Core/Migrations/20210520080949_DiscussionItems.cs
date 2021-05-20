using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

namespace Core.Migrations
{
    public partial class DiscussionItems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DiscussableDao",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscussableDao", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Actor",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Actor", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Actor_DiscussableDao_Id",
                        column: x => x.Id,
                        principalTable: "DiscussableDao",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DiscussionItemDao",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Text = table.Column<string>(type: "text", nullable: false),
                    AuthorId = table.Column<long>(type: "bigint", nullable: false),
                    DiscussableId = table.Column<long>(type: "bigint", nullable: true),
                    Discriminator = table.Column<string>(type: "text", nullable: false),
                    DiscussionItemId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscussionItemDao", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiscussionItemDao_DiscussableDao_DiscussableId",
                        column: x => x.DiscussableId,
                        principalTable: "DiscussableDao",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DiscussionItemDao_DiscussionItemDao_DiscussionItemId",
                        column: x => x.DiscussionItemId,
                        principalTable: "DiscussionItemDao",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DiscussionItemDao_User_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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
                        name: "FK_UserDiscussionItemInteraction_DiscussionItemDao_DiscussionIt~",
                        column: x => x.DiscussionItemId,
                        principalTable: "DiscussionItemDao",
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
                name: "IX_DiscussionItemDao_AuthorId",
                table: "DiscussionItemDao",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscussionItemDao_DiscussableId",
                table: "DiscussionItemDao",
                column: "DiscussableId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscussionItemDao_DiscussionItemId",
                table: "DiscussionItemDao",
                column: "DiscussionItemId");

            migrationBuilder.CreateIndex(
                name: "IX_UserDiscussionItemInteraction_UserId",
                table: "UserDiscussionItemInteraction",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Movie_DiscussableDao_Id",
                table: "Movie",
                column: "Id",
                principalTable: "DiscussableDao",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Toplist_DiscussableDao_Id",
                table: "Toplist",
                column: "Id",
                principalTable: "DiscussableDao",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Movie_DiscussableDao_Id",
                table: "Movie");

            migrationBuilder.DropForeignKey(
                name: "FK_Toplist_DiscussableDao_Id",
                table: "Toplist");

            migrationBuilder.DropTable(
                name: "Actor");

            migrationBuilder.DropTable(
                name: "UserDiscussionItemInteraction");

            migrationBuilder.DropTable(
                name: "DiscussionItemDao");

            migrationBuilder.DropTable(
                name: "DiscussableDao");
        }
    }
}
