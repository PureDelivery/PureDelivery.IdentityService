using PureDelivery.IdentityService.Core.Models;
using PureDelivery.Shared.Contracts.DTOs.Identity.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureDelivery.IdentityService.Core.Factories
{
    public interface ICustomerAddressFactory
    {
        CustomerAddress CreateNewAddress(Guid customerId, CreateAddressRequest request, bool isDefault = false);
        CustomerAddress CreateUpdatedAddress(CustomerAddress existingAddress, UpdateAddressRequest request);
    }
}
