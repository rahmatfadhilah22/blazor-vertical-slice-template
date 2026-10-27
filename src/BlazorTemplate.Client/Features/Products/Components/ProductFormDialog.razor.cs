using BlazorTemplate.Client.Models;
using BlazorTemplate.Client.Services;
using BlazorTemplate.Shared.Components;
using BlazorTemplate.Shared.Components.Form;
using Microsoft.AspNetCore.Components;

namespace BlazorTemplate.Client.Features.Products.Components;

public partial class ProductFormDialog : BaseComponent
{
    [Parameter] public Product? Product { get; set; }
    [Parameter] public DialogGuard Guard { get; set; } = new();
    [Inject] public ApiClient Api { get; set; } = default!;

    private Product _model = new();
    private List<Category> _categories = [];
    private List<Supplier> _suppliers = [];
    private readonly Dictionary<string, string> _errors = [];
    private string? _message;

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
            // ponytail: lookup dimuat sekali, maks 100 baris (batas backend). Ganti ke
            // endpoint lookup khusus kalau katalog melewati 100.
            _categories = (await Api.GetPagedAsync<Category>("Categories/GetRows", 1, 100)).Items.ToList();
            _suppliers = (await Api.GetPagedAsync<Supplier>("Suppliers/GetRows", 1, 100)).Items.ToList();
        }
        catch (Exception ex)
        {
            _message = ex.Message;
        }
    }

    private string? ErrorFor(string field)
        => _errors.TryGetValue(field, out var message) ? message : null;

    private async Task SaveAsync(Product model)
    {
        if (Guard.Busy)
            return;

        _errors.Clear();
        _message = null;
        Guard.Busy = true;
        try
        {
            if (Product is null)
                await Api.PostAsync<Product>("Products/Insert", model);
            else
                await Api.PutAsync($"Products/Update/{model.ProductID}", model);

            DraftStore.Clear<Product>();
            DialogService.Close(true);
        }
        catch (ApiException ex)
        {
            _message = ex.Errors.Count > 0 ? null : ex.Message;
            foreach (var (field, messages) in ex.Errors)
                _errors[field] = string.Join(" ", messages);
        }
        catch (Exception ex)
        {
            _message = ex.Message;
        }
        finally
        {
            Guard.Busy = false;
        }
    }
}
