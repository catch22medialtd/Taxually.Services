using VatRegistration.Infrastructure.Dtos;
using VatRegistration.Infrastructure.Interfaces;

namespace VatRegistration.Infrastructure.Services
{
    public class TaxuallyQueueClient : ITaxuallyQueueClient
    {
        public Task EnqueueAsync(QueueElementDto element)
        {
            // Find out queue element type and process accordingly
            return Task.CompletedTask;
        }
    }
}