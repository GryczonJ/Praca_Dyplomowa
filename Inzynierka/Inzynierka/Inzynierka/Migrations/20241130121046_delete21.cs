using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inzynierka.Migrations
{
    /// <inheritdoc />
    public partial class delete21 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cruises_Yachts_YachtId",
                table: "Cruises");

            migrationBuilder.AddForeignKey(
                name: "FK_Cruises_Yachts_YachtId",
                table: "Cruises",
                column: "YachtId",
                principalTable: "Yachts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cruises_Yachts_YachtId",
                table: "Cruises");

            migrationBuilder.AddForeignKey(
                name: "FK_Cruises_Yachts_YachtId",
                table: "Cruises",
                column: "YachtId",
                principalTable: "Yachts",
                principalColumn: "Id");
        }
    }
}
