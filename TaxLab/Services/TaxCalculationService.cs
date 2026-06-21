using Microsoft.EntityFrameworkCore;
using TaxLab.Data;
using TaxLab.Models;

namespace TaxLab.Services;

public class TaxCalculationService
{
    private readonly TaxLabDbContext _dbContext;

    public TaxCalculationService(TaxLabDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<TaxCalculationResponse> CalculateAsync(decimal annualSalary)
    {
        if (annualSalary < 0)
        {
            return new TaxCalculationResponse
            {
                TotalTax = 0,
                TakeHomePay = annualSalary
            };
        }

        var bands = await _dbContext.TaxRates
            .Where(b => b.BandStart <= annualSalary)
            .ToListAsync();

        var totalTax = bands.Sum(band =>
        {
            var taxableAmount = Math.Min(annualSalary, band.BandFinish ?? annualSalary) - band.BandStart;
            return taxableAmount * band.TaxRate;
        });

        return new TaxCalculationResponse
        {
            TotalTax = Math.Round(totalTax, 2),
            TakeHomePay = Math.Round(annualSalary - totalTax, 2)
        };
    }
}
