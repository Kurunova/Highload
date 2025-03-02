using System.Collections.Concurrent;
using Grpc.Core;
using SocialNetwork.Counters.Grpc.V1;

namespace SocialNetwork.Counters.Grpc.Services;

public class CountersServiceControllerV1 : GrpcCounterService.GrpcCounterServiceBase
{
	private readonly ILogger<CountersServiceControllerV1> _logger;
	private static readonly ConcurrentDictionary<long, int> UnreadMessageCounters = new();

	public CountersServiceControllerV1(ILogger<CountersServiceControllerV1> logger)
	{
		_logger = logger;
	}

	public override Task<GetUnreadMessagesResponse> GetUnreadMessagesCount(GetUnreadMessagesRequest request, ServerCallContext context)
	{
		_logger.LogInformation($"Get unread count for User {request.UserId}");
		UnreadMessageCounters.TryGetValue(request.UserId, out int count);

		return Task.FromResult(new GetUnreadMessagesResponse { Count = count });
	}
	
	public override Task<IncrementUnreadMessagesCountResponse> IncrementUnreadMessagesCount(IncrementUnreadMessagesCountRequest request, ServerCallContext context)
	{
		_logger.LogInformation($"Set unread count: User {request.UserId} -> {request.Count}");
		UnreadMessageCounters[request.UserId] = request.Count;
	
		return Task.FromResult(new IncrementUnreadMessagesCountResponse { Success = true });
	}
	
	public override Task<DecrementUnreadMessagesCountResponse> DecrementUnreadMessagesCount(DecrementUnreadMessagesCountRequest request, ServerCallContext context)
	{
		_logger.LogInformation($"Set unread count: User {request.UserId} -> {request.Count}");
		UnreadMessageCounters[request.UserId] = request.Count;
	
		return Task.FromResult(new DecrementUnreadMessagesCountResponse { Success = true });
	}
}