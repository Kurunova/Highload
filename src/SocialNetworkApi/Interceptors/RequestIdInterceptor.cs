using Grpc.Core;
using Grpc.Core.Interceptors;

namespace SocialNetworkApi.Interceptors;

public class RequestIdInterceptor : Grpc.Core.Interceptors.Interceptor
{
	private readonly IHttpContextAccessor _httpContextAccessor;

	public RequestIdInterceptor(IHttpContextAccessor httpContextAccessor)
	{
		_httpContextAccessor = httpContextAccessor;
	}

	public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(
		TRequest request,
		ClientInterceptorContext<TRequest, TResponse> context, Interceptor.AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
	{
		var headers = context.Options.Headers ?? new Metadata();
		if (_httpContextAccessor.HttpContext?.Request.Headers.TryGetValue("X-Request-ID", out var requestId) == true)
		{
			headers.Add("X-Request-ID", requestId);
		}

		var newOptions = context.Options.WithHeaders(headers);
		var newContext = new ClientInterceptorContext<TRequest, TResponse>(
			context.Method, context.Host, newOptions);

		return base.AsyncUnaryCall(request, newContext, continuation);
	}
}