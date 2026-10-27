using BlazorTemplate.Client.Components;
using BlazorTemplate.Client.Models;
using BlazorTemplate.Client.Services;
using BlazorTemplate.Shared.Components;
using BlazorTemplate.Shared.Components.Form;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace BlazorTemplate.Client.Features.Products.Components;

public partial class ProductFormDialog : FormDialogBase<Product>
{
    [Parameter] public Product? Product { get; set; }

    private Product _model = new();
    private List<Category> _categories = [];
    private List<Supplier> _suppliers = [];

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        if (Product is null)
            _model = DraftStore.GetOrCreate<Product>();

        if (Product is not null)
            _model = new Product
            {
                ProductID = Product.ProductID,
                ProductName = Product.ProductName,
                SupplierID = Product.SupplierID,
                CategoryID = Product.CategoryID,
                QuantityPerUnit = Product.QuantityPerUnit,
                UnitPrice = Product.UnitPrice,
                UnitsInStock = Product.UnitsInStock,
                UnitsOnOrder = Product.UnitsOnOrder,
                ReorderLevel = Product.ReorderLevel,
                Discontinued = Product.Discontinued,
            };

        try
        {
            // Daftar awal untuk menampilkan nilai terpilih; pencarian dropdown memakai LoadXxxAsync.
            _categories = (await Api.GetPagedAsync<Category>("Categories/GetRows", 1, 100)).Items.ToList();
            _suppliers = (await Api.GetPagedAsync<Supplier>("Suppliers/GetRows", 1, 100)).Items.ToList();
        }
        catch (Exception ex)
        {
            _message = ex.Message;
        }
    }

    private async Task LoadCategoriesAsync(LoadDataArgs args)
        => _categories = (await Api.GetPagedAsync<Category>("Categories/GetRows", 1, 50, args.Filter)).Items.ToList();

    private async Task LoadSuppliersAsync(LoadDataArgs args)
        => _suppliers = (await Api.GetPagedAsync<Supplier>("Suppliers/GetRows", 1, 50, args.Filter)).Items.ToList();

    private Task SaveAsync(Product model) => SubmitAsync(model, "Products", Product is null ? null : model.ProductID);
}
