using Microsoft.Extensions.Logging;
using PureDelivery.IdentityService.Core.Factories;
using PureDelivery.IdentityService.Core.Mappers;
using PureDelivery.IdentityService.Core.Models;
using PureDelivery.IdentityService.Core.Repositories;
using PureDelivery.IdentityService.Core.ResponseConstants;
using PureDelivery.IdentityService.Core.ResponseConstants.Enums;
using PureDelivery.IdentityService.Core.Services;
using PureDelivery.Shared.Contracts.Domain.Models;
using PureDelivery.Shared.Contracts.DTOs.Identity;
using PureDelivery.Shared.Contracts.DTOs.Identity.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureDelivery.IdentityService.Core.Services.impl
{
    public class CustomerAddressService : ICustomerAddressService
    {
        private readonly ILogger<CustomerAddressService> _logger;
        private readonly ICustomerAddressRepository _addressRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly ICustomerAddressFactory _customerAddressFactory;

        public CustomerAddressService(
            ICustomerAddressRepository addressRepository,
            ICustomerRepository customerRepository,
            ILogger<CustomerAddressService> logger,
            ICustomerAddressFactory customerAddressFactory)
        {
            _addressRepository = addressRepository ?? throw new ArgumentNullException(nameof(addressRepository));
            _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _customerAddressFactory = customerAddressFactory ?? throw new ArgumentNullException(nameof(customerAddressFactory));
        }

        public async Task<BaseResponse<List<CustomerAddressDto>>> GetCustomerAddressesAsync(Guid customerId, CancellationToken cancellationToken = default)
        {
            try
            {
                var customer = await _customerRepository.GetActiveByIdAsync(customerId, cancellationToken);
                if (customer == null)
                    return BaseResponse<List<CustomerAddressDto>>.Failure(IdentityCoreErrors.CustomerNotFound.ToString());

                var addresses = await _addressRepository.GetByCustomerIdAsync(customerId, cancellationToken);

                _logger.LogInformation("Retrieved {Count} addresses for customer: {CustomerId}", addresses.Count(), customerId);
                return BaseResponse<List<CustomerAddressDto>>.Success(addresses.Select(x => x.ToDto()).ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting addresses for customer: {CustomerId}", customerId);
                return BaseResponse<List<CustomerAddressDto>>.Failure($"Error getting addresses: {ex.Message}");
            }
        }

        public async Task<BaseResponse<CustomerAddressDto>> GetAddressAsync(Guid addressId, CancellationToken cancellationToken = default)
        {
            try
            {
                var address = await _addressRepository.GetByIdAsync(addressId, cancellationToken);
                if (address == null)
                    return BaseResponse<CustomerAddressDto>.Failure(IdentityCoreErrors.AddressNotFound.ToString());

                return BaseResponse<CustomerAddressDto>.Success(address.ToDto());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting address: {AddressId}", addressId);
                return BaseResponse<CustomerAddressDto>.Failure($"Error getting address: {ex.Message}");
            }
        }

        public async Task<BaseResponse<CustomerAddressDto>> GetDefaultAddressAsync(Guid customerId, CancellationToken cancellationToken = default)
        {
            try
            {
                var customer = await _customerRepository.GetActiveByIdAsync(customerId, cancellationToken);
                if (customer == null)
                    return BaseResponse<CustomerAddressDto>.Failure(IdentityCoreErrors.CustomerNotFound.ToString());

                var defaultAddress = await _addressRepository.GetDefaultAddressAsync(customerId, cancellationToken);
                if (defaultAddress == null)
                    return BaseResponse<CustomerAddressDto>.Failure(IdentityCoreErrors.DefaultAddressNotFound.ToString());

                return BaseResponse<CustomerAddressDto>.Success(defaultAddress.ToDto());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting default address for customer: {CustomerId}", customerId);
                return BaseResponse<CustomerAddressDto>.Failure($"Error getting default address: {ex.Message}");
            }
        }

        public async Task<BaseResponse<CustomerAddressDto>> AddAddressAsync(Guid customerId, CreateAddressRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var customer = await _customerRepository.GetActiveByIdAsync(customerId, cancellationToken);
                if (customer == null)
                    return BaseResponse<CustomerAddressDto>.Failure(IdentityCoreErrors.CustomerNotFound.ToString());

                // Проверяем, является ли это первым адресом
                var existingAddresses = await _addressRepository.GetByCustomerIdAsync(customerId, cancellationToken);
                bool isFirstAddress = !existingAddresses.Any();

                // Используем фабрику для создания нового адреса
                var newAddress = _customerAddressFactory.CreateNewAddress(customerId, request, isFirstAddress);

                var createdAddress = await _addressRepository.AddAsync(newAddress, cancellationToken);

                _logger.LogInformation("Address added for customer: {CustomerId}, AddressId: {AddressId}", customerId, createdAddress.Id);
                return BaseResponse<CustomerAddressDto>.Success(createdAddress.ToDto(), SuccessMessages.AddressAdded);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding address for customer: {CustomerId}", customerId);
                return BaseResponse<CustomerAddressDto>.Failure($"Error adding address: {ex.Message}");
            }
        }

        public async Task<BaseResponse<bool>> UpdateAddressAsync(Guid addressId, UpdateAddressRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var existingAddress = await _addressRepository.GetByIdAsync(addressId, cancellationToken);
                if (existingAddress == null)
                    return BaseResponse<bool>.Failure(IdentityCoreErrors.AddressNotFound.ToString());

                var updatedAddress = _customerAddressFactory.CreateUpdatedAddress(existingAddress, request);

                await _addressRepository.UpdateAsync(updatedAddress, cancellationToken);

                _logger.LogInformation("Address updated: {AddressId}", addressId);
                return BaseResponse<bool>.Success(true, SuccessMessages.AddressUpdated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating address: {AddressId}", addressId);
                return BaseResponse<bool>.Failure($"Error updating address: {ex.Message}");
            }
        }

        public async Task<BaseResponse<bool>> DeleteAddressAsync(Guid addressId, CancellationToken cancellationToken = default)
        {
            try
            {
                var address = await _addressRepository.GetByIdAsync(addressId, cancellationToken);
                if (address == null)
                    return BaseResponse<bool>.Failure(IdentityCoreErrors.AddressNotFound.ToString());

                // Change the logic
                if (address.IsDefault)
                {
                    var customerAddresses = await _addressRepository.GetByCustomerIdAsync(address.CustomerId, cancellationToken);
                    if (customerAddresses.Count() > 1)
                    {
                        return BaseResponse<bool>.Failure(IdentityCoreErrors.CannotDeleteDefaultAddress.ToString());
                    }
                }

                await _addressRepository.DeleteAsync(addressId, cancellationToken);

                _logger.LogInformation("Address deleted: {AddressId}", addressId);
                return BaseResponse<bool>.Success(true, SuccessMessages.AddressDeleted);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting address: {AddressId}", addressId);
                return BaseResponse<bool>.Failure($"Error deleting address: {ex.Message}");
            }
        }

        public async Task<BaseResponse<bool>> SetDefaultAddressAsync(Guid customerId, Guid addressId, CancellationToken cancellationToken = default)
        {
            try
            {
                var customer = await _customerRepository.GetActiveByIdAsync(customerId, cancellationToken);
                if (customer == null)
                    return BaseResponse<bool>.Failure(IdentityCoreErrors.CustomerNotFound.ToString());

                var address = await _addressRepository.GetByIdAsync(addressId, cancellationToken);
                if (address == null || address.CustomerId != customerId)
                    return BaseResponse<bool>.Failure(IdentityCoreErrors.AddressNotFound.ToString());

                var result = await _addressRepository.SetDefaultAddressAsync(customerId, addressId, cancellationToken);
                if (!result)
                    return BaseResponse<bool>.Failure("Failed to set default address");

                _logger.LogInformation("Default address set for customer: {CustomerId}, AddressId: {AddressId}", customerId, addressId);
                return BaseResponse<bool>.Success(true, SuccessMessages.DefaultAddressSet);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting default address for customer: {CustomerId}, AddressId: {AddressId}", customerId, addressId);
                return BaseResponse<bool>.Failure($"Error setting default address: {ex.Message}");
            }
        }
    }
}