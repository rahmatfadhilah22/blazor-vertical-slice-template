using BlazorTemplate.Client.Models;
using BlazorTemplate.Client.Services;
using BlazorTemplate.Shared.Components;
using BlazorTemplate.Shared.Components.DataGrid;
using BlazorTemplate.Shared.Components.Form;
using Microsoft.AspNetCore.Components;

namespace BlazorTemplate.Client.Features.Orders.Components;

public partial class OrderGrid : BaseComponent
{
    [Inject] public ApiClient Api { get; set; } = default!;
    [Inject] public NavigationManager Nav { get; set; } = default!;

    private DataGrid<Order>? _grid;

    private string DeleteText => _grid?.SelectedCount is > 0
        ? L.Format("common.deleteCount", _grid.SelectedCount)
        : L["common.delete"];

    private async Task<DataGridPage<Order>> LoadAsync(DataGridLoadArgs args)
    {
        var result = await Api.GetPagedAsync<Order>("Orders/GetRows", args.Page, args.PageSize, args.Search);
        return new DataGridPage<Order>(result.Items, result.Total);
    }

    // Order adalah transaksi: form-nya halaman ber-route, bukan modal — bisa di-link.
    private void OpenForm(Order? order) =>
        Nav.NavigateTo(order is null ? "/orders/new" : $"/orders/{order.OrderID}");

    private async Task DeleteSelectedAsync()
    {
        var ids = _grid?.SelectedItems.Select(o => o.OrderID).ToList() ?? [];
        if (ids.Count == 0)
            return;

        if (!await Confirm.DeleteAsync(DialogService, L, L.Format("common.deleteRows", ids.Count)))
            return;

        try
        {
            await Api.DeleteAsync("Orders/Delete", ids);
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
