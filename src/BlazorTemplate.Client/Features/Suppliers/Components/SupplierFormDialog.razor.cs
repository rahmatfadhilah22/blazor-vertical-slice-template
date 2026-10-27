using BlazorTemplate.Client.Components;
using BlazorTemplate.Client.Models;
using BlazorTemplate.Client.Services;
using BlazorTemplate.Shared.Components;
using BlazorTemplate.Shared.Components.Form;
using Microsoft.AspNetCore.Components;

namespace BlazorTemplate.Client.Features.Suppliers.Components;

public partial class SupplierFormDialog : FormDialogBase<Supplier>
{
    [Parameter] public Supplier? Supplier { get; set; }

    private Supplier _model = new();

    protected override void OnInitialized()
    {
        base.OnInitialized();

        if (Supplier is null)
            _model = DraftStore.GetOrCreate<Supplier>();

        if (Supplier is not null)
            _model = new Supplier
            {
                SupplierID = Supplier.SupplierID,
                CompanyName = Supplier.CompanyName,
                ContactName = Supplier.ContactName,
                ContactTitle = Supplier.ContactTitle,
                Address = Supplier.Address,
                City = Supplier.City,
                Region = Supplier.Region,
                PostalCode = Supplier.PostalCode,
                Country = Supplier.Country,
                Phone = Supplier.Phone,
                Fax = Supplier.Fax,
                Homepage = Supplier.Homepage,
            };
    }

    private Task SaveAsync(Supplier model)
    {
        if (Supplier is null)
            return SubmitAsync(model, "Suppliers/Insert", "Suppliers/Update", null);

        return SubmitAsync(model, "Suppliers/Insert", "Suppliers/Update", model.SupplierID);
    }
}
