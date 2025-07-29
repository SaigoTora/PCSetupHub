using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PCSetupHub.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAccessFieldsToPrivacySetting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CommentWritingAccessId",
                table: "PrivacySetting",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "FriendsAccessId",
                table: "PrivacySetting",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_PrivacySetting_CommentWritingAccessId",
                table: "PrivacySetting",
                column: "CommentWritingAccessId");

            migrationBuilder.CreateIndex(
                name: "IX_PrivacySetting_FriendsAccessId",
                table: "PrivacySetting",
                column: "FriendsAccessId");

            migrationBuilder.AddForeignKey(
                name: "FK_PrivacySetting_PrivacyLevel_CommentWritingAccessId",
                table: "PrivacySetting",
                column: "CommentWritingAccessId",
                principalTable: "PrivacyLevel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PrivacySetting_PrivacyLevel_FriendsAccessId",
                table: "PrivacySetting",
                column: "FriendsAccessId",
                principalTable: "PrivacyLevel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PrivacySetting_PrivacyLevel_CommentWritingAccessId",
                table: "PrivacySetting");

            migrationBuilder.DropForeignKey(
                name: "FK_PrivacySetting_PrivacyLevel_FriendsAccessId",
                table: "PrivacySetting");

            migrationBuilder.DropIndex(
                name: "IX_PrivacySetting_CommentWritingAccessId",
                table: "PrivacySetting");

            migrationBuilder.DropIndex(
                name: "IX_PrivacySetting_FriendsAccessId",
                table: "PrivacySetting");

            migrationBuilder.DropColumn(
                name: "CommentWritingAccessId",
                table: "PrivacySetting");

            migrationBuilder.DropColumn(
                name: "FriendsAccessId",
                table: "PrivacySetting");
        }
    }
}
