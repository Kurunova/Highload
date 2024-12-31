using Serilog;
using Serilog.Context;

namespace SocialNetworkApi.Middlewares;

public class RequestIdMiddleware
{
	private readonly RequestDelegate _next;

	public RequestIdMiddleware(RequestDelegate next)
	{
		_next = next;
	}

	public async Task InvokeAsync(HttpContext context)
	{
		// Проверяем, существует ли X-Request-ID
		if (!context.Request.Headers.TryGetValue("X-Request-ID", out var requestId) || string.IsNullOrWhiteSpace(requestId))
		{
			requestId = Guid.NewGuid().ToString();
			context.Request.Headers["X-Request-ID"] = requestId;
		}

		// Добавляем идентификатор в логирование
		using (LogContext.PushProperty("X-Request-ID", requestId))
		{
			Log.Information("X-Request-ID добавлен: {RequestId}", requestId);
			context.Response.Headers["X-Request-ID"] = requestId;
			await _next(context);
		}
	}
}