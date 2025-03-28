using System.Diagnostics;
using Prometheus;

namespace SocialNetworkApi.Middlewares;

public class MetricsMiddleware
{
	private readonly RequestDelegate _next;
    
	private static readonly Counter RequestCounter = Metrics.CreateCounter(
		"http_requests_total", "Total number of HTTP requests", new CounterConfiguration
		{
			LabelNames = new[] { "method", "endpoint", "status_code" }
		});

	private static readonly Counter ErrorCounter = Metrics.CreateCounter(
		"http_errors_total", "Total number of HTTP errors", new CounterConfiguration
		{
			LabelNames = new[] { "method", "endpoint", "status_code" }
		});

	private static readonly Histogram RequestDuration = Metrics.CreateHistogram(
		"http_request_duration_seconds", "Duration of HTTP requests in seconds", new HistogramConfiguration
		{
			LabelNames = new[] { "method", "endpoint", "status_code" }
		});

	public MetricsMiddleware(RequestDelegate next)
	{
		_next = next;
	}

	public async Task Invoke(HttpContext context)
	{
		var stopwatch = Stopwatch.StartNew();
		try
		{
			await _next(context);
		}
		catch (Exception)
		{
			ErrorCounter.WithLabels(context.Request.Method, context.Request.Path, "500").Inc();
			throw;
		}
		finally
		{
			stopwatch.Stop();
			var statusCode = context.Response.StatusCode.ToString();

			RequestCounter.WithLabels(context.Request.Method, context.Request.Path, statusCode).Inc();
			if (statusCode.StartsWith("5"))
			{
				ErrorCounter.WithLabels(context.Request.Method, context.Request.Path, statusCode).Inc();
			}
            
			RequestDuration.WithLabels(context.Request.Method, context.Request.Path, statusCode)
				.Observe(stopwatch.Elapsed.TotalSeconds);
		}
	}
}