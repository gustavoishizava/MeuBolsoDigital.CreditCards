using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MBD.CreditCards.Application.IntegrationEvents.Events;
using MediatR;
using MeuBolsoDigital.RabbitMQ;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Events;

namespace MBD.CreditCards.API.Workers
{
    public class RabbitMqConsumerWorker : BackgroundService
    {
        private readonly ILogger<RabbitMqConsumerWorker> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IRabbitMqConnection _rabbitMqConnection;
        private readonly string _queueName;
        private readonly string _bankAccountTopic;

        public RabbitMqConsumerWorker(ILogger<RabbitMqConsumerWorker> logger, IServiceProvider serviceProvider, IRabbitMqConnection rabbitMqConnection, IConfiguration configuration)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _rabbitMqConnection = rabbitMqConnection;
            _queueName = configuration["RabbitMqConfiguration:ConsumerQueue"];
            _bankAccountTopic = configuration["RabbitMqConfiguration:BankAccountTopic"];
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"{GetType().Name} started.");

            _rabbitMqConnection.TryConnect(stoppingToken);
            SetupQueue();

            var consumer = new EventingBasicConsumer(_rabbitMqConnection.Channel);
            consumer.Received += async (object sender, BasicDeliverEventArgs eventArgs) =>
            {
                var result = await ProcessMessageAsync(sender, eventArgs);
                if (result)
                    _rabbitMqConnection.Channel.BasicAck(deliveryTag: eventArgs.DeliveryTag, multiple: false);
                else
                    _rabbitMqConnection.Channel.BasicNack(deliveryTag: eventArgs.DeliveryTag, multiple: false, requeue: true);
            };

            _rabbitMqConnection.Channel.BasicConsume(queue: _queueName,
                                                     autoAck: false,
                                                     consumerTag: string.Empty,
                                                     noLocal: true,
                                                     exclusive: false,
                                                     arguments: null,
                                                     consumer: consumer);

            return Task.CompletedTask;
        }

        private void SetupQueue()
        {
            _rabbitMqConnection.Channel.QueueDeclare(queue: _queueName,
                                                     durable: true,
                                                     exclusive: true,
                                                     autoDelete: false,
                                                     arguments: null);

            _rabbitMqConnection.Channel.QueueBind(queue: _queueName,
                                                  exchange: _bankAccountTopic,
                                                  routingKey: "#",
                                                  arguments: null);
        }

        private async Task<bool> ProcessMessageAsync(object sender, BasicDeliverEventArgs eventArgs)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var mediator = scope.ServiceProvider.GetService<IMediator>();

                var filter = $"{eventArgs.Exchange}.{eventArgs.RoutingKey}";
                switch (filter)
                {
                    case RabbitConstants.BANK_ACCOUNT_CREATED:
                        var bankAccount = JsonSerializer.Deserialize<BankAccountCreatedIntegrationEvent>(Encoding.UTF8.GetString(eventArgs.Body.ToArray()), new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
                        await mediator.Publish(bankAccount);
                        return true;
                    default:
                        return true;
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, $"{GetType().Name} error.");
                return false;
            }
        }
    }
}