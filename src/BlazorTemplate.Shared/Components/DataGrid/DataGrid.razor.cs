using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;

namespace BlazorTemplate.Shared.Components.DataGrid;

public partial class DataGrid<TItem> : BaseComponent where TItem : notnull
{
    /// <summary>Ukuran halaman awal. Pengguna bisa mengubahnya lewat pager.</summary>
    [Parameter] public int PageSize { get; set; } = 10;

    /// <summary>Pilihan ukuran halaman di pager. Backend membatasi pageSize 1–100.</summary>
    [Parameter] public int[] PageSizeOptions { get; set; } = [10, 25, 50, 100];

    /// <summary>
    /// Sumber data server-side. Kembalikan baris + total lewat <see cref="DataGridPage{T}"/>.
    /// Total null berarti semua baris sudah dimuat (client-side).
    /// </summary>
    [Parameter, EditorRequired] public Func<DataGridLoadArgs, Task<DataGridPage<TItem>>> LoadData { get; set; } = default!;

    [Parameter] public RenderFragment? ChildContent { get; set; }

    [Parameter] public RenderFragment? EmptyTemplate { get; set; }

    /// <summary>Slot toolbar di atas grid (tombol Tambah/Hapus, dsb.).</summary>
    [Parameter] public RenderFragment? Toolbar { get; set; }

    [Parameter] public bool AllowSearch { get; set; }

    /// <summary>Placeholder pencarian. Null = teks umum dari <see cref="Localizer"/>.</summary>
    [Parameter] public string? SearchPlaceholder { get; set; }

    private string SearchText => SearchPlaceholder ?? L["common.search"];

    /// <summary>Menampilkan kolom checkbox + checkbox pilih-semua di header.</summary>
    [Parameter] public bool AllowSelection { get; set; }

    /// <summary>
    /// Dipanggil setiap pilihan berubah. Bind ke <c>StateHasChanged</c> agar toolbar
    /// pemakai (mis. label "Hapus (n)") ikut ter-render ulang.
    /// </summary>
    [Parameter] public EventCallback SelectionChanged { get; set; }

    /// <summary>
    /// Klik baris (di luar checkbox) — mis. buka halaman detail. Null = baris tidak interaktif.
    /// </summary>
    [Parameter] public EventCallback<TItem> RowClick { get; set; }

    /// <summary>Baris yang sedang terpilih. Pilihan dikosongkan setiap kali data dimuat ulang.</summary>
    public IReadOnlyCollection<TItem> SelectedItems => _selected;

    // Teruskan sebagai callback kosong saat tidak dipakai: Radzen memasang handler <tr>
    // begitu callback punya delegate, walau lambda-nya sendiri tidak melakukan apa-apa.
    private EventCallback<DataGridRowMouseEventArgs<TItem>> RowClickInternal =>
        RowClick.HasDelegate
            ? EventCallback.Factory.Create<DataGridRowMouseEventArgs<TItem>>(
                this, args => RowClick.InvokeAsync(args.Data))
            : default;

    public int SelectedCount => _selected.Count;

    private RadzenDataGrid<TItem>? _grid;
    private IEnumerable<TItem> _rows = [];
    private readonly HashSet<TItem> _selected = [];
    private int _count;
    private int _pageSize;
    private bool _isLoading;
    private bool _loaded;
    private string? _error;
    private string _search = "";
    private CancellationTokenSource? _searchDebounce;

    private bool? SelectAllState
    {
        get
        {
            var current = _rows.ToList();
            var selected = current.Count(item => _selected.Contains(item));
            return selected == 0 ? false : selected == current.Count ? true : null;
        }
    }

    protected override void OnInitialized()
    {
        // Wajib: BaseComponent memasang langganan perubahan bahasa di sini.
        base.OnInitialized();
        _pageSize = PageSize;
    }

    // Grid tidak selalu memanggil LoadData sendiri saat render pertama ketika dibungkus
    // komponen lain. Kick eksplisit di sini; kalau Radzen ternyata sudah memanggil, _loaded
    // sudah true dan tidak ada request kedua.
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && !_loaded && _grid is not null)
            await _grid.Reload();
    }

    private async Task LoadDataInternal(LoadDataArgs args)
    {
        _loaded = true;
        _isLoading = true;

        // Pilihan hanya berlaku untuk baris yang terlihat: jangan sampai id yang sudah
        // tidak ada di layar ikut terhapus.
        _selected.Clear();
        await SelectionChanged.InvokeAsync();

        var request = new DataGridLoadArgs(
            Page: (args.Skip ?? 0) / _pageSize + 1,
            PageSize: _pageSize,
            Search: _search);

        _error = null;
        try
        {
            var page = await LoadData(request);
            _rows = page.Items;
            _count = page.Total ?? page.Items.Count();
        }
        catch (Exception ex)
        {
            // Halaman tetap hidup; grid menampilkan pesan, bukan data kosong yang menyesatkan.
            _rows = [];
            _count = 0;
            _error = ex.Message;
            NotificationService.Notify(NotificationSeverity.Error, L["grid.loadFailed"], ex.Message);
        }
        finally
        {
            _isLoading = false;
        }
    }

    private async Task Toggle(TItem item, bool isSelected)
    {
        if (isSelected)
            _selected.Add(item);
        else
            _selected.Remove(item);

        await SelectionChanged.InvokeAsync();
    }

    private async Task SelectAll(bool isSelected)
    {
        if (isSelected)
            _selected.UnionWith(_rows);
        else
            _selected.Clear();

        await SelectionChanged.InvokeAsync();
    }

    /// <summary>
    /// Muat ulang data. <paramref name="firstPage"/> = true mengembalikan ke halaman 1
    /// (dipakai setelah pencarian, simpan, atau hapus).
    /// </summary>
    public async Task ReloadAsync(bool firstPage = false)
    {
        if (_grid is null)
            return;

        if (firstPage)
            _grid.CurrentPage = 0;

        await _grid.Reload();
    }

    // Debounce 300ms: satu request untuk sekuens ketikan, bukan satu per huruf.
    // Timer terakhir menang; yang sebelumnya dibatalkan.
    private async Task SearchChangedAsync()
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

        await ReloadAsync(firstPage: true);
    }
}
