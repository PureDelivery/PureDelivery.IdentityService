using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureDelivery.IdentityService.Core.Models
{
    public class CustomerRating
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public Guid OrderId { get; set; }
        public Guid CourierId { get; set; }
        public int Rating { get; set; } // 1-5 звезд
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation
        public CustomerProfile CustomerProfile { get; set; } = null!;
    }
}
