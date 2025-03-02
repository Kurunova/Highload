using Grpc.Core;
using SocialNetwork.Counters.Grpc.Domain;
using SocialNetwork.Counters.Grpc.V1;

namespace SocialNetwork.Counters.Grpc.Services;

public class CountersServiceControllerV1 : GrpcCounterService.GrpcCounterServiceBase
{
	private readonly ILogger<CountersServiceControllerV1> _logger;
	private readonly ICountersRepository _countersRepository;

	public CountersServiceControllerV1(ILogger<CountersServiceControllerV1> logger, ICountersRepository countersRepository)
	{
		_logger = logger;
		_countersRepository = countersRepository;
	}

	public override async Task<GetUnreadMessagesResponse> GetUnreadMessagesCount(GetUnreadMessagesRequest request, ServerCallContext context)
	{
		_logger.LogInformation($"Get unread count for User {request.UserId}");

		var count = await _countersRepository.GetUnreadMessagesCountAsync(request.UserId, context.CancellationToken);

		return new GetUnreadMessagesResponse { Count = count };
	}
	
	public override async Task<IncrementUnreadMessagesCountResponse> IncrementUnreadMessagesCount(IncrementUnreadMessagesCountRequest request, ServerCallContext context)
	{
		_logger.LogInformation($"Set unread count: User {request.UserId} -> {request.Count}");

		await _countersRepository.IncrementUnreadMessagesCountAsync(request.UserId, request.Count, context.CancellationToken);
		
		return new IncrementUnreadMessagesCountResponse { Success = true };
	}
	
	public override async Task<DecrementUnreadMessagesCountResponse> DecrementUnreadMessagesCount(DecrementUnreadMessagesCountRequest request, ServerCallContext context)
	{
		_logger.LogInformation($"Set unread count: User {request.UserId} -> {request.Count}");
		
		await _countersRepository.DecrementUnreadMessagesCountAsync(request.UserId, request.Count, context.CancellationToken);
	
		return new DecrementUnreadMessagesCountResponse { Success = true };
	}
}