using BlazorTemplate.Client.Models;
using BlazorTemplate.Client.Services;
using BlazorTemplate.Shared.Components;
using BlazorTemplate.Shared.Components.Form;
using Microsoft.AspNetCore.Components;

namespace BlazorTemplate.Client.Features.Shippers.Components;

public partial class ShipperFormDialog : BaseComponent
{
    [Parameter] public Shipper? Shipper { get; set; }
    [Parameter] public DialogGuard Guard { get; set; } = new();
    [Inject] public ApiClient Api { get; set; } = default!;

    private Shipper _model = new();
    private readonly Dictionary<string, string> _errors = [];
    private string? _message;

    protected override void OnInitialized()
    {
        base.OnInitialized();

        if (Shipper is not null)
            _model = new Shipper
            {
                ShipperID = Shipper.ShipperID,
                CompanyName = Shipper.CompanyName,
                Phone = Shipper.Phone,
            };
    }

    private string? ErrorFor(string field)
        => _errors.TryGetValue(field, out var message) ? message : null;

    private async Task SaveAsync(Shipper model)
    {
        if (Guard.Busy)
            return;

        _errors.Clear();
        _message = null;
        Guard.Busy = true;
        try
        {
            if (Shipper is null)
                await Api.PostAsync<Shipper>("Shippers/Insert", model);
            else
                await Api.PutAsync($"Shippers/Update/{model.ShipperID}", model);

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
