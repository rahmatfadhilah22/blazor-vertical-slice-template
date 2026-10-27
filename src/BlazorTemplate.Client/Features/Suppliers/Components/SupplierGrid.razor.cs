using BlazorTemplate.Client.Models;
using BlazorTemplate.Client.Services;
using BlazorTemplate.Shared.Components;
using BlazorTemplate.Shared.Components.DataGrid;
using BlazorTemplate.Shared.Components.Form;
using Microsoft.AspNetCore.Components;

namespace BlazorTemplate.Client.Features.Suppliers.Components;

public partial class SupplierGrid : BaseComponent
{
    [Inject] public ApiClient Api { get; set; } = default!;

    private DataGrid<Supplier>? _grid;

    private string DeleteText => _grid?.SelectedCount is > 0
        ? L.Format("common.deleteCount", _grid.SelectedCount)
        : L["common.delete"];

    private async Task<DataGridPage<Supplier>> LoadAsync(DataGridLoadArgs args)
    {
        var result = await Api.GetPagedAsync<Supplier>("Suppliers/GetRows", args.Page, args.PageSize, args.Search);
        return new DataGridPage<Supplier>(result.Items, result.Total);
    }

    private async Task OpenFormAsync(Supplier? supplier)
    {
        var guard = new DialogGuard();
        var parameters = new Dictionary<string, object?> { [nameof(SupplierFormDialog.Guard)] = guard };
        if (supplier is not null)
            parameters[nameof(SupplierFormDialog.Supplier)] = supplier;

        object? saved = await DialogService.OpenAsync<SupplierFormDialog>(
            supplier is null
                ? L.Format("common.addEntity", L["entity.supplier"])
                : L.Format("common.editEntity", L["entity.supplier"]),
            parameters,
            new DialogOptions { Width = DialogSize.WidthFor(2), CanClose = () => Task.FromResult(!guard.Busy) });

        if (saved is true)
            await ReloadAsync();
    }

    private async Task DeleteSelectedAsync()
    {
        var ids = _grid?.SelectedItems.Select(s => s.SupplierID).ToList() ?? [];
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
            await Api.DeleteAsync("Suppliers/Delete", ids);
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
