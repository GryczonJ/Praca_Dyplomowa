using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inzynierka.Migrations
{
    /// <inheritdoc />
    public partial class banreason : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "reasonBan",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "reasonBan",
                table: "AspNetUsers");
        }
    }
}
