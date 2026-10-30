using BlazorTemplate.Client.Models;
using BlazorTemplate.Client.Services;
using BlazorTemplate.Shared.Components;
using BlazorTemplate.Shared.Components.DataGrid;
using BlazorTemplate.Shared.Components.Form;
using Microsoft.AspNetCore.Components;

namespace BlazorTemplate.Client.Features.UsStates.Components;

public partial class UsStateGrid : BaseComponent
{
    [Inject] public ApiClient Api { get; set; } = default!;

    private DataGrid<UsState>? _grid;

    private string DeleteText => _grid?.SelectedCount is > 0
        ? L.Format("common.deleteCount", _grid.SelectedCount)
        : L["common.delete"];

    private async Task<DataGridPage<UsState>> LoadAsync(DataGridLoadArgs args)
    {
        var result = await Api.GetPagedAsync<UsState>("UsStates/GetRows", args.Page, args.PageSize, args.Search);
        return new DataGridPage<UsState>(result.Items, result.Total);
    }

    private async Task OpenFormAsync(UsState? state)
    {
        var guard = new DialogGuard();
        var parameters = new Dictionary<string, object?> { [nameof(UsStateFormDialog.Guard)] = guard };
        if (state is not null)
            parameters[nameof(UsStateFormDialog.UsState)] = state;

        object? saved = await DialogService.OpenAsync<UsStateFormDialog>(
            state is null
                ? L.Format("common.addEntity", L["entity.usState"])
                : L.Format("common.editEntity", L["entity.usState"]),
            parameters,
            new DialogOptions { Width = DialogSize.WidthFor(1), CanClose = () => Task.FromResult(!guard.Busy) });

        if (saved is true)
            await ReloadAsync();
    }

    private async Task DeleteSelectedAsync()
    {
        var ids = _grid?.SelectedItems.Select(s => s.StateID).ToList() ?? [];
        if (ids.Count == 0)
            return;

        if (!await Confirm.DeleteAsync(DialogService, L, L.Format("common.deleteRows", ids.Count)))
            return;

        try
        {
            await Api.DeleteAsync("UsStates/Delete", ids);
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
