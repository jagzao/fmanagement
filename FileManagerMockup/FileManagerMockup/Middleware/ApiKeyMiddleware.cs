namespace FileManagerMockup.Middleware
{
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ApiKeyMiddleware> _logger;
        private const string APIKEYNAME = "X-Api-Key";

        public ApiKeyMiddleware(RequestDelegate next, IConfiguration configuration, ILogger<ApiKeyMiddleware> logger)
        {
            _next = next;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            _logger.LogInformation("ApiKeyMiddleware Invoked");

            if (context.Request.Path.StartsWithSegments("/api/Auth"))
            {
                _logger.LogInformation("Skipping API key validation for /api/Auth endpoint.");
                await _next(context);
                return;
            }

            if (!context.Request.Headers.TryGetValue(APIKEYNAME, out var extractedApiKey))
            {
                _logger.LogWarning("Api Key was not provided.");
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Api Key was not provided.");
                return;
            }

            var apiKey = _configuration.GetValue<string>(APIKEYNAME);

            if (apiKey == null || !apiKey.Equals(extractedApiKey))
            {
                _logger.LogWarning("Unauthorized client.");
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized client.");
                return;
            }

            await _next(context);
        }
    }
}