using BlazorTemplate.Client.Components;
using BlazorTemplate.Client.Models;
using BlazorTemplate.Client.Services;
using BlazorTemplate.Shared.Components;
using BlazorTemplate.Shared.Components.Form;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace BlazorTemplate.Client.Features.Categories.Components;

public partial class CategoryFormDialog : FormDialogBase<Category>
{
    [Parameter] public Category? Category { get; set; }
    private Category _model = new();

    protected override async Task OnInitializedAsync()
    {
        // Wajib: BaseComponent memasang langganan perubahan bahasa di sini.
        base.OnInitialized();

        if (Category is null)
        {
            _model = DraftStore.GetOrCreate<Category>();
            return;
        }

        // Ambil baris penuh dari GetRow: GetRows sengaja tidak mengirim picture
        // (kolom besar), jadi menyalin dari baris grid akan mengosongkan gambar.
        _model = await Api.GetAsync<Category>($"Categories/GetRow/{Category.CategoryID}");
    }
    private Task SaveAsync(Category model)
    {
        if (Category is null)
            return SubmitAsync(model, "Categories/Insert", "Categories/Update", null);

        return SubmitAsync(model, "Categories/Insert", "Categories/Update", model.CategoryID);
    }
}
