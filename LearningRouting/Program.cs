using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json;


var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddScoped<Employee, Employee>();
builder.Services.AddRouting(options =>
{
    options.ConstraintMap.Add("myCustomConstraint", typeof(myCustomConstraint));
    
});

var app = builder.Build();
//Routing should always be configured before any middleware that depends on routing, such as authorization or endpoint execution.
//This ensures that the routing information is available for those middleware components to function correctly.
app.UseRouting();
// the old way of doing things
app.UseEndpoints(endpoints =>
{
    endpoints.MapGet("/", async context =>
    {
        await context.Response.WriteAsync("Hello World!");
    });
    //endpoints.MapGet("/employees/{iddd:int}", ([FromRoute(Name ="idddd")] int id) =>
    endpoints.MapGet("/employees/", (Employee employee) =>
    {
        
        return EmployeesRepository.GetEmployeeById(employee.Id) is not null ? Results.Ok(EmployeesRepository.GetEmployeeById(employee.Id    )) : Results.NotFound();

        //var employee = EmployeesRepository.GetEmployeeById(id);
        //return EmployeesRepository.GetEmployeeById(id) is not null ? Results.Ok(employee) : Results.NotFound();

        //if (employee == null)
        //{
        //    return Results.NotFound();
        //}
        //return Results.Ok(employee);
    });
    endpoints.MapPost("/submit", async context =>
    {
        await context.Response.WriteAsync("Form submitted!");
    });
    endpoints.MapPut("/update", async context =>
    {
        await context.Response.WriteAsync("Resource updated!");
    });
    endpoints.MapDelete("/delete/{id:myCustomConstraint}", async context =>
    {
        await context.Response.WriteAsync("Resource deleted!");
    });
});

app.Run();

class myCustomConstraint : IRouteConstraint
{
    public bool Match(HttpContext? httpContext, IRouter? route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
    {
        if (values.TryGetValue(routeKey, out var value) && value is string stringValue)
        {
            return stringValue.Length > 5;
        }
        return false;
    }
}

public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Position { get; set; }

    public double Salary { get; set; }
    public Employee(int id, string name, string position, double salary)
    {
        Id = id;
        Name = name;
        Position = position;
        Salary = salary;
    }
    public static async ValueTask<Employee> BindAsync(HttpContext context)
    {
        using var reader = new StreamReader(context.Request.Body);
        var body = await reader.ReadToEndAsync();
        var employee = JsonSerializer.Deserialize<Employee>(body);
        if (employee is null)
        {
            throw new ArgumentNullException(nameof(employee), "Employee data is required.");
        }
        else
        return employee;
    }
}

static class EmployeesRepository
{
    private static List<Employee> employees = new List<Employee>
    {
        new Employee(1, "John Doe", "Manager", 50000),
        new Employee(2, "Jane Smith", "Developer", 60000),
        new Employee(3, "Bob Johnson", "Designer", 55000)
    };
    public static List<Employee> GetEmployees()
    {
        return employees;
    }
    public static Employee GetEmployeeById(int id)
    {
        return employees.FirstOrDefault(e => e.Id == id);
    }
    public static void AddEmployee(Employee? employee)
    {
        if (employee is not null)
        {
            employees.Add(employee);
        }
    }
    public static void UpdateEmployee(Employee employee)
    {
        var existingEmployee = GetEmployeeById(employee.Id);
        if (existingEmployee != null)
        {
            existingEmployee.Name = employee.Name;
            existingEmployee.Position = employee.Position;
            existingEmployee.Salary = employee.Salary;
        }
    }
    public static void DeleteEmployee(int id)
    {
        var employee = GetEmployeeById(id);
        if (employee != null)
        {
            employees.Remove(employee);
        }
    }
}

