using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ReportServiceAPI.Services
{
    public class ReportMessageConsumerHostedService : BackgroundService
    {
        private readonly ILogger<ReportMessageConsumerHostedService> _logger;
        private readonly IServiceProvider _serviceProvider;

        public ReportMessageConsumerHostedService(
            ILogger<ReportMessageConsumerHostedService> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Report Message Consumer Hosted Service is starting");

            // Resolve the singleton service from the root service provider
            var messageConsumerService = _serviceProvider.GetRequiredService<IMessageConsumerService>();
            await messageConsumerService.StartConsumingAsync(stoppingToken);

            // Keep the service running until cancellation is requested
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Report Message Consumer Hosted Service is stopping");

            var messageConsumerService = _serviceProvider.GetRequiredService<IMessageConsumerService>();
            await messageConsumerService.StopConsumingAsync(cancellationToken);

            await base.StopAsync(cancellationToken);
        }
    }
} 