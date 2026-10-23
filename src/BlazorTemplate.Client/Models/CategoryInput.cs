namespace BlazorTemplate.Client.Models;

public class CategoryInput
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = "";
    public string? Description { get; set; }
    public string? Picture { get; set; }
}
