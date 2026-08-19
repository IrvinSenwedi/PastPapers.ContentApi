using PastPapers.ContentApi.Features.Questions;

namespace PastPapers.ContentApi.Tests.Features.Questions;

public sealed class CreateQuestionRequestValidatorTests
{
    [Fact]
    public void Validate_WithValidRequest_ReturnsNoErrors()
    {
        var request = CreateValidRequest();

        var errors =
            CreateQuestionRequestValidator.Validate(request);

        Assert.Empty(errors);
    }

    [Fact]
    public void Validate_WithInvalidGrade_ReturnsGradeError()
    {
        var request = CreateValidRequest() with
        {
            Grade = 9
        };

        var errors =
            CreateQuestionRequestValidator.Validate(request);

        Assert.Contains("grade", errors.Keys);
    }

    [Theory]
    [InlineData("Physical Sciences")]
    [InlineData("physical_sciences")]
    [InlineData("")]
    public void Validate_WithInvalidSubjectSlug_ReturnsSlugError(
        string subjectSlug)
    {
        var request = CreateValidRequest() with
        {
            SubjectSlug = subjectSlug
        };

        var errors =
            CreateQuestionRequestValidator.Validate(request);

        Assert.Contains("subjectSlug", errors.Keys);
    }

    [Fact]
    public void Validate_WithNonHttpsImageUrl_ReturnsUrlError()
    {
        var request = CreateValidRequest() with
        {
            QuestionImageUrl = "http://example.com/question.webp"
        };

        var errors =
            CreateQuestionRequestValidator.Validate(request);

        Assert.Contains("questionImageUrl", errors.Keys);
    }

    private static CreateQuestionRequest CreateValidRequest()
    {
        return new CreateQuestionRequest(
            SubjectSlug: "physical-sciences",
            Grade: 12,
            TopicSlug: "newtonian-mechanics",
            ExamYear: 2022,
            ExamSeason: "May-June",
            PaperNumber: 1,
            QuestionNumber: "1.2",
            DisplayOrder: 2,
            QuestionImageUrl:
                "https://example.com/question.webp",
            MemoImageUrl:
                "https://example.com/memo.webp");
    }
}