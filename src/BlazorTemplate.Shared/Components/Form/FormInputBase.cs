using Microsoft.AspNetCore.Components;

namespace BlazorTemplate.Shared.Components.Form;

/// <summary>
/// Parameter yang sama untuk semua input berlabel di <see cref="FormField"/>:
/// label, nama untuk validator, nilai, error server, dan lebar.
/// Komponen turunan cukup menulis markup input-nya.
/// </summary>
public abstract class FormInputBase<T> : BaseComponent
{
    [Parameter, EditorRequired] public string Label { get; set; } = "";

    /// <summary>Nama untuk dicocokkan validator Radzen. Kosong = pakai <see cref="Label"/>.</summary>
    [Parameter] public string? Name { get; set; }

    [Parameter] public T Value { get; set; } = default!;

    [Parameter] public EventCallback<T> ValueChanged { get; set; }

    /// <summary>Error dari server; kosong = tidak ada error.</summary>
    [Parameter] public string? Error { get; set; }

    /// <summary>Selebar baris penuh — untuk textarea / deskripsi panjang.</summary>
    [Parameter] public bool FullWidth { get; set; }

    [Parameter] public bool Disabled { get; set; }

    protected string FieldName => string.IsNullOrWhiteSpace(Name) ? Label : Name;
}
