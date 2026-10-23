using BlazorTemplate.Client.Models;
using BlazorTemplate.Client.Services;
using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;

namespace BlazorTemplate.Client.Features.Categories.Components;

public partial class CategoryGrid
{
    private const int PageSize = 10;

    [Inject] public ApiClient Api { get; set; } = default!;
    [Inject] public DialogService DialogService { get; set; } = default!;
    [Inject] public NotificationService NotificationService { get; set; } = default!;

    private RadzenDataGrid<Category>? _grid;
    private List<Category> _rows = [];
    private int _total;
    private string _search = "";
    private string? _error;

    // Menyimpan id, bukan objek: pilihan tetap benar setelah grid reload / pindah halaman.
    private readonly HashSet<int> _selected = [];

    private async Task LoadData(LoadDataArgs args)
    {
        try
        {
            _selected.Clear();

            var page = (args.Skip ?? 0) / PageSize + 1;
            var url = $"Categories/GetRows?page={page}&pageSize={PageSize}";
            if (!string.IsNullOrWhiteSpace(_search))
                url += $"&search={Uri.EscapeDataString(_search)}";

            var result = await Api.GetAsync<PagedResult>(url);
            _rows = result.Items;
            _total = result.Total;
            _error = null;
        }
        catch (Exception ex)
        {
            // Halaman tetap hidup; grid menampilkan pesan, bukan "Belum ada data".
            _rows = [];
            _total = 0;
            _error = ex.Message;
            NotificationService.Notify(NotificationSeverity.Error, "Gagal memuat data", ex.Message);
        }
    }

    // Pencarian selalu kembali ke halaman 1.
    private async Task SearchAsync()
    {
        if (_grid is null)
            return;

        _grid.CurrentPage = 0;
        await _grid.Reload();
    }

    // Debounce 300ms: satu request untuk sekuens ketikan, bukan satu per huruf.
    // Timer terakhir menang; yang sebelumnya di-skip lewat token di bawah.
    private CancellationTokenSource? _searchDebounce;

    private async Task SearchInputChangedAsync()
    {
        if (_searchDebounce is not null)
            await _searchDebounce.CancelAsync();
        var cts = _searchDebounce = new CancellationTokenSource();
        try
        {
            await Task.Delay(300, cts.Token);
        }
        catch (TaskCanceledException)
        {
            return; // masih mengetik; ketikan terakhir yang memicu request
        }

        await SearchAsync();
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
            await SearchAsync();
    }

    private void Toggle(int id, bool isChecked)
    {
        if (isChecked)
            _selected.Add(id);
        else
            _selected.Remove(id);
    }

    private async Task DeleteSelectedAsync()
    {
        if (_selected.Count == 0)
            return;

        var label = _selected.Count == 1
            ? "Hapus 1 kategori?"
            : $"Hapus {_selected.Count} kategori?";
        var confirmed = await DialogService.Confirm(label, "Konfirmasi",
            new ConfirmOptions { OkButtonText = "Hapus", CancelButtonText = "Batal" });
        if (confirmed is not true)
            return;

        try
        {
            // Satu request: backend menghapus semua atau tidak sama sekali (404 kalau ada id hilang).
            await Api.DeleteAsync("Categories/Delete", _selected);
            _selected.Clear();
            await SearchAsync();
            NotificationService.Notify(NotificationSeverity.Success, "Terhapus");
        }
        catch (Exception ex)
        {
            // Daftar tidak di-reload saat gagal, jadi isinya tetap utuh.
            NotificationService.Notify(NotificationSeverity.Error, "Gagal menghapus", ex.Message);
        }
    }
}
