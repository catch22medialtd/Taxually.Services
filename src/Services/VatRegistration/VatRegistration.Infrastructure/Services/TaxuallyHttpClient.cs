using VatRegistration.Infrastructure.Interfaces;

namespace VatRegistration.Infrastructure.Services
{
    public class TaxuallyHttpClient : ITaxuallyHttpClient
    {
        public async Task PostAsync<TRequest>(string url, TRequest request)
        {
            // Mimic call to a external service
            await Task.Delay(1000);
        }
    }
}