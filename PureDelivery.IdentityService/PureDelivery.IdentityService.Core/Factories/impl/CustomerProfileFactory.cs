using PureDelivery.IdentityService.Core.Models;
using PureDelivery.Shared.Contracts.DTOs.Identity.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureDelivery.IdentityService.Core.Factories.impl
{
    public class CustomerProfileFactory : ICustomerProfileFactory
    {
        public CustomerProfile CreateUpdatedProfile(CustomerProfile existingProfile, UpdateProfileRequest request)
        {
            existingProfile.FirstName = request.FirstName.Trim();
            existingProfile.LastName = request.LastName.Trim();

            if (!string.IsNullOrWhiteSpace(request.Phone))
                existingProfile.Phone = request.Phone.Trim();

            if (request.DateOfBirth.HasValue)
                existingProfile.DateOfBirth = request.DateOfBirth.Value;

            if (!string.IsNullOrWhiteSpace(request.PreferredPaymentMethod))
                existingProfile.PreferredPaymentMethod = request.PreferredPaymentMethod.Trim();

            existingProfile.UpdatedAt = DateTime.UtcNow;

            return existingProfile;
        }

    }
}
