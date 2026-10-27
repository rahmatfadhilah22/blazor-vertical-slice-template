namespace BlazorTemplate.Shared.Components;

/// <summary>
/// Argumen LoadData versi grid ini: hanya yang dibutuhkan endpoint server-side.
/// </summary>
/// <param name="Page">Halaman yang diminta, 1-based.</param>
/// <param name="PageSize">Ukuran halaman yang dipakai grid — pakai ini di URL, jangan salin angkanya.</param>
/// <param name="Search">Kata kunci kotak pencarian. Null/kosong bila tidak ada pencarian.</param>
public record DataGridLoadArgs(int Page, int PageSize, string? Search);

/// <summary>
/// Hasil satu LoadData: baris untuk halaman ini + total di server (sebelum paging).
/// <paramref name="Total"/> null = semua baris sudah dimuat, grid memakai jumlah baris.
/// </summary>
public record DataGridPage<T>(IEnumerable<T> Items, int? Total = null);
