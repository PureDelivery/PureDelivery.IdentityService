using PureDelivery.IdentityService.Core.Models;
using PureDelivery.Shared.Contracts.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureDelivery.IdentityService.Core.Services
{
    public interface ICustomerAddressService
    {
        Task<BaseResponse<List<CustomerAddress>>> GetCustomerAddressesAsync(Guid customerId, CancellationToken cancellationToken = default);
        Task<BaseResponse<CustomerAddress>> GetAddressAsync(Guid addressId, CancellationToken cancellationToken = default);
        Task<BaseResponse<CustomerAddress>> GetDefaultAddressAsync(Guid customerId, CancellationToken cancellationToken = default);
        Task<BaseResponse<CustomerAddress>> AddAddressAsync(Guid customerId, CustomerAddress address, CancellationToken cancellationToken = default);
        Task<BaseResponse<bool>> UpdateAddressAsync(Guid addressId, CustomerAddress updatedAddress, CancellationToken cancellationToken = default);
        Task<BaseResponse<bool>> DeleteAddressAsync(Guid addressId, CancellationToken cancellationToken = default);
        Task<BaseResponse<bool>> SetDefaultAddressAsync(Guid customerId, Guid addressId, CancellationToken cancellationToken = default);
        Task<BaseResponse<bool>> ValidateAddressAsync(CustomerAddress address, CancellationToken cancellationToken = default);
        Task<BaseResponse<int>> GetAddressCountAsync(Guid customerId, CancellationToken cancellationToken = default);
    }
}
