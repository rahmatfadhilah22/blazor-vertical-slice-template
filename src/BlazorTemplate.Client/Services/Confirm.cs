using BlazorTemplate.Shared.Localization;
using Radzen;

namespace BlazorTemplate.Client.Services;

/// <summary>Konfirmasi standar aplikasi: tombol destruktif + teks terlokalisasi.</summary>
public static class Confirm
{
    /// <summary>True hanya kalau pengguna benar-benar menekan tombol hapus.</summary>
    public static async Task<bool> DeleteAsync(DialogService dialog, Localizer l, string message)
    {
        var confirmed = await dialog.Confirm(message, l["common.confirm"], new ConfirmOptions
        {
            OkButtonText = l["common.delete"],
            CancelButtonText = l["common.cancel"],
        });

        return confirmed is true;
    }
}
