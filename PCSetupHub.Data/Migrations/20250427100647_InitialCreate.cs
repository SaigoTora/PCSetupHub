using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PCSetupHub.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Color",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Color", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FriendshipStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FriendshipStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Hdd",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Interface = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "money", nullable: true),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hdd", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Motherboard",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Socket = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FormFactor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaxMemory = table.Column<int>(type: "int", nullable: false),
                    MemorySlots = table.Column<byte>(type: "tinyint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "money", nullable: true),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Motherboard", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PcType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PcType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PowerSupply",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Efficiency = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Wattage = table.Column<int>(type: "int", nullable: false),
                    Modular = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "money", nullable: true),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PowerSupply", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Processor",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CoreCount = table.Column<byte>(type: "tinyint", nullable: false),
                    CoreClock = table.Column<float>(type: "real", nullable: false),
                    BoostClock = table.Column<float>(type: "real", nullable: true),
                    TDP = table.Column<int>(type: "int", nullable: false),
                    Graphics = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SMT = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "money", nullable: true),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Processor", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Ram",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MemoryType = table.Column<byte>(type: "tinyint", nullable: false),
                    Frequency = table.Column<int>(type: "int", nullable: false),
                    ModulesCount = table.Column<byte>(type: "tinyint", nullable: false),
                    ModuleCapacity = table.Column<int>(type: "int", nullable: false),
                    FirstWordLatency = table.Column<double>(type: "float", nullable: false),
                    CASLatency = table.Column<double>(type: "float", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "money", nullable: true),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ram", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Ssd",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Capacity = table.Column<double>(type: "float", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cache = table.Column<int>(type: "int", nullable: true),
                    FormFactor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Interface = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "money", nullable: true),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ssd", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VideoCard",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Chipset = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Memory = table.Column<float>(type: "real", nullable: false),
                    CoreClock = table.Column<int>(type: "int", nullable: true),
                    BoostClock = table.Column<int>(type: "int", nullable: true),
                    Length = table.Column<short>(type: "smallint", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "money", nullable: true),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideoCard", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ColorHdd",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ColorId = table.Column<int>(type: "int", nullable: false),
                    HddId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ColorHdd", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ColorHdd_Color_ColorId",
                        column: x => x.ColorId,
                        principalTable: "Color",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ColorHdd_Hdd_HddId",
                        column: x => x.HddId,
                        principalTable: "Hdd",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ColorMotherboard",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ColorId = table.Column<int>(type: "int", nullable: false),
                    MotherboardId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ColorMotherboard", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ColorMotherboard_Color_ColorId",
                        column: x => x.ColorId,
                        principalTable: "Color",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ColorMotherboard_Motherboard_MotherboardId",
                        column: x => x.MotherboardId,
                        principalTable: "Motherboard",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ColorPowerSupply",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ColorId = table.Column<int>(type: "int", nullable: false),
                    PowerSupplyId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ColorPowerSupply", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ColorPowerSupply_Color_ColorId",
                        column: x => x.ColorId,
                        principalTable: "Color",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ColorPowerSupply_PowerSupply_PowerSupplyId",
                        column: x => x.PowerSupplyId,
                        principalTable: "PowerSupply",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ColorRam",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ColorId = table.Column<int>(type: "int", nullable: false),
                    RamId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ColorRam", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ColorRam_Color_ColorId",
                        column: x => x.ColorId,
                        principalTable: "Color",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ColorRam_Ram_RamId",
                        column: x => x.RamId,
                        principalTable: "Ram",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ColorVideoCard",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ColorId = table.Column<int>(type: "int", nullable: false),
                    VideoCardId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ColorVideoCard", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ColorVideoCard_Color_ColorId",
                        column: x => x.ColorId,
                        principalTable: "Color",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ColorVideoCard_VideoCard_VideoCardId",
                        column: x => x.VideoCardId,
                        principalTable: "VideoCard",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PcConfiguration",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TypeId = table.Column<int>(type: "int", nullable: true),
                    ProcessorId = table.Column<int>(type: "int", nullable: true),
                    VideoCardId = table.Column<int>(type: "int", nullable: true),
                    MotherboardId = table.Column<int>(type: "int", nullable: true),
                    PowerSupplyId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PcConfiguration", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PcConfiguration_Motherboard_MotherboardId",
                        column: x => x.MotherboardId,
                        principalTable: "Motherboard",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PcConfiguration_PcType_TypeId",
                        column: x => x.TypeId,
                        principalTable: "PcType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PcConfiguration_PowerSupply_PowerSupplyId",
                        column: x => x.PowerSupplyId,
                        principalTable: "PowerSupply",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PcConfiguration_Processor_ProcessorId",
                        column: x => x.ProcessorId,
                        principalTable: "Processor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PcConfiguration_VideoCard_VideoCardId",
                        column: x => x.VideoCardId,
                        principalTable: "VideoCard",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PcConfigurationHdd",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PcConfigurationId = table.Column<int>(type: "int", nullable: false),
                    HddId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PcConfigurationHdd", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PcConfigurationHdd_Hdd_HddId",
                        column: x => x.HddId,
                        principalTable: "Hdd",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PcConfigurationHdd_PcConfiguration_PcConfigurationId",
                        column: x => x.PcConfigurationId,
                        principalTable: "PcConfiguration",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PcConfigurationRam",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PcConfigurationId = table.Column<int>(type: "int", nullable: false),
                    RamId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PcConfigurationRam", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PcConfigurationRam_PcConfiguration_PcConfigurationId",
                        column: x => x.PcConfigurationId,
                        principalTable: "PcConfiguration",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PcConfigurationRam_Ram_RamId",
                        column: x => x.RamId,
                        principalTable: "Ram",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PcConfigurationSsd",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PcConfigurationId = table.Column<int>(type: "int", nullable: false),
                    SsdId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PcConfigurationSsd", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PcConfigurationSsd_PcConfiguration_PcConfigurationId",
                        column: x => x.PcConfigurationId,
                        principalTable: "PcConfiguration",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PcConfigurationSsd_Ssd_SsdId",
                        column: x => x.SsdId,
                        principalTable: "Ssd",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Login = table.Column<string>(type: "nvarchar(254)", maxLength: 254, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GoogleId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(254)", maxLength: 254, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    PcConfigurationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_PcConfiguration_PcConfigurationId",
                        column: x => x.PcConfigurationId,
                        principalTable: "PcConfiguration",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Comment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CommentatorId = table.Column<int>(type: "int", nullable: true),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comment_User_CommentatorId",
                        column: x => x.CommentatorId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Comment_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Friendship",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InitiatorId = table.Column<int>(type: "int", nullable: false),
                    FriendId = table.Column<int>(type: "int", nullable: false),
                    FriendshipStatusId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "date", nullable: false),
                    AcceptedAt = table.Column<DateTime>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Friendship", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Friendship_FriendshipStatus_FriendshipStatusId",
                        column: x => x.FriendshipStatusId,
                        principalTable: "FriendshipStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Friendship_User_FriendId",
                        column: x => x.FriendId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Friendship_User_InitiatorId",
                        column: x => x.InitiatorId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Message",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SenderId = table.Column<int>(type: "int", nullable: true),
                    ReceiverId = table.Column<int>(type: "int", nullable: true),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Message", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Message_User_ReceiverId",
                        column: x => x.ReceiverId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Message_User_SenderId",
                        column: x => x.SenderId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Color_Name",
                table: "Color",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ColorHdd_ColorId",
                table: "ColorHdd",
                column: "ColorId");

            migrationBuilder.CreateIndex(
                name: "IX_ColorHdd_HddId",
                table: "ColorHdd",
                column: "HddId");

            migrationBuilder.CreateIndex(
                name: "IX_ColorMotherboard_ColorId",
                table: "ColorMotherboard",
                column: "ColorId");

            migrationBuilder.CreateIndex(
                name: "IX_ColorMotherboard_MotherboardId",
                table: "ColorMotherboard",
                column: "MotherboardId");

            migrationBuilder.CreateIndex(
                name: "IX_ColorPowerSupply_ColorId",
                table: "ColorPowerSupply",
                column: "ColorId");

            migrationBuilder.CreateIndex(
                name: "IX_ColorPowerSupply_PowerSupplyId",
                table: "ColorPowerSupply",
                column: "PowerSupplyId");

            migrationBuilder.CreateIndex(
                name: "IX_ColorRam_ColorId",
                table: "ColorRam",
                column: "ColorId");

            migrationBuilder.CreateIndex(
                name: "IX_ColorRam_RamId",
                table: "ColorRam",
                column: "RamId");

            migrationBuilder.CreateIndex(
                name: "IX_ColorVideoCard_ColorId",
                table: "ColorVideoCard",
                column: "ColorId");

            migrationBuilder.CreateIndex(
                name: "IX_ColorVideoCard_VideoCardId",
                table: "ColorVideoCard",
                column: "VideoCardId");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_CommentatorId",
                table: "Comment",
                column: "CommentatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_UserId",
                table: "Comment",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Friendship_FriendId",
                table: "Friendship",
                column: "FriendId");

            migrationBuilder.CreateIndex(
                name: "IX_Friendship_FriendshipStatusId",
                table: "Friendship",
                column: "FriendshipStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Friendship_InitiatorId",
                table: "Friendship",
                column: "InitiatorId");

            migrationBuilder.CreateIndex(
                name: "IX_FriendshipStatus_Status",
                table: "FriendshipStatus",
                column: "Status",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Message_ReceiverId",
                table: "Message",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_Message_SenderId",
                table: "Message",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_PcConfiguration_MotherboardId",
                table: "PcConfiguration",
                column: "MotherboardId");

            migrationBuilder.CreateIndex(
                name: "IX_PcConfiguration_PowerSupplyId",
                table: "PcConfiguration",
                column: "PowerSupplyId");

            migrationBuilder.CreateIndex(
                name: "IX_PcConfiguration_ProcessorId",
                table: "PcConfiguration",
                column: "ProcessorId");

            migrationBuilder.CreateIndex(
                name: "IX_PcConfiguration_TypeId",
                table: "PcConfiguration",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PcConfiguration_VideoCardId",
                table: "PcConfiguration",
                column: "VideoCardId");

            migrationBuilder.CreateIndex(
                name: "IX_PcConfigurationHdd_HddId",
                table: "PcConfigurationHdd",
                column: "HddId");

            migrationBuilder.CreateIndex(
                name: "IX_PcConfigurationHdd_PcConfigurationId",
                table: "PcConfigurationHdd",
                column: "PcConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_PcConfigurationRam_PcConfigurationId",
                table: "PcConfigurationRam",
                column: "PcConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_PcConfigurationRam_RamId",
                table: "PcConfigurationRam",
                column: "RamId");

            migrationBuilder.CreateIndex(
                name: "IX_PcConfigurationSsd_PcConfigurationId",
                table: "PcConfigurationSsd",
                column: "PcConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_PcConfigurationSsd_SsdId",
                table: "PcConfigurationSsd",
                column: "SsdId");

            migrationBuilder.CreateIndex(
                name: "IX_PcType_Name",
                table: "PcType",
                column: "Name",
                unique: true,
                filter: "[Name] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_User_Email",
                table: "User",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_Login",
                table: "User",
                column: "Login",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_PcConfigurationId",
                table: "User",
                column: "PcConfigurationId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ColorHdd");

            migrationBuilder.DropTable(
                name: "ColorMotherboard");

            migrationBuilder.DropTable(
                name: "ColorPowerSupply");

            migrationBuilder.DropTable(
                name: "ColorRam");

            migrationBuilder.DropTable(
                name: "ColorVideoCard");

            migrationBuilder.DropTable(
                name: "Comment");

            migrationBuilder.DropTable(
                name: "Friendship");

            migrationBuilder.DropTable(
                name: "Message");

            migrationBuilder.DropTable(
                name: "PcConfigurationHdd");

            migrationBuilder.DropTable(
                name: "PcConfigurationRam");

            migrationBuilder.DropTable(
                name: "PcConfigurationSsd");

            migrationBuilder.DropTable(
                name: "Color");

            migrationBuilder.DropTable(
                name: "FriendshipStatus");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Hdd");

            migrationBuilder.DropTable(
                name: "Ram");

            migrationBuilder.DropTable(
                name: "Ssd");

            migrationBuilder.DropTable(
                name: "PcConfiguration");

            migrationBuilder.DropTable(
                name: "Motherboard");

            migrationBuilder.DropTable(
                name: "PcType");

            migrationBuilder.DropTable(
                name: "PowerSupply");

            migrationBuilder.DropTable(
                name: "Processor");

            migrationBuilder.DropTable(
                name: "VideoCard");
        }
    }
}
