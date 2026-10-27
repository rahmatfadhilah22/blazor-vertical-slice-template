using BlazorTemplate.Client.Components;
using BlazorTemplate.Client.Models;
using BlazorTemplate.Client.Services;
using BlazorTemplate.Shared.Components;
using BlazorTemplate.Shared.Components.Form;
using Microsoft.AspNetCore.Components;

namespace BlazorTemplate.Client.Features.Customers.Components;

public partial class CustomerFormDialog : FormDialogBase<Customer>
{
    [Parameter] public Customer? Customer { get; set; }

    private Customer _model = new();

    protected override void OnInitialized()
    {
        base.OnInitialized();

        if (Customer is null)
            _model = DraftStore.GetOrCreate<Customer>();

        if (Customer is not null)
            _model = new Customer
            {
                CustomerID = Customer.CustomerID,
                CompanyName = Customer.CompanyName,
                ContactName = Customer.ContactName,
                ContactTitle = Customer.ContactTitle,
                Address = Customer.Address,
                City = Customer.City,
                Region = Customer.Region,
                PostalCode = Customer.PostalCode,
                Country = Customer.Country,
                Phone = Customer.Phone,
                Fax = Customer.Fax,
            };
    }

    private Task SaveAsync(Customer model)
    {
        if (Customer is null)
            return SubmitAsync(model, "Customers/Insert", "Customers/Update", null);

        return SubmitAsync(model, "Customers/Insert", "Customers/Update", model.CustomerID);
    }
}
