using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PastPapers.ContentApi.Features.Topics;

public sealed class TopicConfiguration : IEntityTypeConfiguration<Topic>
{
    public void Configure(EntityTypeBuilder<Topic> builder)
    {
        builder.ToTable(
            "topics",
            table => table.HasCheckConstraint(
                "ck_topics_grade",
                "grade BETWEEN 10 AND 12"));

        builder.HasKey(topic => topic.Id);

        builder.Property(topic => topic.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(topic => topic.SubjectId)
            .HasColumnName("subject_id")
            .IsRequired();

        builder.Property(topic => topic.Name)
            .HasColumnName("name")
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(topic => topic.Slug)
            .HasColumnName("slug")
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(topic => topic.Grade)
            .HasColumnName("grade")
            .IsRequired();

        builder.Property(topic => topic.DisplayOrder)
            .HasColumnName("display_order")
            .HasDefaultValue(0)
            .IsRequired();

        builder.Property(topic => topic.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("now()")
            .IsRequired();

        builder.HasIndex(topic => new
            {
                topic.SubjectId,
                topic.Grade,
                topic.Slug
            })
            .IsUnique();

        builder.HasMany(topic => topic.Questions)
            .WithOne(question => question.Topic)
            .HasForeignKey(question => question.TopicId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}