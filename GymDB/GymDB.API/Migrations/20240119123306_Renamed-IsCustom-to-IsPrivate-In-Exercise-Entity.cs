using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymDB.API.Migrations
{
    /// <inheritdoc />
    public partial class RenamedIsCustomtoIsPrivateInExerciseEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsCustom",
                table: "Exercises",
                newName: "IsPrivate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsPrivate",
                table: "Exercises",
                newName: "IsCustom");
        }
    }
}
