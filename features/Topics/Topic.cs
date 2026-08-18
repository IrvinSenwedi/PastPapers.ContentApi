using PastPapers.ContentApi.Features.Questions;
using PastPapers.ContentApi.Features.Subjects;

namespace PastPapers.ContentApi.Features.Topics;

public sealed class Topic
{
    public Guid Id { get; set; }

    public Guid SubjectId { get; set; }

    public required string Name { get; set; }

    public required string Slug { get; set; }

    public short Grade { get; set; }

    public int DisplayOrder { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public Subject Subject { get; set; } = null!;

    public ICollection<Question> Questions { get; set; } = [];
}