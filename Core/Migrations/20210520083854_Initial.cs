using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

namespace Core.Migrations
{
    public partial class Initial : Migration
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
                name: "User",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Username = table.Column<string>(type: "varchar(767)", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
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
                name: "Movie",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    ImdbId = table.Column<string>(type: "varchar(767)", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movie", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Movie_DiscussableDao_Id",
                        column: x => x.Id,
                        principalTable: "DiscussableDao",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Toplist",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Toplist", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Toplist_DiscussableDao_Id",
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
                    DiscussionItemId = table.Column<long>(type: "bigint", nullable: true),
                    NumberOfStars = table.Column<int>(type: "int", nullable: true)
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
                name: "LoginSession",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Token = table.Column<string>(type: "varchar(767)", nullable: true),
                    ExpiresAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified)),
                    UserId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoginSession", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoginSession_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ToplistMovie",
                columns: table => new
                {
                    ToplistId = table.Column<long>(type: "bigint", nullable: false),
                    MovieId = table.Column<long>(type: "bigint", nullable: false),
                    Position = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ToplistMovie", x => new { x.MovieId, x.ToplistId });
                    table.ForeignKey(
                        name: "FK_ToplistMovie_Movie_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movie",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ToplistMovie_Toplist_ToplistId",
                        column: x => x.ToplistId,
                        principalTable: "Toplist",
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
                name: "IX_LoginSession_Token",
                table: "LoginSession",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LoginSession_UserId",
                table: "LoginSession",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Movie_ImdbId",
                table: "Movie",
                column: "ImdbId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ToplistMovie_ToplistId_Position",
                table: "ToplistMovie",
                columns: new[] { "ToplistId", "Position" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_Username",
                table: "User",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserDiscussionItemInteraction_UserId",
                table: "UserDiscussionItemInteraction",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Actor");

            migrationBuilder.DropTable(
                name: "LoginSession");

            migrationBuilder.DropTable(
                name: "ToplistMovie");

            migrationBuilder.DropTable(
                name: "UserDiscussionItemInteraction");

            migrationBuilder.DropTable(
                name: "Movie");

            migrationBuilder.DropTable(
                name: "Toplist");

            migrationBuilder.DropTable(
                name: "DiscussionItemDao");

            migrationBuilder.DropTable(
                name: "DiscussableDao");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
