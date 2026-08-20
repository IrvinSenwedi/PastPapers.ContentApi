namespace PastPapers.ContentApi.Features.Questions;

public sealed record QuestionFilterOptionsResponse(
    IReadOnlyList<short> ExamYears,
    IReadOnlyList<string> ExamSeasons);