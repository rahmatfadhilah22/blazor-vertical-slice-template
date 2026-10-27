using BlazorTemplate.Client.Models;
using BlazorTemplate.Client.Services;
using BlazorTemplate.Shared.Components;
using BlazorTemplate.Shared.Components.Form;
using Microsoft.AspNetCore.Components;

namespace BlazorTemplate.Client.Features.Suppliers.Components;

public partial class SupplierFormDialog : BaseComponent
{
    [Parameter] public Supplier? Supplier { get; set; }
    [Parameter] public DialogGuard Guard { get; set; } = new();
    [Inject] public ApiClient Api { get; set; } = default!;

    private Supplier _model = new();
    private readonly Dictionary<string, string> _errors = [];
    private string? _message;

    protected override void OnInitialized()
    {
        base.OnInitialized();

        if (Supplier is null)
            _model = DraftStore.GetOrCreate<Supplier>();

        if (Supplier is not null)
            _model = new Supplier
            {
                SupplierID = Supplier.SupplierID,
                CompanyName = Supplier.CompanyName,
                ContactName = Supplier.ContactName,
                ContactTitle = Supplier.ContactTitle,
                Address = Supplier.Address,
                City = Supplier.City,
                Region = Supplier.Region,
                PostalCode = Supplier.PostalCode,
                Country = Supplier.Country,
                Phone = Supplier.Phone,
                Fax = Supplier.Fax,
                Homepage = Supplier.Homepage,
            };
    }

    private string? ErrorFor(string field)
        => _errors.TryGetValue(field, out var message) ? message : null;

    private async Task SaveAsync(Supplier model)
    {
        if (Guard.Busy)
            return;

        _errors.Clear();
        _message = null;
        Guard.Busy = true;
        try
        {
            if (Supplier is null)
                await Api.PostAsync<Supplier>("Suppliers/Insert", model);
            else
                await Api.PutAsync($"Suppliers/Update/{model.SupplierID}", model);

            DraftStore.Clear<Supplier>();
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
