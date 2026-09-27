using System.ComponentModel.DataAnnotations;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddValidation();
var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.MapPost("/employees", (Employee employee) =>
{
    var validationResults = new List<ValidationResult>();
    var validationContext = new ValidationContext(employee, null, null);
    if (!Validator.TryValidateObject(employee, validationContext, validationResults, true))
    {
        return Results.BadRequest(validationResults);
    }
    return Results.Ok(employee);
});

app.Run();

public class Employee:IValidatableObject
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Name is required.")]
    public string Name { get; set; }
    [Employee_EnsureSalary]
    public string Position { get; set; }
    
   
    
    public double Salary { get; set; }
    public Employee(int id, string name, string position, double salary)
    {
        Id = id;
        Name = name;
        Position = position;
        Salary = salary;
    }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Salary < 50001 && Position == "Manager")
        {
            yield return new ValidationResult("Manager's salary is not valid!", new[] { nameof(Salary) });
        }
    }
}