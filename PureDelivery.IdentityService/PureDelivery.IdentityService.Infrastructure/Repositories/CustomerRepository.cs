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
    public class CustomerRepository : BaseRepository<Customer>, ICustomerRepository
    {
        public CustomerRepository(CustomerDbContext context, ILogger<BaseRepository<Customer>> logger) : base(context, logger)
        {
        }

        public async Task<Customer> AddWithProfileAsync(Customer customer, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogDebug("Adding entity {EntityType}", typeof(Customer).Name);

                await _dbSet.AddAsync(customer, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return customer;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding entity {EntityType}", typeof(Customer).Name);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogDebug("Soft deleting customer: {CustomerId}", id);

                var customer = await GetByIdAsync(id, cancellationToken);
                if (customer == null)
                {
                    _logger.LogWarning("Customer {CustomerId} not found for deletion", id);
                    return false;
                }

                customer.IsActive = false;

                _dbSet.Update(customer);
                await _context.SaveChangesAsync(cancellationToken);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting address: {AddressId}", id);
                throw;
            }
        }


        public async Task<Customer?> GetActiveByIdAsync(Guid customerId, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FirstOrDefaultAsync(c => c.Id == customerId && c.IsActive, cancellationToken);
        }

        public async Task<Customer?> GetActiveByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FirstOrDefaultAsync(c => c.Email.ToLower() == email.ToLower() && c.IsActive, cancellationToken);
        }

        public async Task<Customer?> GetWithAddressesAsync(Guid customerId, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogDebug("Getting customer {CustomerId} with addresses", customerId);
                return await _dbSet
                    .Where(c => c.IsActive)
                    .Include(c => c.Addresses.Where(a => a.IsActive))
                    .FirstOrDefaultAsync(c => c.Id == customerId, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting customer {CustomerId} with addresses", customerId);
                throw;
            }
        }

        public async Task<Customer?> GetWithAllDataAsync(Guid customerId, CancellationToken cancellationToken = default)
        {
            var customer = await _dbSet.FirstOrDefaultAsync(c => c.Id == customerId && c.IsActive, cancellationToken);
            if (customer == null) return null;

            await _context.Entry(customer)
                .Reference(c => c.Profile)
                .LoadAsync(cancellationToken);

            await _context.Entry(customer)
                .Collection(c => c.Addresses)
                .Query()
                .Where(a => a.IsActive)
                .LoadAsync(cancellationToken);

            return customer;
        }

        public async Task<Customer?> GetWithProfileAsync(Guid customerId, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogDebug("Getting customer {CustomerId} with profile", customerId);
                return await _dbSet
                    .Include(c => c.Profile)
                    .FirstOrDefaultAsync(c => c.Id == customerId && c.IsActive, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting customer {CustomerId} with profile", customerId);
                throw;
            }
        }

        public async Task<bool> IsEmailUniqueAsync(string email, Guid? excludeCustomerId = null, CancellationToken cancellationToken = default)
        {
            return !await _dbSet.AnyAsync(c =>
                c.IsActive &&
                c.Email.ToLower() == email.ToLower() &&
                (!excludeCustomerId.HasValue || c.Id != excludeCustomerId.Value),
                cancellationToken);
        }

        public async Task<bool> UpdatePasswordAsync(Guid customerId, string password, CancellationToken cancellationToken = default)
        {
            try
            {
                var customer = _dbSet.FirstOrDefault(c => c.Id != customerId && c.IsActive);

                if (customer == null)
                {
                    _logger.LogWarning("Customer {CustomerId} not found for password update", customerId);
                    return false;
                }

                _logger.LogDebug("Updating password for customer {CustomerId}", customerId);
                customer.PasswordHash = password;
                _dbSet.Update(customer);
                await _context.SaveChangesAsync(cancellationToken);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting customer {CustomerId} with profile", customerId);
                throw;
            }
        }

        public async Task<Customer?> GetActiveWithProfileByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _dbSet.Include(c => c.Profile).FirstOrDefaultAsync(c => c.Email.ToLower() == email.ToLower() && c.IsActive, cancellationToken);
        }
    }
}
