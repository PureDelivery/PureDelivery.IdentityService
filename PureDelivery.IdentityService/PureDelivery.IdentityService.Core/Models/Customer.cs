using PureDelivery.Shared.Contracts.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureDelivery.IdentityService.Core.Models
{
    public class Customer
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public UserRole Role { get; set; } = UserRole.Customer;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        // Email confirmation fields
        public bool IsEmailConfirmed { get; set; } = false;
        public string? EmailConfirmationOtp { get; set; }
        public DateTime? EmailConfirmationOtpExpiry { get; set; }
        public int EmailConfirmationAttempts { get; set; } = 0;
        public DateTime? LastOtpSentAt { get; set; }

        // Navigation properties
        public CustomerProfile? Profile { get; set; }
        public ICollection<CustomerAddress> Addresses { get; set; } = new List<CustomerAddress>();
    }
}