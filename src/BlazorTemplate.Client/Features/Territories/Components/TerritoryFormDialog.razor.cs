using BlazorTemplate.Client.Models;
using BlazorTemplate.Client.Services;
using BlazorTemplate.Shared.Components;
using BlazorTemplate.Shared.Components.Form;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace BlazorTemplate.Client.Features.Territories.Components;

public partial class TerritoryFormDialog : BaseComponent
{
    [Parameter] public Territory? Territory { get; set; }
    [Parameter] public DialogGuard Guard { get; set; } = new();
    [Inject] public ApiClient Api { get; set; } = default!;

    private Territory _model = new();
    private List<Region> _regions = [];
    private readonly Dictionary<string, string> _errors = [];
    private string? _message;

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
        => _regions = (await Api.GetPagedAsync<Region>("Regions/GetRows", 1, 50, args.Filter)).Items.ToList();

    private string? ErrorFor(string field)
        => _errors.TryGetValue(field, out var message) ? message : null;

    private async Task SaveAsync(Territory model)
    {
        if (Guard.Busy)
            return;

        _errors.Clear();
        _message = null;
        Guard.Busy = true;
        try
        {
            if (Territory is null)
                await Api.PostAsync<Territory>("Territories/Insert", model);
            else
                await Api.PutAsync($"Territories/Update/{model.TerritoryID}", model);

            DraftStore.Clear<Territory>();
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
