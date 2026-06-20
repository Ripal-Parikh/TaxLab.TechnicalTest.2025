using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaxLab.Data.Migrations;

/// <inheritdoc />
public partial class InitialCreate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "TaxRates",
            columns: table => new
            {
                TaxRateId = table.Column<Guid>(type: "TEXT", nullable: false),
                BandStart = table.Column<decimal>(type: "TEXT", nullable: false),
                BandFinish = table.Column<decimal>(type: "TEXT", nullable: true),
                TaxRate = table.Column<decimal>(type: "TEXT", nullable: false),
                CreatedDateUTC = table.Column<DateTime>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_TaxRates", x => x.TaxRateId);
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "TaxRates");
    }
}
