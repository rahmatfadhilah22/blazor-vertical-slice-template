using BlazorTemplate.Client.Models;
using BlazorTemplate.Client.Services;
using BlazorTemplate.Shared.Components;
using BlazorTemplate.Shared.Components.DataGrid;
using BlazorTemplate.Shared.Components.Form;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace BlazorTemplate.Client.Features.Categories.Components;

public partial class CategoryGrid : BaseComponent
{
    [Inject] public ApiClient Api { get; set; } = default!;

    private DataGrid<Category>? _grid;

    private async Task<DataGridPage<Category>> LoadAsync(DataGridLoadArgs args)
    {
        var result = await Api.GetPagedAsync<Category>("Categories/GetRows",
        args.Page, args.PageSize, args.Search);

        return new DataGridPage<Category>(result.Items, result.Total);
    }

    private async Task OpenFormAsync(Category? category)
    {
        // Guard dibagi dua arah: dialog menandai sedang menyimpan, CanClose menolak
        // X / Escape / klik overlay selama itu. Tanpa ini dialog bisa ditutup di tengah
        // request — hasilnya hilang dan grid tidak reload.
        var guard = new DialogGuard();
        var parameters = new Dictionary<string, object?>
        {
            [nameof(CategoryFormDialog.Guard)] = guard,
        };
        if (category is not null)
            parameters[nameof(CategoryFormDialog.Category)] = category;

        object? saved = await DialogService.OpenAsync<CategoryFormDialog>(
            category is null ? L["category.add"] : L["category.edit"],
            parameters,
            new DialogOptions
            {
                Width = DialogSize.WidthFor(2),
                // CanClose bertipe Func<Task<bool>>, bukan Func<bool>.
                CanClose = () => Task.FromResult(!guard.Busy),
            });

        if (saved is true)
            await ReloadAsync();
    }

    private async Task DeleteSelectedAsync()
    {
        var ids = _grid?.SelectedItems.Select(c => c.CategoryID).ToList() ?? [];
        if (ids.Count == 0)
            return;

        var confirmed = await DialogService.Confirm(
            L.Format("category.deleteConfirm", ids.Count),
            L["common.confirm"],
            new ConfirmOptions { OkButtonText = L["common.delete"], CancelButtonText = L["common.cancel"] });
        if (confirmed is not true)
            return;

        try
        {
            // Satu request: backend menghapus semua atau tidak sama sekali (404 kalau ada id hilang).
            await Api.DeleteAsync("Categories/Delete", ids);
            await ReloadAsync();
            NotificationService.Notify(NotificationSeverity.Success, L["category.deleted"]);
        }
        catch (Exception ex)
        {
            // Daftar tidak di-reload saat gagal, jadi isinya tetap utuh.
            NotificationService.Notify(NotificationSeverity.Error, L["common.deleteFailed"], ex.Message);
        }
    }

    // Selalu kembali ke halaman 1: setelah simpan/hapus barisnya bisa berpindah halaman.
    private Task ReloadAsync() => _grid?.ReloadAsync(firstPage: true) ?? Task.CompletedTask;
}
