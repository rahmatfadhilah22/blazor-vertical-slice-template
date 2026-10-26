using BlazorTemplate.Client.Models;
using BlazorTemplate.Client.Services;
using BlazorTemplate.Shared.Components;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace BlazorTemplate.Client.Features.Categories.Components;

public partial class CategoryFormDialog : BaseComponent
{
    [Parameter] public Category? Category { get; set; }

    [Inject] public ApiClient Api { get; set; } = default!;

    private readonly CategoryInput _input = new();
    private readonly Dictionary<string, string> _errors = [];
    private string? _message;
    private bool _saving;

    protected override void OnInitialized()
    {
        if (Category is null)
            return;

        _input.CategoryId = Category.CategoryID;
        _input.CategoryName = Category.CategoryName;
        _input.Description = Category.Description;
        _input.Picture = Category.Picture;
    }

    private async Task SaveAsync()
    {
        _errors.Clear();
        _message = null;

        // Server tetap otoritas; ini hanya umpan balik cepat sebelum request.
        if (string.IsNullOrWhiteSpace(_input.CategoryName))
            _errors["CategoryName"] = "Nama kategori wajib diisi.";
        else if (_input.CategoryName.Length > 15)
            _errors["CategoryName"] = "Nama kategori maksimal 15 karakter.";

        if (_errors.Count > 0)
            return;

        _saving = true;
        try
        {
            if (Category is null)
                await Api.PostAsync<Category>("Categories/Insert", _input);
            else
                await Api.PutAsync($"Categories/Update/{_input.CategoryId}", _input);

            DialogService.Close(true);
        }
        catch (ApiException ex)
        {
            // 400 validasi: server mengirim errors per field.
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
            _saving = false;
        }
    }
}
