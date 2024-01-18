using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymDB.API.Migrations
{
    /// <inheritdoc />
    public partial class addedexerciseentityreal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Exercises",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(130)", maxLength: 130, nullable: false),
                    Instructions = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    MuscleGroups = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Difficulty = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: false),
                    Equipment = table.Column<string>(type: "text", nullable: true),
                    IsCustom = table.Column<bool>(type: "boolean", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exercises", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Exercises_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Exercises_UserId",
                table: "Exercises",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Exercises");
        }
    }
}
