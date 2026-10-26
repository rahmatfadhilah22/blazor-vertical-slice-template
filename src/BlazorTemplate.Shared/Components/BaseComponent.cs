using Microsoft.AspNetCore.Components;
using Radzen;

namespace BlazorTemplate.Shared.Components;

/// <summary>
/// Basis semua komponen aplikasi. Menampung service yang dipakai hampir di mana-mana,
/// supaya tiap komponen tidak menulis ulang baris <c>[Inject]</c>.
/// Tambahkan service ke sini hanya kalau dipakai di banyak komponen — service khusus
/// satu fitur tetap di-inject di komponennya sendiri.
/// </summary>
public abstract class BaseComponent : ComponentBase
{
    [Inject] public DialogService DialogService { get; set; } = default!;
    [Inject] public NotificationService NotificationService { get; set; } = default!;
}
