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

    protected override void OnInitialized()
    {
        // Wajib: BaseComponent memasang langganan perubahan bahasa di sini.
        base.OnInitialized();

        if (Category is null)
            _model = DraftStore.GetOrCreate<Category>();

        if (Category is null)
            return;

        _model.CategoryID = Category.CategoryID;
        _model.CategoryName = Category.CategoryName;
        _model.Description = Category.Description;
        _model.Picture = Category.Picture;
    }
    private Task SaveAsync(Category model)
    {
        if (Category is null)
            return SubmitAsync(model, "Categories", null);

        return SubmitAsync(model, "Categories", model.CategoryID);
    }
}
