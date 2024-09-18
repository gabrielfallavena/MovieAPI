using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class newUpdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Review_Movie_MovieId",
                table: "Review");

            migrationBuilder.DropForeignKey(
                name: "FK_Review_User_UserId",
                table: "Review");

            migrationBuilder.DropIndex(
                name: "IX_Review_MovieId",
                table: "Review");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Review",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "MovieReview",
                columns: table => new
                {
                    MovieId = table.Column<int>(type: "int", nullable: false),
                    ReviewsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovieReview", x => new { x.MovieId, x.ReviewsId });
                    table.ForeignKey(
                        name: "FK_MovieReview_Movie_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movie",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MovieReview_Review_ReviewsId",
                        column: x => x.ReviewsId,
                        principalTable: "Review",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "UserUser",
                columns: table => new
                {
                    FriendsId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserUser", x => new { x.FriendsId, x.UserId });
                    table.ForeignKey(
                        name: "FK_UserUser_User_FriendsId",
                        column: x => x.FriendsId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserUser_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_MovieReview_ReviewsId",
                table: "MovieReview",
                column: "ReviewsId");

            migrationBuilder.CreateIndex(
                name: "IX_UserUser_UserId",
                table: "UserUser",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Review_User_UserId",
                table: "Review",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Review_User_UserId",
                table: "Review");

            migrationBuilder.DropTable(
                name: "MovieReview");

            migrationBuilder.DropTable(
                name: "UserUser");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Review",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Review_MovieId",
                table: "Review",
                column: "MovieId");

            migrationBuilder.AddForeignKey(
                name: "FK_Review_Movie_MovieId",
                table: "Review",
                column: "MovieId",
                principalTable: "Movie",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Review_User_UserId",
                table: "Review",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id");
        }
    }
}
