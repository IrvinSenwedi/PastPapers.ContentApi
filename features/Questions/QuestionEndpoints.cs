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
            errors["examYear"] = ["Exam year must be between 1996 and 2100."];
        }

        if (paperNumber is < 1 or > 4)
        {
            errors["paperNumber"] = ["Paper number must be between 1 and 4."];
        }

        if (page < 1)
        {
            errors["page"] = ["Page must be at least 1."];
        }

        if (pageSize is < 1 or > 100)
        {
            errors["pageSize"] = ["Page size must be between 1 and 100."];
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
                question => question.PaperNumber == paperNumber.Value);
        }

        if (!string.IsNullOrWhiteSpace(questionNumber))
        {
            var prefix = questionNumber!.Trim();

            query = query.Where(
                question => question.QuestionNumber.StartsWith(prefix));
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
            : (int)Math.Ceiling(totalCount / (double)pageSize);

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
}