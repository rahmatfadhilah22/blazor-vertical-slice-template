namespace BlazorTemplate.Client.Models;

/// <summary>Bentuk respons endpoint GetRows backend (page 1-based, pageSize divalidasi server).</summary>
public class PagedResult<T>
{
    public List<T> Items { get; set; } = [];
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int Total { get; set; }
    public int TotalPages { get; set; }
}
