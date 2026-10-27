using BlazorTemplate.Client.Models;
using BlazorTemplate.Client.Services;
using BlazorTemplate.Shared.Components;
using BlazorTemplate.Shared.Components.Form;
using Microsoft.AspNetCore.Components;

namespace BlazorTemplate.Client.Features.Regions.Components;

public partial class RegionFormDialog : BaseComponent
{
    [Parameter] public Region? Region { get; set; }
    [Parameter] public DialogGuard Guard { get; set; } = new();
    [Inject] public ApiClient Api { get; set; } = default!;

    private Region _model = new();
    private readonly Dictionary<string, string> _errors = [];
    private string? _message;

    protected override void OnInitialized()
    {
        base.OnInitialized();

        if (Region is null)
            _model = DraftStore.GetOrCreate<Region>();

        if (Region is not null)
            _model = new Region { RegionID = Region.RegionID, RegionDescription = Region.RegionDescription };
    }

    private string? ErrorFor(string field)
        => _errors.TryGetValue(field, out var message) ? message : null;

    private async Task SaveAsync(Region model)
    {
        if (Guard.Busy)
            return;

        _errors.Clear();
        _message = null;
        Guard.Busy = true;
        try
        {
            if (Region is null)
                await Api.PostAsync<Region>("Regions/Insert", model);
            else
                await Api.PutAsync($"Regions/Update/{model.RegionID}", model);

            DraftStore.Clear<Region>();
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
