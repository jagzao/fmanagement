using Serilog;
using FM.Repository;
using FileManagerMockup.Middleware;
using MediatR;
using Microsoft.AspNetCore.Http.Features;
using FM.Cqrs.Queries;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication;
using FM.Cqrs.Queries.Files;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    Log.Information("Iniciando el host");
}catch(Exception ex)
{
    Log.Fatal(ex, "Host termino inesperadamente");
}
finally
{
    Log.CloseAndFlush();
}

builder.Host.UseSerilog(Log.Logger);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "FileManagerMockup", Version = "v1" });

    c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Description = "API Key needed to access the endpoints. API Key should be in the format 'X-Api-Key: {key}'",
        Name = "X-Api-Key",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "ApiKeyScheme"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "ApiKey"
                },
                Scheme = "ApiKeyScheme",
                Name = "X-Api-Key",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

builder.Services.AddAuthentication("DummyScheme")
    .AddScheme<AuthenticationSchemeOptions, DummyAuthenticationHandler>("DummyScheme", null);

builder.Services.AddSingleton<DapperContext>();

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(CreateBillQuery).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(DeleteBillQuery).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(UpdateBillQuery).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(GetBillByIdQuery).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(GetAllBillsQuery).Assembly);

    cfg.RegisterServicesFromAssembly(typeof(DeleteFileQuery).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(GenerateReportQuery).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(GetAllFilesQuery).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(GetFileByIdQuery).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(RegisterFileUploadQuery).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(UpdateFileQuery).Assembly);
});

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 1073741824;
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        builder =>
        {
            builder.WithOrigins("http://127.0.0.1:5173", "https://localhost:7257")
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials();
        });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ApiKeyMiddleware>();

app.MapControllers();

app.Run();