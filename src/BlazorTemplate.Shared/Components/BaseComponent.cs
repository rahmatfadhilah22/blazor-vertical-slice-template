using Microsoft.AspNetCore.Components;
using Radzen;
using BlazorTemplate.Shared.Localization;

namespace BlazorTemplate.Shared.Components;

/// <summary>
/// Basis semua komponen aplikasi. Menampung service yang dipakai hampir di mana-mana,
/// supaya tiap komponen tidak menulis ulang baris <c>[Inject]</c>.
/// Tambahkan service ke sini hanya kalau dipakai di banyak komponen — service khusus
/// satu fitur tetap di-inject di komponennya sendiri.
///
/// Komponen turunan yang meng-override <c>OnInitialized</c> wajib memanggil
/// <c>base.OnInitialized()</c>: di situlah langganan perubahan bahasa dipasang.
/// </summary>
public abstract class BaseComponent : ComponentBase, IDisposable
{
    [Inject] public DialogService DialogService { get; set; } = default!;
    [Inject] public NotificationService NotificationService { get; set; } = default!;

    /// <summary>Teks UI. Pakai <c>@L["kunci"]</c>; kunci ada di <see cref="Localizer"/>.</summary>
    [Inject] public Localizer L { get; set; } = default!;

    protected override void OnInitialized() => L.Changed += StateHasChanged;

    public void Dispose()
    {
        L.Changed -= StateHasChanged;
        GC.SuppressFinalize(this);
    }}
