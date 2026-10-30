using BlazorTemplate.Client.Components;
using BlazorTemplate.Client.Models;
using BlazorTemplate.Client.Services;
using BlazorTemplate.Shared.Components.Form;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace BlazorTemplate.Client.Features.Orders.Pages;

public partial class OrderForm : FormDialogBase<Order>
{
    /// <summary>Dari route <c>/orders/{Id}</c>. Null = <c>/orders/new</c>.</summary>
    [Parameter] public int? Id { get; set; }

    [Inject] public NavigationManager Nav { get; set; } = default!;

    private Order _model = new();
    private List<Customer> _customers = [];
    private List<Employee> _employees = [];
    private List<Shipper> _shippers = [];
    private bool _loading = true;
    private bool _notFound;
    private bool _lookupsLoaded;

    // Navigasi /orders/new → /orders/{id} tidak membuat instance baru, jadi pemuatan
    // data ada di OnParametersSetAsync, bukan OnInitialized.
    private object? _loadedFor;

    protected override async Task OnParametersSetAsync()
    {
        var key = (object?)Id ?? "new";
        if (Equals(_loadedFor, key))
            return;

        _loadedFor = key;
        _loading = true;
        _notFound = false;

        try
        {
            if (!_lookupsLoaded)
            {
                // Daftar awal untuk menampilkan nilai terpilih; pencarian dropdown memakai LoadXxxAsync.
                _customers = (await Api.GetPagedAsync<Customer>("Customers/GetRows", 1, 100)).Items.ToList();
                _employees = (await Api.GetPagedAsync<Employee>("Employees/GetRows", 1, 100)).Items.ToList();
                _shippers = (await Api.GetPagedAsync<Shipper>("Shippers/GetRows", 1, 100)).Items.ToList();
                _lookupsLoaded = true;
            }

            _model = Id is null
                ? DraftStore.GetOrCreate<Order>()
                : await Api.GetAsync<Order>($"Orders/GetRow/{Id}");
        }
        catch (ApiException ex) when (ex.StatusCode == 404)
        {
            _notFound = true;
        }
        catch (Exception ex)
        {
            _message = ex.Message;
        }
        finally
        {
            _loading = false;
        }
    }

    private async Task LoadCustomersAsync(LoadDataArgs args)
    {
        _customers = (await Api.GetPagedAsync<Customer>("Customers/GetRows", 1, 50, args.Filter)).Items.ToList();
    }

    private async Task LoadEmployeesAsync(LoadDataArgs args)
    {
        _employees = (await Api.GetPagedAsync<Employee>("Employees/GetRows", 1, 50, args.Filter)).Items.ToList();
    }

    private async Task LoadShippersAsync(LoadDataArgs args)
    {
        _shippers = (await Api.GetPagedAsync<Shipper>("Shippers/GetRows", 1, 50, args.Filter)).Items.ToList();
    }

    private Task SaveAsync(Order model)
    {
        return Id is null
            ? SubmitAsync(model, "Orders/Insert", "Orders/Update", null)
            : SubmitAsync(model, "Orders/Insert", "Orders/Update", Id);
    }

    // Setelah simpan tetap di halaman: setelah header biasanya langsung isi item.
    // Insert belum punya nomor, jadi URL-nya baru bisa ditentukan dari balikan server.
    protected override Task OnSavedAsync(Order? saved)
    {
        NotificationService.Notify(NotificationSeverity.Success, L["common.saved"]);
        if (saved is not null)
            Nav.NavigateTo($"/orders/{saved.OrderID}");

        return Task.CompletedTask;
    }
}
