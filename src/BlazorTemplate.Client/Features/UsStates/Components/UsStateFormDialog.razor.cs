using BlazorTemplate.Client.Components;
using BlazorTemplate.Client.Models;
using BlazorTemplate.Client.Services;
using BlazorTemplate.Shared.Components;
using BlazorTemplate.Shared.Components.Form;
using Microsoft.AspNetCore.Components;

namespace BlazorTemplate.Client.Features.UsStates.Components;

public partial class UsStateFormDialog : FormDialogBase<UsState>
{
    [Parameter] public UsState? UsState { get; set; }

    private UsState _model = new();

    protected override void OnInitialized()
    {
        base.OnInitialized();

        if (UsState is null)
            _model = DraftStore.GetOrCreate<UsState>();

        if (UsState is not null)
            _model = new UsState
            {
                StateID = UsState.StateID,
                StateName = UsState.StateName,
                StateAbbr = UsState.StateAbbr,
                StateRegion = UsState.StateRegion,
            };
    }

    private Task SaveAsync(UsState model)
    {
        if (UsState is null)
            return SubmitAsync(model, "UsStates", null);

        return SubmitAsync(model, "UsStates", model.StateID);
    }
}
