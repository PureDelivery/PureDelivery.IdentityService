using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PureDelivery.IdentityService.Core.Models;
using PureDelivery.IdentityService.Core.Repositories;
using PureDelivery.IdentityService.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureDelivery.IdentityService.Infrastructure.Repositories
{
    public class CustomerAddressRepository : BaseRepository<CustomerAddress>, ICustomerAddressRepository
    {
        public CustomerAddressRepository(CustomerDbContext context, ILogger<CustomerAddressRepository> logger)
            : base(context, logger)
        {
        }

        public async Task<IEnumerable<CustomerAddress>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogDebug("Getting addresses for customer: {CustomerId}", customerId);
                return await _dbSet
                    .Where(a => a.CustomerId == customerId && a.IsActive)
                    .Include(a => a.Customer)
                    .OrderByDescending(a => a.IsDefault)
                    .ThenBy(a => a.Label)
                    .ToListAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting addresses for customer: {CustomerId}", customerId);
                throw;
            }
        }

        public async Task<CustomerAddress?> GetDefaultAddressAsync(Guid customerId, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogDebug("Getting default address for customer: {CustomerId}", customerId);
                return await _dbSet
                    .Include(a => a.Customer)
                    .FirstOrDefaultAsync(a => a.CustomerId == customerId && a.IsDefault && a.IsActive, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting default address for customer: {CustomerId}", customerId);
                throw;
            }
        }

        public async Task<bool> SetDefaultAddressAsync(Guid customerId, Guid addressId, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogDebug("Setting default address {AddressId} for customer: {CustomerId}", addressId, customerId);

                // Сначала убираем флаг default у всех адресов клиента
                var customerAddresses = await _dbSet
                    .Where(a => a.CustomerId == customerId && a.IsActive)
                    .ToListAsync(cancellationToken);

                foreach (var address in customerAddresses)
                {
                    address.IsDefault = false;
                    address.UpdatedAt = DateTime.UtcNow;
                }

                // Устанавливаем новый default адрес
                var targetAddress = customerAddresses.FirstOrDefault(a => a.Id == addressId);
                if (targetAddress == null)
                {
                    _logger.LogWarning("Address {AddressId} not found for customer: {CustomerId}", addressId, customerId);
                    return false;
                }

                targetAddress.IsDefault = true;
                targetAddress.UpdatedAt = DateTime.UtcNow;

                _dbSet.UpdateRange(customerAddresses);
                await _context.SaveChangesAsync(cancellationToken);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting default address {AddressId} for customer: {CustomerId}", addressId, customerId);
                throw;
            }
        }

        public async Task<CustomerAddress> AddAsync(CustomerAddress entity, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Adding address for customer: {CustomerId}", entity.CustomerId);

                // Устанавливаем дефолтные значения
                entity.Id = Guid.NewGuid();
                entity.CreatedAt = DateTime.UtcNow;
                entity.UpdatedAt = DateTime.UtcNow;
                entity.IsActive = true;

                // Если это первый адрес клиента, делаем его дефолтным
                var existingAddressesCount = await _dbSet
                    .CountAsync(a => a.CustomerId == entity.CustomerId && a.IsActive, cancellationToken);

                if (existingAddressesCount == 0)
                {
                    entity.IsDefault = true;
                    _logger.LogDebug("Setting first address as default for customer: {CustomerId}", entity.CustomerId);
                }

                _dbSet.Add(entity);
                await _context.SaveChangesAsync(cancellationToken);
                return entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding address for customer: {CustomerId}", entity.CustomerId);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogDebug("Soft deleting address: {AddressId}", id);

                var address = await GetByIdAsync(id, cancellationToken);
                if (address == null)
                {
                    _logger.LogWarning("Address {AddressId} not found for deletion", id);
                    return false;
                }

                // Мягкое удаление - просто деактивируем
                address.IsActive = false;
                address.UpdatedAt = DateTime.UtcNow;

                // Если удаляется дефолтный адрес, назначаем дефолтным другой адрес
                if (address.IsDefault)
                {
                    var nextDefaultAddress = await _dbSet
                        .Where(a => a.CustomerId == address.CustomerId && a.IsActive && a.Id != id)
                        .FirstOrDefaultAsync(cancellationToken);

                    if (nextDefaultAddress != null)
                    {
                        nextDefaultAddress.IsDefault = true;
                        nextDefaultAddress.UpdatedAt = DateTime.UtcNow;
                        _dbSet.Update(nextDefaultAddress);
                        _logger.LogDebug("Set new default address {NewDefaultId} for customer: {CustomerId}",
                            nextDefaultAddress.Id, address.CustomerId);
                    }
                }

                _dbSet.Update(address);
                await _context.SaveChangesAsync(cancellationToken);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting address: {AddressId}", id);
                throw;
            }
        }

        public override async Task<IEnumerable<CustomerAddress>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogDebug("Getting all active addresses");
                return await _dbSet
                    .Where(a => a.IsActive)
                    .Include(a => a.Customer)
                    .OrderBy(a => a.CustomerId)
                    .ThenByDescending(a => a.IsDefault)
                    .ToListAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all addresses");
                throw;
            }
        }
    }
}
