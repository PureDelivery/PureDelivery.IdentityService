using Microsoft.Extensions.Logging;
using PureDelivery.IdentityService.Core.DTOs;
using PureDelivery.IdentityService.Core.Models;
using PureDelivery.IdentityService.Core.Repositories;
using PureDelivery.Shared.Contracts.Domain.Models;

namespace PureDelivery.IdentityService.Core.Services.impl;

public class CustomerRatingService(
    ICustomerRatingRepository ratingRepo,
    ICustomerProfileRepository profileRepo,
    ILogger<CustomerRatingService> logger) : ICustomerRatingService
{
    public async Task<BaseResponse<CustomerRatingDto>> SubmitRatingAsync(
        Guid customerId, SubmitCustomerRatingRequest request, CancellationToken ct = default)
    {
        try
        {
            var profile = await profileRepo.GetByCustomerIdAsync(customerId, ct);
            if (profile == null)
                return BaseResponse<CustomerRatingDto>.Failure("Customer not found.");

            if (await ratingRepo.HasRatedAsync(customerId, request.OrderId, request.CourierId, ct))
                return BaseResponse<CustomerRatingDto>.Failure("You have already rated this customer for this order.");

            var rating = new CustomerRating
            {
                Id        = Guid.NewGuid(),
                CustomerId = customerId,
                OrderId    = request.OrderId,
                CourierId  = request.CourierId,
                Rating     = request.Rating,
                Comment    = request.Comment?.Trim(),
                CreatedAt  = DateTime.UtcNow,
            };

            await ratingRepo.AddAsync(rating, ct);

            logger.LogInformation("Rating {Rating} submitted for customer {CustomerId} on order {OrderId}",
                request.Rating, customerId, request.OrderId);

            return BaseResponse<CustomerRatingDto>.Success(new CustomerRatingDto
            {
                Id         = rating.Id,
                CustomerId = rating.CustomerId,
                OrderId    = rating.OrderId,
                CourierId  = rating.CourierId,
                Rating     = rating.Rating,
                Comment    = rating.Comment,
                CreatedAt  = rating.CreatedAt,
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error submitting rating for customer {CustomerId}", customerId);
            return BaseResponse<CustomerRatingDto>.Failure($"Error: {ex.Message}");
        }
    }

    public async Task<BaseResponse<PagedCustomerRatingsDto>> GetRatingsAsync(
        Guid customerId, int page, int pageSize, CancellationToken ct = default)
    {
        try
        {
            var ratings = await ratingRepo.GetByCustomerIdAsync(customerId, page, pageSize, ct);
            var total   = await ratingRepo.GetTotalCountAsync(customerId, ct);
            var avg     = total > 0 ? ratings.Average(r => (double)r.Rating) : 0.0;

            return BaseResponse<PagedCustomerRatingsDto>.Success(new PagedCustomerRatingsDto
            {
                Items = ratings.Select(r => new CustomerRatingDto
                {
                    Id         = r.Id,
                    CustomerId = r.CustomerId,
                    OrderId    = r.OrderId,
                    CourierId  = r.CourierId,
                    Rating     = r.Rating,
                    Comment    = r.Comment,
                    CreatedAt  = r.CreatedAt,
                }).ToList(),
                TotalCount    = total,
                Page          = page,
                PageSize      = pageSize,
                AverageRating = Math.Round(avg, 2),
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting ratings for customer {CustomerId}", customerId);
            return BaseResponse<PagedCustomerRatingsDto>.Failure($"Error: {ex.Message}");
        }
    }
}
