using PureDelivery.IdentityService.Core.Models;
using PureDelivery.Shared.Contracts.Domain.Models;
using PureDelivery.Shared.Contracts.DTOs.Identity.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureDelivery.IdentityService.Core.Services
{
    public interface ICustomerProfileService
    {
        Task<BaseResponse<bool>> UpdateProfileAsync(Guid customerId, UpdateProfileRequest profile, CancellationToken cancellationToken = default);
        Task<BaseResponse<bool>> AddLoyaltyPointsAsync(Guid customerId, decimal points, string reason, CancellationToken cancellationToken = default);
        Task<BaseResponse<bool>> SpendLoyaltyPointsAsync(Guid customerId, decimal points, string reason, CancellationToken cancellationToken = default);
        Task<BaseResponse<decimal>> GetLoyaltyPointsBalanceAsync(Guid customerId, CancellationToken cancellationToken = default);
        Task<BaseResponse<bool>> UpdateLastOrderDateAsync(Guid customerId, DateTime orderDate, CancellationToken cancellationToken = default);
        Task<BaseResponse<bool>> GradeUser(Guid customerId, int grade, CancellationToken cancellationToken = default);
        Task<BaseResponse<bool>> UpdateAvatarAsync(Guid customerId, string avatarUrl, CancellationToken cancellationToken = default);
    }
}
