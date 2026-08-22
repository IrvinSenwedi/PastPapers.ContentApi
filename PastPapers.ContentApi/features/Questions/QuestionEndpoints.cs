using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using PastPapers.ContentApi.Common.Pagination;
using PastPapers.ContentApi.Common.Security;
using PastPapers.ContentApi.Data;

namespace PastPapers.ContentApi.Features.Questions;

public static class QuestionEndpoints
{
    public static IEndpointRouteBuilder MapQuestionEndpoints(
        this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints
            .MapGroup("/api/questions")
            .WithTags("Questions");

        group.MapGet("", GetQuestionsAsync)
            .WithName("GetQuestions")
            .WithSummary("Gets published questions for a topic")
            .Produces<PagedResponse<QuestionResponse>>()
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/{id:guid}", GetQuestionByIdAsync)
            .WithName("GetQuestionById")
            .WithSummary("Gets a published question by ID")
            .Produces<QuestionResponse>()
            .Produces(StatusCodes.Status404NotFound);

        group.MapPost("", CreateQuestionAsync)
            .WithName("CreateQuestion")
            .WithSummary(
                "Creates or updates a question from the ingestion pipeline")
            .AddEndpointFilter<ContentIngestionKeyFilter>()
            .Produces<QuestionIngestionResponse>(
                StatusCodes.Status200OK)
            .Produces<QuestionIngestionResponse>(
                StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status401Unauthorized);

        group.MapPatch("/{id:guid}/status", UpdateQuestionStatusAsync)
            .WithName("UpdateQuestionStatus")
            .WithSummary("Updates the publishing status of a question")
            .AddEndpointFilter<ContentIngestionKeyFilter>()
            .Produces<QuestionIngestionResponse>()
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound);
        
        group.MapGet(
                "/filter-options",
                GetQuestionFilterOptionsAsync)
            .WithName("GetQuestionFilterOptions")
            .WithSummary(
                "Gets available question filters for a topic")
            .Produces<QuestionFilterOptionsResponse>()
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/by-ids", GetQuestionsByIdsAsync)
            .WithName("GetQuestionsByIds")
            .WithSummary(
              "Gets published questions by their IDs")
            .Produces<List<QuestionResponse>>()
            .ProducesValidationProblem();

            

