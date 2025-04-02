using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Persistence;
using Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Application.Interfaces;
using Infrastructure.Messaging;
using Infrastructure.Services;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Register persistence
            services.AddPersistence(configuration);
            
            // Register message services
            services.AddSingleton<IMessageService, RabbitMQService>();
            services.AddSingleton<IMessageConsumerService, RabbitMQConsumerService>();
            
            // Register processing services
            services.AddScoped<IReportProcessingService, ReportProcessingService>();
            
            return services;
        }
    }
} 