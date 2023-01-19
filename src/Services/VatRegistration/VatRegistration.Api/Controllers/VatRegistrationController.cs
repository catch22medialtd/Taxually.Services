using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using VatRegistration.Api.Interfaces;
using VatRegistration.Api.Models;

namespace VatRegistration.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VatRegistrationController : ControllerBase
    {
        private readonly IVatRegistrationBuilderService _vatRegistrationBuilderService;
        private readonly ILogger<VatRegistrationController> _logger;

        public VatRegistrationController(IVatRegistrationBuilderService vatRegistrationBuilderService, ILogger<VatRegistrationController> logger)
        {
            _vatRegistrationBuilderService = vatRegistrationBuilderService; 
            _logger = logger;
        }

        [HttpPost]
        [Route("RegisterCompanyForVatNumber")]
        [SwaggerOperation(Summary = "Registers UK, French or German Companies for Vat")]
        public async Task<IActionResult> RegisterCompanyForVatNumber([FromBody] VatRegistrationRequestModel request)
        {
            await _vatRegistrationBuilderService
                .RegisterCompanyForVatNumber(request);

            return Ok();
        }
    }
}