namespace TaxLab.Models;

public class TaxCalculationRequest
{
    /// <summary>Annual salary, supports comma-formatted input e.g. "60,000"</summary>
    public string AnnualSalary { get; set; } = string.Empty;
}
