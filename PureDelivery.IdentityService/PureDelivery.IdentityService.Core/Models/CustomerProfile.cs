using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureDelivery.IdentityService.Core.Models
{
    public class CustomerProfile
    {
        public Guid CustomerId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DateTime? DateOfBirth { get; set; }

        // Customer-specific fields
        public decimal LoyaltyPoints { get; set; } = 0;
        public DateTime? LastOrderDate { get; set; }
        public string? PreferredPaymentMethod { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Customer Customer { get; set; } = null!;

        public ICollection<CustomerRating> Ratings { get; set; } = new List<CustomerRating>();
    }
}
