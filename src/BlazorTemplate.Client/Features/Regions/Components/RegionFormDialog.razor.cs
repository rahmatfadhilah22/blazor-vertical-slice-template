using BlazorTemplate.Client.Components;
using BlazorTemplate.Client.Models;
using BlazorTemplate.Client.Services;
using BlazorTemplate.Shared.Components;
using BlazorTemplate.Shared.Components.Form;
using Microsoft.AspNetCore.Components;

namespace BlazorTemplate.Client.Features.Regions.Components;

public partial class RegionFormDialog : FormDialogBase<Region>
{
    [Parameter] public Region? Region { get; set; }

    private Region _model = new();

    protected override void OnInitialized()
    {
        base.OnInitialized();

        if (Region is null)
            _model = DraftStore.GetOrCreate<Region>();

        if (Region is not null)
            _model = new Region { RegionID = Region.RegionID, RegionDescription = Region.RegionDescription };
    }

    private Task SaveAsync(Region model)
    {
        if (Region is null)
            return SubmitAsync(model, "Regions/Insert", "Regions/Update", null);

        return SubmitAsync(model, "Regions/Insert", "Regions/Update", model.RegionID);
    }
}
