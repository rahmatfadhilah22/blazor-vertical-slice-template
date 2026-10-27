using BlazorTemplate.Client.Models;
using BlazorTemplate.Client.Services;
using BlazorTemplate.Shared.Components;
using BlazorTemplate.Shared.Components.Form;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace BlazorTemplate.Client.Features.Orders.Components;

public partial class OrderFormDialog : BaseComponent
{
    [Parameter] public Order? Order { get; set; }
    [Parameter] public DialogGuard Guard { get; set; } = new();
    [Inject] public ApiClient Api { get; set; } = default!;

    private Order _model = new();
    private List<Customer> _customers = [];
    private List<Employee> _employees = [];
    private List<Shipper> _shippers = [];
    private readonly Dictionary<string, string> _errors = [];
    private string? _message;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        if (Order is null)
            _model = DraftStore.GetOrCreate<Order>();

        if (Order is not null)
            _model = new Order
            {
                OrderID = Order.OrderID,
                CustomerID = Order.CustomerID,
                EmployeeID = Order.EmployeeID,
                OrderDate = Order.OrderDate,
                RequiredDate = Order.RequiredDate,
                ShippedDate = Order.ShippedDate,
                ShipVia = Order.ShipVia,
                Freight = Order.Freight,
                ShipName = Order.ShipName,
                ShipAddress = Order.ShipAddress,
                ShipCity = Order.ShipCity,
                ShipRegion = Order.ShipRegion,
                ShipPostalCode = Order.ShipPostalCode,
                ShipCountry = Order.ShipCountry,
            };

        try
        {
            // Daftar awal untuk menampilkan nilai terpilih; pencarian dropdown memakai LoadXxxAsync.
            _customers = (await Api.GetPagedAsync<Customer>("Customers/GetRows", 1, 100)).Items.ToList();
            _employees = (await Api.GetPagedAsync<Employee>("Employees/GetRows", 1, 100)).Items.ToList();
            _shippers = (await Api.GetPagedAsync<Shipper>("Shippers/GetRows", 1, 100)).Items.ToList();
        }
        catch (Exception ex)
        {
            _message = ex.Message;
        }
    }

    private async Task LoadCustomersAsync(LoadDataArgs args)
        => _customers = (await Api.GetPagedAsync<Customer>("Customers/GetRows", 1, 50, args.Filter)).Items.ToList();

    private async Task LoadEmployeesAsync(LoadDataArgs args)
        => _employees = (await Api.GetPagedAsync<Employee>("Employees/GetRows", 1, 50, args.Filter)).Items.ToList();

    private async Task LoadShippersAsync(LoadDataArgs args)
        => _shippers = (await Api.GetPagedAsync<Shipper>("Shippers/GetRows", 1, 50, args.Filter)).Items.ToList();

    private string? ErrorFor(string field)
        => _errors.TryGetValue(field, out var message) ? message : null;

    private async Task SaveAsync(Order model)
    {
        if (Guard.Busy)
            return;

        _errors.Clear();
        _message = null;
        Guard.Busy = true;
        try
        {
            if (Order is null)
                await Api.PostAsync<Order>("Orders/Insert", model);
            else
                await Api.PutAsync($"Orders/Update/{model.OrderID}", model);

            DraftStore.Clear<Order>();
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
