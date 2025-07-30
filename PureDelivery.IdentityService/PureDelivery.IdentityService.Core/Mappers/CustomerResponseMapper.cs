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

        public static CustomerSummaryDto ToMainPageDto(this Customer customer)
        {
            return new CustomerSummaryDto
            {
                Id = customer.Id,
                LoyaltyPoints = customer.Profile?.LoyaltyPoints ?? 0,
                UserGrade = customer.Profile?.Ratings?.Any() == true
                    ? (decimal)customer.Profile.Ratings.Average(r => r.Rating)
                    : 0m,
                TotalRatings = customer.Profile?.Ratings?.Count ?? 0,
                AvatarUrl = customer.Profile?.AvatarUrl ?? string.Empty,
                FullName = $"{customer.Profile?.FirstName} {customer.Profile?.LastName}".Trim(),
            };
        }

        public static CustomerLoyaltyDto ToLoyaltyDto(this Customer customer)
        {
            return new CustomerLoyaltyDto
            {
                Id = customer.Id,
                LoyaltyPoints = customer.Profile?.LoyaltyPoints ?? 0
            };
        }

        public static CustomerProfileInfoDto ToProfileInfoDto(this Customer customer)
        {
            return new CustomerProfileInfoDto
            {
                Id = customer.Id,
                Email = customer.Email,
                FirstName = customer.Profile?.FirstName ?? string.Empty,
                LastName = customer.Profile?.LastName ?? string.Empty,
                Phone = customer.Profile?.Phone ?? string.Empty,
                DateOfBirth = customer.Profile?.DateOfBirth,
                AvatarUrl = customer.Profile?.AvatarUrl,
                PreferredPaymentMethod = customer.Profile?.PreferredPaymentMethod,
                CreatedAt = customer.CreatedAt,
                UpdatedAt = customer.Profile?.UpdatedAt ?? customer.CreatedAt
            };
        }

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
                UpdatedAt = profile.UpdatedAt,
                AvatarUrl = profile.AvatarUrl,
                TotalRatings = profile.Ratings?.Count ?? 0,
                UserGrade = profile.Ratings?.Any() == true
                    ? (decimal)profile.Ratings.Average(r => r.Rating)
                    : 0m
            };
        }

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
