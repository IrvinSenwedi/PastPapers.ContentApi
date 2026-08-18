
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PastPapers.ContentApi.Features.Questions;

public sealed class QuestionConfiguration : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        builder.ToTable(
            "questions",
            table =>
            {
                table.HasCheckConstraint(
                    "ck_questions_exam_year",
                    "exam_year BETWEEN 1996 AND 2100");

                table.HasCheckConstraint(
                    "ck_questions_paper_number",
                    "paper_number BETWEEN 1 AND 4");
            });

        builder.HasKey(question => question.Id);

        builder.Property(question => question.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(question => question.TopicId)
            .HasColumnName("topic_id")
            .IsRequired();

        builder.Property(question => question.ExamYear)
            .HasColumnName("exam_year")
            .IsRequired();

        builder.Property(question => question.ExamSeason)
            .HasColumnName("exam_season")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(question => question.PaperNumber)
            .HasColumnName("paper_number")
            .IsRequired();

        builder.Property(question => question.QuestionNumber)
            .HasColumnName("question_number")
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(question => question.QuestionImageUrl)
            .HasColumnName("question_image_url")
            .HasColumnType("text")
            .IsRequired();

        builder.Property(question => question.DisplayOrder)
            .HasColumnName("display_order")
            .IsRequired();

        builder.Property(question => question.MemoImageUrl)
            .HasColumnName("memo_image_url")
            .HasColumnType("text")
            .IsRequired();

        builder.Property(question => question.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(QuestionStatus.Draft)
            .IsRequired();

        builder.Property(question => question.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("now()")
            .IsRequired();

        builder.Property(question => question.UpdatedAt)
            .HasColumnName("updated_at")
            .HasDefaultValueSql("now()")
            .IsRequired();

        builder.HasIndex(question => new
            {
                question.TopicId,
                question.ExamYear,
                question.ExamSeason,
                question.PaperNumber,
                question.DisplayOrder,
                question.QuestionNumber
            })
            .IsUnique();

        builder.HasIndex(question => question.Status);
    }
}