namespace BlazorTemplate.Shared.Components.Form;

/// <summary>
/// Draft form Tambah yang disimpan di memori tab. Isi yang sudah diketik tetap ada
/// saat dialog ditutup lewat X/Escape, dan baru dibuang setelah simpan berhasil.
/// Edit tidak memakai draft: formnya selalu mulai dari data tersimpan.
/// </summary>
public static class DraftStore
{
    private static readonly Dictionary<Type, object> Drafts = [];

    public static T GetOrCreate<T>() where T : class, new()
    {
        if (!Drafts.TryGetValue(typeof(T), out var draft))
            Drafts[typeof(T)] = draft = new T();

        return (T)draft;
    }

    public static void Clear<T>() => Drafts.Remove(typeof(T));
}
