using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PastPapers.ContentApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddQuestionDisplayOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_questions_topic_id_exam_year_exam_season_paper_number_quest~",
                schema: "content",
                table: "questions");

            migrationBuilder.AddColumn<int>(
                name: "display_order",
                schema: "content",
                table: "questions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_questions_topic_id_exam_year_exam_season_paper_number_displ~",
                schema: "content",
                table: "questions",
                columns: new[] { "topic_id", "exam_year", "exam_season", "paper_number", "display_order", "question_number" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_questions_topic_id_exam_year_exam_season_paper_number_displ~",
                schema: "content",
                table: "questions");

            migrationBuilder.DropColumn(
                name: "display_order",
                schema: "content",
                table: "questions");

            migrationBuilder.CreateIndex(
                name: "IX_questions_topic_id_exam_year_exam_season_paper_number_quest~",
                schema: "content",
                table: "questions",
                columns: new[] { "topic_id", "exam_year", "exam_season", "paper_number", "question_number" },
                unique: true);
        }
    }
}
