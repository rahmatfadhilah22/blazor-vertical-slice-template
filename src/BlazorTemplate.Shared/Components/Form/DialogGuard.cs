namespace BlazorTemplate.Shared.Components;

/// <summary>
/// Penghubung status simpan antara pemanggil DialogService.OpenAsync dan isi dialog.
/// Dialog menulis <see cref="Busy"/>, pemanggil membacanya di DialogOptions.CanClose
/// supaya X / Escape / klik overlay tidak menutup dialog saat request masih jalan
/// (menutup di tengah simpan membuat dialog kehilangan hasil dan grid tidak reload).
/// </summary>
public sealed class DialogGuard
{
    public bool Busy { get; set; }
}
