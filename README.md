# BlazorTemplate

Blazor WebAssembly client untuk backend Categories. Feature-sliced, tanpa abstraksi: satu `ApiClient` bersama, URL endpoint ditulis langsung di komponen fiturnya, satu halaman, satu dialog.

> **Catatan:** versi awal ini adalah Blazor WebAssembly standalone. Host MAUI Blazor Hybrid ditunda; RCL `BlazorTemplate.Client` tidak berubah saat host MAUI ditambahkan nanti.

## Prasyarat

- .NET SDK 10 (`dotnet --list-sdks` harus menampilkan 10.x)
- Backend berjalan di `http://localhost:5207`
- Radzen.Blazor 12.x (dipatok `Version="12.*"` di RCL; restore saat ini: 12.1.6)
- Tidak butuh Node/npm, tidak ada workload tambahan untuk WASM

## CORS wajib di backend

Blazor WebAssembly mengirim request dari origin `http://localhost:5310`. Backend harus mengizinkan origin tersebut, kalau tidak semua request diblokir browser dan grid menampilkan notification error.

Di repo backend (jangan diubah dari repo ini), tambahkan:

```csharp
builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
    p.WithOrigins("http://localhost:5310").AllowAnyHeader().AllowAnyMethod()));
app.UseCors();   // sebelum UseAuthorization
```

Untuk host MAUI nanti origin-nya `https://0.0.0.0`, jadi tambahkan juga origin itu bila kedua host dipakai bersamaan.

Verifikasi cepat bahwa backend mengirim header CORS:

```powershell
curl -i -H "Origin: http://localhost:5310" "http://localhost:5207/api/Categories/GetRows?page=1&pageSize=10"
# harus ada: Access-Control-Allow-Origin: http://localhost:5310
```

## Menjalankan

```powershell
dotnet run --project src/BlazorTemplate.Client.App
```

Port tetap `http://localhost:5310` (diatur di `src/BlazorTemplate.Client.App/Properties/launchSettings.json`). Kalau port itu sudah dipakai, `dotnet run` gagal dengan `address already in use` — matikan proses lama, jangan ganti port, karena origin ini yang diizinkan CORS backend.

```powershell
netstat -ano | findstr :5310    # kolom terakhir = PID
taskkill /PID <PID> /F
```

Setelah setiap `dotnet build`, nama asset di `_framework` berubah hash. Server hanya memegang daftar asset saat start, jadi **restart server dan hard refresh browser** (`Ctrl+Shift+R`). Kalau tidak: 404 pada `_framework/*.wasm`, integrity check gagal, dan halaman kosong.

Build:

```powershell
dotnet build
```

## API contract

Base URL: `http://localhost:5207`

| Method | Path | Request | Success |
|---|---|---|---|
| GET | `/api/Categories/GetRows?page=1&pageSize=10&search=tea` | — | 200 PagedResult |
| GET | `/api/Categories/GetRow/{id}` | — | 200 Category, 404 jika tidak ada |
| POST | `/api/Categories/Insert` | CategoryInput (JSON) | 201 Category |
| PUT | `/api/Categories/Update/{id}` | CategoryInput (JSON) | 204, 404 jika tidak ada |
| DELETE | `/api/Categories/Delete` | `int[]` (JSON body, mis. `[1,2]`) | 204; 404 jika ada id tidak ditemukan (tidak ada yang dihapus) |

```jsonc
// PagedResult
{ "items": [ /* Category */ ], "page": 1, "pageSize": 10, "total": 8, "totalPages": 1 }

// Category
{ "categoryID": 1, "categoryName": "Beverages", "description": "Soft drinks", "picture": "" }

// CategoryInput — categoryId 0 pada insert = server yang menentukan id
{ "categoryId": 0, "categoryName": "Snacks", "description": "Chips", "picture": null }
```

Validasi server (form ikut meniru, server tetap otoritas): `categoryName` wajib, maks 15 karakter; `categoryId` 0–32767; `page >= 1`; `pageSize` 1–100. `page` 1-based, `pageSize` tetap 10 di v1.

Error memakai RFC 9457 ProblemDetails; yang ditampilkan ke pengguna hanya `title`. Validasi mengembalikan 400 dengan `errors: { "field": ["pesan"] }`.

## Mengubah API base URL

Satu tempat: `Api:BaseUrl` di `src/BlazorTemplate.Client.App/wwwroot/appsettings.json`. Tidak ada URL lain di kode.

```json
{ "Api": { "BaseUrl": "http://localhost:5207/api" } }
```

