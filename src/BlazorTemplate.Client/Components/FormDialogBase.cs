using BlazorTemplate.Client.Services;
using BlazorTemplate.Shared.Components;
using BlazorTemplate.Shared.Components.Form;
using Microsoft.AspNetCore.Components;

namespace BlazorTemplate.Client.Components;

/// <summary>
/// Kerangka simpan dialog: status sibuk, error per field, draft, dan penutupan setelah
/// sukses. Turunan cukup menulis isi form dan satu baris <c>SaveAsync</c> yang memanggil
/// <see cref="SubmitAsync"/>.
/// </summary>
public abstract class FormDialogBase<T> : BaseComponent where T : class
{
    [Parameter] public DialogGuard Guard { get; set; } = new();

    [Inject] public ApiClient Api { get; set; } = default!;

    protected readonly Dictionary<string, string> _errors = [];
    protected string? _message;

    protected string? ErrorFor(string field)
    {
        if (_errors.TryGetValue(field, out var message))
            return message;

        return null;
    }

    /// <summary>
    /// Simpan model. <paramref name="id"/> null = insert, selain itu update.
    /// URL action dioper pemanggil karena nama action tiap backend bisa berbeda.
    /// </summary>
    protected async Task SubmitAsync(T model, string insertUrl, string updateUrl, object? id)
    {
        if (Guard.Busy)
            return;

        _errors.Clear();
        _message = null;
        Guard.Busy = true;
        try
        {
            if (id is null)
                await Api.PostAsync<T>(insertUrl, model);
            else
                await Api.PutAsync($"{updateUrl}/{id}", model);

            DraftStore.Clear<T>();
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
