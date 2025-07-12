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
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public CustomerProfile? Profile { get; set; }
    public ICollection<CustomerAddress> Addresses { get; set; }
}
}
