using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeisureReviewsAPI.Migrations
{
    /// <inheritdoc />
    public partial class CreateCommentRate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CommentRates",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsPositive = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CommentId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentRates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommentRates_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommentRates_Comments_CommentId",
                        column: x => x.CommentId,
                        principalTable: "Comments",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CommentRates_CommentId",
                table: "CommentRates",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentRates_UserId",
                table: "CommentRates",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommentRates");
        }
    }
}
