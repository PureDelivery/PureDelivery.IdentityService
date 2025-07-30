using Microsoft.AspNetCore.Mvc;
using PureDelivery.IdentityService.Core.Models;
using PureDelivery.IdentityService.Core.Services;
using PureDelivery.Shared.Contracts.Domain.Models;
using PureDelivery.Shared.Contracts.DTOs.Identity;
using PureDelivery.Shared.Contracts.DTOs.Identity.Requests;
using System.ComponentModel.DataAnnotations;

namespace PureDelivery.IdentityService.Controllers
{
    [ApiController]
    [Route("api/v1/identity/[controller]")]
    [Produces("application/json")]
    public class CustomerAddressController : ControllerBase
    {
        private readonly ICustomerAddressService _addressService;
        private readonly ILogger<CustomerAddressController> _logger;

        public CustomerAddressController(
            ICustomerAddressService addressService,
            ILogger<CustomerAddressController> logger)
        {
            _addressService = addressService;
            _logger = logger;
        }

        /// <summary>
        /// Получение всех адресов клиента
        /// </summary>
        /// <param name="customerId">ID клиента</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Список адресов клиента</returns>
        [HttpGet("customer/{customerId:guid}")]
        [ProducesResponseType(typeof(BaseResponse<List<CustomerAddressDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<List<CustomerAddressDto>>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BaseResponse<List<CustomerAddressDto>>>> GetCustomerAddresses(
            [FromRoute] Guid customerId,
            CancellationToken cancellationToken = default)
        {
            var result = await _addressService.GetCustomerAddressesAsync(customerId, cancellationToken);

            if (!result.IsSuccess)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Получение адреса по ID
        /// </summary>
        /// <param name="addressId">ID адреса</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Информация об адресе</returns>
        [HttpGet("{addressId:guid}")]
        [ProducesResponseType(typeof(BaseResponse<CustomerAddressDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<CustomerAddressDto>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BaseResponse<CustomerAddressDto>>> GetAddress(
            [FromRoute] Guid addressId,
            CancellationToken cancellationToken = default)
        {
            var result = await _addressService.GetAddressAsync(addressId, cancellationToken);

            if (!result.IsSuccess)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Получение адреса по умолчанию для клиента
        /// </summary>
        /// <param name="customerId">ID клиента</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Адрес по умолчанию</returns>
        [HttpGet("customer/{customerId:guid}/default")]
        [ProducesResponseType(typeof(BaseResponse<CustomerAddressDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<CustomerAddressDto>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BaseResponse<CustomerAddress>>> GetDefaultAddress(
            [FromRoute] Guid customerId,
            CancellationToken cancellationToken = default)
        {
            var result = await _addressService.GetDefaultAddressAsync(customerId, cancellationToken);

            if (!result.IsSuccess)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Добавление нового адреса для клиента
        /// </summary>
        /// <param name="customerId">ID клиента</param>
        /// <param name="address">Данные нового адреса</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Созданный адрес</returns>
        [HttpPost("customer/{customerId:guid}")]
        [ProducesResponseType(typeof(BaseResponse<CustomerAddressDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(BaseResponse<CustomerAddressDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<CustomerAddressDto>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BaseResponse<CustomerAddress>>> AddAddress(
            [FromRoute] Guid customerId,
            [FromBody] CreateAddressRequest address,
            CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(BaseResponse<CustomerAddress>.Failure("Invalid request data"));
            }

            var result = await _addressService.AddAddressAsync(customerId, address, cancellationToken);

            if (!result.IsSuccess)
            {
                if (result.Message?.Contains("not found") == true)
                {
                    return NotFound(result);
                }
                return BadRequest(result);
            }

            return CreatedAtAction(nameof(GetAddress), new { addressId = result.Data!.Id }, result);
        }

        /// <summary>
        /// Обновление существующего адреса
        /// </summary>
        /// <param name="addressId">ID адреса</param>
        /// <param name="address">Обновленные данные адреса</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Результат обновления</returns>
        [HttpPut("{addressId:guid}")]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BaseResponse<bool>>> UpdateAddress(
            [FromRoute] Guid addressId,
            [FromBody] UpdateAddressRequest address,
            CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(BaseResponse<bool>.Failure("Invalid request data"));
            }

            var result = await _addressService.UpdateAddressAsync(addressId, address, cancellationToken);

            if (!result.IsSuccess)
            {
                if (result.Message?.Contains("not found") == true)
                {
                    return NotFound(result);
                }
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Удаление адреса
        /// </summary>
        /// <param name="addressId">ID адреса</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Результат удаления</returns>
        [HttpDelete("{addressId:guid}")]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BaseResponse<bool>>> DeleteAddress(
            [FromRoute] Guid addressId,
            CancellationToken cancellationToken = default)
        {
            var result = await _addressService.DeleteAddressAsync(addressId, cancellationToken);

            if (!result.IsSuccess)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Установка адреса по умолчанию
        /// </summary>
        /// <param name="customerId">ID клиента</param>
        /// <param name="addressId">ID адреса</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Результат установки</returns>
        [HttpPut("customer/{customerId:guid}/default/{addressId:guid}")]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BaseResponse<bool>>> SetDefaultAddress(
            [FromRoute] Guid customerId,
            [FromRoute] Guid addressId,
            CancellationToken cancellationToken = default)
        {
            var result = await _addressService.SetDefaultAddressAsync(customerId, addressId, cancellationToken);

            if (!result.IsSuccess)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
    }
}