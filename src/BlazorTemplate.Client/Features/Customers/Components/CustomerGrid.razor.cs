using BlazorTemplate.Client.Models;
using BlazorTemplate.Client.Services;
using BlazorTemplate.Shared.Components;
using BlazorTemplate.Shared.Components.DataGrid;
using BlazorTemplate.Shared.Components.Form;
using Microsoft.AspNetCore.Components;

namespace BlazorTemplate.Client.Features.Customers.Components;

public partial class CustomerGrid : BaseComponent
{
    [Inject] public ApiClient Api { get; set; } = default!;

    private DataGrid<Customer>? _grid;

    private string DeleteText => _grid?.SelectedCount is > 0
        ? L.Format("common.deleteCount", _grid.SelectedCount)
        : L["common.delete"];

    private async Task<DataGridPage<Customer>> LoadAsync(DataGridLoadArgs args)
    {
        var result = await Api.GetPagedAsync<Customer>("Customers/GetRows", args.Page, args.PageSize, args.Search);
        return new DataGridPage<Customer>(result.Items, result.Total);
    }

    private async Task OpenFormAsync(Customer? customer)
    {
        var guard = new DialogGuard();
        var parameters = new Dictionary<string, object?> { [nameof(CustomerFormDialog.Guard)] = guard };
        if (customer is not null)
            parameters[nameof(CustomerFormDialog.Customer)] = customer;

        object? saved = await DialogService.OpenAsync<CustomerFormDialog>(
            customer is null
                ? L.Format("common.addEntity", L["entity.customer"])
                : L.Format("common.editEntity", L["entity.customer"]),
            parameters,
            new DialogOptions { Width = DialogSize.WidthFor(2), CanClose = () => Task.FromResult(!guard.Busy) });

        if (saved is true)
            await ReloadAsync();
    }

    private async Task DeleteSelectedAsync()
    {
        var ids = _grid?.SelectedItems.Select(c => c.CustomerID).ToList() ?? [];
        if (ids.Count == 0)
            return;

        if (!await Confirm.DeleteAsync(DialogService, L, L.Format("common.deleteRows", ids.Count)))
            return;

        try
        {
            await Api.DeleteAsync("Customers/Delete", ids);
            await ReloadAsync();
            Toast.Deleted(NotificationService, L);
        }
        catch (Exception ex)
        {
            Toast.DeleteFailed(NotificationService, L, ex);
        }
    }

    private Task ReloadAsync() => _grid?.ReloadAsync(firstPage: true) ?? Task.CompletedTask;
}
