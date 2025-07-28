using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PCSetupHub.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDeleteBehaviors : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comment_User_CommentatorId",
                table: "Comment");

            migrationBuilder.DropForeignKey(
                name: "FK_Comment_User_UserId",
                table: "Comment");

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_User_CommentatorId",
                table: "Comment",
                column: "CommentatorId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_User_UserId",
                table: "Comment",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comment_User_CommentatorId",
                table: "Comment");

            migrationBuilder.DropForeignKey(
                name: "FK_Comment_User_UserId",
                table: "Comment");

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_User_CommentatorId",
                table: "Comment",
                column: "CommentatorId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_User_UserId",
                table: "Comment",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
