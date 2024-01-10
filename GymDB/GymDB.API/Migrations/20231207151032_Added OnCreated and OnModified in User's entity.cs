using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymDB.API.Migrations
{
    /// <inheritdoc />
    public partial class AddedOnCreatedandOnModifiedinUsersentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "OnCreated",
                table: "Users",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<DateTime>(
                name: "OnModified",
                table: "Users",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OnCreated",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "OnModified",
                table: "Users");
        }
    }
}
