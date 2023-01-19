using VatRegistration.Api.Models;

namespace VatRegistration.Api.Interfaces
{
    public interface IVatRegistrationBuilderService
    {
        Task RegisterCompanyForVatNumber(VatRegistrationRequestModel request);
    }
}