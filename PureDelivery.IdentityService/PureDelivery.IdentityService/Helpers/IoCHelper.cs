using Microsoft.EntityFrameworkCore;
using PureDelivery.Common.Configuration.Services;
using PureDelivery.IdentityService.Core.Configuration;
using PureDelivery.IdentityService.Infrastructure.Data;

namespace PureDelivery.IdentityService.Helpers
{
    public static class IoCHelper
    {
        /// <summary>
        /// Конфигурация базы данных
        /// </summary>
        public static async Task ConfigureDatabaseAsync(WebApplicationBuilder builder)
        {
            var dbConfig = await LoadDatabaseConfigurationAsync(builder);

            builder.Services.AddDbContext<CustomerDbContext>(options =>
            {
                ConfigureSqlServer(options, dbConfig);
                ConfigureLogging(options, dbConfig);
            });

            LogDatabaseConfiguration(dbConfig);
        }

        /// <summary>
        /// Загрузка конфигурации базы данных
        /// </summary>
        static async Task<IdentityServiceConfig> LoadDatabaseConfigurationAsync(WebApplicationBuilder builder)
        {
            using var tempServiceProvider = builder.Services.BuildServiceProvider();
   
            var configProvider = tempServiceProvider.GetRequiredService<ICustomConfigurationProvider>();
            return await configProvider.GetConfigurationAsync<IdentityServiceConfig>("IdentityService");
        }

        /// <summary>
        /// Конфигурация SQL Server
        /// </summary>
        static void ConfigureSqlServer(DbContextOptionsBuilder options, IdentityServiceConfig config)
        {
            options.UseSqlServer(config.ConnectionString, sqlOptions =>
            {
                if (config.Database.EnableRetryOnFailure)
                {
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: config.Database.MaxRetryCount,
                        maxRetryDelay: TimeSpan.FromSeconds(config.Database.MaxRetryDelay),
                        errorNumbersToAdd: null);
                }

                sqlOptions.CommandTimeout(config.Database.CommandTimeout);
            });
        }

        /// <summary>
        /// Конфигурация логирования EF
        /// </summary>
        static void ConfigureLogging(DbContextOptionsBuilder options, IdentityServiceConfig config)
        {
            if (config.Database.EnableSensitiveDataLogging)
            {
                options.EnableSensitiveDataLogging();
            }
        }

        /// <summary>
        /// Логирование конфигурации БД
        /// </summary>
        static void LogDatabaseConfiguration(IdentityServiceConfig config)
        {
            Console.WriteLine($"✅ Identity Service Database Configuration:");
            Console.WriteLine($"   Source: {config.Database.EnableSensitiveDataLogging}");
            Console.WriteLine($"   Command Timeout: {config.Database.EnableSensitiveDataLogging}s");
            Console.WriteLine($"   Retry Enabled: {config.Database.EnableSensitiveDataLogging}");
            Console.WriteLine($"   Max Retry Count: {config.Database.EnableSensitiveDataLogging}");
            Console.WriteLine($"   Max Retry Delay: {config.Database.EnableSensitiveDataLogging}s");
            Console.WriteLine($"   Sensitive Logging: {config.Database.EnableSensitiveDataLogging}");
        }
    }
}
