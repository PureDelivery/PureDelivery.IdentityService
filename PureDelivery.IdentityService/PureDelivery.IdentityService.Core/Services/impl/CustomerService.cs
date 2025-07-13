using Microsoft.Extensions.Logging;
using PureDelivery.IdentityService.Core.Helpers;
using PureDelivery.IdentityService.Core.Mappers;
using PureDelivery.IdentityService.Core.Models;
using PureDelivery.IdentityService.Core.Repositories;
using PureDelivery.IdentityService.Core.ResponseConstants;
using PureDelivery.IdentityService.Core.ResponseConstants.Enums;
using PureDelivery.Shared.Contracts.Common.Services;
using PureDelivery.Shared.Contracts.Domain.Models;
using PureDelivery.Shared.Contracts.DTOs.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public CustomerService(
            ICustomerRepository customerRepository,
            ILogger<CustomerService> logger,
            ISessionService sessionService)
        {
            _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
        }




        // TODO: Add 2 more services

        // Create DTOs needed for all 3 services and corresponding mappers for them



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

        public async Task<BaseResponse<AuthDto>> AuthenticateAsync(string email, string password, CancellationToken cancellationToken = default)
        {
            try
            {
                var customer = await _customerRepository.GetActiveByEmailAsync(email, cancellationToken);

                if (customer == null)
                    return BaseResponse<AuthDto>.Failure(IdentityCoreErrors.CustomerNotFound.ToString());

                if (!PasswordHelper.VerifyPassword(password, customer.PasswordHash))
                    return BaseResponse<AuthDto>.Failure(IdentityCoreErrors.InvalidCredentials.ToString());


                var customerSessionDto = customer.ToSessionDto();
                var sessionId = await _sessionService.AddCustomerSessionDataAsync(customer.Id.ToString(), customerSessionDto);

                _logger.LogInformation("Customer authenticated and session created: {CustomerId}, SessionId: {SessionId}", customer.Id, sessionId);

                var authDto = customer.ToAuthDto(sessionId);

                _logger.LogInformation("Customer authenticated: {CustomerId}", customer.Id);
                return BaseResponse<AuthDto>.Success(authDto, SuccessMessages.AuthenticationSuccessful);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error authenticating customer with email: {Email}", email);
                return BaseResponse<AuthDto>.Failure($"Authentication failed: {ex.Message}");
            }
        }

        public async Task<BaseResponse<bool>> ChangePasswordAsync(Guid customerId, string currentPassword, string newPassword, CancellationToken cancellationToken = default)
        {
            try
            {
                var customer = await _customerRepository.GetByIdAsync(customerId, cancellationToken);

                if (customer == null)
                    return BaseResponse<bool>.Failure(IdentityCoreErrors.CustomerNotFound.ToString());

                if (!PasswordHelper.VerifyPassword(currentPassword, customer.PasswordHash))
                    return BaseResponse<bool>.Failure(IdentityCoreErrors.InvalidPassword.ToString());

                customer.PasswordHash = PasswordHelper.HashPassword(newPassword);
                await _customerRepository.UpdatePasswordAsync(customer.Id, customer.PasswordHash, cancellationToken);

                _logger.LogInformation("Password changed for customer: {CustomerId}", customerId);
                return BaseResponse<bool>.Success(true, SuccessMessages.PasswordChanged);   
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error authenticating customer: {ex.Message}");
                return BaseResponse<bool>.Failure($"Authentication failed: {ex.Message}");
            }
        }

        public async Task<BaseResponse<CreateCustomerResultDto>> CreateCustomerAsync(string email, string password, CustomerProfile profile, CancellationToken cancellationToken = default)
        {
            try
            {
                if (!await _customerRepository.IsEmailUniqueAsync(email, cancellationToken: cancellationToken))
                    return BaseResponse<CreateCustomerResultDto>.Failure(IdentityCoreErrors.EmailAlreadyExists.ToString());

                var customer = CreateCustomer(email, password, profile);

                _logger.LogInformation("Creating customer with email: {Email}", email);
                var createdCustomer = await _customerRepository.AddWithProfileAsync(customer, customer.Profile, cancellationToken);

                return BaseResponse<CreateCustomerResultDto>.Success(createdCustomer.ToCreateResultDto(), SuccessMessages.CustomerCreated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating customer with email: {Email}", email);
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


        private static Customer CreateCustomer(string email, string password, CustomerProfile customerProfile)
        {
            return new Customer
            {
                Id = Guid.NewGuid(),
                Email = email.ToLower(),
                PasswordHash = PasswordHelper.HashPassword(password),
                CreatedAt = DateTime.UtcNow,
                Profile = customerProfile ?? new CustomerProfile()
            };
        }
    }
}
