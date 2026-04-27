using PureDelivery.IdentityService.Core.DTOs;
using PureDelivery.Shared.Contracts.Domain.Models;

namespace PureDelivery.IdentityService.Core.Services;

public interface ICustomerRatingService
{
    Task<BaseResponse<CustomerRatingDto>> SubmitRatingAsync(Guid customerId, SubmitCustomerRatingRequest request, CancellationToken ct = default);
    Task<BaseResponse<PagedCustomerRatingsDto>> GetRatingsAsync(Guid customerId, int page, int pageSize, CancellationToken ct = default);
}
