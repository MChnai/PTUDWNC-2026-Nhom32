using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;
namespace CulinaryBlog.Application.Common.Behaviors;
public class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        var timer = Stopwatch.StartNew();
        try { return await next(); }
        finally
        {
            // Log only the request type and duration, never passwords/tokens or request bodies.
            if (timer.ElapsedMilliseconds > 500) logger.LogWarning("Slow handler {Handler}: {ElapsedMs} ms", typeof(TRequest).Name, timer.ElapsedMilliseconds);
            else logger.LogInformation("Handler {Handler}: {ElapsedMs} ms", typeof(TRequest).Name, timer.ElapsedMilliseconds);
        }
    }
}
