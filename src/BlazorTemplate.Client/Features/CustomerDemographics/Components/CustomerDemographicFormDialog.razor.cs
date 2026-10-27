using BlazorTemplate.Client.Components;
using BlazorTemplate.Client.Models;
using BlazorTemplate.Client.Services;
using BlazorTemplate.Shared.Components;
using BlazorTemplate.Shared.Components.Form;
using Microsoft.AspNetCore.Components;

namespace BlazorTemplate.Client.Features.CustomerDemographics.Components;

public partial class CustomerDemographicFormDialog : FormDialogBase<CustomerDemographic>
{
    [Parameter] public CustomerDemographic? Demographic { get; set; }

    private CustomerDemographic _model = new();

    protected override void OnInitialized()
    {
        base.OnInitialized();

        if (Demographic is null)
            _model = DraftStore.GetOrCreate<CustomerDemographic>();

        if (Demographic is not null)
            _model = new CustomerDemographic
            {
                CustomerTypeID = Demographic.CustomerTypeID,
                CustomerDesc = Demographic.CustomerDesc,
            };
    }

    private Task SaveAsync(CustomerDemographic model) => SubmitAsync(model, "CustomerDemographics", Demographic is null ? null : model.CustomerTypeID);
}