`WebAssemblyHostBuilder.CreateDefault` memuat file ini otomatis. `appsettings.Development.json` juga didukung dan menimpa nilainya saat environment Development. Kalau key tidak ada, aplikasi gagal start dengan pesan jelas — bukan diam-diam menembak ke URL salah.

Trailing slash ditambahkan otomatis di `Program.cs`; URL endpoint ditulis relatif (`Categories/...`) di komponen, jadi base URL boleh dengan atau tanpa `/` di akhir. Jangan sertakan `/api` dua kali.

> File config di `wwwroot` ikut ter-bundle ke browser dan bisa dibaca siapa pun. Jangan taruh secret di sini. Untuk nilai rahasia, ambil dari API yang mengeluarkan token.

## Struktur

```
src/BlazorTemplate.Shared/           RCL net10.0, komponen lintas fitur di atas Radzen
  Components/Button.razor(+.cs)         wrapper RadzenButton + enum ButtonVariant
  Components/PageHeader.razor           judul halaman + slot aksi
  Components/EmptyState.razor           kondisi kosong / gagal muat
  Components/FieldError.razor           pesan error di bawah field
src/BlazorTemplate.Client/           RCL net10.0, fitur aplikasi
  Models/                               Category, PagedResult, CategoryInput
  Services/ApiClient.cs                 satu jalur HTTP: typed result atau ApiException
  Features/Categories/Pages/            CategoryListPage — komposisi saja, tanpa Radzen
  Features/Categories/Components/       CategoryGrid, CategoryFormDialog (+ .razor.cs)
  Layout/MainLayout.razor               RadzenTheme (material) + RadzenComponents
src/BlazorTemplate.Client.App/       Blazor WASM host
  Program.cs                            registrasi service + HttpClient + base URL
  LoggingHandler.cs                     log semua request/response ke console browser
  wwwroot/index.html                    theme CSS + blazor boot
  wwwroot/appsettings.json              Api:BaseUrl
  wwwroot/css/theme.css                 warna terpusat (CSS variable Radzen)
```

### Aturan API

`ApiClient` adalah satu-satunya yang menyentuh `HttpClient`. Method-nya selalu: mengembalikan body bertipe pada sukses, atau melempar `ApiException` (HTTP error, body bukan JSON, body kosong, backend tak terjangkau, timeout). Pemanggil hanya menangani satu jenis exception — `ApiException.Errors` berisi error per field dari 400, `Message` berisi `title` ProblemDetails.

URL endpoint ditulis langsung di komponen pemakainya:

```csharp
var result = await Api.GetAsync<PagedResult>($"Categories/GetRows?page={page}&pageSize={PageSize}");
```

Tidak ada kelas per-fitur di atas `ApiClient`. Satu endpoint dipakai di satu tempat; lapisan tambahan hanya menambah file yang harus dibuka saat menelusuri bug, tanpa menghapus duplikasi. Pisahkan jadi service hanya kalau satu endpoint benar-benar dipanggil dari dua komponen atau butuh logika bersama (retry, cache, gabungan beberapa call).

### Aturan pemisahan page / component

Page **hanya** menyusun komposisi — tidak ada `RadzenDataGrid`, `DialogService`, atau panggilan API di dalamnya. Semua detail library dan logika ada di komponen fitur.

```razor
@page "/categories"
<PageHeader Title="Categories" Subtitle="Master data kategori produk" />
<CategoryGrid />
```

Komponen memakai code-behind (`X.razor` + `X.razor.cs`). Markup di `.razor`, semua state dan method di `.razor.cs` dengan `public partial class X`. Jangan pakai `@inject` di `.razor` untuk service yang juga ada di code-behind — akan bentrok `CS0102`. Inject di code-behind saja:

```csharp
[Inject] public DialogService DialogService { get; set; } = default!;
```

RCL memakai `FrameworkReference Microsoft.AspNetCore.App` dengan `PrivateAssets="all"` agar host WASM (yang tidak punya runtime pack AspNetCore) tetap bisa me-referensikannya.

### Aturan komponen shared

Kelas utilitas yang dipakai harus benar-benar ada di Radzen 12: `rz-color-danger`, `rz-text-caption`, `rz-text-secondary-color`, `rz-text-body2`, `rz-mb-*`, `rz-p-*`, `rz-display-flex`, `rz-gap-*`, `rz-align-items-*`, `rz-justify-content-*`. `rz-text-danger` dan `rz-text-sm` **tidak ada** (dulu sempat dipakai dan diam-diam tidak berwarna).

### Hapus massal

