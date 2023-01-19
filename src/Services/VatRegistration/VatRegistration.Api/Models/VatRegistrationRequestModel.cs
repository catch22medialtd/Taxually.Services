namespace VatRegistration.Api.Models
{
    public class VatRegistrationRequestModel
    {
        public string CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string CountryCode { get; set; }
    }
}