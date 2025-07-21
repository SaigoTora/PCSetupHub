using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PCSetupHub.Data.Migrations
{
    /// <inheritdoc />
    public partial class CreatePrivacySettingsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_PrivacyLevel_FollowersAccessId",
                table: "User");

            migrationBuilder.DropForeignKey(
                name: "FK_User_PrivacyLevel_FollowingsAccessId",
                table: "User");

            migrationBuilder.DropForeignKey(
                name: "FK_User_PrivacyLevel_MessagesAccessId",
                table: "User");

            migrationBuilder.DropForeignKey(
                name: "FK_User_PrivacyLevel_PcConfigAccessId",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_User_FollowersAccessId",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_User_FollowingsAccessId",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_User_MessagesAccessId",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_User_PcConfigAccessId",
                table: "User");

            migrationBuilder.DropColumn(
                name: "FollowersAccessId",
                table: "User");

            migrationBuilder.DropColumn(
                name: "FollowingsAccessId",
                table: "User");

            migrationBuilder.DropColumn(
                name: "MessagesAccessId",
                table: "User");

            migrationBuilder.DropColumn(
                name: "PcConfigAccessId",
                table: "User");

            migrationBuilder.CreateTable(
                name: "PrivacySetting",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    FollowersAccessId = table.Column<int>(type: "int", nullable: false),
                    FollowingsAccessId = table.Column<int>(type: "int", nullable: false),
                    MessagesAccessId = table.Column<int>(type: "int", nullable: false),
                    PcConfigAccessId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrivacySetting", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrivacySetting_PrivacyLevel_FollowersAccessId",
                        column: x => x.FollowersAccessId,
                        principalTable: "PrivacyLevel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PrivacySetting_PrivacyLevel_FollowingsAccessId",
                        column: x => x.FollowingsAccessId,
                        principalTable: "PrivacyLevel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PrivacySetting_PrivacyLevel_MessagesAccessId",
                        column: x => x.MessagesAccessId,
                        principalTable: "PrivacyLevel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PrivacySetting_PrivacyLevel_PcConfigAccessId",
                        column: x => x.PcConfigAccessId,
                        principalTable: "PrivacyLevel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PrivacySetting_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PrivacySetting_FollowersAccessId",
                table: "PrivacySetting",
                column: "FollowersAccessId");

            migrationBuilder.CreateIndex(
                name: "IX_PrivacySetting_FollowingsAccessId",
                table: "PrivacySetting",
                column: "FollowingsAccessId");

            migrationBuilder.CreateIndex(
                name: "IX_PrivacySetting_MessagesAccessId",
                table: "PrivacySetting",
                column: "MessagesAccessId");

            migrationBuilder.CreateIndex(
                name: "IX_PrivacySetting_PcConfigAccessId",
                table: "PrivacySetting",
                column: "PcConfigAccessId");

            migrationBuilder.CreateIndex(
                name: "IX_PrivacySetting_UserId",
                table: "PrivacySetting",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PrivacySetting");

            migrationBuilder.AddColumn<int>(
                name: "FollowersAccessId",
                table: "User",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FollowingsAccessId",
                table: "User",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MessagesAccessId",
                table: "User",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PcConfigAccessId",
                table: "User",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_User_FollowersAccessId",
                table: "User",
                column: "FollowersAccessId");

            migrationBuilder.CreateIndex(
                name: "IX_User_FollowingsAccessId",
                table: "User",
                column: "FollowingsAccessId");

            migrationBuilder.CreateIndex(
                name: "IX_User_MessagesAccessId",
                table: "User",
                column: "MessagesAccessId");

            migrationBuilder.CreateIndex(
                name: "IX_User_PcConfigAccessId",
                table: "User",
                column: "PcConfigAccessId");

            migrationBuilder.AddForeignKey(
                name: "FK_User_PrivacyLevel_FollowersAccessId",
                table: "User",
                column: "FollowersAccessId",
                principalTable: "PrivacyLevel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_User_PrivacyLevel_FollowingsAccessId",
                table: "User",
                column: "FollowingsAccessId",
                principalTable: "PrivacyLevel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_User_PrivacyLevel_MessagesAccessId",
                table: "User",
                column: "MessagesAccessId",
                principalTable: "PrivacyLevel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_User_PrivacyLevel_PcConfigAccessId",
                table: "User",
                column: "PcConfigAccessId",
                principalTable: "PrivacyLevel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
