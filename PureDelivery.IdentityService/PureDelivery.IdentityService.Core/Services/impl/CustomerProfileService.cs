using Microsoft.Extensions.Logging;
using PureDelivery.IdentityService.Core.Factories;
using PureDelivery.IdentityService.Core.Models;
using PureDelivery.IdentityService.Core.Repositories;
using PureDelivery.IdentityService.Core.ResponseConstants;
using PureDelivery.IdentityService.Core.ResponseConstants.Enums;
using PureDelivery.IdentityService.Core.Services;
using PureDelivery.Shared.Contracts.Domain.Models;
using PureDelivery.Shared.Contracts.DTOs.Identity.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureDelivery.IdentityService.Core.Services.impl
{
    public class CustomerProfileService : ICustomerProfileService
    {
        private readonly ILogger<CustomerProfileService> _logger;
        private readonly ICustomerProfileRepository _profileRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly ICustomerProfileFactory _customerProfileFactory;

        public CustomerProfileService(
            ICustomerProfileRepository profileRepository,
            ICustomerRepository customerRepository,
            ICustomerProfileFactory customerProfileFactory,
            ILogger<CustomerProfileService> logger)
        {
            this._profileRepository = profileRepository ?? throw new ArgumentNullException(nameof(profileRepository));
            this._customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
            this._customerProfileFactory = customerProfileFactory ?? throw new ArgumentNullException(nameof(customerProfileFactory));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<BaseResponse<bool>> UpdateProfileAsync(Guid customerId, UpdateProfileRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var customer = await _customerRepository.GetActiveByIdAsync(customerId, cancellationToken);
                if (customer == null)
                    return BaseResponse<bool>.Failure(IdentityCoreErrors.CustomerNotFound.ToString());

                var existingProfile = await _profileRepository.GetByCustomerIdAsync(customerId, cancellationToken);
                if (existingProfile == null)
                    return BaseResponse<bool>.Failure(IdentityCoreErrors.ProfileNotFound.ToString());

                var updatedProfile = _customerProfileFactory.CreateUpdatedProfile(existingProfile, request);
                await _profileRepository.UpdateAsync(updatedProfile, cancellationToken);

                _logger.LogInformation("Profile updated for customer: {CustomerId}", customerId);
                return BaseResponse<bool>.Success(true, SuccessMessages.ProfileUpdated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating profile for customer: {CustomerId}", customerId);
                return BaseResponse<bool>.Failure($"Error updating profile: {ex.Message}");
            }
        }

        public async Task<BaseResponse<bool>> UpdateAvatarAsync(Guid customerId, string avatarUrl, CancellationToken cancellationToken = default)
        {
            try
            {
                var customer = await _customerRepository.GetActiveByIdAsync(customerId, cancellationToken);
                if (customer == null)
                    return BaseResponse<bool>.Failure(IdentityCoreErrors.CustomerNotFound.ToString());

                var result = await _profileRepository.UpdateAvatarAsync(customerId, avatarUrl, cancellationToken);
                if (!result)
                    return BaseResponse<bool>.Failure("Failed to update avatar");

                _logger.LogInformation("Avatar updated for customer: {CustomerId}", customerId);
                return BaseResponse<bool>.Success(true, "Avatar updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating avatar for customer: {CustomerId}", customerId);
                return BaseResponse<bool>.Failure($"Error updating avatar: {ex.Message}");
            }
        }

        public async Task<BaseResponse<bool>> AddLoyaltyPointsAsync(Guid customerId, decimal points, string reason, CancellationToken cancellationToken = default)
        {
            try
            {
                var customer = await _customerRepository.GetActiveByIdAsync(customerId, cancellationToken);
                if (customer == null)
                    return BaseResponse<bool>.Failure(IdentityCoreErrors.CustomerNotFound.ToString());

                if (points <= 0)
                    return BaseResponse<bool>.Failure("Points must be greater than zero");

                var result = await _profileRepository.UpdateLoyaltyPointsAsync(customerId, points, cancellationToken);
                if (!result)
                    return BaseResponse<bool>.Failure("Failed to add loyalty points");

                _logger.LogInformation("Loyalty points added for customer: {CustomerId}, Points: {Points}, Reason: {Reason}",
                    customerId, points, reason);
                return BaseResponse<bool>.Success(true, SuccessMessages.LoyaltyPointsAdded);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding loyalty points for customer: {CustomerId}", customerId);
                return BaseResponse<bool>.Failure($"Error adding loyalty points: {ex.Message}");
            }
        }

        public async Task<BaseResponse<bool>> SpendLoyaltyPointsAsync(Guid customerId, decimal points, string reason, CancellationToken cancellationToken = default)
        {
            try
            {
                var customer = await _customerRepository.GetActiveByIdAsync(customerId, cancellationToken);
                if (customer == null)
                    return BaseResponse<bool>.Failure(IdentityCoreErrors.CustomerNotFound.ToString());

                if (points <= 0)
                    return BaseResponse<bool>.Failure("Points must be greater than zero");

                var profile = await _profileRepository.GetByCustomerIdAsync(customerId, cancellationToken);
                if (profile == null)
                    return BaseResponse<bool>.Failure(IdentityCoreErrors.ProfileNotFound.ToString());

                if (profile.LoyaltyPoints < points)
                    return BaseResponse<bool>.Failure("Insufficient loyalty points");

                var result = await _profileRepository.UpdateLoyaltyPointsAsync(customerId, -points, cancellationToken);
                if (!result)
                    return BaseResponse<bool>.Failure("Failed to spend loyalty points");

                _logger.LogInformation("Loyalty points spent for customer: {CustomerId}, Points: {Points}, Reason: {Reason}",
                    customerId, points, reason);
                return BaseResponse<bool>.Success(true, SuccessMessages.LoyaltyPointsSpent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error spending loyalty points for customer: {CustomerId}", customerId);
                return BaseResponse<bool>.Failure($"Error spending loyalty points: {ex.Message}");
            }
        }

        public async Task<BaseResponse<decimal>> GetLoyaltyPointsBalanceAsync(Guid customerId, CancellationToken cancellationToken = default)
        {
            try
            {
                var customer = await _customerRepository.GetActiveByIdAsync(customerId, cancellationToken);
                if (customer == null)
                    return BaseResponse<decimal>.Failure(IdentityCoreErrors.CustomerNotFound.ToString());

                var profile = await _profileRepository.GetByCustomerIdAsync(customerId, cancellationToken);
                if (profile == null)
                    return BaseResponse<decimal>.Failure(IdentityCoreErrors.ProfileNotFound.ToString());

                return BaseResponse<decimal>.Success(profile.LoyaltyPoints);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting loyalty points balance for customer: {CustomerId}", customerId);
                return BaseResponse<decimal>.Failure($"Error getting loyalty points balance: {ex.Message}");
            }
        }

        public async Task<BaseResponse<bool>> UpdateLastOrderDateAsync(Guid customerId, DateTime orderDate, CancellationToken cancellationToken = default)
        {
            try
            {
                var customer = await _customerRepository.GetActiveByIdAsync(customerId, cancellationToken);
                if (customer == null)
                    return BaseResponse<bool>.Failure(IdentityCoreErrors.CustomerNotFound.ToString());

                var result = await _profileRepository.UpdateLastOrderDateAsync(customerId, orderDate, cancellationToken);
                if (!result)
                    return BaseResponse<bool>.Failure("Failed to update last order date");

                _logger.LogInformation("Last order date updated for customer: {CustomerId}, Date: {OrderDate}", customerId, orderDate);
                return BaseResponse<bool>.Success(true, SuccessMessages.LastOrderDateUpdated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating last order date for customer: {CustomerId}", customerId);
                return BaseResponse<bool>.Failure($"Error updating last order date: {ex.Message}");
            }
        }

        public async Task<BaseResponse<bool>> GradeUser(Guid customerId, int grade, CancellationToken cancellationToken = default)
        {
            try
            {
                var customer = await _customerRepository.GetActiveByIdAsync(customerId, cancellationToken);
                if (customer == null)
                    return BaseResponse<bool>.Failure(IdentityCoreErrors.CustomerNotFound.ToString());

                if (grade < 1 || grade > 5)
                    return BaseResponse<bool>.Failure("Grade must be between 1 and 5");

                // This would typically be called from an order service
                // For now, we'll just log it
                _logger.LogInformation("Customer graded: {CustomerId}, Grade: {Grade}", customerId, grade);
                return BaseResponse<bool>.Success(true, SuccessMessages.CustomerGraded);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error grading customer: {CustomerId}", customerId);
                return BaseResponse<bool>.Failure($"Error grading customer: {ex.Message}");
            }
        }
    }
}