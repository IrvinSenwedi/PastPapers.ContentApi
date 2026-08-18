using System;
namespace PastPapers.ContentApi.Features.Topics;

public sealed record TopicResponse(
    Guid Id,
    string Name,
    string Slug,
    short Grade,
    int DisplayOrder,
    Guid SubjectId,
    string SubjectName,
    string SubjectSlug,
    int QuestionCount);