using VatRegistration.Api.Interfaces;
using VatRegistration.Api.Models;
using VatRegistration.Infrastructure.Dtos;
using VatRegistration.Infrastructure.Interfaces;

namespace VatRegistration.Api.Services
{
    public class VatRegistrationBuilderService : IVatRegistrationBuilderService
    {
        private readonly ITaxuallyHttpClient _taxuallyHttpClient;
        private readonly ITaxuallyQueueClient _taxuallyQueueClient;
        private readonly IFileGenerator _fileGenerator;
        private readonly IConfiguration _config;

        public VatRegistrationBuilderService(ITaxuallyHttpClient taxuallyHttpClient, ITaxuallyQueueClient taxuallyQueueClient, IFileGenerator fileGenerator, IConfiguration config)
        {
            _taxuallyHttpClient = taxuallyHttpClient;
            _taxuallyQueueClient = taxuallyQueueClient;
            _fileGenerator = fileGenerator;
            _config = config;
        }

        public async Task RegisterCompanyForVatNumber(VatRegistrationRequestModel request)
        {
            switch (request.CountryCode)
            {
                case "GB":
                    await RegisterUKCompanyForVatNo(request);
                    break;
                case "FR":
                    await RegisterFrenchCompanyForVatNo(request);
                    break;
                case "DE":
                    await RegisterGermanCompanyForVatNo(request);
                    break;
                default:
                    throw new ArgumentException($"{nameof(request.CountryCode)} is not supported");
            }
        }

        private async Task RegisterUKCompanyForVatNo(VatRegistrationRequestModel request)
        {
            var url = _config.GetValue<string>("AppSettings:UkTaxApiUrl");
            
            await _taxuallyHttpClient.PostAsync(url, request);
        }

        private async Task RegisterFrenchCompanyForVatNo(VatRegistrationRequestModel request)
        {
            var filename = _config.GetValue<string>("AppSettings:CsvFilename");
            var contents = _fileGenerator.GenerateCsvFile(request.CompanyId, request.CompanyName);
            var queueElement = new CsvFileQueueElementDto { Filename = filename, Contents = contents };

            await _taxuallyQueueClient.EnqueueAsync(queueElement);
        }

        private async Task RegisterGermanCompanyForVatNo(VatRegistrationRequestModel request)
        {
            var filename = _config.GetValue<string>("AppSettings:XmlFilename");
            var contents = _fileGenerator.GenerateXmlFile(request.CompanyId, request.CompanyName);
            var queueElement = new XmlFileQueueElementDto { Filename = filename, Contents = contents };

            await _taxuallyQueueClient.EnqueueAsync(queueElement);
        }
    }
}