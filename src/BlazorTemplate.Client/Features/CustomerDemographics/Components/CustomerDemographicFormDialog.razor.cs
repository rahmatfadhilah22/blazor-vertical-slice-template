using BlazorTemplate.Client.Models;
using BlazorTemplate.Client.Services;
using BlazorTemplate.Shared.Components;
using BlazorTemplate.Shared.Components.Form;
using Microsoft.AspNetCore.Components;

namespace BlazorTemplate.Client.Features.CustomerDemographics.Components;

public partial class CustomerDemographicFormDialog : BaseComponent
{
    [Parameter] public CustomerDemographic? Demographic { get; set; }
    [Parameter] public DialogGuard Guard { get; set; } = new();
    [Inject] public ApiClient Api { get; set; } = default!;

    private CustomerDemographic _model = new();
    private readonly Dictionary<string, string> _errors = [];
    private string? _message;

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

    private string? ErrorFor(string field)
        => _errors.TryGetValue(field, out var message) ? message : null;

    private async Task SaveAsync(CustomerDemographic model)
    {
        if (Guard.Busy)
            return;

        _errors.Clear();
        _message = null;
        Guard.Busy = true;
        try
        {
            if (Demographic is null)
                await Api.PostAsync<CustomerDemographic>("CustomerDemographics/Insert", model);
            else
                await Api.PutAsync($"CustomerDemographics/Update/{model.CustomerTypeID}", model);

            DraftStore.Clear<CustomerDemographic>();
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
