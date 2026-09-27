namespace LearnMiddleware.MiddlewareComponents
{
    public class MyCustomMiddleware : IMiddleware
    {
        public Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            // Custom middleware logic here
            return next(context);
        }
    }
}
