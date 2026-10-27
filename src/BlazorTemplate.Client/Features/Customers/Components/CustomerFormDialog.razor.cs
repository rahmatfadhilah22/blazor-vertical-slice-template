using BlazorTemplate.Client.Models;
using BlazorTemplate.Client.Services;
using BlazorTemplate.Shared.Components;
using BlazorTemplate.Shared.Components.Form;
using Microsoft.AspNetCore.Components;

namespace BlazorTemplate.Client.Features.Customers.Components;

public partial class CustomerFormDialog : BaseComponent
{
    [Parameter] public Customer? Customer { get; set; }
    [Parameter] public DialogGuard Guard { get; set; } = new();
    [Inject] public ApiClient Api { get; set; } = default!;

    private Customer _model = new();
    private readonly Dictionary<string, string> _errors = [];
    private string? _message;

    protected override void OnInitialized()
    {
        base.OnInitialized();

        if (Customer is null)
            _model = DraftStore.GetOrCreate<Customer>();

        if (Customer is not null)
            _model = new Customer
            {
                CustomerID = Customer.CustomerID,
                CompanyName = Customer.CompanyName,
                ContactName = Customer.ContactName,
                ContactTitle = Customer.ContactTitle,
                Address = Customer.Address,
                City = Customer.City,
                Region = Customer.Region,
                PostalCode = Customer.PostalCode,
                Country = Customer.Country,
                Phone = Customer.Phone,
                Fax = Customer.Fax,
            };
    }

    private string? ErrorFor(string field)
        => _errors.TryGetValue(field, out var message) ? message : null;

    private async Task SaveAsync(Customer model)
    {
        if (Guard.Busy)
            return;

        _errors.Clear();
        _message = null;
        Guard.Busy = true;
        try
        {
            if (Customer is null)
                await Api.PostAsync<Customer>("Customers/Insert", model);
            else
                await Api.PutAsync($"Customers/Update/{model.CustomerID}", model);

            DraftStore.Clear<Customer>();
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
