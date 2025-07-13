using PureDelivery.IdentityService.Core.Models;
using PureDelivery.Shared.Contracts.DTOs.Identity;
using PureDelivery.Shared.Contracts.DTOs.SessionDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureDelivery.IdentityService.Core.Mappers
{
    public static class CustomerResponseMapper
    {
        /// <summary>
        /// Маппинг для аутентификации
        /// </summary>
        public static AuthDto ToAuthDto(this Customer customer, string sessionId)
        {
            return new AuthDto
            {
                CustomerId = customer.Id,
                Email = customer.Email,
                FullName = $"{customer.Profile?.FirstName} {customer.Profile?.LastName}".Trim(),
                SessionId = sessionId,
                AuthenticatedAt = DateTime.UtcNow,
                Profile = customer.Profile?.ToDto()
            };
        }

        /// <summary>
        /// Маппинг краткой информации
        /// </summary>
        public static CustomerSummaryDto ToSummaryDto(this Customer customer)
        {
            return new CustomerSummaryDto
            {
                Id = customer.Id,
                Email = customer.Email,
                FullName = $"{customer.Profile?.FirstName} {customer.Profile?.LastName}".Trim(),
                IsActive = customer.IsActive,
                CreatedAt = customer.CreatedAt
            };
        }

        /// <summary>
        /// Маппинг полной информации
        /// </summary>
        public static CustomerDetailDto ToDetailDto(this Customer customer)
        {
            return new CustomerDetailDto
            {
                Id = customer.Id,
                Email = customer.Email,
                IsActive = customer.IsActive,
                CreatedAt = customer.CreatedAt,
                Profile = customer.Profile?.ToDto(),
                Addresses = customer.Addresses?.Where(a => a.IsActive).Select(a => a.ToDto()).ToList() ?? new()
            };
        }

        /// <summary>
        /// Маппинг клиента с профилем
        /// </summary>
        public static CustomerWithProfileDto ToWithProfileDto(this Customer customer)
        {
            return new CustomerWithProfileDto
            {
                Id = customer.Id,
                Email = customer.Email,
                IsActive = customer.IsActive,
                CreatedAt = customer.CreatedAt,
                Profile = customer.Profile?.ToDto()
            };
        }

        /// <summary>
        /// Маппинг клиента с адресами
        /// </summary>
        public static CustomerWithAddressesDto ToWithAddressesDto(this Customer customer)
        {
            return new CustomerWithAddressesDto
            {
                Id = customer.Id,
                Email = customer.Email,
                IsActive = customer.IsActive,
                CreatedAt = customer.CreatedAt,
                Addresses = customer.Addresses?.Where(a => a.IsActive).Select(a => a.ToDto()).ToList() ?? new()
            };
        }

        /// <summary>
        /// Маппинг для результата создания
        /// </summary>
        public static CreateCustomerResultDto ToCreateResultDto(this Customer customer)
        {
            return new CreateCustomerResultDto
            {
                CustomerId = customer.Id,
                Email = customer.Email,
                CreatedAt = customer.CreatedAt,
                Message = "Customer created successfully"
            };
        }

        /// <summary>
        /// Маппинг профиля
        /// </summary>
        public static CustomerProfileDto ToDto(this CustomerProfile profile)
        {
            return new CustomerProfileDto
            {
                FirstName = profile.FirstName,
                LastName = profile.LastName,
                Phone = profile.Phone,
                DateOfBirth = profile.DateOfBirth,
                LoyaltyPoints = profile.LoyaltyPoints,
                LastOrderDate = profile.LastOrderDate,
                PreferredPaymentMethod = profile.PreferredPaymentMethod,
                CreatedAt = profile.CreatedAt,
                UpdatedAt = profile.UpdatedAt
            };
        }

        /// <summary>
        /// Маппинг адреса
        /// </summary>
        public static CustomerAddressDto ToDto(this CustomerAddress address)
        {
            return new CustomerAddressDto
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
                Floor = address.Floor,
                DeliveryInstructions = address.DeliveryInstructions,
                IsDefault = address.IsDefault,
                IsActive = address.IsActive,
                CreatedAt = address.CreatedAt,
                UpdatedAt = address.UpdatedAt
            };
        }
    }
}
