using System.ComponentModel.DataAnnotations;

public class Employee_EnsureSalary : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var employee = (Employee)validationContext.ObjectInstance;
        if (employee is not null && employee.Salary < 50000 && employee.Position == "Manager")
        {
            return new ValidationResult("Manager's salary cannot be less than 50000.");
        }   
        return base.IsValid(value, validationContext);
    }
}

