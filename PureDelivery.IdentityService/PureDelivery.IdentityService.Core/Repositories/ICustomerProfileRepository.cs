using PureDelivery.IdentityService.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureDelivery.IdentityService.Core.Repositories
{

    /// <summary>
    /// Интерфейс репозитория для CustomerProfile
    /// </summary>
    public interface ICustomerProfileRepository
    {
        Task<CustomerProfile?> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
        Task<bool> UpdateLoyaltyPointsAsync(Guid customerId, decimal points, CancellationToken cancellationToken = default);
        Task<bool> UpdateLastOrderDateAsync(Guid customerId, DateTime orderDate, CancellationToken cancellationToken = default);
        Task<CustomerProfile> UpdateAsync(CustomerProfile profile, CancellationToken cancellationToken = default);

        Task<bool> AddCustomerRatingGradeAsync(Guid customerId, Guid orderId, Guid courierId, int grade, string? comment = null, CancellationToken cancellationToken = default);

    }
}
