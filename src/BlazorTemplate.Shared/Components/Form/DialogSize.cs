namespace BlazorTemplate.Shared.Components.Form;

/// <summary>
/// Lebar dialog untuk <see cref="FormDialog{TItem}"/>.
/// Tidak diletakkan sebagai static method di FormDialog karena FormDialog generik —
/// memanggilnya butuh argumen tipe (FormDialog&lt;Category&gt;.WidthFor(2)) di setiap pemanggil.
/// </summary>
public static class DialogSize
{
    /// <summary>
    /// Lebar sesuai jumlah kolom field (±320px per kolom), dibatasi 92vw supaya tidak
    /// mentok tepi layar. 3 kolom ≈ selebar DataGrid; 2 kolom untuk form pendek.
    /// </summary>
    public static string WidthFor(int columns) => $"min({columns * 320 + 80}px, 92vw)";
}
