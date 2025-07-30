using PureDelivery.IdentityService.Core.Models;
using PureDelivery.Shared.Contracts.Domain.Models;
using PureDelivery.Shared.Contracts.DTOs.Identity;
using PureDelivery.Shared.Contracts.DTOs.Identity.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureDelivery.IdentityService.Core.Services
{
    public interface ICustomerService
    {
        Task<BaseResponse<CreateCustomerResultDto>> CreateCustomerAsync(CreateCustomerRequest customerCreate, CancellationToken cancellationToken = default);
        Task<BaseResponse<CustomerDetailDto>> GetCustomerFullDataAsync(Guid customerId, CancellationToken cancellationToken = default);
        Task<BaseResponse<AuthDto>> AuthenticateAsync(AuthenticateRequest authenticateRequest, CancellationToken cancellationToken = default);
        Task<BaseResponse<bool>> DeleteCustomerAsync(Guid customerId, CancellationToken cancellationToken = default);
        Task<BaseResponse<bool>> IsEmailAvailableAsync(string email, CancellationToken cancellationToken = default);
        Task<BaseResponse<CustomerSummaryDto>> GetCustomerSummary(Guid customerId, CancellationToken cancellationToken = default);

        Task<BaseResponse<CustomerLoyaltyDto>> GetCustomerLoyaltyAsync(Guid customerId, CancellationToken cancellationToken = default);
        Task<BaseResponse<CustomerProfileInfoDto>> GetCustomerProfileInfoAsync(Guid customerId, CancellationToken cancellationToken = default);

        Task<BaseResponse<bool>> LogoutAsync(string sessionId, CancellationToken cancellationToken = default);
        Task<BaseResponse<bool>> ConfirmEmailAsync(ConfirmEmailRequest request, CancellationToken cancellationToken = default);
        Task<BaseResponse<bool>> ResendOtpAsync(ResendOtpRequest request, CancellationToken cancellationToken = default);

        Task<BaseResponse<bool>> RequestForgotPasswordAsync(string email, CancellationToken cancellationToken = default);
        Task<BaseResponse<bool>> ChangePasswordWithOtpAsync(ChangePasswordWithOtpRequest request, CancellationToken cancellationToken = default);

        Task<BaseResponse<bool>> ChangePasswordAsync(ChangePasswordRequest request, CancellationToken cancellationToken = default);
    }
}
