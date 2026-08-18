using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace PastPapers.ContentApi.Common.Security;

public sealed class ContentIngestionKeyFilter(
    IConfiguration configuration) : IEndpointFilter
{
    private const string HeaderName = "X-Content-Ingestion-Key";

    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        var expectedKey = configuration["ContentIngestion:ApiKey"];

        if (string.IsNullOrWhiteSpace(expectedKey))
        {
            throw new InvalidOperationException(
                "The content-ingestion API key is not configured.");
        }

        if (!context.HttpContext.Request.Headers.TryGetValue(
                HeaderName,
                out var suppliedHeader))
        {
            return TypedResults.Unauthorized();
        }

        var suppliedKey = suppliedHeader.ToString();

        var expectedBytes = Encoding.UTF8.GetBytes(expectedKey);
        var suppliedBytes = Encoding.UTF8.GetBytes(suppliedKey);

        var isValid = expectedBytes.Length == suppliedBytes.Length &&
                      CryptographicOperations.FixedTimeEquals(
                          expectedBytes,
                          suppliedBytes);

        if (!isValid)
        {
            return TypedResults.Unauthorized();
        }

        return await next(context);
    }
}