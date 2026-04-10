using Microsoft.Extensions.Logging;
using PureDelivery.IdentityService.Core.Helpers;
using PureDelivery.IdentityService.Core.Models;
using PureDelivery.IdentityService.Core.Repositories;
using PureDelivery.Shared.Contracts.Domain.Enums;
using PureDelivery.Shared.Contracts.Domain.Models;
using PureDelivery.Shared.Contracts.DTOs.Identity.Requests;
using PureDelivery.Shared.Contracts.DTOs.Identity.Responses;

namespace PureDelivery.IdentityService.Core.Services.impl
{
    public class UserCredentialService : IUserCredentialService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ILogger<UserCredentialService> _logger;

        public UserCredentialService(
            ICustomerRepository customerRepository,
            ILogger<UserCredentialService> logger)
        {
            _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<BaseResponse<RegisterUserCredentialResult>> RegisterAsync(
            RegisterUserCredentialRequest request,
            CancellationToken cancellationToken = default)
        {
            try
            {
                if (request.Role == UserRole.Customer)
                    return BaseResponse<RegisterUserCredentialResult>.Failure(
                        "Use the public /register endpoint to create customers.");

                // Проверяем уникальность email по всем записям (включая неактивные)
                var existing = await _customerRepository.GetByEmailAsync(request.Email, cancellationToken);
                if (existing != null)
                    return BaseResponse<RegisterUserCredentialResult>.Failure("Email already exists.");

                var user = new Customer
                {
                    Id = Guid.NewGuid(),
                    Email = request.Email.ToLower().Trim(),
                    PasswordHash = PasswordHelper.HashPassword(request.Password),
                    Role = request.Role,
                    IsActive = true,
                    IsEmailConfirmed = true, // создаётся администратором/сервисом — OTP не нужен
                    CreatedAt = DateTime.UtcNow
                };

                await _customerRepository.AddWithProfileAsync(user, cancellationToken);

                _logger.LogInformation("Created credentials for {Role} user: {UserId}", user.Role, user.Id);

                return BaseResponse<RegisterUserCredentialResult>.Success(new RegisterUserCredentialResult
                {
                    UserId = user.Id,
                    Email = user.Email,
                    Role = user.Role,
                    CreatedAt = user.CreatedAt
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating credentials for {Email}", request.Email);
                return BaseResponse<RegisterUserCredentialResult>.Failure($"Error creating credentials: {ex.Message}");
            }
        }

        public async Task<BaseResponse<bool>> DeactivateAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _customerRepository.DeleteAsync(userId, cancellationToken);
                if (!result)
                    return BaseResponse<bool>.Failure("User not found.");

                _logger.LogInformation("Deactivated user credentials: {UserId}", userId);
                return BaseResponse<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deactivating user: {UserId}", userId);
                return BaseResponse<bool>.Failure($"Error deactivating user: {ex.Message}");
            }
        }
    }
}
