using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Application.Features.Reports.Models;
using Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Infrastructure.Messaging
{
    public class RabbitMQConsumerService : IMessageConsumerService, IDisposable
    {
        private readonly ILogger<RabbitMQConsumerService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly string _queueName = "report-processing";
        private readonly ConnectionFactory _factory;
        private IConnection _connection;
        private IModel _channel;
        private bool _isConsuming;

        public RabbitMQConsumerService(
            ILogger<RabbitMQConsumerService> logger,
            IConfiguration configuration,
            IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _configuration = configuration;
            _serviceScopeFactory = serviceScopeFactory;

            _factory = new ConnectionFactory
            {
                HostName = _configuration["RabbitMQ:HostName"] ?? "localhost",
                UserName = _configuration["RabbitMQ:UserName"] ?? "guest",
                Password = _configuration["RabbitMQ:Password"] ?? "guest",
                Port = int.Parse(_configuration["RabbitMQ:Port"] ?? "5672"),
            };
        }

        public Task StartConsumingAsync(CancellationToken cancellationToken)
        {
            if (_isConsuming)
            {
                return Task.CompletedTask;
            }

            _connection = _factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(
                queue: _queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            // Fair dispatch - don't give more than one message to a worker at a time
            _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                try
                {
                    _logger.LogInformation("Received message: {Message}", message);
                    
                    // Deserialize the message
                    var reportMessage = JsonConvert.DeserializeObject<ReportProcessingMessage>(message);
                    
                    // Process the report using a scoped service
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var reportProcessingService = scope.ServiceProvider.GetRequiredService<IReportProcessingService>();
                        await reportProcessingService.ProcessReportAsync(reportMessage);
                    }
                    
                    // Acknowledge the message
                    _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    
                    _logger.LogInformation("Report {ReportId} processed successfully", reportMessage.ReportId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing report message");
                    
                    // Negative acknowledgment - message will be requeued
                    _channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
                }
            };

            _channel.BasicConsume(
                queue: _queueName,
                autoAck: false, // Manual acknowledgment
                consumer: consumer);

            _isConsuming = true;
            _logger.LogInformation("Report processing consumer started");

            return Task.CompletedTask;
        }

        public Task StopConsumingAsync(CancellationToken cancellationToken)
        {
            if (!_isConsuming)
            {
                return Task.CompletedTask;
            }

            _channel?.Close();
            _connection?.Close();
            _isConsuming = false;
            
            _logger.LogInformation("Report processing consumer stopped");
            
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
            
            GC.SuppressFinalize(this);
        }
    }
} 