using BlazorTemplate.Client.Components;
using BlazorTemplate.Client.Models;
using BlazorTemplate.Client.Services;
using BlazorTemplate.Shared.Components;
using BlazorTemplate.Shared.Components.Form;
using Microsoft.AspNetCore.Components;

namespace BlazorTemplate.Client.Features.Shippers.Components;

public partial class ShipperFormDialog : FormDialogBase<Shipper>
{
    [Parameter] public Shipper? Shipper { get; set; }

    private Shipper _model = new();

    protected override void OnInitialized()
    {
        base.OnInitialized();

        if (Shipper is null)
            _model = DraftStore.GetOrCreate<Shipper>();

        if (Shipper is not null)
            _model = new Shipper
            {
                ShipperID = Shipper.ShipperID,
                CompanyName = Shipper.CompanyName,
                Phone = Shipper.Phone,
            };
    }

    private Task SaveAsync(Shipper model)
    {
        if (Shipper is null)
            return SubmitAsync(model, "Shippers/Insert", "Shippers/Update", null);

        return SubmitAsync(model, "Shippers/Insert", "Shippers/Update", model.ShipperID);
    }
}
