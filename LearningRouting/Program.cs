var builder = WebApplication.CreateBuilder(args);

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

