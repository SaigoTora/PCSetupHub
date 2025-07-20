using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PCSetupHub.Data.Migrations
{
	/// <inheritdoc />
	public partial class AddPrivacyLevelsToUser : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AlterColumn<string>(
				name: "Login",
				table: "User",
				type: "nvarchar(255)",
				maxLength: 255,
				nullable: false,
				oldClrType: typeof(string),
				oldType: "nvarchar(254)",
				oldMaxLength: 254);

			migrationBuilder.AlterColumn<string>(
				name: "Email",
				table: "User",
				type: "nvarchar(255)",
				maxLength: 255,
				nullable: false,
				oldClrType: typeof(string),
				oldType: "nvarchar(254)",
				oldMaxLength: 254);

			migrationBuilder.AddColumn<int>(
				name: "FollowersAccessId",
				table: "User",
				type: "int",
				nullable: false,
				defaultValue: 1);

			migrationBuilder.AddColumn<int>(
				name: "FollowingsAccessId",
				table: "User",
				type: "int",
				nullable: false,
				defaultValue: 1);

			migrationBuilder.AddColumn<int>(
				name: "MessagesAccessId",
				table: "User",
				type: "int",
				nullable: false,
				defaultValue: 1);

			migrationBuilder.AddColumn<int>(
				name: "PcConfigAccessId",
				table: "User",
				type: "int",
				nullable: false,
				defaultValue: 1);

			migrationBuilder.CreateTable(
				name: "PrivacyLevel",
				columns: table => new
				{
					Id = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_PrivacyLevel", x => x.Id);
				});

			migrationBuilder.InsertData(
				table: "PrivacyLevel",
				columns: ["Id", "Name"],
				values: new object[,]
				{
					{ 1, "Everyone" },
					{ 2, "Friends, followers and followings" },
					{ 3, "Friends and followings" },
					{ 4, "Friends only" }
				});

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

			migrationBuilder.CreateIndex(
				name: "IX_PrivacyLevel_Name",
				table: "PrivacyLevel",
				column: "Name",
				unique: true);

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

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
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

			migrationBuilder.DropTable(
				name: "PrivacyLevel");

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

			migrationBuilder.AlterColumn<string>(
				name: "Login",
				table: "User",
				type: "nvarchar(254)",
				maxLength: 254,
				nullable: false,
				oldClrType: typeof(string),
				oldType: "nvarchar(255)",
				oldMaxLength: 255);

			migrationBuilder.AlterColumn<string>(
				name: "Email",
				table: "User",
				type: "nvarchar(254)",
				maxLength: 254,
				nullable: false,
				oldClrType: typeof(string),
				oldType: "nvarchar(255)",
				oldMaxLength: 255);
		}
	}
}