        return endpoints;
    }

    private static async Task<
        Results<
            Ok<PagedResponse<QuestionResponse>>,
            ValidationProblem,
            NotFound>>
        GetQuestionsAsync(
            AppDbContext dbContext,
            Guid? topicId = null,
            short? examYear = null,
            string? season = null,
            short? paperNumber = null,
            string? questionNumber = null,
            int page = 1,
            int pageSize = 20,
            CancellationToken cancellationToken = default)
    {
        var errors = new Dictionary<string, string[]>();

        if (topicId is null || topicId == Guid.Empty)
        {
            errors["topicId"] = ["A valid topic ID is required."];
        }

        if (examYear is < 1996 or > 2100)
        {
            errors["examYear"] =
                ["Exam year must be between 1996 and 2100."];
        }

        if (paperNumber is < 1 or > 4)
        {
            errors["paperNumber"] =
                ["Paper number must be between 1 and 4."];
        }

        if (page < 1)
        {
            errors["page"] = ["Page must be at least 1."];
        }

        if (pageSize is < 1 or > 100)
        {
            errors["pageSize"] =
                ["Page size must be between 1 and 100."];
        }

        if (errors.Count > 0)
        {
            return TypedResults.ValidationProblem(errors);
        }

        var topicExists = await dbContext.Topics
            .AsNoTracking()
            .AnyAsync(
                topic => topic.Id == topicId!.Value,
                cancellationToken);

        if (!topicExists)
        {
            return TypedResults.NotFound();
        }

        var query = dbContext.Questions
            .AsNoTracking()
            .Where(question =>
                question.TopicId == topicId!.Value &&
                question.Status == QuestionStatus.Published);

        if (examYear.HasValue)
        {
            query = query.Where(
                question => question.ExamYear == examYear.Value);
        }

        if (!string.IsNullOrWhiteSpace(season))
        {
            var normalisedSeason = season!.Trim();

            query = query.Where(
                question => EF.Functions.ILike(
                    question.ExamSeason,
                    normalisedSeason));
        }

        if (paperNumber.HasValue)
        {
            query = query.Where(
                question =>
                    question.PaperNumber == paperNumber.Value);
        }

        if (!string.IsNullOrWhiteSpace(questionNumber))
        {
            var prefix = questionNumber!.Trim();

            query = query.Where(
                question =>
                    question.QuestionNumber.StartsWith(prefix));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(question => question.ExamYear)
            .ThenBy(question => question.ExamSeason)
            .ThenBy(question => question.PaperNumber)
            .ThenBy(question => question.DisplayOrder)
            .Select(QuestionProjections.ToResponse)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var totalPages = totalCount == 0
            ? 0
            : (int)Math.Ceiling(
                totalCount / (double)pageSize);

        var response = new PagedResponse<QuestionResponse>(
            items,
            page,
            pageSize,
            totalCount,
            totalPages);

        return TypedResults.Ok(response);
    }

    private static async Task<
        Results<Ok<QuestionResponse>, NotFound>>
        GetQuestionByIdAsync(
            Guid id,
            AppDbContext dbContext,
            CancellationToken cancellationToken)
    {
        var question = await dbContext.Questions
            .AsNoTracking()
            .Where(question =>
                question.Id == id &&
                question.Status == QuestionStatus.Published)
            .Select(QuestionProjections.ToResponse)
            .SingleOrDefaultAsync(cancellationToken);

        return question is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(question);
    }

    private static async Task<IResult> CreateQuestionAsync(
        CreateQuestionRequest request,
        AppDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var errors =
            CreateQuestionRequestValidator.Validate(request);

        if (errors.Count > 0)
        {
            return TypedResults.ValidationProblem(errors);
        }

        var subjectSlug = request.SubjectSlug!.Trim();
        var topicSlug = request.TopicSlug!.Trim();
        var examSeason = request.ExamSeason!.Trim();
        var questionNumber = request.QuestionNumber!.Trim();

        var topic = await dbContext.Topics
            .AsNoTracking()
            .Where(topic =>
                topic.Subject.Slug == subjectSlug &&
                topic.Grade == request.Grade &&
                topic.Slug == topicSlug)
            .Select(topic => new
            {
                topic.Id
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (topic is null)
        {
            return TypedResults.ValidationProblem(
                new Dictionary<string, string[]>
                {
                    ["topicSlug"] =
                    [
                        "No topic matches the supplied subject, " +
                        "grade and topic."
                    ]
                });
        }

        var existingQuestion = await dbContext.Questions
            .SingleOrDefaultAsync(
                question =>
                    question.TopicId == topic.Id &&
                    question.ExamYear == request.ExamYear &&
                    question.ExamSeason == examSeason &&
                    question.PaperNumber == request.PaperNumber &&
                    question.QuestionNumber == questionNumber,
                cancellationToken);

        if (existingQuestion is not null)
        {
            existingQuestion.DisplayOrder =
                request.DisplayOrder;

            existingQuestion.QuestionImageUrl =
                request.QuestionImageUrl!.Trim();

            existingQuestion.MemoImageUrl =
                request.MemoImageUrl!.Trim();

            await dbContext.SaveChangesAsync(cancellationToken);

            return TypedResults.Ok(
                new QuestionIngestionResponse(
                    existingQuestion.Id,
                    existingQuestion.Status.ToString()));
        }

        var question = new Question
        {
            TopicId = topic.Id,
            ExamYear = request.ExamYear,
            ExamSeason = examSeason,
            PaperNumber = request.PaperNumber,
            QuestionNumber = questionNumber,
            DisplayOrder = request.DisplayOrder,
            QuestionImageUrl =
                request.QuestionImageUrl!.Trim(),
            MemoImageUrl =
                request.MemoImageUrl!.Trim(),
            Status = QuestionStatus.Draft
        };

        dbContext.Questions.Add(question);
        await dbContext.SaveChangesAsync(cancellationToken);

        var response = new QuestionIngestionResponse(
            question.Id,
            question.Status.ToString());

        return TypedResults.Created(
            $"/api/questions/{question.Id}",
            response);
    }

    
    private static async Task<IResult> UpdateQuestionStatusAsync(
        Guid id,
        UpdateQuestionStatusRequest request,
        AppDbContext dbContext,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Status) ||
            !Enum.TryParse<QuestionStatus>(
                request.Status,
                ignoreCase: true,
                out var status))
        {
            return TypedResults.ValidationProblem(
                new Dictionary<string, string[]>
                {
                    ["status"] =
                    [
                        "Status must be Draft, Published or Archived."
                    ]
                });
        }

        var question = await dbContext.Questions
            .SingleOrDefaultAsync(
                question => question.Id == id,
                cancellationToken);

        if (question is null)
        {
            return TypedResults.NotFound();
        }

        question.Status = status;

        await dbContext.SaveChangesAsync(cancellationToken);

        return TypedResults.Ok(
            new QuestionIngestionResponse(
                question.Id,
                question.Status.ToString()));
    }
    
    private static async Task<
            Results<
                Ok<QuestionFilterOptionsResponse>,
                ValidationProblem,
                NotFound>>
        GetQuestionFilterOptionsAsync(
            AppDbContext dbContext,
            Guid? topicId = null,
            CancellationToken cancellationToken = default)

    {
        if (topicId is null || topicId == Guid.Empty)
        {
            return TypedResults.ValidationProblem(
                new Dictionary<string, string[]>
                {
                    ["topicId"] =
                        ["A valid topic ID is required."]
                });
        }

        var topicExists = await dbContext.Topics
            .AsNoTracking()
            .AnyAsync(
                topic => topic.Id == topicId.Value,
                cancellationToken);

        if (!topicExists)
        {
            return TypedResults.NotFound();
        }

        var publishedQuestions = dbContext.Questions
            .AsNoTracking()
            .Where(question =>
                question.TopicId == topicId.Value &&
                question.Status ==
                QuestionStatus.Published);

        var examYears = await publishedQuestions
            .Select(question => question.ExamYear)
            .Distinct()
            .OrderByDescending(year => year)
            .ToListAsync(cancellationToken);

        var examSeasons = await publishedQuestions
            .Select(question => question.ExamSeason)
            .Distinct()
            .OrderBy(season => season)
            .ToListAsync(cancellationToken);

        return TypedResults.Ok(
            new QuestionFilterOptionsResponse(
                examYears,
                examSeasons));
    }


    private static async Task<
    Results<
        Ok<List<QuestionResponse>>,
        ValidationProblem>>
    GetQuestionsByIdsAsync(
        AppDbContext dbContext,
        string? ids = null,
        CancellationToken cancellationToken = default)
{
    if (string.IsNullOrWhiteSpace(ids))
    {
        return TypedResults.ValidationProblem(
            new Dictionary<string, string[]>
            {
                ["ids"] =
                    ["At least one question ID is required."]
            });
    }

    var rawIds = ids.Split(
        ',',
        StringSplitOptions.RemoveEmptyEntries |
        StringSplitOptions.TrimEntries);

    if (rawIds.Length is < 1 or > 50)
    {
        return TypedResults.ValidationProblem(
            new Dictionary<string, string[]>
            {
                ["ids"] =
                [
                    "Provide between 1 and 50 question IDs."
                ]
            });
    }

    var questionIds = new List<Guid>();

    foreach (var rawId in rawIds)
    {
        if (!Guid.TryParse(rawId, out var questionId) ||
            questionId == Guid.Empty)
        {
            return TypedResults.ValidationProblem(
                new Dictionary<string, string[]>
                {
                    ["ids"] =
                    [
                        $"'{rawId}' is not a valid question ID."
                    ]
                });
        }

        if (!questionIds.Contains(questionId))
        {
            questionIds.Add(questionId);
        }
    }

    var questions = await dbContext.Questions
        .AsNoTracking()
        .Where(question =>
            questionIds.Contains(question.Id) &&
            question.Status ==
                QuestionStatus.Published)
        .Select(QuestionProjections.ToResponse)
        .ToListAsync(cancellationToken);

    var questionsById = questions.ToDictionary(
        question => question.Id);

    var orderedQuestions = questionIds
        .Where(questionsById.ContainsKey)
        .Select(questionId => questionsById[questionId])
        .ToList();

    return TypedResults.Ok(orderedQuestions);
}
}