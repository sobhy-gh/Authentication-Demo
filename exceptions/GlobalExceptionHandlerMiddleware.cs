namespace AuthenticationDemo.exceptions
{
	public class GlobalExceptionHandlerMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILogger _logger;

		public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger logger)
		{
			_next = next;
			_logger = logger;
		}

		public async Task InvokeAsync(HttpContext context)
		{

		    try{

				await _next(context); 
			}
			catch(Exception ex) 
			{

				_logger.LogError(ex, "Unhandeled exception !");

				context.Response.ContentType = "application/json";

				var statusCode = ex switch
				{
					KeyNotFoundException => 404,
					ArgumentException => 400,
					UnauthorizedAccessException => 401,
					InvalidOperationException => 409,
					_ => 500
				}; 

				context.Response.StatusCode = statusCode;

				await context.Response.WriteAsJsonAsync(new
				{
					statusCode = statusCode,
					message = "Something went wrong !"
				}); 
			}
		}

	}
}
