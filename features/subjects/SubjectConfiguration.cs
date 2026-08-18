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
        
        
        builder.HasData(
            new
            {
                Id = Guid.Parse("d8475ca5-e465-4c16-a445-e9f043054976"),
                Name = "Mathematics",
                Slug = "mathematics",
                CreatedAt = new DateTimeOffset(
                    2026, 8, 18, 0, 0, 0, TimeSpan.Zero)
            },
            new
            {
                Id = Guid.Parse("4d8aa865-32d6-41fa-a1cf-f9a09fdcd7cc"),
                Name = "Physical Sciences",
                Slug = "physical-sciences",
                CreatedAt = new DateTimeOffset(
                    2026, 8, 18, 0, 0, 0, TimeSpan.Zero)
            });

        builder.HasMany(subject => subject.Topics);

        builder.HasMany(subject => subject.Topics)
            .WithOne(topic => topic.Subject)
            .HasForeignKey(topic => topic.SubjectId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}