using PureDelivery.IdentityService.Core.Helpers;
using PureDelivery.IdentityService.Core.Models;
using PureDelivery.Shared.Contracts.DTOs.Identity.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureDelivery.IdentityService.Core.Mappers
{
    public static class CustomerRequestMapper
    {
        public static Customer ToCustomer(this CreateCustomerRequest request)
        {
            return new Customer
            {
                Id = Guid.NewGuid(),
                Email = request.Email.ToLower().Trim(),
                PasswordHash = PasswordHelper.HashPassword(request.Password),
                Role = PureDelivery.Shared.Contracts.Domain.Enums.UserRole.Customer,
                IsActive = false,
                CreatedAt = DateTime.UtcNow,
                IsEmailConfirmed = false,
                EmailConfirmationOtp = null,
                EmailConfirmationOtpExpiry = null,
                EmailConfirmationAttempts = 0,
                LastOtpSentAt = null,

                Profile = request.ToCustomerProfile()
            };
        }

        public static Customer ToCustomerWithOtp(this CreateCustomerRequest request, string otpCode, DateTime otpExpiry)
        {
            var customer = request.ToCustomer();

            customer.EmailConfirmationOtp = otpCode;
            customer.EmailConfirmationOtpExpiry = otpExpiry;
            customer.LastOtpSentAt = DateTime.UtcNow;

            return customer;
        }

        public static CustomerProfile ToCustomerProfile(this CreateCustomerRequest request)
        {
            return new CustomerProfile
            {
                FirstName = request.FirstName?.Trim() ?? string.Empty,
                LastName = request.LastName?.Trim() ?? string.Empty,
                Phone = request.Phone?.Trim() ?? string.Empty,
                DateOfBirth = request.DateOfBirth,
                LoyaltyPoints = 0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }
    }
}