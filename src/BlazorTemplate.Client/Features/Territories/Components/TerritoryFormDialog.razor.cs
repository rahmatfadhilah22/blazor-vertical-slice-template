using BlazorTemplate.Client.Components;
using BlazorTemplate.Client.Models;
using BlazorTemplate.Client.Services;
using BlazorTemplate.Shared.Components;
using BlazorTemplate.Shared.Components.Form;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace BlazorTemplate.Client.Features.Territories.Components;

public partial class TerritoryFormDialog : FormDialogBase<Territory>
{
    [Parameter] public Territory? Territory { get; set; }

    private Territory _model = new();
    private List<Region> _regions = [];

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        if (Territory is null)
            _model = DraftStore.GetOrCreate<Territory>();

        if (Territory is not null)
            _model = new Territory
            {
                TerritoryID = Territory.TerritoryID,
                TerritoryDescription = Territory.TerritoryDescription,
                RegionID = Territory.RegionID,
            };

        try
        {
            // Daftar awal untuk menampilkan nilai terpilih; pencarian dropdown memakai LoadRegionsAsync.
            var result = await Api.GetPagedAsync<Region>("Regions/GetRows", 1, 100);
            _regions = result.Items.ToList();
        }
        catch (Exception ex)
        {
            _message = ex.Message;
        }
    }

    private async Task LoadRegionsAsync(LoadDataArgs args)
    {
        _regions = (await Api.GetPagedAsync<Region>("Regions/GetRows", 1, 50, args.Filter)).Items.ToList();
    }

    private Task SaveAsync(Territory model)
    {
        if (Territory is null)
            return SubmitAsync(model, "Territories", null);

        return SubmitAsync(model, "Territories", model.TerritoryID);
    }
}
