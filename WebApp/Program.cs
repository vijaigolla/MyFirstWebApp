using System.Security.Cryptography.X509Certificates;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args); //create kestrel server
// we can configure the kestrel server here

var app = builder.Build();

//app.MapGet("/", () => "Hello World!");// middleware to handle GET requests to the root URL
app.Run(async (HttpContext context) =>
{
    

    if (context.Request.Path.StartsWithSegments("/employees"))
    {
        if (context.Request.Method == "GET")
        {
             if (context.Request.Path.StartsWithSegments("/employees"))
            {
                if (context.Request.Query.ContainsKey("Id"))
                {
                    if (int.TryParse(context.Request.Query["Id"], out int Employeeid))
                    {
                        var employee = EmployeesRepository.GetEmployeeById(Employeeid);
                        if (employee != null)
                        {
                            await context.Response.WriteAsync($"Name: {employee.Name}, Position: {employee.Position}, Salary: {employee.Salary}\r\n");
                        }
                        else
                        {
                            context.Response.StatusCode = 404;
                            await context.Response.WriteAsync("Employee not found");
                        }
                    }
                }
                else
                {
                    //await context.Response.WriteAsync("Employee List");
                    var employees = EmployeesRepository.GetEmployees();
                    foreach (var employee in employees)
                    {
                        await context.Response.WriteAsync($"Name: {employee.Name}, Position: {employee.Position}, Salary: {employee.Salary}\r\n");
                    }
                }
            }
           
        }
        else if (context.Request.Method == "POST")
        {
            
            {
                using var reader = new StreamReader(context.Request.Body);
                var body = await reader.ReadToEndAsync();
                var employee = JsonSerializer.Deserialize<Employee>(body);

                EmployeesRepository.AddEmployee(employee);
                context.Response.StatusCode = 201;
            }
        }
        else if ((context.Request.Method == "DELETE") && (context.Request.Headers["Authorization"] == "true"))
        {
            
            {
                if (context.Request.Query.ContainsKey("Id"))
                {
                    if (int.TryParse(context.Request.Query["Id"], out int Employeeid))
                    {
                        EmployeesRepository.DeleteEmployee(Employeeid);
                    }
                }


            }
        }
        else if (context.Request.Method == "PUT")
        {
           
            {
                using var reader = new StreamReader(context.Request.Body);
                var body = await reader.ReadToEndAsync();
                var employee = JsonSerializer.Deserialize<Employee>(body);

                EmployeesRepository.UpdateEmployee(employee);
                context.Response.StatusCode = 204;
            }
        }
    }
    else if (context.Request.Path.StartsWithSegments("/"))
    {
        context.Response.Headers["content-Type"] = "text/html";
        await context.Response.WriteAsync($"The method is :{context.Request.Method}</br>");
        await context.Response.WriteAsync($"The url is :{context.Request.Path}</br>");
        await context.Response.WriteAsync($"The <b>Headers </b?are :</br>");
        foreach (var key in context.Request.Headers.Keys)
        {
            await context.Response.WriteAsync($"<b>{key}</b>:{context.Request.Headers[key]}</br>");
        }
    }
    else if  (context.Request.Path.StartsWithSegments("/query"))
    {
        var query = context.Request.Query;
        await context.Response.WriteAsync($"Query Parameters:\r\n");
        foreach (var key in query.Keys)
        {
            await context.Response.WriteAsync($"{key}: {query[key]}\r\n");
        }
    }
});
app.Run();
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