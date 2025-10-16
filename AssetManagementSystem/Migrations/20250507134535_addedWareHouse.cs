using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AssetManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class addedWareHouse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "modifiedDate",
                table: "DT_USER",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "warehouse",
                table: "DT_STOCK",
                type: "int",
                nullable: true,
                defaultValue: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "modifiedDate",
                table: "DT_USER");

            migrationBuilder.DropColumn(
                name: "warehouse",
                table: "DT_STOCK");
        }
    }
}
