using Microsoft.EntityFrameworkCore;
using TaxLab.Data;
using TaxLab.Data.Entities;
using TaxLab.Services;
using Xunit;

namespace TaxLab.Tests;

public class TaxCalculationServiceTests
{
    /// <summary>
    /// Builds an in-memory DbContext seeded with NZ tax bands:
    ///   0       – 14,000   @ 11.5%
    ///   14,000  – 48,000   @ 21.0%
    ///   48,000  – 70,000   @ 31.5%
    ///   70,000  – (no limit) @ 35.5%
    /// </summary>
    private static TaxLabDbContext BuildDbContext()
    {
        var options = new DbContextOptionsBuilder<TaxLabDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new TaxLabDbContext(options);

        context.TaxRates.AddRange(
            new TaxRateEntry { TaxRateId = Guid.NewGuid(), BandStart = 0,      BandFinish = 14_000,  TaxRate = 0.115m, CreatedDateUTC = DateTime.UtcNow },
            new TaxRateEntry { TaxRateId = Guid.NewGuid(), BandStart = 14_000, BandFinish = 48_000,  TaxRate = 0.21m,  CreatedDateUTC = DateTime.UtcNow },
            new TaxRateEntry { TaxRateId = Guid.NewGuid(), BandStart = 48_000, BandFinish = 70_000,  TaxRate = 0.315m, CreatedDateUTC = DateTime.UtcNow },
            new TaxRateEntry { TaxRateId = Guid.NewGuid(), BandStart = 70_000, BandFinish = null,    TaxRate = 0.355m, CreatedDateUTC = DateTime.UtcNow }
        );
        context.SaveChanges();

        return context;
    }

    private static TaxCalculationService BuildService() =>
        new TaxCalculationService(BuildDbContext());

    [Fact]
    public async Task NegativeSalary_ReturnsTotalTaxZeroAndTakeHomeEqualsSalary()
    {
        var service = BuildService();
        var result = await service.CalculateAsync(-1000m);

        Assert.Equal(0m, result.TotalTax);
        Assert.Equal(-1000m, result.TakeHomePay);
    }

    [Fact]
    public async Task ZeroSalary_ReturnsTotalTaxZeroAndTakeHomeZero()
    {
        var service = BuildService();
        var result = await service.CalculateAsync(0m);

        Assert.Equal(0m, result.TotalTax);
        Assert.Equal(0m, result.TakeHomePay);
    }

    [Fact]
    public async Task SalaryWithinSingleBand_CalculatesCorrectly()
    {
        // 10,000 @ 11.5% = 1,150 tax
        var service = BuildService();
        var result = await service.CalculateAsync(10_000m);

        Assert.Equal(1_150m, result.TotalTax);
        Assert.Equal(8_850m, result.TakeHomePay);
    }

    [Fact]
    public async Task SalarySpanningMultipleBands_CalculatesCorrectly()
    {
        // Band 1: 14,000 * 11.5% =  1,610.00
        // Band 2: 34,000 * 21.0% =  7,140.00
        // Band 3: 12,000 * 31.5% =  3,780.00
        // Total:                   12,530.00
        var service = BuildService();
        var result = await service.CalculateAsync(60_000m);

        Assert.Equal(12_530m, result.TotalTax);
        Assert.Equal(47_470m, result.TakeHomePay);
    }

    [Fact]
    public async Task SalaryAtBandBoundary_CalculatesCorrectly()
    {
        // Exactly 14,000 — only first band applies
        // 14,000 * 11.5% = 1,610
        var service = BuildService();
        var result = await service.CalculateAsync(14_000m);

        Assert.Equal(1_610m, result.TotalTax);
        Assert.Equal(12_390m, result.TakeHomePay);
    }

    [Fact]
    public async Task SalaryInTopBand_NullBandFinish_CalculatesCorrectly()
    {
        // Band 1: 14,000 * 11.5%  = 1,610.00
        // Band 2: 34,000 * 21.0%  = 7,140.00
        // Band 3: 22,000 * 31.5%  = 6,930.00
        // Band 4: 10,000 * 35.5%  = 3,550.00
        // Total:                   19,230.00
        var service = BuildService();
        var result = await service.CalculateAsync(80_000m);

        Assert.Equal(19_230m, result.TotalTax);
        Assert.Equal(60_770m, result.TakeHomePay);
    }
}