Tidak ada tombol hapus per baris. Kolom pertama grid berisi checkbox; pilihan disimpan sebagai `HashSet<int>` berisi `CategoryID`, lalu satu tombol **Hapus (n)** di toolbar mengirim semuanya dalam satu request DELETE dengan body `[1,2]`.

> Catatan: spesifikasi awal menyebut query string (`?ids=1&ids=2`), tapi backend aslinya menolak dengan 415 kalau body JSON tidak ada — `ids` dibaca dari body (dipastikan dengan probe curl).

### Pencarian

Tidak ada tombol Cari. Input pencarian (native `<input>` dengan kelas `rz-textbox`) memicu request otomatis dengan **debounce 300ms** — satu request per jeda ketikan, bukan satu per huruf. Setiap request juga mengembalikan grid ke halaman 1 dan mengosongkan pilihan checkbox.

- Pilihan dikosongkan setiap `LoadData` (ganti halaman / pencarian / reload), supaya tidak ada id terhapus yang sudah tidak terlihat di layar.
- Tombol nonaktif saat tidak ada yang dipilih.
- Backend bersifat semua-atau-tidak-sama-sekali: kalau satu id sudah hilang, 404 dan tidak ada yang terhapus. Daftar tidak di-reload saat gagal, jadi isi grid tetap utuh.

### Debugging

Semua request/response tercatat di **console browser** (F12), bukan terminal — aplikasi jalan di WASM, tidak ada server di belakangnya. Format: `--> GET url`, `<-- 200 url`, `<-- 500 url` + body error.

Kalau halaman blank tanpa pesan: cek tab Console untuk 404 `_framework/*` atau kegagalan integrity check — itu tanda server jalan dengan asset build lama (lihat catatan restart di atas).

## Tema dan warna

Tema: **Material** (gratis), dipilih di `src/BlazorTemplate.Client/Layout/MainLayout.razor`:

```razor
<RadzenTheme Theme="material" />
```

Tema gratis lain di Radzen 12: `standard`, `default`, `humanistic`, `software`. Tema Material 3 / Fluent / Zen butuh **Radzen Blazor Pro** (berbayar) — tidak jalan dengan paket gratis.

Warna terpusat di `src/BlazorTemplate.Client.App/wwwroot/css/theme.css`. Itu menimpa CSS variable Radzen, jadi semua komponen Radzen otomatis mengikuti:

```css
:root:root {
  --rz-primary: #1565C0;        /* biru, dipakai header, tombol primary, dsb. */
  --rz-secondary: #455A64;      /* biru abu-abu */
  --rz-header-background-color: var(--rz-primary);
  --rz-border-radius: 4px;
}
```

Warna status (success/warning/danger/info) masih default Material. Spesifikasi `:root:root` (dobel) disengaja: `RadzenTheme` menyuntikkan stylesheet theme saat runtime, dan dobel menjamin override kita menang tanpa mempedulikan urutan.

## Komponen shared

`BlazorTemplate.Shared` berisi komponen dasar yang dipakai lintas fitur. Semua komponen punya code-behind (`.razor` markup + `.razor.cs` logika), kecuali yang hanya markup murni:

| Komponen | Fungsi | Parameter utama |
|---|---|---|
| `Button` | satu-satunya tempat yang tahu `RadzenButton`; developer pakai `Variant` (`ButtonVariant.Primary/Secondary/Success/Danger/Warning/Info/Light/Ghost`), tidak perlu tahu `ButtonStyle`/`Severity` Radzen | `Text`, `Icon`, `Variant`, `Size`, `IsBusy` (otomatis disabled + busy text), `Click` |
| `PageHeader` | judul halaman + slot aksi di kanan | `Title`, `Subtitle`, `Actions` (ChildContent) |
| `EmptyState` | kondisi kosong / gagal muat, center | `Message`, `Icon`, `ChildContent` (mis. tombol "Coba lagi") |
| `FieldError` | pesan error di bawah field form | `Message` |

`BlazorTemplate.Shared` **hanya** berisi komponen yang sudah dipakai minimal dua tempat. Jangan membungkus `RadzenDataGrid`, `RadzenTextBox`, atau komponen Radzen lain yang punya banyak parameter — wrapper seperti itu harus meneruskan puluhan parameter dan berubah setiap upgrade Radzen. Pakai Radzen langsung; taruh hanya keputusan desain (spacing, tipografi, warna semantik) di shared.

## Gap v1

- Belum ada autentikasi
- Belum ada offline support
- Belum ada test
- Kolom `picture` tidak ditampilkan dan tidak bisa diedit
- Host MAUI Blazor Hybrid belum dibuat (yang ada: Blazor WebAssembly)
