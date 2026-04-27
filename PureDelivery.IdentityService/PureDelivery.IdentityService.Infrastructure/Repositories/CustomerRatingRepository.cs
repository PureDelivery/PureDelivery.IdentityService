using Microsoft.EntityFrameworkCore;
using PureDelivery.IdentityService.Core.Models;
using PureDelivery.IdentityService.Core.Repositories;
using PureDelivery.IdentityService.Infrastructure.Data;

namespace PureDelivery.IdentityService.Infrastructure.Repositories;

public class CustomerRatingRepository(CustomerDbContext context) : ICustomerRatingRepository
{
    public async Task<bool> HasRatedAsync(Guid customerId, Guid orderId, Guid courierId, CancellationToken ct = default) =>
        await context.Set<CustomerRating>().AnyAsync(
            r => r.CustomerId == customerId && r.OrderId == orderId && r.CourierId == courierId, ct);

    public async Task<CustomerRating> AddAsync(CustomerRating rating, CancellationToken ct = default)
    {
        context.Set<CustomerRating>().Add(rating);
        // CustomerProfileCustomerId is a shadow FK — must be set explicitly.
        // The profile PK is CustomerId, so FK value == CustomerId of rated customer.
        context.Entry(rating).Property("CustomerProfileCustomerId").CurrentValue = rating.CustomerId;
        await context.SaveChangesAsync(ct);
        return rating;
    }

    public async Task<List<CustomerRating>> GetByCustomerIdAsync(
        Guid customerId, int page, int pageSize, CancellationToken ct = default) =>
        await context.Set<CustomerRating>()
            .Where(r => r.CustomerId == customerId)
            .OrderByDescending(r => r.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

    public async Task<int> GetTotalCountAsync(Guid customerId, CancellationToken ct = default) =>
        await context.Set<CustomerRating>().CountAsync(r => r.CustomerId == customerId, ct);
}
