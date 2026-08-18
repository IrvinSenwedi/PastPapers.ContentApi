namespace PastPapers.ContentApi.Features.Subjects;

public sealed record SubjectResponse(
    Guid Id,
    string Name,
    string Slug);