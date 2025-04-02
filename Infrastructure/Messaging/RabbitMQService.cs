using Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Messaging
{
    public class RabbitMQService : IMessageService
    {
        private readonly string _hostName;
        private readonly string _username;
        private readonly string _password;
        private readonly int _port;

        public RabbitMQService(IConfiguration configuration)
        {
            _hostName = configuration["RabbitMQ:HostName"] ?? "localhost";
            _username = configuration["RabbitMQ:UserName"] ?? "guest";
            _password = configuration["RabbitMQ:Password"] ?? "guest";
            _port = int.Parse(configuration["RabbitMQ:Port"] ?? "5672");
        }

        public Task PublishAsync<T>(string queueName, T message)
        {
            var factory = new ConnectionFactory()
            {
                HostName = _hostName,
                UserName = _username,
                Password = _password,
                Port = _port
            };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            // Declare the queue (creating it if it doesn't exist)
            channel.QueueDeclare(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            // Serialize the message to JSON
            var messageBody = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

            // Publish the message to the queue
            var properties = channel.CreateBasicProperties();
            properties.Persistent = true; // Make message persistent

            channel.BasicPublish(
                exchange: "",
                routingKey: queueName,
                basicProperties: properties,
                body: messageBody);

            return Task.CompletedTask;
        }
    }
} 