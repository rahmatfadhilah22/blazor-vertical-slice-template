namespace BlazorTemplate.Shared.Components;

/// <summary>
/// Makna tombol, bukan nama tipe library. Pemetaan ke Radzen ada di Button.razor,
/// jadi mengganti library hanya menyentuh satu file itu.
/// </summary>
public enum ButtonVariant
{
    Primary,
    Secondary,
    Success,
    Danger,
    Warning,
    Info,
    Light,
    Ghost,
}

// Ukuran sengaja memakai Radzen.ButtonSize apa adanya: hanya 3 nilai, tidak ada
// logika pemetaan, dan menamainya ulang hanya menciptakan tabrakan nama
// (CS0104) yang harus dibayar di setiap file yang memakai tombol.
