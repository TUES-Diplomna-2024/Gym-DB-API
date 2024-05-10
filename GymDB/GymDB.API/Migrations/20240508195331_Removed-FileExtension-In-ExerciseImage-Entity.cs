using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymDB.API.Migrations
{
    /// <inheritdoc />
    public partial class RemovedFileExtensionInExerciseImageEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileExtension",
                table: "ExerciseImages");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FileExtension",
                table: "ExerciseImages",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }
    }
}
