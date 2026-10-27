using BlazorTemplate.Client.Models;
using BlazorTemplate.Client.Services;
using BlazorTemplate.Shared.Components;
using BlazorTemplate.Shared.Components.DataGrid;
using BlazorTemplate.Shared.Components.Form;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace BlazorTemplate.Client.Features.Orders.Components;

public partial class OrderGrid : BaseComponent
{
    [Inject] public ApiClient Api { get; set; } = default!;

    private DataGrid<Order>? _grid;

    private string DeleteText => _grid?.SelectedCount is > 0
        ? L.Format("common.deleteCount", _grid.SelectedCount)
        : L["common.delete"];

    private async Task<DataGridPage<Order>> LoadAsync(DataGridLoadArgs args)
    {
        var result = await Api.GetPagedAsync<Order>("Orders/GetRows", args.Page, args.PageSize, args.Search);
        return new DataGridPage<Order>(result.Items, result.Total);
    }

    private async Task OpenFormAsync(Order? order)
    {
        var guard = new DialogGuard();
        var parameters = new Dictionary<string, object?> { [nameof(OrderFormDialog.Guard)] = guard };
        if (order is not null)
            parameters[nameof(OrderFormDialog.Order)] = order;

        object? saved = await DialogService.OpenAsync<OrderFormDialog>(
            order is null
                ? L.Format("common.addEntity", L["entity.order"])
                : L.Format("common.editEntity", L["entity.order"]),
            parameters,
            new DialogOptions { Width = DialogSize.WidthFor(2), CanClose = () => Task.FromResult(!guard.Busy) });

        if (saved is true)
            await ReloadAsync();
    }

    private async Task DeleteSelectedAsync()
    {
        var ids = _grid?.SelectedItems.Select(o => o.OrderID).ToList() ?? [];
        if (ids.Count == 0)
            return;

        var confirmed = await DialogService.Confirm(
            L.Format("common.deleteRows", ids.Count),
            L["common.confirm"],
            new ConfirmOptions { OkButtonText = L["common.delete"], CancelButtonText = L["common.cancel"] });
        if (confirmed is not true)
            return;

        try
        {
            await Api.DeleteAsync("Orders/Delete", ids);
            await ReloadAsync();
            NotificationService.Notify(NotificationSeverity.Success, L["common.deleted"]);
        }
        catch (Exception ex)
        {
            NotificationService.Notify(NotificationSeverity.Error, L["common.deleteFailed"], ex.Message);
        }
    }

    private Task ReloadAsync() => _grid?.ReloadAsync(firstPage: true) ?? Task.CompletedTask;
}
