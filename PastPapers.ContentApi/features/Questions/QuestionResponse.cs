namespace PastPapers.ContentApi.Features.Questions;

public sealed record QuestionResponse(
    Guid Id,
    string QuestionNumber,
    int DisplayOrder,
    short ExamYear,
    string ExamSeason,
    short PaperNumber,
    string QuestionImageUrl,
    string MemoImageUrl,
    Guid TopicId,
    string TopicName,
    string TopicSlug,
    short Grade,
    Guid SubjectId,
    string SubjectName,
    string SubjectSlug);