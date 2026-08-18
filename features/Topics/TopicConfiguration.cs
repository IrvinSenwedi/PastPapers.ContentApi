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

        builder.HasData(
    new
    {
        Id = Guid.Parse("2fe0d037-2212-4134-8dd5-59000f040d90"),
        SubjectId = Guid.Parse("d8475ca5-e465-4c16-a445-e9f043054976"),
        Name = "Algebra, Equations, and Inequalities",
        Slug = "algebra-equations-and-inequalities",
        Grade = (short)12,
        DisplayOrder = 1,
        CreatedAt = new DateTimeOffset(
            2026, 8, 18, 0, 0, 0, TimeSpan.Zero)
    },
    new
    {
        Id = Guid.Parse("50461d4e-9824-4d5c-ae08-3fcaa46bfd9f"),
        SubjectId = Guid.Parse("4d8aa865-32d6-41fa-a1cf-f9a09fdcd7cc"),
        Name = "Newtonian Mechanics",
        Slug = "newtonian-mechanics",
        Grade = (short)12,
        DisplayOrder = 1,
        CreatedAt = new DateTimeOffset(
            2026, 8, 18, 0, 0, 0, TimeSpan.Zero)
    },
    new
    {
        Id = Guid.Parse("62bbbf58-791b-40dd-8bb4-1294a04a5897"),
        SubjectId = Guid.Parse("4d8aa865-32d6-41fa-a1cf-f9a09fdcd7cc"),
        Name = "Doppler Effect",
        Slug = "doppler-effect",
        Grade = (short)12,
        DisplayOrder = 2,
        CreatedAt = new DateTimeOffset(
            2026, 8, 18, 0, 0, 0, TimeSpan.Zero)
    },
    new
    {
        Id = Guid.Parse("2a9eaed1-51a4-47d1-958a-c87982cdcf4d"),
        SubjectId = Guid.Parse("4d8aa865-32d6-41fa-a1cf-f9a09fdcd7cc"),
        Name = "Electricity and Magnetism",
        Slug = "electricity-and-magnetism",
        Grade = (short)12,
        DisplayOrder = 3,
        CreatedAt = new DateTimeOffset(
            2026, 8, 18, 0, 0, 0, TimeSpan.Zero)
    },
    new
    {
        Id = Guid.Parse("6472c568-1ac8-447d-8526-66196e7a164b"),
        SubjectId = Guid.Parse("4d8aa865-32d6-41fa-a1cf-f9a09fdcd7cc"),
        Name = "Electrodynamics",
        Slug = "electrodynamics",
        Grade = (short)12,
        DisplayOrder = 4,
        CreatedAt = new DateTimeOffset(
            2026, 8, 18, 0, 0, 0, TimeSpan.Zero)
    },
    new
    {
        Id = Guid.Parse("70022de8-3747-4a58-a872-378cf299b9e1"),
        SubjectId = Guid.Parse("4d8aa865-32d6-41fa-a1cf-f9a09fdcd7cc"),
        Name = "Optical Phenomena",
        Slug = "optical-phenomena",
        Grade = (short)12,
        DisplayOrder = 5,
        CreatedAt = new DateTimeOffset(
            2026, 8, 18, 0, 0, 0, TimeSpan.Zero)
    });


        builder.HasMany(topic => topic.Questions)
            .WithOne(question => question.Topic)
            .HasForeignKey(question => question.TopicId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}