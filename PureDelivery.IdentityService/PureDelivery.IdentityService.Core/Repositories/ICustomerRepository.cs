using PureDelivery.IdentityService.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureDelivery.IdentityService.Core.Repositories
{
    /// <summary>
    /// Интерфейс репозитория для Customer
    /// </summary>
    public interface ICustomerRepository : IBaseRepository<Customer>
    {
        Task<Customer?> GetActiveByIdAsync(Guid customerId, CancellationToken cancellationToken = default);
        Task<Customer?> GetActiveByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<Customer?> GetActiveWithProfileByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<Customer?> GetWithProfileAsync(Guid customerId, CancellationToken cancellationToken = default);
        Task<Customer?> GetWithAddressesAsync(Guid customerId, CancellationToken cancellationToken = default);
        Task<Customer?> GetWithAllDataAsync(Guid customerId, CancellationToken cancellationToken = default);
        Task<bool> IsEmailUniqueAsync(string email, Guid? excludeCustomerId = null, CancellationToken cancellationToken = default);
        Task<bool> UpdatePasswordAsync(Guid customerId, string password, CancellationToken cancellationToken = default);
        Task<Customer> AddWithProfileAsync(Customer customer, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> ConfirmEmailAsync(Guid customerId, CancellationToken cancellationToken = default);
        Task<bool> UpdateOtpAsync(Guid customerId, string otp, DateTime expiry, CancellationToken cancellationToken = default);
        Task<Customer?> GetWithRatingsAsync(Guid customerId, CancellationToken cancellationToken = default);
    }
}
