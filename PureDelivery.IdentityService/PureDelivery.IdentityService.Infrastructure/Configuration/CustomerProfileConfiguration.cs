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
    public class CustomerProfileConfiguration : IEntityTypeConfiguration<CustomerProfile>
    {
        public void Configure(EntityTypeBuilder<CustomerProfile> builder)
        {
            // Primary key
            builder.HasKey(e => e.CustomerId);

            // Properties
            builder.Property(e => e.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.LastName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.Phone)
                .HasMaxLength(20);

            builder.Property(e => e.DateOfBirth)
                .IsRequired(false);

            // Customer-specific properties
            builder.Property(e => e.LoyaltyPoints)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(e => e.LastOrderDate)
                .IsRequired(false);

            builder.Property(e => e.PreferredPaymentMethod)
                .HasMaxLength(50);

            builder.Property(e => e.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(e => e.UpdatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(e => e.AvatarUrl)
                .HasMaxLength(500)
                .IsRequired(false);

            // Indexes
            builder.HasIndex(e => e.Phone)
                .HasDatabaseName("CustomerProfiles_Phone");

            builder.HasIndex(e => new { e.FirstName, e.LastName })
                .HasDatabaseName("CustomerProfiles_Name");

            builder.HasIndex(e => e.LoyaltyPoints)
                .HasDatabaseName("CustomerProfiles_LoyaltyPoints");

            // Table configuration
            builder.ToTable("CustomerProfiles");
        }
    }
}
