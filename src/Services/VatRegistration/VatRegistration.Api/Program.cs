using VatRegistration.Api.Interfaces;
using VatRegistration.Api.Services;
using VatRegistration.Infrastructure.Interfaces;
using VatRegistration.Infrastructure.Services;
using VatRegistration.Infrastructure.Utilties;

namespace VatRegistration.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddTransient<IVatRegistrationBuilderService, VatRegistrationBuilderService>();
            builder.Services.AddTransient<ITaxuallyHttpClient, TaxuallyHttpClient>();
            builder.Services.AddTransient<ITaxuallyQueueClient, TaxuallyQueueClient>();
            builder.Services.AddTransient<IFileGenerator, FileGenerator>();
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}