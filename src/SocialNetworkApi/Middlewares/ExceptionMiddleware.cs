using System.Net;
using SocialNetwork.Application.Exceptions;

namespace SocialNetworkApi.Middlewares;

public class ExceptionMiddleware
{
	private readonly RequestDelegate _next;

	public ExceptionMiddleware(RequestDelegate next)
	{
		_next = next;
	}

	public async Task Invoke(HttpContext context)
	{
		try
		{
			await _next(context);
		}
		catch (Exception ex)
		{
			await HandleExceptionAsync(context, ex);
		}
	}
	
	private static Task HandleExceptionAsync(HttpContext context, Exception exception)
	{
		var code = HttpStatusCode.InternalServerError; 
		if (exception is ArgumentException or ValidationException) 
			code = HttpStatusCode.BadRequest;

		var result = System.Text.Json.JsonSerializer.Serialize(new { error = exception.Message });
		context.Response.ContentType = "application/json";
		context.Response.StatusCode = (int)code;
		return context.Response.WriteAsync(result);
	}
}