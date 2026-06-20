using System.Text;
using TaxLab.Data;
using TaxLab.Data.Entities;

namespace TaxLab.Worker;

public class TaxRateImporter
{
    private readonly TaxLabDbContext _dbContext;

    public TaxRateImporter(TaxLabDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task ImportAsync(string csvPath)
    {
        if (!File.Exists(csvPath))
        {
            Console.WriteLine($"CSV file not found: {csvPath}");
            return;
        }

        var entries = ParseCsv(csvPath);

        if (entries.Count == 0)
        {
            Console.WriteLine("No tax rate entries found in CSV.");
            return;
        }

        foreach (var entry in entries)
        {
            _dbContext.TaxRates.Add(entry);
        }

        await _dbContext.SaveChangesAsync();
        Console.WriteLine($"Imported {entries.Count} tax rate band(s).");
    }

    private static List<TaxRateEntry> ParseCsv(string csvPath)
    {
        var entries = new List<TaxRateEntry>();
        var lines = File.ReadAllLines(csvPath);

        // Skip metadata rows — find the header row starting with "Band Start"
        int dataStartIndex = -1;
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].StartsWith("Band Start", StringComparison.OrdinalIgnoreCase))
            {
                dataStartIndex = i + 1;
                break;
            }
        }

        if (dataStartIndex < 0)
        {
            Console.WriteLine("Could not find 'Band Start' header row in CSV.");
            return entries;
        }

        for (int i = dataStartIndex; i < lines.Length; i++)
        {
            var line = lines[i].Trim();
            if (string.IsNullOrWhiteSpace(line)) continue;

            var columns = SplitCsvLine(line);
            if (columns.Count < 3) continue;

            // BandStart must be a valid number — skip total/summary rows
            if (!TryParseDecimal(columns[0], out var bandStart)) continue;

            decimal? bandFinish = null;
            var bandFinishRaw = columns[1].Trim();
            if (!string.IsNullOrWhiteSpace(bandFinishRaw) &&
                !bandFinishRaw.Equals("and over", StringComparison.OrdinalIgnoreCase))
            {
                if (!TryParseDecimal(bandFinishRaw, out var bf)) continue;
                bandFinish = bf;
            }

            var rateStr = columns[2].Replace("%", "").Trim();
            if (!decimal.TryParse(rateStr, out var ratePercent)) continue;

            entries.Add(new TaxRateEntry
            {
                TaxRateId = Guid.NewGuid(),
                BandStart = bandStart,
                BandFinish = bandFinish,
                TaxRate = ratePercent / 100m,
                CreatedDateUTC = DateTime.UtcNow
            });
        }

        return entries;
    }

    private static bool TryParseDecimal(string value, out decimal result)
    {
        // Strip quotes and thousand-separator commas (e.g. "14,000.00" → 14000.00)
        var cleaned = value.Replace("\"", "").Replace(",", "").Trim();
        return decimal.TryParse(cleaned, out result);
    }

    private static List<string> SplitCsvLine(string line)
    {
        // Handles quoted fields that may contain commas
        var fields = new List<string>();
        var current = new StringBuilder();
        bool inQuotes = false;

        foreach (char c in line)
        {
            if (c == '"')
            {
                inQuotes = !inQuotes;
            }
            else if (c == ',' && !inQuotes)
            {
                fields.Add(current.ToString());
                current.Clear();
            }
            else
            {
                current.Append(c);
            }
        }

        fields.Add(current.ToString());
        return fields;
    }
}
