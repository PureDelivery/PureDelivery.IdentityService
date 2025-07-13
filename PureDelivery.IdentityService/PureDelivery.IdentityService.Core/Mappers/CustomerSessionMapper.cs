using PureDelivery.IdentityService.Core.Models;
using PureDelivery.Shared.Contracts.DTOs.SessionDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureDelivery.IdentityService.Core.Mappers
{
    public static class CustomerSessionMapper
    {
        public static CustomerSessionDto ToSessionDto(this Customer customer)
        {
            var defaultAddress = customer.Addresses?.FirstOrDefault(a => a.IsDefault);

            return new CustomerSessionDto
            {
                Id = customer.Id,
                Email = customer.Email,
                FullName = $"{customer.Profile?.FirstName} {customer.Profile?.LastName}".Trim(),
                Phone = customer.Profile?.Phone ?? string.Empty,
                LoyaltyPoints = customer.Profile?.LoyaltyPoints ?? 0,
                DefaultAddress = defaultAddress?.ToSessionDto()
            };
        }

        public static CustomerAddressSessionDto? ToSessionDto(this CustomerAddress? address)
        {
            if (address == null) return null;

            return new CustomerAddressSessionDto
            {
                Id = address.Id,
                Label = address.Label,
                FullAddress = address.FullAddress,
                City = address.City,
                PostalCode = address.PostalCode,
                Latitude = address.Latitude,
                Longitude = address.Longitude,
                Building = address.Building,
                Apartment = address.Apartment,
                Floor = address.Floor
            };
        }
    }
}
