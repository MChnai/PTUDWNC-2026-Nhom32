using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Npgsql;
namespace CulinaryBlog.API.Middleware;
public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext http)
    {
        try { await next(http); }
        catch (Exception error) when (!http.Response.HasStarted)
        {
            if (error is OperationCanceledException && http.RequestAborted.IsCancellationRequested) return;
            logger.LogError(error, "Request failed: {Path}", http.Request.Path);
            IResult result = error switch
            {
                ValidationException e => Results.ValidationProblem(e.Errors.GroupBy(x => x.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(x => x.ErrorMessage).ToArray()), statusCode: 422, title: "Vui lòng kiểm tra thông tin"),
                DbUpdateConcurrencyException => Results.Problem(statusCode: 409, detail: "Dữ liệu đã thay đổi. Hãy tải lại trước khi tiếp tục."),
                DbUpdateException { InnerException: PostgresException { SqlState: "23505" } } => Results.Problem(statusCode: 409, detail: "Tên, email hoặc dữ liệu đã tồn tại."),
                DbUpdateException { InnerException: PostgresException { SqlState: "23503" } } => Results.Problem(statusCode: 409, detail: "Dữ liệu đang được sử dụng hoặc không còn tồn tại."),
                BadHttpRequestException => Results.Problem(statusCode: 400, detail: "Nội dung yêu cầu không hợp lệ."),
                _ => Results.Problem(statusCode: 500, title: "Đã xảy ra lỗi", detail: "Vui lòng thử lại sau.", extensions: new Dictionary<string, object?> { ["traceId"] = http.TraceIdentifier })
            };
            await result.ExecuteAsync(http);
        }
    }
}
