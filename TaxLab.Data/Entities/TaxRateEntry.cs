namespace TaxLab.Data.Entities;

public class TaxRateEntry
{
    public Guid TaxRateId { get; set; }

    /// <summary>Inclusive lower bound for the tax band.</summary>
    public decimal BandStart { get; set; }

    /// <summary>Inclusive upper bound; null means no upper limit (top band).</summary>
    public decimal? BandFinish { get; set; }

    /// <summary>Tax rate for this band (e.g. 0.175 = 17.5%).</summary>
    public decimal TaxRate { get; set; }

    public DateTime CreatedDateUTC { get; set; }
}
