using Grpc.Net.ClientFactory;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Counters.Grpc.V1;
using SocialNetworkApi.Services;

namespace SocialNetworkApi.Controllers;

[ApiController]
[Route("api/v1/counters")]
public class CountersV1Controller : BaseController
{
    private readonly GrpcCounterService.GrpcCounterServiceClient _grpcCountersServiceClient;

    public CountersV1Controller(
        JwtTokenService jwtTokenService,
        GrpcClientFactory grpcClientFactory) : base(jwtTokenService)
    {
        _grpcCountersServiceClient = grpcClientFactory.CreateClient<GrpcCounterService.GrpcCounterServiceClient>("GrpcCounterServiceClientV1");
    }
    
    /// <summary>
    /// Получает количество непрочитанных сообщений у пользователя
    /// </summary>
    [HttpGet("unread/{userId}")]
    public async Task<IActionResult> GetUnreadMessagesCount(long userId, CancellationToken cancellationToken)
    {
        var getUnreadMessagesRequest = new SocialNetwork.Counters.Grpc.V1.GetUnreadMessagesRequest { UserId = userId };
        var count = await _grpcCountersServiceClient.GetUnreadMessagesCountAsync(getUnreadMessagesRequest);
        return Ok(new { UserId = userId, UnreadMessages = count });
    }
}