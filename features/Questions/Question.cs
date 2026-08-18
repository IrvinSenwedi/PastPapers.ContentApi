using System;
using PastPapers.ContentApi.Features.Topics;

namespace PastPapers.ContentApi.Features.Questions;

public sealed class Question
{
    public Guid Id { get; set; }

    public Guid TopicId { get; set; }

    public short ExamYear { get; set; }

    public required string ExamSeason { get; set; }

    public short PaperNumber { get; set; }

    public required string QuestionNumber { get; set; }

    public required string QuestionImageUrl { get; set; }

    public required string MemoImageUrl { get; set; }

    public QuestionStatus Status { get; set; } = QuestionStatus.Draft;

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }

    public Topic Topic { get; set; } = null!;
}