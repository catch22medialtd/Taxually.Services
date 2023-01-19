using VatRegistration.Infrastructure.Dtos;

namespace VatRegistration.Infrastructure.Interfaces
{
    public interface ITaxuallyQueueClient
    {
        Task EnqueueAsync(QueueElementDto element);
    }
}