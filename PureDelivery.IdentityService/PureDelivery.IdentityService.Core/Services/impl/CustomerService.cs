using Microsoft.Extensions.Logging;
using PureDelivery.Common.Configuration.Services;
using PureDelivery.IdentityService.Core.Configuration;
using PureDelivery.IdentityService.Core.Helpers;
using PureDelivery.IdentityService.Core.Mappers;
using PureDelivery.IdentityService.Core.Models;
using PureDelivery.IdentityService.Core.Repositories;
using PureDelivery.IdentityService.Core.ResponseConstants;
using PureDelivery.IdentityService.Core.ResponseConstants.Enums;
using PureDelivery.Shared.Contracts.Common.Services;
using PureDelivery.Shared.Contracts.Domain.Models;
using PureDelivery.Shared.Contracts.DTOs.Identity;
using PureDelivery.Shared.Contracts.DTOs.Identity.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PureDelivery.IdentityService.Core.Services.impl
{
    public class CustomerService : ICustomerService
    {
        private readonly ILogger<CustomerService> _logger;
        private readonly ICustomerRepository _customerRepository;
        private readonly ISessionService _sessionService;
        private readonly IOtpService _otpService;
        private readonly IEmailService _emailService;
        private readonly OtpSettings _otpSettings;

        public CustomerService(
            ICustomerRepository customerRepository,
            ILogger<CustomerService> logger,
            ISessionService sessionService,
            IOtpService otpService,
            IEmailService emailService,
            ICustomConfigurationProvider configProvider)
        {
            _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
            _otpService = otpService ?? throw new ArgumentNullException(nameof(otpService));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _otpSettings = configProvider.GetConfigurationAsync<OtpSettings>("Otp").GetAwaiter().GetResult();
        }

        public async Task<BaseResponse<bool>> DeleteCustomerAsync(Guid customerId, CancellationToken cancellationToken = default)
        {
            try
            {
                var customer = await _customerRepository.GetActiveByIdAsync(customerId, cancellationToken);

                if (customer == null)
                    return BaseResponse<bool>.Failure(IdentityCoreErrors.CustomerNotFound.ToString());

                _logger.LogInformation("Activating customer with ID: {CustomerId}", customerId);

                await _customerRepository.DeleteAsync(customer.Id, cancellationToken);
                return BaseResponse<bool>.Success(true, SuccessMessages.CustomerDeactivated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deactivating customer: {CustomerId}", customerId);
                return BaseResponse<bool>.Failure($"Error deactivating customer: {ex.Message}");
            }
        }

        public async Task<BaseResponse<AuthDto>> AuthenticateAsync(AuthenticateRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var customer = await _customerRepository.GetActiveWithProfileByEmailAsync(request.Email, cancellationToken);

                if (customer == null)
                    return BaseResponse<AuthDto>.Failure(IdentityCoreErrors.CustomerNotFound.ToString());

                if (!PasswordHelper.VerifyPassword(request.Password, customer.PasswordHash))
                    return BaseResponse<AuthDto>.Failure(IdentityCoreErrors.InvalidCredentials.ToString());

                var customerSessionDto = customer.ToSessionDto();

                var session = await _sessionService.CreateSessionWithDataAsync(
                            customer.Id.ToString(),
                            customerSessionDto,
                            request
                        );

                _logger.LogInformation("Customer authenticated and session created: {CustomerId}, SessionId: {SessionId}", customer.Id, session.SessionId);

                var authDto = customer.ToAuthDto(session.SessionId);

                _logger.LogInformation("Customer authenticated: {CustomerId}", customer.Id);
                return BaseResponse<AuthDto>.Success(authDto, SuccessMessages.AuthenticationSuccessful);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error authenticating customer with email: {Email}", request.Email);
                return BaseResponse<AuthDto>.Failure($"Authentication failed: {ex.Message}");
            }
        }

        public async Task<BaseResponse<bool>> LogoutAsync(string sessionId, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrEmpty(sessionId))
                    return BaseResponse<bool>.Failure("Session ID is required");

                var result = await _sessionService.DeleteSessionAsync(sessionId);

                if (!result)
                {
                    _logger.LogWarning("Session {SessionId} not found for logout", sessionId);
                    return BaseResponse<bool>.Failure("Session not found");
                }

                _logger.LogInformation("Customer logged out successfully. SessionId: {SessionId}", sessionId);
                return BaseResponse<bool>.Success(true, "Logged out successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout for session: {SessionId}", sessionId);
                return BaseResponse<bool>.Failure($"Logout failed: {ex.Message}");
            }
        }
        public async Task<BaseResponse<bool>> RequestForgotPasswordAsync(string email, CancellationToken cancellationToken = default)
        {
            try
            {
                var customer = await _customerRepository.GetActiveByEmailAsync(email, cancellationToken);
                if (customer == null)
                    return BaseResponse<bool>.Failure(IdentityCoreErrors.CustomerNotFound.ToString());

                var otpCode = _otpService.GenerateOtp();
                var otpExpiry = _otpService.GetOtpExpiryTime(_otpSettings.ExpiryMinutes);

                await _customerRepository.UpdateOtpAsync(customer.Id, otpCode, otpExpiry, cancellationToken);
                await _emailService.SendPasswordChangeOtpEmailAsync(email, otpCode, cancellationToken);

                _logger.LogInformation("Password change OTP sent to: {Email}", email);
                return BaseResponse<bool>.Success(true, "OTP code sent to your email");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error requesting password change for: {Email}", email);
                return BaseResponse<bool>.Failure($"Error requesting password change: {ex.Message}");
            }
        }

        public async Task<BaseResponse<bool>> ChangePasswordWithOtpAsync(ChangePasswordWithOtpRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var customer = await _customerRepository.GetActiveByEmailAsync(request.Email, cancellationToken);
                if (customer == null)
                    return BaseResponse<bool>.Failure(IdentityCoreErrors.CustomerNotFound.ToString());

                if (!_otpService.ValidateOtp(request.OtpCode, customer.EmailConfirmationOtp, customer.EmailConfirmationOtpExpiry ?? new DateTime()))
                    return BaseResponse<bool>.Failure(IdentityCoreErrors.InvalidOrExpiredOtp.ToString());

                var newPasswordHash = PasswordHelper.HashPassword(request.NewPassword);
                await _customerRepository.UpdatePasswordAsync(customer.Id, newPasswordHash, cancellationToken);

                // Очищаем OTP после использования
                await _customerRepository.UpdateOtpAsync(customer.Id, null, DateTime.MinValue, cancellationToken);

                _logger.LogInformation("Password changed successfully for: {Email}", request.Email);
                return BaseResponse<bool>.Success(true, "Password changed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password for: {Email}", request.Email);
                return BaseResponse<bool>.Failure($"Error changing password: {ex.Message}");
            }
        }

        public async Task<BaseResponse<CreateCustomerResultDto>> CreateCustomerAsync(CreateCustomerRequest createCustomer, CancellationToken cancellationToken = default)
        {
            try
            {
                if (!await _customerRepository.IsEmailUniqueAsync(createCustomer.Email, cancellationToken: cancellationToken))
                    return BaseResponse<CreateCustomerResultDto>.Failure(IdentityCoreErrors.EmailAlreadyExists.ToString());

                var otpCode = _otpService.GenerateOtp();
                var otpExpiry = _otpService.GetOtpExpiryTime(_otpSettings.ExpiryMinutes);

                var customer = createCustomer.ToCustomerWithOtp(otpCode, otpExpiry);

                _logger.LogInformation("Creating customer with email: {Email}", createCustomer.Email);
                var createdCustomer = await _customerRepository.AddWithProfileAsync(customer, cancellationToken);

                await _emailService.SendOtpEmailAsync(customer.Email, otpCode, cancellationToken);

                return BaseResponse<CreateCustomerResultDto>.Success(createdCustomer.ToCreateResultDto(), SuccessMessages.CustomerCreated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating customer with email: {Email}", createCustomer.Email);
                return BaseResponse<CreateCustomerResultDto>.Failure($"Error creating customer: {ex.Message}");
            }
        }

        public async Task<BaseResponse<List<CustomerSummaryDto>>> GetActiveCustomersAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var customers = await _customerRepository.GetAllAsync(cancellationToken);
                var customerDtos = customers.Select(c => c.ToSummaryDto()).ToList();

                return BaseResponse<List<CustomerSummaryDto>>.Success(customerDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active customers");
                return BaseResponse<List<CustomerSummaryDto>>.Failure($"Error getting active customers: {ex.Message}");
            }
        }

        public async Task<BaseResponse<CustomerSummaryDto>> GetCustomerByIdAsync(Guid customerId, CancellationToken cancellationToken = default)
        {
            try
            {
                var customer = await _customerRepository.GetByIdAsync(customerId, cancellationToken);

                if (customer == null)
                    return BaseResponse<CustomerSummaryDto>.Failure(IdentityCoreErrors.CustomerNotFound.ToString());

                return BaseResponse<CustomerSummaryDto>.Success(customer.ToSummaryDto());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting customer by ID {customerId}");
                return BaseResponse<CustomerSummaryDto>.Failure($"Error getting customer: {ex.Message}");
            }
        }

        public async Task<BaseResponse<CustomerSummaryDto>> GetCustomerByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            try
            {
                var customer = await _customerRepository.GetActiveByEmailAsync(email, cancellationToken);

                if (customer == null)
                    return BaseResponse<CustomerSummaryDto>.Failure(IdentityCoreErrors.CustomerNotFound.ToString());

                return BaseResponse<CustomerSummaryDto>.Success(customer.ToSummaryDto());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting customer by email: {Email}", email);
                return BaseResponse<CustomerSummaryDto>.Failure($"Error getting customer: {ex.Message}");
            }
        }

        public async Task<BaseResponse<CustomerWithProfileDto>> GetCustomerWithProfileAsync(Guid customerId, CancellationToken cancellationToken = default)
        {
            try
            {
                var customer = await _customerRepository.GetWithProfileAsync(customerId, cancellationToken);

                if (customer == null)
                    return BaseResponse<CustomerWithProfileDto>.Failure(IdentityCoreErrors.CustomerNotFound.ToString());

                return BaseResponse<CustomerWithProfileDto>.Success(customer.ToWithProfileDto());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting customer by ID {customerId}");
                return BaseResponse<CustomerWithProfileDto>.Failure($"Error getting customer: {ex.Message}");
            }
        }

        public async Task<BaseResponse<CustomerWithAddressesDto>> GetCustomerWithAddressesAsync(Guid customerId, CancellationToken cancellationToken = default)
        {
            try
            {
                var customer = await _customerRepository.GetWithAddressesAsync(customerId, cancellationToken);

                if (customer == null)
                    return BaseResponse<CustomerWithAddressesDto>.Failure(IdentityCoreErrors.CustomerNotFound.ToString());

                return BaseResponse<CustomerWithAddressesDto>.Success(customer.ToWithAddressesDto());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting customer by ID {customerId}");
                return BaseResponse<CustomerWithAddressesDto>.Failure($"Error getting customer: {ex.Message}");
            }
        }

        public async Task<BaseResponse<CustomerDetailDto>> GetCustomerFullDataAsync(Guid customerId, CancellationToken cancellationToken = default)
        {
            try
            {
                var customer = await _customerRepository.GetWithAllDataAsync(customerId, cancellationToken);

                if (customer == null)
                    return BaseResponse<CustomerDetailDto>.Failure(IdentityCoreErrors.CustomerNotFound.ToString());

                return BaseResponse<CustomerDetailDto>.Success(customer.ToDetailDto());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting customer by ID {customerId}");
                return BaseResponse<CustomerDetailDto>.Failure($"Error getting customer: {ex.Message}");
            }
        }

        public async Task<BaseResponse<bool>> IsEmailAvailableAsync(string email, CancellationToken cancellationToken = default)
        {
            try
            {
                var isUnique = await _customerRepository.IsEmailUniqueAsync(email, cancellationToken: cancellationToken);
                return BaseResponse<bool>.Success(isUnique);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking email availability: {Email}", email);
                return BaseResponse<bool>.Failure($"Error checking email availability: {ex.Message}");
            }
        }

        public async Task<BaseResponse<bool>> ConfirmEmailAsync(ConfirmEmailRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var customer = await _customerRepository.GetByEmailAsync(request.Email, cancellationToken);
                if (customer == null)
                    return BaseResponse<bool>.Failure(IdentityCoreErrors.CustomerNotFound.ToString());

                if (customer.IsEmailConfirmed)
                    return BaseResponse<bool>.Failure("Email already confirmed");

                if (!_otpService.ValidateOtp(request.OtpCode, customer.EmailConfirmationOtp, customer.EmailConfirmationOtpExpiry ?? DateTime.MinValue))
                    return BaseResponse<bool>.Failure("Invalid OTP code");

                await _customerRepository.ConfirmEmailAsync(customer.Id, cancellationToken);

                await _emailService.SendWelcomeEmailAsync(customer.Email, customer.Profile?.FirstName ?? "Customer", cancellationToken);

                _logger.LogInformation("Email confirmed for customer: {CustomerId}", customer.Id);
                return BaseResponse<bool>.Success(true, "Email confirmed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error confirming email for: {Email}", request.Email);
                return BaseResponse<bool>.Failure($"Error: {ex.Message}");
            }
        }

        public async Task<BaseResponse<bool>> ResendOtpAsync(ResendOtpRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var customer = await _customerRepository.GetByEmailAsync(request.Email, cancellationToken);
                if (customer == null)
                    return BaseResponse<bool>.Failure(IdentityCoreErrors.CustomerNotFound.ToString());

                if (customer.IsEmailConfirmed)
                    return BaseResponse<bool>.Failure("Email already confirmed");

                if (customer.LastOtpSentAt.HasValue &&
                       DateTime.UtcNow.Subtract(customer.LastOtpSentAt.Value).TotalMinutes < _otpSettings.ResendCooldownMinutes)
                    return BaseResponse<bool>.Failure($"Wait {_otpSettings.ResendCooldownMinutes} minute before requesting new code");


                var newOtp = _otpService.GenerateOtp();
                var expiry = _otpService.GetOtpExpiryTime(_otpSettings.ExpiryMinutes);

                await _customerRepository.UpdateOtpAsync(customer.Id, newOtp, expiry, cancellationToken);
                await _emailService.SendOtpEmailAsync(customer.Email, newOtp, cancellationToken);

                _logger.LogInformation("New OTP sent to {Email}", request.Email);
                return BaseResponse<bool>.Success(true, "New code sent");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resending OTP to: {Email}", request.Email);
                return BaseResponse<bool>.Failure($"Error: {ex.Message}");
            }
        }

        public async Task<BaseResponse<bool>> ChangePasswordAsync(ChangePasswordRequest changePasswordRequest, CancellationToken cancellationToken = default)
        {
            try
            {
                var customer = await _customerRepository.GetByIdAsync(changePasswordRequest.CustomerId, cancellationToken);

                if (customer == null)
                    return BaseResponse<bool>.Failure(IdentityCoreErrors.CustomerNotFound.ToString());

                if (!PasswordHelper.VerifyPassword(changePasswordRequest.CurrentPassword, customer.PasswordHash))
                    return BaseResponse<bool>.Failure(IdentityCoreErrors.InvalidPassword.ToString());

                customer.PasswordHash = PasswordHelper.HashPassword(changePasswordRequest.NewPassword);
                await _customerRepository.UpdatePasswordAsync(customer.Id, customer.PasswordHash, cancellationToken);

                _logger.LogInformation("Password changed for customer: {CustomerId}", changePasswordRequest.CustomerId);
                return BaseResponse<bool>.Success(true, SuccessMessages.PasswordChanged);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error authenticating customer: {ex.Message}");
                return BaseResponse<bool>.Failure($"Authentication failed: {ex.Message}");
            }
        }
    }
}
