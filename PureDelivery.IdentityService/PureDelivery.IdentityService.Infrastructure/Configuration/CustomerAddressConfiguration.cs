using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PureDelivery.IdentityService.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureDelivery.IdentityService.Infrastructure.Configuration
{
    public class CustomerAddressConfiguration : IEntityTypeConfiguration<CustomerAddress>
    {
        public void Configure(EntityTypeBuilder<CustomerAddress> builder)
        {
            // Primary key
            builder.HasKey(e => e.Id);

            // Properties
            builder.Property(e => e.Label).HasMaxLength(200);
            builder.Property(e => e.FullAddress).IsRequired().HasMaxLength(500);
            builder.Property(e => e.City).HasMaxLength(100);
            builder.Property(e => e.PostalCode).HasMaxLength(20);
            builder.Property(e => e.Latitude).HasPrecision(18, 6);
            builder.Property(e => e.Longitude).HasPrecision(18, 6);
            builder.Property(e => e.Building).HasMaxLength(100);
            builder.Property(e => e.Apartment).HasMaxLength(50);
            builder.Property(e => e.Floor).HasMaxLength(20);
            builder.Property(e => e.DeliveryInstructions).HasMaxLength(200);
            builder.Property(e => e.IsDefault).IsRequired().HasDefaultValue(false);
            builder.Property(e => e.IsActive).IsRequired().HasDefaultValue(true);
            builder.Property(e => e.CreatedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");
            builder.Property(e => e.UpdatedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");

            // Indexes
            builder.HasIndex(e => e.CustomerId).HasDatabaseName("CustomerAddresses_CustomerId");
            builder.HasIndex(e => new { e.CustomerId, e.IsDefault }).HasDatabaseName("CustomerAddresses_CustomerId_IsDefault");
            builder.HasIndex(e => e.City).HasDatabaseName("CustomerAddresses_City");
            builder.HasIndex(e => new { e.Latitude, e.Longitude }).HasDatabaseName("CustomerAddresses_Location");
            builder.HasIndex(e => e.IsActive).HasDatabaseName("CustomerAddresses_IsActive");

            // Check constraints (теперь проще - без проверки роли!)
            builder.ToTable("CustomerAddresses", t =>
            {
                t.HasCheckConstraint("CustomerAddresses_SingleDefault",
                    "IsDefault = 0 OR (SELECT COUNT(*) FROM CustomerAddresses ca2 WHERE ca2.CustomerId = CustomerId AND ca2.IsDefault = 1 AND ca2.IsActive = 1) = 1");
            });
        }
    }
}
