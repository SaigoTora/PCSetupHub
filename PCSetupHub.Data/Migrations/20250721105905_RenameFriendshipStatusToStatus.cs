using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PCSetupHub.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameFriendshipStatusToStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Friendship_FriendshipStatus_FriendshipStatusId",
                table: "Friendship");

            migrationBuilder.RenameColumn(
                name: "FriendshipStatusId",
                table: "Friendship",
                newName: "StatusId");

            migrationBuilder.RenameIndex(
                name: "IX_Friendship_FriendshipStatusId",
                table: "Friendship",
                newName: "IX_Friendship_StatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_Friendship_FriendshipStatus_StatusId",
                table: "Friendship",
                column: "StatusId",
                principalTable: "FriendshipStatus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Friendship_FriendshipStatus_StatusId",
                table: "Friendship");

            migrationBuilder.RenameColumn(
                name: "StatusId",
                table: "Friendship",
                newName: "FriendshipStatusId");

            migrationBuilder.RenameIndex(
                name: "IX_Friendship_StatusId",
                table: "Friendship",
                newName: "IX_Friendship_FriendshipStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_Friendship_FriendshipStatus_FriendshipStatusId",
                table: "Friendship",
                column: "FriendshipStatusId",
                principalTable: "FriendshipStatus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
