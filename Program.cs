using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using PastPapers.ContentApi.Data;
using PastPapers.ContentApi.Features.Questions;
using PastPapers.ContentApi.Features.Subjects;
using PastPapers.ContentApi.Features.Topics;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();

var connectionString = builder.Configuration
                           .GetConnectionString("ContentDatabase")
                       ?? throw new InvalidOperationException(
                           "Connection string 'ContentDatabase' is not configured.");

var contentIngestionKey =
    builder.Configuration["ContentIngestion:ApiKey"];

if (string.IsNullOrWhiteSpace(contentIngestionKey))
{
    throw new InvalidOperationException(
        "Configuration value 'ContentIngestion:ApiKey' is required.");
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>(
        name: "content_database");

var app = builder.Build();

app.UseExceptionHandler();
app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/", () => TypedResults.Ok(
        new
        {
            service = "PastPapers.ContentApi",
            status = "running"
        }))
    .ExcludeFromDescription();

app.MapHealthChecks(
    "/health",
    new HealthCheckOptions());

app.MapSubjectEndpoints();
app.MapTopicEndpoints();
app.MapQuestionEndpoints();

app.Run();