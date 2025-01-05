using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using System.Threading.Tasks;

public class ProcessingTimeMiddleware
{
    private readonly RequestDelegate _next;

    public ProcessingTimeMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        // Tiếp tục xử lý request
        await _next(context);

        stopwatch.Stop();

        // Thêm thời gian xử lý vào header của response
        context.Response.Headers.Add("X-Processing-Time", $"{stopwatch.ElapsedMilliseconds} ms");
    }
}
