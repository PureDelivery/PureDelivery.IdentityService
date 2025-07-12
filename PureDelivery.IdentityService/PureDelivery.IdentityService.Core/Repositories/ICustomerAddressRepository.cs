using PureDelivery.IdentityService.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureDelivery.IdentityService.Core.Repositories
{
    /// <summary>
    /// Интерфейс репозитория для CustomerAddress
    /// </summary>
    public interface ICustomerAddressRepository : IBaseRepository<CustomerAddress>
    {
        Task<IEnumerable<CustomerAddress>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
        Task<CustomerAddress?> GetDefaultAddressAsync(Guid customerId, CancellationToken cancellationToken = default);
        Task<bool> SetDefaultAddressAsync(Guid customerId, Guid addressId, CancellationToken cancellationToken = default);

        Task<CustomerAddress> AddAsync(CustomerAddress entity, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
