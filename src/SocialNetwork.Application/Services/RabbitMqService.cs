using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SocialNetwork.Application.Configurations;

namespace SocialNetwork.Application.Services;

public class RabbitMqService
{
	private readonly ILogger<RabbitMqService> _logger;
	private readonly RabbitMqSettings _settings;

	public RabbitMqService(ILogger<RabbitMqService> logger, IOptions<RabbitMqSettings> options)
	{
		_logger = logger;
		_settings = options.Value;
	}

	public string RoutingKeyPrefix => _settings.RoutingKeyPrefix;
	
	public async Task PublishPostEvent(string routingKey, byte[] body, CancellationToken cancellationToken)
	{
		try
		{
			var routingKeyWithPrefix = $"{_settings.RoutingKeyPrefix}{routingKey}";
			var factory = new ConnectionFactory
			{
				HostName = _settings.HostName,
				UserName = _settings.UserName,
				Password = _settings.Password
			};
			await using var connection = await factory.CreateConnectionAsync(cancellationToken);
			await using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

			await channel.ExchangeDeclareAsync(_settings.Exchange, ExchangeType.Topic, cancellationToken: cancellationToken);

			await channel.BasicPublishAsync(_settings.Exchange, routingKeyWithPrefix, body, cancellationToken);
		}
		catch (Exception ex)
		{
			_logger.LogError($"Error during publish message {routingKey} {Encoding.UTF8.GetString(body)}: {ex}");
			throw;
		}
	}

	public async Task Consume(AsyncEventHandler<BasicDeliverEventArgs> receivedAction, CancellationToken cancellationToken)
	{
		try
		{
			string routingKeyPattern = $"{_settings.RoutingKeyPrefix}*";
			var factory = new ConnectionFactory
			{
				HostName = _settings.HostName,
				UserName = _settings.UserName,
				Password = _settings.Password
			};
			await using var connection = await factory.CreateConnectionAsync(cancellationToken);
			await using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

			await channel.ExchangeDeclareAsync(_settings.Exchange, ExchangeType.Topic, cancellationToken: cancellationToken);
			await channel.QueueDeclareAsync(_settings.Queue, durable: true, exclusive: false, autoDelete: false, arguments: null, cancellationToken: cancellationToken);
			await channel.QueueBindAsync(_settings.Queue, _settings.Exchange, routingKeyPattern, cancellationToken: cancellationToken);

			var consumer = new AsyncEventingBasicConsumer(channel);
			consumer.ReceivedAsync += receivedAction;

			await channel.BasicConsumeAsync(queue: _settings.Queue, autoAck: true, consumer: consumer, cancellationToken: cancellationToken);
			
			await Task.Delay(Timeout.Infinite, cancellationToken);
		}
		catch (Exception ex)
		{
			_logger.LogError($"Error during setup consumer: {ex}");
			throw;
		}
	}
}