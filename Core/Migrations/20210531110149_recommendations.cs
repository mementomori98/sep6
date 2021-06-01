using Microsoft.EntityFrameworkCore.Migrations;

namespace Core.Migrations
{
    public partial class recommendations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Public",
                table: "Toplist",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "TmdbId",
                table: "Movie",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "RecommendationRequestDao",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    TopRatedRecommendationPage = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecommendationRequestDao", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_RecommendationRequestDao_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReviewRecommendation",
                columns: table => new
                {
                    ReviewId = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    NumberOfPagesShown = table.Column<int>(type: "int", nullable: false),
                    HasMore = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    MovieImdbId = table.Column<string>(type: "text", nullable: true),
                    MovieTmdbId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewRecommendation", x => x.ReviewId);
                    table.ForeignKey(
                        name: "FK_ReviewRecommendation_DiscussionItem_ReviewId",
                        column: x => x.ReviewId,
                        principalTable: "DiscussionItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReviewRecommendation_RecommendationRequestDao_UserId",
                        column: x => x.UserId,
                        principalTable: "RecommendationRequestDao",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReviewRecommendation_UserId",
                table: "ReviewRecommendation",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReviewRecommendation");

            migrationBuilder.DropTable(
                name: "RecommendationRequestDao");

            migrationBuilder.DropColumn(
                name: "Public",
                table: "Toplist");

            migrationBuilder.DropColumn(
                name: "TmdbId",
                table: "Movie");
        }
    }
}
