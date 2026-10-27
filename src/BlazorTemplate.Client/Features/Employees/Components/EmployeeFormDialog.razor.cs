using BlazorTemplate.Client.Models;
using BlazorTemplate.Client.Services;
using BlazorTemplate.Shared.Components;
using BlazorTemplate.Shared.Components.Form;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace BlazorTemplate.Client.Features.Employees.Components;

public partial class EmployeeFormDialog : BaseComponent
{
    [Parameter] public Employee? Employee { get; set; }
    [Parameter] public DialogGuard Guard { get; set; } = new();
    [Inject] public ApiClient Api { get; set; } = default!;

    private Employee _model = new();
    private List<Employee> _employees = [];
    private readonly Dictionary<string, string> _errors = [];
    private string? _message;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        if (Employee is null)
            _model = DraftStore.GetOrCreate<Employee>();

        if (Employee is not null)
            _model = new Employee
            {
                EmployeeID = Employee.EmployeeID,
                LastName = Employee.LastName,
                FirstName = Employee.FirstName,
                Title = Employee.Title,
                TitleOfCourtesy = Employee.TitleOfCourtesy,
                BirthDate = Employee.BirthDate,
                HireDate = Employee.HireDate,
                Address = Employee.Address,
                City = Employee.City,
                Region = Employee.Region,
                PostalCode = Employee.PostalCode,
                Country = Employee.Country,
                HomePhone = Employee.HomePhone,
                Extension = Employee.Extension,
                Notes = Employee.Notes,
                ReportsTo = Employee.ReportsTo,
                PhotoPath = Employee.PhotoPath,
            };

        try
        {
            // Daftar awal untuk menampilkan nilai terpilih; pencarian dropdown memakai LoadEmployeesAsync.
            _employees = (await Api.GetPagedAsync<Employee>("Employees/GetRows", 1, 100)).Items.ToList();
        }
        catch (Exception ex)
        {
            _message = ex.Message;
        }
    }

    private async Task LoadEmployeesAsync(LoadDataArgs args)
        => _employees = (await Api.GetPagedAsync<Employee>("Employees/GetRows", 1, 50, args.Filter)).Items.ToList();

    private string? ErrorFor(string field)
        => _errors.TryGetValue(field, out var message) ? message : null;

    private async Task SaveAsync(Employee model)
    {
        if (Guard.Busy)
            return;

        _errors.Clear();
        _message = null;
        Guard.Busy = true;
        try
        {
            if (Employee is null)
                await Api.PostAsync<Employee>("Employees/Insert", model);
            else
                await Api.PutAsync($"Employees/Update/{model.EmployeeID}", model);

            DraftStore.Clear<Employee>();
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
