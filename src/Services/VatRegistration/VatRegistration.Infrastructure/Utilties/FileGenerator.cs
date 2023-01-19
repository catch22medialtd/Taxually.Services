using System.Text;
using System.Xml.Serialization;
using VatRegistration.Infrastructure.Interfaces;

namespace VatRegistration.Infrastructure.Utilties
{
    public class FileGenerator : IFileGenerator
    {
        public byte[] GenerateCsvFile(string companyId, string companyName)
        {
            var csv = new StringBuilder();

            csv.AppendLine("CompanyId,CompanyName");
            csv.AppendLine($"{companyId}{companyName}");

            return Encoding.UTF8.GetBytes(csv.ToString());
        }

        public string GenerateXmlFile(string companyId, string companyName)
        {
            using var stringwriter = new StringWriter();
            var schema = new VatRegistrationDocumentXmlSchema { CompanyId = companyId, CompanyName = companyName };
            var serializer = new XmlSerializer(typeof(VatRegistrationDocumentXmlSchema));

            serializer.Serialize(stringwriter, schema);

            return stringwriter.ToString();
        }
    }
}