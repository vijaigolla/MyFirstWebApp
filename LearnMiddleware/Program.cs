using LearnMiddleware.MiddlewareComponents;

var builder = WebApplication.CreateBuilder(args);
//register the custom middleware as a transient service in the DI container.
builder.Services.AddTransient<MyCustomMiddleware>();    

var app = builder.Build();

app.Use(async(HttpContext context, RequestDelegate next) =>
{
    await context.Response.WriteAsync("This is my first middleware\r\n");
    await next(context);
    await context.Response.WriteAsync("This is my first middleware after next\r\n");
});
//Use the custom middleware in the pipeline!!
app.UseMiddleware<MyCustomMiddleware>();
////Terminates the pipeline and does not call the next middleware in the pipeline.
//app.Run(async(context) =>
//{
//    await context.Response.WriteAsync("Middleware 1.1 processed\r\n");
//});
//using MapWhen to conditionally branch the middleware pipeline based on the request path and method.
app.MapWhen(context => { return context.Request.Path.StartsWithSegments("/Employee") && context.Request.Method == "GET" && context.Request.Query.ContainsKey("Id"); }, appBuilder =>
{
    appBuilder.Use(async (context, next) =>
    {
        await context.Response.WriteAsync("This is my first-1 middleware in Employee\r\n");
        await next(context);
        await context.Response.WriteAsync("This is my first-1 middleware in Employee after next\r\n");
    });
    appBuilder.Use(async (context, next) =>
    {
        await context.Response.WriteAsync("This is my second-2 middleware in Employee\r\n");
        await next(context);
        await context.Response.WriteAsync("This is my second-2 middleware in Employee after next\r\n");
    });
    appBuilder.Run(async (context) =>
    {
        await context.Response.WriteAsync("Middleware 1.1 processed\r\n");
    });
}); )

app.Map("/map1", appBuilder =>
{
    appBuilder.Use(async (context, next) =>
    {
        await context.Response.WriteAsync("This is my first middleware in map1\r\n");
        await next(context);
        await context.Response.WriteAsync("This is my first middleware in map1 after next\r\n");
    });
    appBuilder.Use(async (context, next) =>
    {
        await context.Response.WriteAsync("This is my second middleware in map1\r\n");
        await next(context);
        await context.Response.WriteAsync("This is my second middleware in map1 after next\r\n");
    });
    appBuilder.Run(async (context) =>
    {
        await context.Response.WriteAsync("Middleware 1.1 processed\r\n");
    });
});

app.Use(async(HttpContext context, RequestDelegate next) =>
{
    await context.Response.WriteAsync("This is my second middleware\r\n");
    await next(context);
    await context.Response.WriteAsync("This is my second middleware after next\r\n");
});
app.Use(async(HttpContext context, RequestDelegate next) =>
{
    await context.Response.WriteAsync("This is my third middleware\r\n");
    await next(context);
    await context.Response.WriteAsync("This is my third middleware after next\r\n");
});


app.Run();
