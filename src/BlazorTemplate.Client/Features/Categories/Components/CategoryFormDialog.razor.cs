using BlazorTemplate.Client.Models;
using BlazorTemplate.Client.Services;
using BlazorTemplate.Shared.Components;
using BlazorTemplate.Shared.Components.Form;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace BlazorTemplate.Client.Features.Categories.Components;

public partial class CategoryFormDialog : BaseComponent
{
    [Parameter] public Category? Category { get; set; }
    [Parameter] public DialogGuard Guard { get; set; } = new();
    [Inject] public ApiClient Api { get; set; } = default!;
    private readonly Category _model = new();
    private readonly Dictionary<string, string> _errors = [];
    private string? _message;

    protected override void OnInitialized()
    {
        // Wajib: BaseComponent memasang langganan perubahan bahasa di sini.
        base.OnInitialized();

        if (Category is null)
            return;

        _model.CategoryID = Category.CategoryID;
        _model.CategoryName = Category.CategoryName;
        _model.Description = Category.Description;
        _model.Picture = Category.Picture;
    }
    private string? ErrorFor(string field)
        => _errors.TryGetValue(field, out var message) ? message : null;

    private async Task SaveAsync(Category model)
    {
        if (Guard.Busy)
            return;

        _errors.Clear();
        _message = null;
        Guard.Busy = true;
        try
        {
            if (model.CategoryID == 0)
                await Api.PostAsync<Category>("Categories/Insert", model);
            else
                await Api.PutAsync($"Categories/Update/{model.CategoryID}", model);
            DialogService.Close(true);
        }
        catch (ApiException ex)
        {
            _message = ex.Errors.Count > 0 ? null : ex.Message;
            foreach (var (field, messages) in ex.Errors)
                _errors[field] = string.Join(" ", messages);
        }
        catch (Exception ex)
        {
            _message = ex.Message;
        }
        finally
        {
            Guard.Busy = false;
        }
    }
}
