using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace PastPapers.ContentApi.Tests.Features.Questions;

public sealed class QuestionEndpointSecurityTests
{
    private const string ValidIngestionKey =
        "integration-test-ingestion-key";

    [Fact]
    public async Task CreateQuestion_WithoutIngestionKey_ReturnsUnauthorized()
    {
        await using var application = CreateApplication();
        using var client = application.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/questions",
            CreateValidRequest());

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateQuestion_WithInvalidIngestionKey_ReturnsUnauthorized()
    {
        await using var application = CreateApplication();
        using var client = application.CreateClient();

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            "/api/questions");

        request.Headers.Add(
            "X-Content-Ingestion-Key",
            "incorrect-key");

        request.Content = JsonContent.Create(CreateValidRequest());

        var response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    private static WebApplicationFactory<Program> CreateApplication()
    {
        Environment.SetEnvironmentVariable(
            "ConnectionStrings__ContentDatabase",
            "Host=localhost;Port=5432;Database=test;Username=test;Password=test");

        Environment.SetEnvironmentVariable(
            "ContentIngestion__ApiKey",
            ValidIngestionKey);

        return new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Testing");
            });
    }

    private static object CreateValidRequest()
    {
        return new
        {
            subjectSlug = "physical-sciences",
            grade = 12,
            topicSlug = "newtonian-mechanics",
            examYear = 2022,
            examSeason = "May-June",
            paperNumber = 1,
            questionNumber = "1.2",
            displayOrder = 12,
            questionImageUrl =
                "https://example.com/question.webp",
            memoImageUrl =
                "https://example.com/memo.webp"
        };
    }
}
