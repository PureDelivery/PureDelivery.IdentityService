using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PureDelivery.Common.Configuration.Services;
using PureDelivery.IdentityService.Core.Configuration;
using PureDelivery.IdentityService.Core.Models;
using PureDelivery.IdentityService.Infrastructure.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureDelivery.IdentityService.Infrastructure.Data
{
    public class CustomerDbContext : DbContext
    {
        private readonly IServiceProvider _serviceProvider;

        public CustomerDbContext(DbContextOptions<CustomerDbContext> options, IServiceProvider serviceProvider)
            : base(options)
        {
            _serviceProvider = serviceProvider;
        }

        public DbSet<Customer> Customers { get; set; } = null!;
        public DbSet<CustomerProfile> CustomerProfiles { get; set; } = null!;
        public DbSet<CustomerAddress> CustomerAddresses { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply configurations
            modelBuilder.ApplyConfiguration(new CustomerConfiguration());
            modelBuilder.ApplyConfiguration(new CustomerProfileConfiguration());
            modelBuilder.ApplyConfiguration(new CustomerAddressConfiguration());

            // Configure relationships
            ConfigureRelationships(modelBuilder);
        }

        private static void ConfigureRelationships(ModelBuilder modelBuilder)
        {
            // Customer -> CustomerProfile (One-to-One)
            modelBuilder.Entity<Customer>()
                .HasOne(c => c.Profile)
                .WithOne(p => p.Customer)
                .HasForeignKey<CustomerProfile>(p => p.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Customer -> CustomerAddresses (One-to-Many)
            modelBuilder.Entity<Customer>()
                .HasMany(c => c.Addresses)
                .WithOne(a => a.Customer)
                .HasForeignKey(a => a.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CustomerProfile>()
                .HasMany(p => p.Ratings)
                .WithOne(r => r.CustomerProfile)
                .OnDelete(DeleteBehavior.Cascade);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                try
                {
                    // Получаем конфигурационный провайдер из DI
                    var configurationProvider = _serviceProvider.GetService<ICustomConfigurationProvider>();
                    var logger = _serviceProvider.GetService<ILogger<CustomerDbContext>>();

                    if (configurationProvider != null)
                    {
                        // Получаем конфигурацию для IdentityService
                        var identityConfig = configurationProvider.GetConfigurationAsync<IdentityServiceConfig>("IdentityService")
                            .GetAwaiter()
                            .GetResult();

                        logger?.LogInformation("Using connection string from configuration provider");

                        // Настройка подключения к SQL Server
                        optionsBuilder.UseSqlServer(identityConfig.ConnectionString, sqlOptions =>
                        {
                            if (identityConfig.Database.EnableRetryOnFailure)
                            {
                                sqlOptions.EnableRetryOnFailure(
                                    maxRetryCount: identityConfig.Database.MaxRetryCount,
                                    maxRetryDelay: TimeSpan.FromSeconds(identityConfig.Database.MaxRetryDelay),
                                    errorNumbersToAdd: null);
                            }

                            sqlOptions.CommandTimeout(identityConfig.Database.CommandTimeout);
                        });

                        if (identityConfig.Database.EnableSensitiveDataLogging)
                        {
                            optionsBuilder.EnableSensitiveDataLogging();
                            logger?.LogWarning("Sensitive data logging is enabled");
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException("Configuration provider not found in DI container");
                    }
                }
                catch (Exception ex)
                {
                    var logger = _serviceProvider.GetService<ILogger<CustomerDbContext>>();
                    logger?.LogError(ex, "Failed to load configuration, using fallback connection string");

                    throw;
                }
            }
        }
    }
}