using PureDelivery.IdentityService.Core.Models;

namespace PureDelivery.IdentityService.Core.Repositories;

public interface ICustomerRatingRepository
{
    Task<bool> HasRatedAsync(Guid customerId, Guid orderId, Guid courierId, CancellationToken ct = default);
    Task<CustomerRating> AddAsync(CustomerRating rating, CancellationToken ct = default);
    Task<List<CustomerRating>> GetByCustomerIdAsync(Guid customerId, int page, int pageSize, CancellationToken ct = default);
    Task<int> GetTotalCountAsync(Guid customerId, CancellationToken ct = default);
}
