namespace PastPapers.ContentApi.Features.Questions;

public sealed record CreateQuestionRequest(
    string? SubjectSlug,
    short Grade,
    string? TopicSlug,
    short ExamYear,
    string? ExamSeason,
    short PaperNumber,
    string? QuestionNumber,
    int DisplayOrder,
    string? QuestionImageUrl,
    string? MemoImageUrl);