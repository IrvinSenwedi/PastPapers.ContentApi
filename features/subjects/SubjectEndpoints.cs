using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using PastPapers.ContentApi.Data;

namespace PastPapers.ContentApi.Features.Subjects;

public static class SubjectEndpoints
{
    public static IEndpointRouteBuilder MapSubjectEndpoints(
        this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints
            .MapGroup("/api/subjects")
            .WithTags("Subjects");

        group.MapGet("", GetSubjectsAsync)
            .WithName("GetSubjects")
            .WithSummary("Gets all available subjects")
            .Produces<List<SubjectResponse>>();

        return endpoints;
    }

    private static async Task<Ok<List<SubjectResponse>>> GetSubjectsAsync(
        AppDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var subjects = await dbContext.Subjects
            .AsNoTracking()
            .OrderBy(subject => subject.Name)
            .Select(subject => new SubjectResponse(
                subject.Id,
                subject.Name,
                subject.Slug))
            .ToListAsync(cancellationToken);

        return TypedResults.Ok(subjects);
    }
}