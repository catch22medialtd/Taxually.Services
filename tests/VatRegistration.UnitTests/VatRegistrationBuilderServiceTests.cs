using Microsoft.Extensions.Configuration;
using Moq;
using VatRegistration.Api.Models;
using VatRegistration.Api.Services;
using VatRegistration.Infrastructure.Dtos;
using VatRegistration.Infrastructure.Interfaces;

namespace VatRegistration.UnitTests
{
    public class VatRegistrationBuilderServiceTests
    {
        private IConfigurationRoot _config;

        [OneTimeSetUp]
        public void Setup()
        {
            var inMemoryConfig = new Dictionary<string, string> {
                {"AppSettings:UkTaxApiUrl", "https://api.uktax.gov.uk"},
                {"AppSettings:CsvFilename", "vat-registration-csv"},
                {"AppSettings:XmlFilename", "vat-registration-xml"}
            };

            _config = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemoryConfig)
                .Build();
        }

        [Test]
        public void RegisterCompanyForVatNumber_NonSupportedCountryCodeGiven_ThrowsArgumentException()
        {
            // Arrange
            var request = new VatRegistrationRequestModel { CompanyId = "123", CompanyName = "Test", CountryCode = "ZZ" };
            var httpClient = new Mock<ITaxuallyHttpClient>();
            var builder = new VatRegistrationBuilderService(httpClient.Object, It.IsAny<ITaxuallyQueueClient>(), It.IsAny<IFileGenerator>(), _config);

            // Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(async () => await builder.RegisterCompanyForVatNumber(request));

            Assert.That(ex.Message, Is.EqualTo($"{nameof(request.CountryCode)} is not supported"));
        }

        [Test]
        public async Task RegisterCompanyForVatNumber_UKCompanyCodeGiven_HttpClientApiIsCalledWithCorrectParams()
        {
            // Arrange
            const string url = "https://api.uktax.gov.uk";
            var request = new VatRegistrationRequestModel { CompanyId = "123", CompanyName = "Test", CountryCode = "GB" };
            var mock = new Mock<ITaxuallyHttpClient>();
            var builder = new VatRegistrationBuilderService(mock.Object, It.IsAny<ITaxuallyQueueClient>(), It.IsAny<IFileGenerator>(), _config);

            // Act
            await builder.RegisterCompanyForVatNumber(request);

            // Assert
            mock.Verify(m => m.PostAsync(url, request), Times.Once);
        }

        [Test]
        public async Task RegisterCompanyForVatNumber_FrenchCompanyCodeGiven_QueueClientIsCalledWithCorrectParams()
        {
            // Arrange
            var request = new VatRegistrationRequestModel { CompanyId = "123", CompanyName = "Test", CountryCode = "FR" };
            var mockQueueClient = new Mock<ITaxuallyQueueClient>();
            var mockFileGenerator = new Mock<IFileGenerator>();

            QueueElementDto paramObject = null;

            mockQueueClient.Setup(m => m.EnqueueAsync(It.IsAny<QueueElementDto>()))
                    .Callback<QueueElementDto>((obj) => paramObject = obj);

            var fileContent = new byte[3] { 1, 2, 3 };

            mockFileGenerator
                .Setup(m => m.GenerateCsvFile(request.CompanyId, request.CompanyName))
                .Returns(fileContent);

            var builder = new VatRegistrationBuilderService(It.IsAny<ITaxuallyHttpClient>(), mockQueueClient.Object, mockFileGenerator.Object, _config);

            // Act
            await builder.RegisterCompanyForVatNumber(request);

            // Assert
            mockQueueClient.Verify(m => m.EnqueueAsync(It.IsAny<QueueElementDto>()), Times.Once);
            mockFileGenerator.Verify(m => m.GenerateCsvFile(request.CompanyId, request.CompanyName), Times.Once);
            Assert.That(paramObject.Filename, Is.EqualTo("vat-registration-csv"));
            Assert.That((paramObject as CsvFileQueueElementDto).Contents, Is.EqualTo(fileContent));
            Assert.That(paramObject, Is.InstanceOf<CsvFileQueueElementDto>());
        }

        [Test]
        public async Task RegisterCompanyForVatNumber_GermanCompanyCodeGiven_QueueClientIsCalledWithCorrectParams()
        {
            // Arrange
            var request = new VatRegistrationRequestModel { CompanyId = "123", CompanyName = "Test", CountryCode = "DE" };
            var mockQueueClient = new Mock<ITaxuallyQueueClient>();
            var mockFileGenerator = new Mock<IFileGenerator>();

            QueueElementDto paramObject = null;

            mockQueueClient
                .Setup(m => m.EnqueueAsync(It.IsAny<QueueElementDto>()))
                .Callback<QueueElementDto>((obj) => paramObject = obj);

            const string fileContent = "<Some random xml content>";

            mockFileGenerator
                .Setup(m => m.GenerateXmlFile(request.CompanyId, request.CompanyName))
                .Returns(fileContent);

            var builder = new VatRegistrationBuilderService(It.IsAny<ITaxuallyHttpClient>(), mockQueueClient.Object, mockFileGenerator.Object, _config);

            // Act
            await builder.RegisterCompanyForVatNumber(request);

            // Assert
            mockQueueClient.Verify(m => m.EnqueueAsync(It.IsAny<QueueElementDto>()), Times.Once);
            mockFileGenerator.Verify(m => m.GenerateXmlFile(request.CompanyId, request.CompanyName), Times.Once);
            Assert.That(paramObject.Filename, Is.EqualTo("vat-registration-xml"));
            Assert.That((paramObject as XmlFileQueueElementDto).Contents, Is.EqualTo(fileContent));
            Assert.That(paramObject, Is.InstanceOf<XmlFileQueueElementDto>());
        }
    }
}