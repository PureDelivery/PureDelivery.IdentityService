using PureDelivery.Common.Configuration.Extensions;
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
builder.Services.AddConfigurationProvider(builder.Configuration);

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