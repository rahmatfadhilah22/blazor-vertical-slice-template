using System.Globalization;

namespace BlazorTemplate.Shared;

/// <summary>Format tampilan yang tidak boleh ikut culture browser.</summary>
public static class Formats
{
    /// <summary>Northwind menyimpan uang dalam USD: tampilkan "$1,234.56", bukan "Rp".</summary>
    public static readonly CultureInfo Usd = CultureInfo.GetCultureInfo("en-US");
}
