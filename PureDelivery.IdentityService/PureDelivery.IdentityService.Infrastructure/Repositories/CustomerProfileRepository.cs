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
    public class CustomerProfileRepository : BaseRepository<CustomerProfile>, ICustomerProfileRepository
    {
        public CustomerProfileRepository(CustomerDbContext context, ILogger<CustomerProfileRepository> logger)
            : base(context, logger)
        {
        }

        public async Task<CustomerProfile?> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogDebug("Getting customer profile for customer: {CustomerId}", customerId);
                return await _dbSet
                    .Include(p => p.Customer)
                    .FirstOrDefaultAsync(p => p.CustomerId == customerId, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting customer profile for customer: {CustomerId}", customerId);
                throw;
            }
        }



        public async Task<bool> UpdateLastOrderDateAsync(Guid customerId, DateTime orderDate, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogDebug("Updating last order date for customer {CustomerId} to {OrderDate}", customerId, orderDate);

                var profile = await GetByCustomerIdAsync(customerId, cancellationToken);
                if (profile == null)
                {
                    _logger.LogWarning("Customer profile not found for customer: {CustomerId}", customerId);
                    return false;
                }

                profile.LastOrderDate = orderDate;
                profile.UpdatedAt = DateTime.UtcNow;

                _dbSet.Update(profile);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating last order date for customer: {CustomerId}", customerId);
                throw;
            }
        }

        public async Task<bool> UpdateLoyaltyPointsAsync(Guid customerId, decimal points, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogDebug("Updating loyalty points for customer {CustomerId} to {Points}", customerId, points);

                var profile = await GetByCustomerIdAsync(customerId, cancellationToken);
                if (profile == null)
                {
                    _logger.LogWarning("Customer profile not found for customer: {CustomerId}", customerId);
                    return false;
                }

                profile.LoyaltyPoints = points;
                profile.UpdatedAt = DateTime.UtcNow;

                _dbSet.Update(profile);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating loyalty points for customer: {CustomerId}", customerId);
                throw;
            }
        }
    }
}
