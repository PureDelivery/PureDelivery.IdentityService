using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureDelivery.IdentityService.Core.Models
{
    public class CustomerAddress
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid CustomerId { get; set; }

        [MaxLength(200)]
        public string Label { get; set; } = string.Empty; // "Home", "Work", "Office"

        [Required]
        [MaxLength(500)]
        public string FullAddress { get; set; } = string.Empty;

        [MaxLength(100)]
        public string City { get; set; } = string.Empty;

        [MaxLength(20)]
        public string PostalCode { get; set; } = string.Empty;

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        [MaxLength(100)]
        public string Building { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Apartment { get; set; } = string.Empty;

        [MaxLength(20)]
        public string Floor { get; set; } = string.Empty;

        [MaxLength(200)]
        public string DeliveryInstructions { get; set; } = string.Empty;

        public bool IsDefault { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Customer Customer { get; set; } = null!;
    }
}
