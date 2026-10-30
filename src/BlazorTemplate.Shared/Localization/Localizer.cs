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
        ["menu.people"] = "SDM",
        ["menu.transactions"] = "Transaksi",
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
        ["common.edit"] = "Ubah",
        ["common.delete"] = "Hapus",
        ["common.deleteCount"] = "Hapus ({0})",
        ["common.deleteFailed"] = "Gagal menghapus",
        ["common.deleted"] = "Data dihapus.",
        ["common.confirm"] = "Konfirmasi",
        ["common.cancel"] = "Batal",
        ["common.save"] = "Simpan",
        ["common.saving"] = "Menyimpan...",
        ["common.retry"] = "Coba lagi",
        ["common.optional"] = "Opsional",
        ["common.search"] = "Cari...",
        ["common.action"] = "Aksi",
        ["common.required"] = "{0} wajib diisi.",
        ["common.error"] = "Terjadi kesalahan.",
        ["api.unreachable"] = "Tidak bisa menghubungi server. Backend mati atau CORS menolak origin ini.",
        ["api.timeout"] = "Permintaan ke server timeout.",
        ["api.badResponse"] = "Respons server tidak bisa dibaca.",
        ["api.emptyResponse"] = "Respons kosong dari server.",
        ["api.requestFailed"] = "Request gagal ({0}).",

        ["notFound.kicker"] = "Error 404",
        ["notFound.title"] = "Halaman tidak ditemukan",
        ["notFound.text"] = "Alamat ini tidak ada, atau sudah dipindahkan:",
        ["notFound.home"] = "Ke beranda",
        ["notFound.row"] = "baris tidak ditemukan",

        ["grid.selectAll"] = "Pilih semua di halaman ini",
        ["grid.selectRow"] = "Pilih baris",
        ["grid.empty"] = "Belum ada data.",
        ["grid.loadFailed"] = "Gagal memuat data",

        ["common.addEntity"] = "Tambah {0}",
        ["common.editEntity"] = "Ubah {0}",
        ["common.deleteRows"] = "Hapus {0} baris terpilih?",
        ["common.searchFor"] = "Cari {0}...",
        ["common.transactions"] = "Transaksi",
        ["common.yes"] = "Ya",
        ["common.no"] = "Tidak",
        ["common.back"] = "Kembali",
        ["common.saved"] = "Tersimpan.",

        ["order.subtitle"] = "Kelola pesanan dan pengiriman",
        ["order.notFound"] = "Pesanan tidak ditemukan. Mungkin sudah dihapus.",

        ["entity.customer"] = "Pelanggan",
        ["entity.customers"] = "Pelanggan",
        ["entity.employee"] = "Karyawan",
        ["entity.employees"] = "Karyawan",
        ["entity.order"] = "Pesanan",
        ["entity.orders"] = "Pesanan",
        ["entity.product"] = "Produk",
        ["entity.products"] = "Produk",
        ["entity.supplier"] = "Pemasok",
        ["entity.suppliers"] = "Pemasok",
        ["entity.shipper"] = "Pengirim",
        ["entity.shippers"] = "Pengirim",
        ["entity.region"] = "Wilayah",
        ["entity.regions"] = "Wilayah",
        ["entity.territory"] = "Wilayah jual",
        ["entity.territories"] = "Wilayah jual",
        ["entity.usState"] = "Negara bagian",
        ["entity.usStates"] = "Negara bagian",
        ["entity.customerDemographic"] = "Demografi pelanggan",
        ["entity.customerDemographics"] = "Demografi pelanggan",

        ["field.name"] = "Nama",
        ["field.firstName"] = "Nama depan",
        ["field.lastName"] = "Nama belakang",
        ["field.companyName"] = "Nama perusahaan",
        ["field.contactName"] = "Nama kontak",
        ["field.contactTitle"] = "Jabatan kontak",
        ["field.address"] = "Alamat",
        ["field.city"] = "Kota",
        ["field.region"] = "Wilayah",
        ["field.postalCode"] = "Kode pos",
        ["field.country"] = "Negara",
        ["field.phone"] = "Telepon",
        ["field.fax"] = "Fax",
        ["field.homePhone"] = "Telepon rumah",
        ["field.extension"] = "Ekstensi",
        ["field.homepage"] = "Situs web",
        ["field.notes"] = "Catatan",
        ["field.description"] = "Deskripsi",
        ["field.title"] = "Jabatan",
        ["field.titleOfCourtesy"] = "Sapaan",
        ["field.birthDate"] = "Tanggal lahir",
        ["field.hireDate"] = "Tanggal masuk",
        ["field.reportsTo"] = "Atasan",
        ["field.photoPath"] = "Path foto",
        ["field.productName"] = "Nama produk",
        ["field.quantityPerUnit"] = "Kemasan",
        ["field.unitPrice"] = "Harga satuan",
        ["field.unitsInStock"] = "Stok",
        ["field.unitsOnOrder"] = "Dalam pesanan",
        ["field.reorderLevel"] = "Batas pesan ulang",
        ["field.discontinued"] = "Dihentikan",
        ["field.category"] = "Kategori",
        ["field.supplier"] = "Pemasok",
        ["field.orderDate"] = "Tanggal pesanan",
        ["field.requiredDate"] = "Tanggal dibutuhkan",
        ["field.shippedDate"] = "Tanggal kirim",
        ["field.shipVia"] = "Pengirim",
        ["field.freight"] = "Ongkos kirim",
        ["field.shipName"] = "Nama penerima",
        ["field.shipAddress"] = "Alamat kirim",
        ["field.shipCity"] = "Kota kirim",
        ["field.shipRegion"] = "Wilayah kirim",
        ["field.shipPostalCode"] = "Kode pos kirim",
        ["field.shipCountry"] = "Negara kirim",
        ["field.customerID"] = "ID pelanggan",
        ["field.employeeID"] = "ID karyawan",
        ["field.regionID"] = "ID wilayah",
        ["field.regionDescription"] = "Deskripsi wilayah",
        ["field.territoryID"] = "ID wilayah jual",
        ["field.territoryDescription"] = "Deskripsi wilayah jual",
        ["field.stateID"] = "ID negara bagian",
        ["field.stateName"] = "Nama negara bagian",
        ["field.stateAbbr"] = "Singkatan",
        ["field.stateRegion"] = "Wilayah",
        ["field.customerTypeID"] = "ID tipe pelanggan",
        ["field.customerDesc"] = "Deskripsi",
    };

    private static readonly Dictionary<string, string> En = new()
    {
        ["menu.masterData"] = "Master Data",
        ["menu.people"] = "People",
        ["menu.transactions"] = "Transactions",
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
        ["common.edit"] = "Edit",
        ["common.delete"] = "Delete",
        ["common.deleteCount"] = "Delete ({0})",
        ["common.deleteFailed"] = "Delete failed",
        ["common.deleted"] = "Deleted.",
        ["common.confirm"] = "Confirm",
        ["common.cancel"] = "Cancel",
        ["common.save"] = "Save",
        ["common.saving"] = "Saving...",
        ["common.retry"] = "Retry",
        ["common.optional"] = "Optional",
        ["common.search"] = "Search...",
        ["common.action"] = "Action",
        ["common.required"] = "{0} is required.",
        ["common.error"] = "Something went wrong.",
        ["api.unreachable"] = "Cannot reach the server. The backend is down or CORS rejected this origin.",
        ["api.timeout"] = "The request timed out.",
        ["api.badResponse"] = "The server response could not be read.",
        ["api.emptyResponse"] = "Empty response from the server.",
        ["api.requestFailed"] = "Request failed ({0}).",

        ["notFound.kicker"] = "Error 404",
        ["notFound.title"] = "Page not found",
        ["notFound.text"] = "This address doesn't exist, or it has moved:",
        ["notFound.home"] = "Go home",
        ["notFound.row"] = "row not found",

        ["grid.selectAll"] = "Select all on this page",
        ["grid.selectRow"] = "Select row",
        ["grid.empty"] = "No data yet.",
        ["grid.loadFailed"] = "Failed to load data",

        ["common.addEntity"] = "Add {0}",
        ["common.editEntity"] = "Edit {0}",
        ["common.deleteRows"] = "Delete {0} selected rows?",
        ["common.searchFor"] = "Search {0}...",
        ["common.transactions"] = "Transactions",
        ["common.yes"] = "Yes",
        ["common.no"] = "No",
        ["common.back"] = "Back",
        ["common.saved"] = "Saved.",

        ["order.subtitle"] = "Manage orders and shipping",
        ["order.notFound"] = "Order not found. It may have been deleted.",

        ["entity.customer"] = "Customer",
        ["entity.customers"] = "Customers",
        ["entity.employee"] = "Employee",
        ["entity.employees"] = "Employees",
        ["entity.order"] = "Order",
        ["entity.orders"] = "Orders",
        ["entity.product"] = "Product",
        ["entity.products"] = "Products",
        ["entity.supplier"] = "Supplier",
        ["entity.suppliers"] = "Suppliers",
        ["entity.shipper"] = "Shipper",
        ["entity.shippers"] = "Shippers",
        ["entity.region"] = "Region",
        ["entity.regions"] = "Regions",
        ["entity.territory"] = "Territory",
        ["entity.territories"] = "Territories",
        ["entity.usState"] = "US state",
        ["entity.usStates"] = "US states",
        ["entity.customerDemographic"] = "Customer demographic",
        ["entity.customerDemographics"] = "Customer demographics",

        ["field.name"] = "Name",
        ["field.firstName"] = "First name",
        ["field.lastName"] = "Last name",
        ["field.companyName"] = "Company name",
        ["field.contactName"] = "Contact name",
        ["field.contactTitle"] = "Contact title",
        ["field.address"] = "Address",
        ["field.city"] = "City",
        ["field.region"] = "Region",
        ["field.postalCode"] = "Postal code",
        ["field.country"] = "Country",
        ["field.phone"] = "Phone",
        ["field.fax"] = "Fax",
        ["field.homePhone"] = "Home phone",
        ["field.extension"] = "Extension",
        ["field.homepage"] = "Homepage",
        ["field.notes"] = "Notes",
        ["field.description"] = "Description",
        ["field.title"] = "Title",
        ["field.titleOfCourtesy"] = "Courtesy title",
        ["field.birthDate"] = "Birth date",
        ["field.hireDate"] = "Hire date",
        ["field.reportsTo"] = "Reports to",
        ["field.photoPath"] = "Photo path",
        ["field.productName"] = "Product name",
        ["field.quantityPerUnit"] = "Quantity per unit",
        ["field.unitPrice"] = "Unit price",
        ["field.unitsInStock"] = "Units in stock",
        ["field.unitsOnOrder"] = "Units on order",
        ["field.reorderLevel"] = "Reorder level",
        ["field.discontinued"] = "Discontinued",
        ["field.category"] = "Category",
        ["field.supplier"] = "Supplier",
        ["field.orderDate"] = "Order date",
        ["field.requiredDate"] = "Required date",
        ["field.shippedDate"] = "Shipped date",
        ["field.shipVia"] = "Ship via",
        ["field.freight"] = "Freight",
        ["field.shipName"] = "Ship name",
        ["field.shipAddress"] = "Ship address",
        ["field.shipCity"] = "Ship city",
        ["field.shipRegion"] = "Ship region",
        ["field.shipPostalCode"] = "Ship postal code",
        ["field.shipCountry"] = "Ship country",
        ["field.customerID"] = "Customer ID",
        ["field.employeeID"] = "Employee ID",
        ["field.regionID"] = "Region ID",
        ["field.regionDescription"] = "Region description",
        ["field.territoryID"] = "Territory ID",
        ["field.territoryDescription"] = "Territory description",
        ["field.stateID"] = "State ID",
        ["field.stateName"] = "State name",
        ["field.stateAbbr"] = "Abbreviation",
        ["field.stateRegion"] = "State region",
        ["field.customerTypeID"] = "Customer type ID",
        ["field.customerDesc"] = "Description",
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
