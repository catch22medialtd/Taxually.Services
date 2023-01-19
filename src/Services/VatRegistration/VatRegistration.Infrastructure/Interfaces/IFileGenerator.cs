namespace VatRegistration.Infrastructure.Interfaces
{
    public interface IFileGenerator
    {
        byte[] GenerateCsvFile(string companyId, string companyName);
        string GenerateXmlFile(string companyId, string companyName);
    }
}