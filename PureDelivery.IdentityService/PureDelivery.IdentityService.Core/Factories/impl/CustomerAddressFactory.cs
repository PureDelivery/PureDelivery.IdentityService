using PureDelivery.IdentityService.Core.Models;
using PureDelivery.Shared.Contracts.DTOs.Identity.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureDelivery.IdentityService.Core.Factories.impl
{
    public class CustomerAddressFactory : ICustomerAddressFactory
    {
        public CustomerAddress CreateNewAddress(Guid customerId, CreateAddressRequest request, bool isDefault = false)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return new CustomerAddress
            {
                Id = Guid.NewGuid(),
                CustomerId = customerId,
                Label = request.Label,
                FullAddress = request.FullAddress,
                City = request.City,
                PostalCode = request.PostalCode,
                Building = request.Building,
                Apartment = request.Apartment,
                Floor = request.Floor.ToString(),
                DeliveryInstructions = request.DeliveryInstructions,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                IsDefault = isDefault,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }

        public CustomerAddress CreateUpdatedAddress(CustomerAddress existingAddress, UpdateAddressRequest request)
        {
            if (existingAddress == null)
                throw new ArgumentNullException(nameof(existingAddress));

            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return new CustomerAddress
            {
                Id = existingAddress.Id,
                CustomerId = existingAddress.CustomerId,
                Label = request.Label,
                FullAddress = request.FullAddress,
                City = request.City,
                PostalCode = request.PostalCode,
                Building = request.Building,
                Apartment = request.Apartment,
                Floor = request.Floor.ToString(),
                DeliveryInstructions = request.DeliveryInstructions,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                IsDefault = existingAddress.IsDefault,
                CreatedAt = existingAddress.CreatedAt,
                UpdatedAt = DateTime.UtcNow
            };
        }
    }
}