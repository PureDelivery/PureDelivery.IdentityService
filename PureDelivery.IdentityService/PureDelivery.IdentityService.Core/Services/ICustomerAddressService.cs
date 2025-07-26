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
    public interface ICustomerAddressService
    {
        Task<BaseResponse<List<CustomerAddressDto>>> GetCustomerAddressesAsync(Guid customerId, CancellationToken cancellationToken = default);
        Task<BaseResponse<CustomerAddressDto>> GetAddressAsync(Guid addressId, CancellationToken cancellationToken = default);
        Task<BaseResponse<CustomerAddressDto>> GetDefaultAddressAsync(Guid customerId, CancellationToken cancellationToken = default);
        Task<BaseResponse<CustomerAddressDto>> AddAddressAsync(Guid customerId, CreateAddressRequest address, CancellationToken cancellationToken = default);
        Task<BaseResponse<bool>> UpdateAddressAsync(Guid addressId, UpdateAddressRequest updatedAddress, CancellationToken cancellationToken = default);
        Task<BaseResponse<bool>> DeleteAddressAsync(Guid addressId, CancellationToken cancellationToken = default);
        Task<BaseResponse<bool>> SetDefaultAddressAsync(Guid customerId, Guid addressId, CancellationToken cancellationToken = default);
    }
}
