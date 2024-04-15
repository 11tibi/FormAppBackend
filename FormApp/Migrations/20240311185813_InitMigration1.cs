using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FormApp.Migrations
{
    /// <inheritdoc />
    public partial class InitMigration1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SelectedOptions_AnswerId",
                table: "SelectedOptions");

            migrationBuilder.DropIndex(
                name: "IX_Responses_FormId",
                table: "Responses");

            migrationBuilder.DropIndex(
                name: "IX_Answers_QuestionId",
                table: "Answers");

            migrationBuilder.CreateIndex(
                name: "IX_SelectedOptions_AnswerId",
                table: "SelectedOptions",
                column: "AnswerId");

            migrationBuilder.CreateIndex(
                name: "IX_Responses_FormId",
                table: "Responses",
                column: "FormId");

            migrationBuilder.CreateIndex(
                name: "IX_Answers_QuestionId",
                table: "Answers",
                column: "QuestionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SelectedOptions_AnswerId",
                table: "SelectedOptions");

            migrationBuilder.DropIndex(
                name: "IX_Responses_FormId",
                table: "Responses");

            migrationBuilder.DropIndex(
                name: "IX_Answers_QuestionId",
                table: "Answers");

            migrationBuilder.CreateIndex(
                name: "IX_SelectedOptions_AnswerId",
                table: "SelectedOptions",
                column: "AnswerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Responses_FormId",
                table: "Responses",
                column: "FormId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Answers_QuestionId",
                table: "Answers",
                column: "QuestionId",
                unique: true);
        }
    }
}
