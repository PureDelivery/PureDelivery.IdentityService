using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PureDelivery.IdentityService.Core.Models;

namespace PureDelivery.IdentityService.Infrastructure.Configuration
{
    public class CustomerAddressConfiguration : IEntityTypeConfiguration<CustomerAddress>
    {
        public void Configure(EntityTypeBuilder<CustomerAddress> builder)
        {
            // Primary key
            builder.HasKey(e => e.Id);

            // Properties
            builder.Property(e => e.Label)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(e => e.FullAddress)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(e => e.City)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(e => e.PostalCode)
                .HasMaxLength(20)
                .IsRequired();

            // ИСПРАВЛЕНИЕ: Правильная точность для координат
            builder.Property(e => e.Latitude)
                .HasPrecision(10, 8); // Достаточно для GPS координат

            builder.Property(e => e.Longitude)
                .HasPrecision(11, 8); // Достаточно для GPS координат

            builder.Property(e => e.Building)
                .HasMaxLength(100)
                .HasDefaultValue(string.Empty);

            builder.Property(e => e.Apartment)
                .HasMaxLength(50)
                .HasDefaultValue(string.Empty);

            builder.Property(e => e.Floor)
                .HasMaxLength(20)
                .HasDefaultValue(string.Empty);

            builder.Property(e => e.DeliveryInstructions)
                .HasMaxLength(500) // Увеличил с 200
                .HasDefaultValue(string.Empty);

            builder.Property(e => e.IsDefault)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(e => e.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(e => e.UpdatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            // Indexes
            builder.HasIndex(e => e.CustomerId)
                .HasDatabaseName("CustomerAddresses_CustomerId");

            builder.HasIndex(e => new { e.CustomerId, e.IsDefault })
                .HasDatabaseName("CustomerAddresses_CustomerId_IsDefault");

            builder.HasIndex(e => e.City)
                .HasDatabaseName("CustomerAddresses_City");

            builder.HasIndex(e => new { e.Latitude, e.Longitude })
                .HasDatabaseName("CustomerAddresses_Location");

            builder.HasIndex(e => e.IsActive)
                .HasDatabaseName("CustomerAddresses_IsActive");

            // ИСПРАВЛЕНИЕ: Убираем сложный Check Constraint
            // Вместо него будем контролировать логику в коде

            // Простые Check Constraints (если нужны)
            builder.HasCheckConstraint("CK_CustomerAddress_Coordinates_Latitude",
                "[Latitude] IS NULL OR ([Latitude] >= -90 AND [Latitude] <= 90)");

            builder.HasCheckConstraint("CK_CustomerAddress_Coordinates_Longitude",
                "[Longitude] IS NULL OR ([Longitude] >= -180 AND [Longitude] <= 180)");

            // Table configuration
            builder.ToTable("CustomerAddresses");
        }
    }
}