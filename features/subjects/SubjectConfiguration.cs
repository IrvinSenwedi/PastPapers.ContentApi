using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PastPapers.ContentApi.Features.Subjects;

public sealed class SubjectConfiguration : IEntityTypeConfiguration<Subject>
{
    public void Configure(EntityTypeBuilder<Subject> builder)
    {
        builder.ToTable("subjects");

        builder.HasKey(subject => subject.Id);

        builder.Property(subject => subject.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(subject => subject.Name)
            .HasColumnName("name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(subject => subject.Slug)
            .HasColumnName("slug")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(subject => subject.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("now()")
            .IsRequired();

        builder.HasIndex(subject => subject.Name)
            .IsUnique();

        builder.HasIndex(subject => subject.Slug)
            .IsUnique();

        builder.HasMany(subject => subject.Topics)
            .WithOne(topic => topic.Subject)
            .HasForeignKey(topic => topic.SubjectId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}