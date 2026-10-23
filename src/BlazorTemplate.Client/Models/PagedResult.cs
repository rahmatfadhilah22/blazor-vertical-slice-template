namespace BlazorTemplate.Client.Models;

public class PagedResult
{
    public List<Category> Items { get; set; } = [];
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int Total { get; set; }
    public int TotalPages { get; set; }
}
