using Microsoft.AspNetCore.Mvc;
using TaxLab.Models;
using TaxLab.Services;

namespace TaxLab.Controllers;

[ApiController]
[Route("[controller]")]
public class TaxCalculationsController : ControllerBase
{
    private readonly TaxCalculationService _taxCalculationService;

    public TaxCalculationsController(TaxCalculationService taxCalculationService)
    {
        _taxCalculationService = taxCalculationService;
    }

    [HttpPost]
    public async Task<ActionResult<TaxCalculationResponse>> Post([FromBody] TaxCalculationRequest request)
    {
        // Strip commas to support formatted input e.g. "60,000"
        if (!decimal.TryParse(request.AnnualSalary.Replace(",", ""), out var salary))
            return BadRequest("annualSalary is not a valid number.");

        return await _taxCalculationService.CalculateAsync(salary);
    }
}
