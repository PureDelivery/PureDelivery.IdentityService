using Microsoft.EntityFrameworkCore;
using PureDelivery.Common.Configuration.Extensions;
using PureDelivery.IdentityService.Core.Repositories;
using PureDelivery.IdentityService.Core.Services;
using PureDelivery.IdentityService.Core.Services.impl;
using PureDelivery.IdentityService.Infrastructure.Data;
using PureDelivery.IdentityService.Infrastructure.Repositories;
using PureDelivery.Infrastructure.Redis.Extensions;
using PureDelivery.Infrastructure.Redis.Services.impl;
using PureDelivery.Shared.Contracts.Common.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Настройка Serilog
builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.WithProperty("Service", "IdentityService")
        .WriteTo.Console()
        .WriteTo.File("logs/identity-service-log-.txt", rollingInterval: RollingInterval.Day);
});

// Конфигурация






// Delete?
builder.Services.AddRedisServices("Redis");
//builder.Services.AddScoped<ISessionService, RedisSessionService>();
// Delete?





builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddConfigurationProvider(builder.Configuration);

builder.Services.AddDbContext<CustomerDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Контроллеры
builder.Services.AddControllers();

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "PureDelivery Identity Service API",
        Version = "v1",
        Description = "API для управления клиентами и аутентификацией",
        Contact = new()
        {
            Name = "PureDelivery Team",
            Email = "support@puredelivery.com"
        }
    });

    // Добавляем XML комментарии если есть
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }

    // Настройка авторизации в Swagger (если будет JWT)
    c.AddSecurityDefinition("Bearer", new()
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."
    });

    c.AddSecurityRequirement(new()
    {
        {
            new()
            {
                Reference = new()
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Identity Service API v1");
        c.RoutePrefix = "swagger"; // Swagger будет доступен по /swagger
        c.DisplayRequestDuration();
        c.EnableTryItOutByDefault();
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();