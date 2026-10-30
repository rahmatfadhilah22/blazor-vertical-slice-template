namespace BlazorTemplate.Client.Models;

public class Territory
{
    public string TerritoryID { get; set; } = "";
    public string TerritoryDescription { get; set; } = "";
    public int RegionID { get; set; }

    // Hasil join GetRows, hanya untuk tampilan grid.
    public string? RegionDescription { get; set; }
}
