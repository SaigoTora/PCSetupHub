using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PCSetupHub.Data.Migrations
{
	/// <inheritdoc />
	public partial class AddUserIdToPcConfigurations : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<int>(
				name: "UserId",
				table: "PcConfiguration",
				type: "int",
				nullable: true);

			migrationBuilder.Sql(
				@"UPDATE PcConfiguration
                SET UserId = u.Id
                FROM PcConfiguration pc
                JOIN [User] u ON u.PcConfigurationId = pc.Id");

			migrationBuilder.AlterColumn<int>(
				name: "UserId",
				table: "PcConfiguration",
				type: "int",
				nullable: false,
				oldClrType: typeof(int),
				oldType: "int",
				oldNullable: true);

			migrationBuilder.DropForeignKey(
				name: "FK_User_PcConfiguration_PcConfigurationId",
				table: "User");

			migrationBuilder.DropIndex(
				name: "IX_User_PcConfigurationId",
				table: "User");

			migrationBuilder.DropColumn(
				name: "PcConfigurationId",
				table: "User");

			migrationBuilder.CreateIndex(
				name: "IX_PcConfiguration_UserId",
				table: "PcConfiguration",
				column: "UserId",
				unique: true);

			migrationBuilder.AddForeignKey(
				name: "FK_PcConfiguration_User_UserId",
				table: "PcConfiguration",
				column: "UserId",
				principalTable: "User",
				principalColumn: "Id",
				onDelete: ReferentialAction.Cascade);
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<int>(
				name: "PcConfigurationId",
				table: "User",
				type: "int",
				nullable: true);

			migrationBuilder.Sql(
				@"UPDATE [User]
                SET PcConfigurationId = pc.Id
                FROM [User] u
                JOIN PcConfiguration pc ON pc.UserId = u.Id");

			migrationBuilder.AlterColumn<int>(
				name: "PcConfigurationId",
				table: "User",
				type: "int",
				nullable: false,
				oldClrType: typeof(int),
				oldType: "int",
				oldNullable: true);

			migrationBuilder.DropForeignKey(
				name: "FK_PcConfiguration_User_UserId",
				table: "PcConfiguration");

			migrationBuilder.DropIndex(
				name: "IX_PcConfiguration_UserId",
				table: "PcConfiguration");

			migrationBuilder.DropColumn(
				name: "UserId",
				table: "PcConfiguration");

			migrationBuilder.CreateIndex(
				name: "IX_User_PcConfigurationId",
				table: "User",
				column: "PcConfigurationId",
				unique: true);

			migrationBuilder.AddForeignKey(
				name: "FK_User_PcConfiguration_PcConfigurationId",
				table: "User",
				column: "PcConfigurationId",
				principalTable: "PcConfiguration",
				principalColumn: "Id",
				onDelete: ReferentialAction.Cascade);
		}
	}
}
