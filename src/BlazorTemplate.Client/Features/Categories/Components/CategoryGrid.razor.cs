using BlazorTemplate.Client.Models;
using BlazorTemplate.Client.Services;
using BlazorTemplate.Shared.Components;
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
        var parameters = new Dictionary<string, object?>();
        if (category is not null)
            parameters[nameof(CategoryFormDialog.Category)] = category;

        var saved = await DialogService.OpenAsync<CategoryFormDialog>(
            category is null ? "Tambah Kategori" : "Ubah Kategori",
            parameters,
            new DialogOptions { Width = "440px" });

        if (saved)
            await ReloadAsync();
    }

    private async Task DeleteSelectedAsync()
    {
        var ids = _grid?.SelectedItems.Select(c => c.CategoryID).ToList() ?? [];
        if (ids.Count == 0)
            return;

        var label = ids.Count == 1
            ? "Hapus 1 kategori?"
            : $"Hapus {ids.Count} kategori?";
        var confirmed = await DialogService.Confirm(label, "Konfirmasi",
            new ConfirmOptions { OkButtonText = "Hapus", CancelButtonText = "Batal" });
        if (confirmed is not true)
            return;

        try
        {
            // Satu request: backend menghapus semua atau tidak sama sekali (404 kalau ada id hilang).
            await Api.DeleteAsync("Categories/Delete", ids);
            await ReloadAsync();
            NotificationService.Notify(NotificationSeverity.Success, "Terhapus");
        }
        catch (Exception ex)
        {
            // Daftar tidak di-reload saat gagal, jadi isinya tetap utuh.
            NotificationService.Notify(NotificationSeverity.Error, "Gagal menghapus", ex.Message);
        }
    }

    // Selalu kembali ke halaman 1: setelah simpan/hapus barisnya bisa berpindah halaman.
    private Task ReloadAsync() => _grid?.ReloadAsync(firstPage: true) ?? Task.CompletedTask;
}
