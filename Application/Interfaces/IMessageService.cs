using System;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IMessageService
    {
        Task PublishAsync<T>(string queueName, T message);
    }
} 