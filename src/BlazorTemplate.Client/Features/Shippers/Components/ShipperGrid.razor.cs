using BlazorTemplate.Client.Models;
using BlazorTemplate.Client.Services;
using BlazorTemplate.Shared.Components;
using BlazorTemplate.Shared.Components.DataGrid;
using BlazorTemplate.Shared.Components.Form;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace BlazorTemplate.Client.Features.Shippers.Components;

public partial class ShipperGrid : BaseComponent
{
    [Inject] public ApiClient Api { get; set; } = default!;

    private DataGrid<Shipper>? _grid;

    private string DeleteText => _grid?.SelectedCount is > 0
        ? L.Format("common.deleteCount", _grid.SelectedCount)
        : L["common.delete"];

    private async Task<DataGridPage<Shipper>> LoadAsync(DataGridLoadArgs args)
    {
        var result = await Api.GetPagedAsync<Shipper>("Shippers/GetRows", args.Page, args.PageSize, args.Search);
        return new DataGridPage<Shipper>(result.Items, result.Total);
    }

    private async Task OpenFormAsync(Shipper? shipper)
    {
        var guard = new DialogGuard();
        var parameters = new Dictionary<string, object?> { [nameof(ShipperFormDialog.Guard)] = guard };
        if (shipper is not null)
            parameters[nameof(ShipperFormDialog.Shipper)] = shipper;

        object? saved = await DialogService.OpenAsync<ShipperFormDialog>(
            shipper is null
                ? L.Format("common.addEntity", L["entity.shipper"])
                : L.Format("common.editEntity", L["entity.shipper"]),
            parameters,
            new DialogOptions { Width = DialogSize.WidthFor(1), CanClose = () => Task.FromResult(!guard.Busy) });

        if (saved is true)
            await ReloadAsync();
    }

    private async Task DeleteSelectedAsync()
    {
        var ids = _grid?.SelectedItems.Select(s => s.ShipperID).ToList() ?? [];
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
            await Api.DeleteAsync("Shippers/Delete", ids);
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
