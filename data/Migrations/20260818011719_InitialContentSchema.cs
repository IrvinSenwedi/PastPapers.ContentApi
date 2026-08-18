using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PastPapers.ContentApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialContentSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "content");

            migrationBuilder.CreateTable(
                name: "subjects",
                schema: "content",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    slug = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_subjects", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "topics",
                schema: "content",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    subject_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    slug = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    grade = table.Column<short>(type: "smallint", nullable: false),
                    display_order = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_topics", x => x.id);
                    table.CheckConstraint("ck_topics_grade", "grade BETWEEN 10 AND 12");
                    table.ForeignKey(
                        name: "FK_topics_subjects_subject_id",
                        column: x => x.subject_id,
                        principalSchema: "content",
                        principalTable: "subjects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "questions",
                schema: "content",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    topic_id = table.Column<Guid>(type: "uuid", nullable: false),
                    exam_year = table.Column<short>(type: "smallint", nullable: false),
                    exam_season = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    paper_number = table.Column<short>(type: "smallint", nullable: false),
                    question_number = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    question_image_url = table.Column<string>(type: "text", nullable: false),
                    memo_image_url = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Draft"),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_questions", x => x.id);
                    table.CheckConstraint("ck_questions_exam_year", "exam_year BETWEEN 1996 AND 2100");
                    table.CheckConstraint("ck_questions_paper_number", "paper_number BETWEEN 1 AND 4");
                    table.ForeignKey(
                        name: "FK_questions_topics_topic_id",
                        column: x => x.topic_id,
                        principalSchema: "content",
                        principalTable: "topics",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_questions_status",
                schema: "content",
                table: "questions",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_questions_topic_id_exam_year_exam_season_paper_number_quest~",
                schema: "content",
                table: "questions",
                columns: new[] { "topic_id", "exam_year", "exam_season", "paper_number", "question_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_subjects_name",
                schema: "content",
                table: "subjects",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_subjects_slug",
                schema: "content",
                table: "subjects",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_topics_subject_id_grade_slug",
                schema: "content",
                table: "topics",
                columns: new[] { "subject_id", "grade", "slug" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "questions",
                schema: "content");

            migrationBuilder.DropTable(
                name: "topics",
                schema: "content");

            migrationBuilder.DropTable(
                name: "subjects",
                schema: "content");
        }
    }
}
