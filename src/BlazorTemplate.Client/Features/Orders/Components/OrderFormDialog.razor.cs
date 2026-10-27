using BlazorTemplate.Client.Components;
using BlazorTemplate.Client.Models;
using BlazorTemplate.Client.Services;
using BlazorTemplate.Shared.Components;
using BlazorTemplate.Shared.Components.Form;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace BlazorTemplate.Client.Features.Orders.Components;

public partial class OrderFormDialog : FormDialogBase<Order>
{
    [Parameter] public Order? Order { get; set; }

    private Order _model = new();
    private List<Customer> _customers = [];
    private List<Employee> _employees = [];
    private List<Shipper> _shippers = [];

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

    private Task SaveAsync(Order model) => SubmitAsync(model, "Orders", Order is null ? null : model.OrderID);
}
