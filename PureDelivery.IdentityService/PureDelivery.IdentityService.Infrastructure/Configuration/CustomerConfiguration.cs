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
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            // Primary key
            builder.HasKey(e => e.Id);

            // Properties
            builder.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(e => e.PasswordHash)
                .IsRequired()
                .HasMaxLength(512);

            builder.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(e => e.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            // Indexes
            builder.HasIndex(e => e.Email)
                .IsUnique()
                .HasDatabaseName("Customers_Email");

            builder.HasIndex(e => e.IsActive)
                .HasDatabaseName("Customers_IsActive");

            builder.HasIndex(e => e.CreatedAt)
                .HasDatabaseName("Customers_CreatedAt");

            // Table configuration
            builder.ToTable("Customers");
        }
    }
}
