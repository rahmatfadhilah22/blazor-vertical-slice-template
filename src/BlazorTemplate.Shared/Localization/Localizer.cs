namespace BlazorTemplate.Shared.Localization;

public enum AppLanguage
{
    Id,
    En,
}

/// <summary>
/// Terjemahan label UI. Cakupannya sengaja hanya teks tetap milik aplikasi (menu, judul
/// kolom, tombol, pesan validasi). Isi data dari server tampil apa adanya dan tidak
/// diterjemahkan.
///
/// Memakai dictionary, bukan .resx + IStringLocalizer: di Blazor WASM resx menambah
/// satelit assembly ke build dan boot manifest, sementara yang perlu diterjemahkan di sini
/// hanya puluhan label. Kuncinya string biasa; kunci yang tidak dikenal dikembalikan apa
/// adanya supaya salah ketik langsung terlihat di layar.
/// </summary>
public sealed class Localizer
{
    private static readonly Dictionary<string, string> Id = new()
    {
        ["menu.masterData"] = "Master Data",
        ["menu.categories"] = "Kategori",

        ["category.title"] = "Kategori",
        ["category.subtitle"] = "Master data kategori produk",
        ["category.search"] = "Cari nama atau deskripsi...",
        ["category.name"] = "Nama",
        ["category.description"] = "Deskripsi",
        ["category.nameLabel"] = "Nama kategori",
        ["category.nameHint"] = "Maks. 15 karakter",
        ["category.add"] = "Tambah Kategori",
        ["category.edit"] = "Ubah Kategori",
        ["category.deleteConfirm"] = "Hapus kategori terpilih ({0})?",
        ["category.deleted"] = "Terhapus",

        ["common.add"] = "Tambah",
        ["common.delete"] = "Hapus",
        ["common.deleteCount"] = "Hapus ({0})",
        ["common.deleteFailed"] = "Gagal menghapus",
        ["common.confirm"] = "Konfirmasi",
        ["common.cancel"] = "Batal",
        ["common.save"] = "Simpan",
        ["common.saving"] = "Menyimpan...",
        ["common.retry"] = "Coba lagi",
        ["common.optional"] = "Opsional",
        ["common.search"] = "Cari...",
        ["common.action"] = "Aksi",
        ["common.required"] = "{0} wajib diisi.",

        ["notFound.kicker"] = "Error 404",
        ["notFound.title"] = "Halaman tidak ditemukan",
        ["notFound.text"] = "Alamat ini tidak ada, atau sudah dipindahkan:",
        ["notFound.home"] = "Ke beranda",
        ["notFound.row"] = "baris tidak ditemukan",

        ["grid.selectAll"] = "Pilih semua di halaman ini",
        ["grid.selectRow"] = "Pilih baris",
        ["grid.empty"] = "Belum ada data.",
        ["grid.loadFailed"] = "Gagal memuat data",
    };

    private static readonly Dictionary<string, string> En = new()
    {
        ["menu.masterData"] = "Master Data",
        ["menu.categories"] = "Categories",

        ["category.title"] = "Categories",
        ["category.subtitle"] = "Product category master data",
        ["category.search"] = "Search name or description...",
        ["category.name"] = "Name",
        ["category.description"] = "Description",
        ["category.nameLabel"] = "Category name",
        ["category.nameHint"] = "Max. 15 characters",
        ["category.add"] = "Add Category",
        ["category.edit"] = "Edit Category",
        ["category.deleteConfirm"] = "Delete selected categories ({0})?",
        ["category.deleted"] = "Deleted",

        ["common.add"] = "Add",
        ["common.delete"] = "Delete",
        ["common.deleteCount"] = "Delete ({0})",
        ["common.deleteFailed"] = "Delete failed",
        ["common.confirm"] = "Confirm",
        ["common.cancel"] = "Cancel",
        ["common.save"] = "Save",
        ["common.saving"] = "Saving...",
        ["common.retry"] = "Retry",
        ["common.optional"] = "Optional",
        ["common.search"] = "Search...",
        ["common.action"] = "Action",
        ["common.required"] = "{0} is required.",

        ["notFound.kicker"] = "Error 404",
        ["notFound.title"] = "Page not found",
        ["notFound.text"] = "This address doesn't exist, or it has moved:",
        ["notFound.home"] = "Go home",
        ["notFound.row"] = "row not found",

        ["grid.selectAll"] = "Select all on this page",
        ["grid.selectRow"] = "Select row",
        ["grid.empty"] = "No data yet.",
        ["grid.loadFailed"] = "Failed to load data",
    };

    public AppLanguage Language { get; private set; } = AppLanguage.Id;

    /// <summary>
    /// Kunci yang hanya ada di satu bahasa. Kosong = kedua dictionary lengkap.
    /// Dicek sekali saat start (khusus DEBUG): kunci yang hilang tidak melempar error,
    /// ia hanya muncul sebagai teks mentah di layar.
    /// </summary>
    public static IReadOnlyList<string> FindMissingKeys() =>
        Id.Keys.Except(En.Keys).Concat(En.Keys.Except(Id.Keys)).ToList();

    /// <summary>Dipicu setelah bahasa berganti. <see cref="Components.BaseComponent"/> memakainya untuk render ulang.</summary>
    public event Action? Changed;

    /// <summary>Teks untuk bahasa aktif. Kunci tak dikenal dikembalikan apa adanya.</summary>
    public string this[string key] => (Language == AppLanguage.En ? En : Id).GetValueOrDefault(key, key);

    /// <summary>Teks berformat, mis. <c>Format("common.required", "Nama")</c>.</summary>
    public string Format(string key, params object?[] args) => string.Format(this[key], args);

    public void Set(AppLanguage language)
    {
        if (Language == language)
            return;

        Language = language;
        Changed?.Invoke();
    }
}
