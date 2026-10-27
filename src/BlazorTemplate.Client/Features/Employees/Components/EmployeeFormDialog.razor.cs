using BlazorTemplate.Client.Components;
using BlazorTemplate.Client.Models;
using BlazorTemplate.Client.Services;
using BlazorTemplate.Shared.Components;
using BlazorTemplate.Shared.Components.Form;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace BlazorTemplate.Client.Features.Employees.Components;

public partial class EmployeeFormDialog : FormDialogBase<Employee>
{
    [Parameter] public Employee? Employee { get; set; }

    private Employee _model = new();
    private List<Employee> _employees = [];

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
    {
        _employees = (await Api.GetPagedAsync<Employee>("Employees/GetRows", 1, 50, args.Filter)).Items.ToList();
    }

    private Task SaveAsync(Employee model)
    {
        if (Employee is null)
            return SubmitAsync(model, "Employees", null);

        return SubmitAsync(model, "Employees", model.EmployeeID);
    }
}
