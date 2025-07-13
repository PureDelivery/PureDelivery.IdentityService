using PureDelivery.IdentityService.Core.Models;
using PureDelivery.Shared.Contracts.Domain.Models;
using PureDelivery.Shared.Contracts.DTOs.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureDelivery.IdentityService.Core.Services
{
    public interface ICustomerService
    {
        Task<BaseResponse<CreateCustomerResultDto>> CreateCustomerAsync(string email, string password, CustomerProfile profile, CancellationToken cancellationToken = default);
        Task<BaseResponse<CustomerSummaryDto>> GetCustomerByIdAsync(Guid customerId, CancellationToken cancellationToken = default);
        Task<BaseResponse<CustomerSummaryDto>> GetCustomerByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<BaseResponse<CustomerWithProfileDto>> GetCustomerWithProfileAsync(Guid customerId, CancellationToken cancellationToken = default);
        Task<BaseResponse<CustomerWithAddressesDto>> GetCustomerWithAddressesAsync(Guid customerId, CancellationToken cancellationToken = default);
        Task<BaseResponse<CustomerDetailDto>> GetCustomerFullDataAsync(Guid customerId, CancellationToken cancellationToken = default);
        Task<BaseResponse<List<CustomerSummaryDto>>> GetActiveCustomersAsync(CancellationToken cancellationToken = default);
        Task<BaseResponse<AuthDto>> AuthenticateAsync(string email, string password, CancellationToken cancellationToken = default);
        Task<BaseResponse<bool>> ChangePasswordAsync(Guid customerId, string currentPassword, string newPassword, CancellationToken cancellationToken = default);
        Task<BaseResponse<bool>> DeleteCustomerAsync(Guid customerId, CancellationToken cancellationToken = default);
        Task<BaseResponse<bool>> IsEmailAvailableAsync(string email, CancellationToken cancellationToken = default);
    }
}
