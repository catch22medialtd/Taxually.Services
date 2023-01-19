namespace VatRegistration.Infrastructure.Interfaces
{
    public interface ITaxuallyHttpClient
    {
        Task PostAsync<TRequest>(string url, TRequest request);
    }
}