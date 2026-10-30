using BlazorTemplate.Shared.Localization;
using Radzen;

namespace BlazorTemplate.Client.Services;

/// <summary>
/// Notifikasi standar untuk aksi CRUD. Mengganti pasangan
/// <c>NotificationService.Notify(NotificationSeverity.X, L["..."])</c> yang berulang di
/// setiap grid, sekaligus menjamin judul dan nada pesannya sama.
/// </summary>
public static class Toast
{
    public static void Saved(NotificationService n, Localizer l) =>
        n.Notify(NotificationSeverity.Success, l["common.saved"]);

    public static void Deleted(NotificationService n, Localizer l, string? text = null) =>
        n.Notify(NotificationSeverity.Success, text ?? l["common.deleted"]);

    public static void DeleteFailed(NotificationService n, Localizer l, Exception ex) =>
        n.Notify(NotificationSeverity.Error, l["common.deleteFailed"], ex.Message);
}
