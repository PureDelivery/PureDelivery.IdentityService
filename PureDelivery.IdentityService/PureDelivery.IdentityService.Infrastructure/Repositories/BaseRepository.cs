using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PureDelivery.IdentityService.Core.Repositories;
using PureDelivery.IdentityService.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PureDelivery.IdentityService.Infrastructure.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        protected readonly CustomerDbContext _context;
        protected readonly DbSet<T> _dbSet;
        protected readonly ILogger<BaseRepository<T>> _logger;

        public BaseRepository(CustomerDbContext context, ILogger<BaseRepository<T>> logger)
        {
            _context = context;
            _logger = logger;
            _dbSet = _context.Set<T>();
        }

        public virtual async Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogDebug("Counting entities of type {EntityType}", typeof(T).Name);
                return await _dbSet.CountAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting entities of type {EntityType}", typeof(T).Name);
                throw;
            }
        }

        public virtual async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogDebug("Checking if entity {EntityType} with id {Id} exists", typeof(T).Name, id);
                return await _dbSet.FindAsync(new object[] { id }, cancellationToken) != null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking existence of entity {EntityType} with id {Id}", typeof(T).Name, id);
                throw;
            }
        }

        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogDebug("Finding entities {EntityType} with predicate", typeof(T).Name);
                return await _dbSet.Where(predicate).ToListAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error finding entities {EntityType} with predicate", typeof(T).Name);
                throw;
            }
        }

        public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogDebug("Getting first entity {EntityType} with predicate", typeof(T).Name);
                return await _dbSet.FirstOrDefaultAsync(predicate, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting first entity {EntityType} with predicate", typeof(T).Name);
                throw;
            }
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogDebug("Getting all entities of type {EntityType}", typeof(T).Name);
                return await _dbSet.ToListAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all entities of type {EntityType}", typeof(T).Name);
                throw;
            }
        }

        public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogDebug("Getting entity {EntityType} with id {Id}", typeof(T).Name, id);
                return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting entity {EntityType} with id {Id}", typeof(T).Name, id);
                throw;
            }
        }
    }
}
