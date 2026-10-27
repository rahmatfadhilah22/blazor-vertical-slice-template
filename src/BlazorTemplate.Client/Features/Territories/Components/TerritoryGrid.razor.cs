using BlazorTemplate.Client.Models;
using BlazorTemplate.Client.Services;
using BlazorTemplate.Shared.Components;
using BlazorTemplate.Shared.Components.DataGrid;
using BlazorTemplate.Shared.Components.Form;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace BlazorTemplate.Client.Features.Territories.Components;

public partial class TerritoryGrid : BaseComponent
{
    [Inject] public ApiClient Api { get; set; } = default!;

    private DataGrid<Territory>? _grid;

    private string DeleteText => _grid?.SelectedCount is > 0
        ? L.Format("common.deleteCount", _grid.SelectedCount)
        : L["common.delete"];

    private async Task<DataGridPage<Territory>> LoadAsync(DataGridLoadArgs args)
    {
        var result = await Api.GetPagedAsync<Territory>("Territories/GetRows", args.Page, args.PageSize, args.Search);
        return new DataGridPage<Territory>(result.Items, result.Total);
    }

    private async Task OpenFormAsync(Territory? territory)
    {
        var guard = new DialogGuard();
        var parameters = new Dictionary<string, object?> { [nameof(TerritoryFormDialog.Guard)] = guard };
        if (territory is not null)
            parameters[nameof(TerritoryFormDialog.Territory)] = territory;

        object? saved = await DialogService.OpenAsync<TerritoryFormDialog>(
            territory is null
                ? L.Format("common.addEntity", L["entity.territory"])
                : L.Format("common.editEntity", L["entity.territory"]),
            parameters,
            new DialogOptions { Width = DialogSize.WidthFor(1), CanClose = () => Task.FromResult(!guard.Busy) });

        if (saved is true)
            await ReloadAsync();
    }

    private async Task DeleteSelectedAsync()
    {
        var ids = _grid?.SelectedItems.Select(t => t.TerritoryID).ToList() ?? [];
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
            await Api.DeleteAsync("Territories/Delete", ids);
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
