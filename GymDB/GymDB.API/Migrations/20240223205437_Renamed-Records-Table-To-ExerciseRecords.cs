using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymDB.API.Migrations
{
    /// <inheritdoc />
    public partial class RenamedRecordsTableToExerciseRecords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Records_Exercises_ExerciseId",
                table: "Records");

            migrationBuilder.DropForeignKey(
                name: "FK_Records_Users_UserId",
                table: "Records");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Records",
                table: "Records");

            migrationBuilder.RenameTable(
                name: "Records",
                newName: "ExerciseRecords");

            migrationBuilder.RenameIndex(
                name: "IX_Records_UserId",
                table: "ExerciseRecords",
                newName: "IX_ExerciseRecords_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Records_ExerciseId",
                table: "ExerciseRecords",
                newName: "IX_ExerciseRecords_ExerciseId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExerciseRecords",
                table: "ExerciseRecords",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ExerciseRecords_Exercises_ExerciseId",
                table: "ExerciseRecords",
                column: "ExerciseId",
                principalTable: "Exercises",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ExerciseRecords_Users_UserId",
                table: "ExerciseRecords",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExerciseRecords_Exercises_ExerciseId",
                table: "ExerciseRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_ExerciseRecords_Users_UserId",
                table: "ExerciseRecords");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ExerciseRecords",
                table: "ExerciseRecords");

            migrationBuilder.RenameTable(
                name: "ExerciseRecords",
                newName: "Records");

            migrationBuilder.RenameIndex(
                name: "IX_ExerciseRecords_UserId",
                table: "Records",
                newName: "IX_Records_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_ExerciseRecords_ExerciseId",
                table: "Records",
                newName: "IX_Records_ExerciseId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Records",
                table: "Records",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Records_Exercises_ExerciseId",
                table: "Records",
                column: "ExerciseId",
                principalTable: "Exercises",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Records_Users_UserId",
                table: "Records",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
