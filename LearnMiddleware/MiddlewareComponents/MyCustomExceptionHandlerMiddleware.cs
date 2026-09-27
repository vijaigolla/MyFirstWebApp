namespace LearnMiddleware.MiddlewareComponents
{
    public class MyCustomExceptionHandlerMiddleware : IMiddleware   
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);

            }
            catch (Exception ex)
            {
                // Handle the exception and return a custom response
                context.Response.StatusCode = 500;
                if (!context.Response.HasStarted)
                {
                    context.Response.ContentType = "text/plain";
                }
                await context.Response.WriteAsync($"An error occurred: {ex.Message}");
            }
        }
    }
}
