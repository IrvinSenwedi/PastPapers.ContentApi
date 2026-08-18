using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using PastPapers.ContentApi.Data;
using PastPapers.ContentApi.Features.Questions;

namespace PastPapers.ContentApi.Features.Topics;

public static class TopicEndpoints
{
    public static IEndpointRouteBuilder MapTopicEndpoints(
        this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints
            .MapGroup("/api/topics")
            .WithTags("Topics");

        group.MapGet("", GetTopicsAsync)
            .WithName("GetTopics")
            .WithSummary("Gets topics filtered by grade and subject")
            .Produces<List<TopicResponse>>()
            .ProducesValidationProblem();

        return endpoints;
    }

    private static async Task<
        Results<Ok<List<TopicResponse>>, ValidationProblem>>
        GetTopicsAsync(
            AppDbContext dbContext,
            short grade = 12,
            string? subject = null,
            CancellationToken cancellationToken = default)
    {
        if (grade is < 10 or > 12)
        {
            return TypedResults.ValidationProblem(
                new Dictionary<string, string[]>
                {
                    ["grade"] = ["Grade must be between 10 and 12."]
                });
        }

        var query = dbContext.Topics
            .AsNoTracking()
            .Where(topic => topic.Grade == grade);

        if (!string.IsNullOrWhiteSpace(subject))
        {
            var subjectSlug = subject!.Trim().ToLowerInvariant();

            query = query.Where(
                topic => topic.Subject.Slug == subjectSlug);
        }

        var topics = await query
            .OrderBy(topic => topic.Subject.Name)
            .ThenBy(topic => topic.DisplayOrder)
            .ThenBy(topic => topic.Name)
            .Select(topic => new TopicResponse(
                topic.Id,
                topic.Name,
                topic.Slug,
                topic.Grade,
                topic.DisplayOrder,
                topic.SubjectId,
                topic.Subject.Name,
                topic.Subject.Slug,
                topic.Questions.Count(question =>
                    question.Status == QuestionStatus.Published)))
            .ToListAsync(cancellationToken);

        return TypedResults.Ok(topics);
    }
}