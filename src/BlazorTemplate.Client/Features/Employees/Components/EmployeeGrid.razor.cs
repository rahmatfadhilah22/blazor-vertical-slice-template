using BlazorTemplate.Client.Models;
using BlazorTemplate.Client.Services;
using BlazorTemplate.Shared.Components;
using BlazorTemplate.Shared.Components.DataGrid;
using BlazorTemplate.Shared.Components.Form;
using Microsoft.AspNetCore.Components;

namespace BlazorTemplate.Client.Features.Employees.Components;

public partial class EmployeeGrid : BaseComponent
{
    [Inject] public ApiClient Api { get; set; } = default!;

    private DataGrid<Employee>? _grid;

    private string DeleteText => _grid?.SelectedCount is > 0
        ? L.Format("common.deleteCount", _grid.SelectedCount)
        : L["common.delete"];

    private async Task<DataGridPage<Employee>> LoadAsync(DataGridLoadArgs args)
    {
        var result = await Api.GetPagedAsync<Employee>("Employees/GetRows", args.Page, args.PageSize, args.Search);
        return new DataGridPage<Employee>(result.Items, result.Total);
    }

    private async Task OpenFormAsync(Employee? employee)
    {
        var guard = new DialogGuard();
        var parameters = new Dictionary<string, object?> { [nameof(EmployeeFormDialog.Guard)] = guard };
        if (employee is not null)
            parameters[nameof(EmployeeFormDialog.Employee)] = employee;

        object? saved = await DialogService.OpenAsync<EmployeeFormDialog>(
            employee is null
                ? L.Format("common.addEntity", L["entity.employee"])
                : L.Format("common.editEntity", L["entity.employee"]),
            parameters,
            new DialogOptions { Width = DialogSize.WidthFor(2), CanClose = () => Task.FromResult(!guard.Busy) });

        if (saved is true)
            await ReloadAsync();
    }

    private async Task DeleteSelectedAsync()
    {
        var ids = _grid?.SelectedItems.Select(e => e.EmployeeID).ToList() ?? [];
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
            await Api.DeleteAsync("Employees/Delete", ids);
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
