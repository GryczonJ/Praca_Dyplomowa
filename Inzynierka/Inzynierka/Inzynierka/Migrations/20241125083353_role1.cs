using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inzynierka.Migrations
{
    /// <inheritdoc />
    public partial class role1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    certificates = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    age = table.Column<int>(type: "int", nullable: false),
                    surName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    aboutMe = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    firstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    lastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    banned = table.Column<bool>(type: "bit", nullable: false),
                    Public = table.Column<bool>(type: "bit", nullable: false),
                    PhotosId = table.Column<int>(type: "int", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    message = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ReadDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Charters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    startDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    endDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    location = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    YachtId = table.Column<int>(type: "int", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Charters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Charters_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Resservation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    startDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    endDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CharterId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resservation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Resservation_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Resservation_Charters_CharterId",
                        column: x => x.CharterId,
                        principalTable: "Charters",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Message = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CharterId = table.Column<int>(type: "int", nullable: true),
                    CruisesId = table.Column<int>(type: "int", nullable: true),
                    YachtsId = table.Column<int>(type: "int", nullable: true),
                    YachtSaleId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comments_AspNetUsers_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Comments_AspNetUsers_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Comments_Charters_CharterId",
                        column: x => x.CharterId,
                        principalTable: "Charters",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CruiseJoinRequest",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CruiseId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CapitanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CruiseJoinRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CruiseJoinRequest_AspNetUsers_CapitanId",
                        column: x => x.CapitanId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CruiseJoinRequest_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Cruises",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    destination = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    start_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    end_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    maxParticipants = table.Column<int>(type: "int", nullable: false),
                    YachtId = table.Column<int>(type: "int", nullable: false),
                    CapitanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cruises", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cruises_AspNetUsers_CapitanId",
                        column: x => x.CapitanId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CruisesParticipants",
                columns: table => new
                {
                    UsersId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CruisesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CruisesParticipants", x => new { x.UsersId, x.CruisesId });
                    table.ForeignKey(
                        name: "FK_CruisesParticipants_AspNetUsers_UsersId",
                        column: x => x.UsersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CruisesParticipants_Cruises_CruisesId",
                        column: x => x.CruisesId,
                        principalTable: "Cruises",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "FavoriteCruises",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CruiseId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FavoriteCruises", x => new { x.UserId, x.CruiseId });
                    table.ForeignKey(
                        name: "FK_FavoriteCruises_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FavoriteCruises_Cruises_CruiseId",
                        column: x => x.CruiseId,
                        principalTable: "Cruises",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "FavoriteYachtsForSale",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    YachtSaleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FavoriteYachtsForSale", x => new { x.UserId, x.YachtSaleId });
                    table.ForeignKey(
                        name: "FK_FavoriteYachtsForSale_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Image",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    link = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    YachtSaleId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Image", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Yachts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    type = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    manufacturer = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    model = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    year = table.Column<DateTime>(type: "datetime2", nullable: false),
                    length = table.Column<double>(type: "float", nullable: false),
                    width = table.Column<double>(type: "float", nullable: false),
                    crew = table.Column<int>(type: "int", nullable: false),
                    cabins = table.Column<int>(type: "int", nullable: false),
                    beds = table.Column<int>(type: "int", nullable: false),
                    toilets = table.Column<int>(type: "int", nullable: false),
                    showers = table.Column<int>(type: "int", nullable: false),
                    location = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    capacity = table.Column<int>(type: "int", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ImageId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Yachts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Yachts_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Yachts_Image_ImageId",
                        column: x => x.ImageId,
                        principalTable: "Image",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "YachtSale",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    saleDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    location = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    availabilityStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    YachtId = table.Column<int>(type: "int", nullable: false),
                    BuyerUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OwnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YachtSale", x => x.Id);
                    table.ForeignKey(
                        name: "FK_YachtSale_AspNetUsers_BuyerUserId",
                        column: x => x.BuyerUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_YachtSale_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_YachtSale_Yachts_YachtId",
                        column: x => x.YachtId,
                        principalTable: "Yachts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Reports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    message = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ModeratorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SuspectUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SuspectCruiseId = table.Column<int>(type: "int", nullable: false),
                    SuspectYachtId = table.Column<int>(type: "int", nullable: false),
                    DocumentVerificationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SuspectYachtSaleId = table.Column<int>(type: "int", nullable: false),
                    SuspectCharterId = table.Column<int>(type: "int", nullable: false),
                    SuspectCommentId = table.Column<int>(type: "int", nullable: false),
                    SuspectRoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reports_AspNetRoles_DocumentVerificationId",
                        column: x => x.DocumentVerificationId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Reports_AspNetRoles_SuspectRoleId",
                        column: x => x.SuspectRoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Reports_AspNetUsers_ModeratorId",
                        column: x => x.ModeratorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Reports_AspNetUsers_SuspectUserId",
                        column: x => x.SuspectUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Reports_Charters_SuspectCharterId",
                        column: x => x.SuspectCharterId,
                        principalTable: "Charters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Reports_Comments_SuspectCommentId",
                        column: x => x.SuspectCommentId,
                        principalTable: "Comments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Reports_Cruises_SuspectCruiseId",
                        column: x => x.SuspectCruiseId,
                        principalTable: "Cruises",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Reports_YachtSale_SuspectYachtSaleId",
                        column: x => x.SuspectYachtSaleId,
                        principalTable: "YachtSale",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Reports_Yachts_SuspectYachtId",
                        column: x => x.SuspectYachtId,
                        principalTable: "Yachts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_PhotosId",
                table: "AspNetUsers",
                column: "PhotosId");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Charters_OwnerId",
                table: "Charters",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Charters_YachtId",
                table: "Charters",
                column: "YachtId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_CharterId",
                table: "Comments",
                column: "CharterId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_CreatorId",
                table: "Comments",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_CruisesId",
                table: "Comments",
                column: "CruisesId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ProfileId",
                table: "Comments",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_YachtSaleId",
                table: "Comments",
                column: "YachtSaleId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_YachtsId",
                table: "Comments",
                column: "YachtsId");

            migrationBuilder.CreateIndex(
                name: "IX_CruiseJoinRequest_CapitanId",
                table: "CruiseJoinRequest",
                column: "CapitanId");

            migrationBuilder.CreateIndex(
                name: "IX_CruiseJoinRequest_CruiseId",
                table: "CruiseJoinRequest",
                column: "CruiseId");

            migrationBuilder.CreateIndex(
                name: "IX_CruiseJoinRequest_UserId",
                table: "CruiseJoinRequest",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Cruises_CapitanId",
                table: "Cruises",
                column: "CapitanId");

            migrationBuilder.CreateIndex(
                name: "IX_Cruises_YachtId",
                table: "Cruises",
                column: "YachtId");

            migrationBuilder.CreateIndex(
                name: "IX_CruisesParticipants_CruisesId",
                table: "CruisesParticipants",
                column: "CruisesId");

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteCruises_CruiseId",
                table: "FavoriteCruises",
                column: "CruiseId");

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteYachtsForSale_YachtSaleId",
                table: "FavoriteYachtsForSale",
                column: "YachtSaleId");

            migrationBuilder.CreateIndex(
                name: "IX_Image_YachtSaleId",
                table: "Image",
                column: "YachtSaleId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_DocumentVerificationId",
                table: "Reports",
                column: "DocumentVerificationId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_ModeratorId",
                table: "Reports",
                column: "ModeratorId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_SuspectCharterId",
                table: "Reports",
                column: "SuspectCharterId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_SuspectCommentId",
                table: "Reports",
                column: "SuspectCommentId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_SuspectCruiseId",
                table: "Reports",
                column: "SuspectCruiseId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_SuspectRoleId",
                table: "Reports",
                column: "SuspectRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_SuspectUserId",
                table: "Reports",
                column: "SuspectUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_SuspectYachtId",
                table: "Reports",
                column: "SuspectYachtId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_SuspectYachtSaleId",
                table: "Reports",
                column: "SuspectYachtSaleId");

            migrationBuilder.CreateIndex(
                name: "IX_Resservation_CharterId",
                table: "Resservation",
                column: "CharterId");

            migrationBuilder.CreateIndex(
                name: "IX_Resservation_UserId",
                table: "Resservation",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Yachts_ImageId",
                table: "Yachts",
                column: "ImageId");

            migrationBuilder.CreateIndex(
                name: "IX_Yachts_OwnerId",
                table: "Yachts",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_YachtSale_BuyerUserId",
                table: "YachtSale",
                column: "BuyerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_YachtSale_OwnerId",
                table: "YachtSale",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_YachtSale_YachtId",
                table: "YachtSale",
                column: "YachtId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Image_PhotosId",
                table: "AspNetUsers",
                column: "PhotosId",
                principalTable: "Image",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Charters_Yachts_YachtId",
                table: "Charters",
                column: "YachtId",
                principalTable: "Yachts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Cruises_CruisesId",
                table: "Comments",
                column: "CruisesId",
                principalTable: "Cruises",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_YachtSale_YachtSaleId",
                table: "Comments",
                column: "YachtSaleId",
                principalTable: "YachtSale",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Yachts_YachtsId",
                table: "Comments",
                column: "YachtsId",
                principalTable: "Yachts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CruiseJoinRequest_Cruises_CruiseId",
                table: "CruiseJoinRequest",
                column: "CruiseId",
                principalTable: "Cruises",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Cruises_Yachts_YachtId",
                table: "Cruises",
                column: "YachtId",
                principalTable: "Yachts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FavoriteYachtsForSale_YachtSale_YachtSaleId",
                table: "FavoriteYachtsForSale",
                column: "YachtSaleId",
                principalTable: "YachtSale",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Image_YachtSale_YachtSaleId",
                table: "Image",
                column: "YachtSaleId",
                principalTable: "YachtSale",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Yachts_AspNetUsers_OwnerId",
                table: "Yachts");

            migrationBuilder.DropForeignKey(
                name: "FK_YachtSale_AspNetUsers_BuyerUserId",
                table: "YachtSale");

            migrationBuilder.DropForeignKey(
                name: "FK_YachtSale_AspNetUsers_OwnerId",
                table: "YachtSale");

            migrationBuilder.DropForeignKey(
                name: "FK_Yachts_Image_ImageId",
                table: "Yachts");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "CruiseJoinRequest");

            migrationBuilder.DropTable(
                name: "CruisesParticipants");

            migrationBuilder.DropTable(
                name: "FavoriteCruises");

            migrationBuilder.DropTable(
                name: "FavoriteYachtsForSale");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "Reports");

            migrationBuilder.DropTable(
                name: "Resservation");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "Charters");

            migrationBuilder.DropTable(
                name: "Cruises");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Image");

            migrationBuilder.DropTable(
                name: "YachtSale");

            migrationBuilder.DropTable(
                name: "Yachts");
        }
    }
}
