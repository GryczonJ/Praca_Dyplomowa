using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inzynierka.Migrations
{
    /// <inheritdoc />
    public partial class banowanie : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "banned",
                table: "YachtSale",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "banned",
                table: "Yachts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "banned",
                table: "Resservation",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "banned",
                table: "Image",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "banned",
                table: "Cruises",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "banned",
                table: "Comments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "banned",
                table: "Charters",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "banned",
                table: "YachtSale");

            migrationBuilder.DropColumn(
                name: "banned",
                table: "Yachts");

            migrationBuilder.DropColumn(
                name: "banned",
                table: "Resservation");

            migrationBuilder.DropColumn(
                name: "banned",
                table: "Image");

            migrationBuilder.DropColumn(
                name: "banned",
                table: "Cruises");

            migrationBuilder.DropColumn(
                name: "banned",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "banned",
                table: "Charters");
        }
    }
}
