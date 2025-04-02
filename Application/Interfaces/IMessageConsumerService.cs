using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IMessageConsumerService
    {
        Task StartConsumingAsync(CancellationToken cancellationToken);
        Task StopConsumingAsync(CancellationToken cancellationToken);
    }
} 