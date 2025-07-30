using Microsoft.AspNetCore.Mvc;
using PureDelivery.IdentityService.Core.Services;
using PureDelivery.Shared.Contracts.Domain.Models;
using PureDelivery.Shared.Contracts.DTOs.Identity;
using System.ComponentModel.DataAnnotations;

namespace PureDelivery.IdentityService.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly ILogger<CustomerController> _logger;

        public CustomerController(
            ICustomerService customerService,
            ILogger<CustomerController> logger)
        {
            _customerService = customerService;
            _logger = logger;
        }

        /// <summary>
        /// Получение клиента по ID
        /// </summary>
        /// <param name="id">ID клиента</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Краткая информация о клиенте</returns>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(BaseResponse<CustomerSummaryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<CustomerSummaryDto>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BaseResponse<CustomerSummaryDto>>> GetCustomerById(
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = await _customerService.GetCustomerByIdAsync(id, cancellationToken);

            if (!result.IsSuccess)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Получение клиента по email
        /// </summary>
        /// <param name="email">Email клиента</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Краткая информация о клиенте</returns>
        [HttpGet("by-email/{email}")]
        [ProducesResponseType(typeof(BaseResponse<CustomerSummaryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<CustomerSummaryDto>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BaseResponse<CustomerSummaryDto>>> GetCustomerByEmail(
            [FromRoute][EmailAddress] string email,
            CancellationToken cancellationToken = default)
        {
            var result = await _customerService.GetCustomerByEmailAsync(email, cancellationToken);

            if (!result.IsSuccess)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Получение клиента с профилем
        /// </summary>
        /// <param name="id">ID клиента</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Клиент с профилем</returns>
        [HttpGet("{id:guid}/profile")]
        [ProducesResponseType(typeof(BaseResponse<CustomerWithProfileDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<CustomerWithProfileDto>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BaseResponse<CustomerWithProfileDto>>> GetCustomerWithProfile(
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = await _customerService.GetCustomerWithProfileAsync(id, cancellationToken);

            if (!result.IsSuccess)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Получение клиента с адресами
        /// </summary>
        /// <param name="id">ID клиента</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Клиент с адресами</returns>
        [HttpGet("{id:guid}/addresses")]
        [ProducesResponseType(typeof(BaseResponse<CustomerWithAddressesDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<CustomerWithAddressesDto>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BaseResponse<CustomerWithAddressesDto>>> GetCustomerWithAddresses(
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = await _customerService.GetCustomerWithAddressesAsync(id, cancellationToken);

            if (!result.IsSuccess)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Получение полной информации о клиенте
        /// </summary>
        /// <param name="id">ID клиента</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Полная информация о клиенте</returns>
        [HttpGet("{id:guid}/full")]
        [ProducesResponseType(typeof(BaseResponse<CustomerDetailDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<CustomerDetailDto>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BaseResponse<CustomerDetailDto>>> GetCustomerFullData(
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = await _customerService.GetCustomerFullDataAsync(id, cancellationToken);

            if (!result.IsSuccess)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Получение списка активных клиентов
        /// </summary>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Список активных клиентов</returns>
        [HttpGet("active")]
        [ProducesResponseType(typeof(BaseResponse<List<CustomerSummaryDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<BaseResponse<List<CustomerSummaryDto>>>> GetActiveCustomers(
            CancellationToken cancellationToken = default)
        {
            var result = await _customerService.GetActiveCustomersAsync(cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Деактивация клиента
        /// </summary>
        /// <param name="id">ID клиента</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Результат деактивации</returns>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BaseResponse<bool>>> DeleteCustomer(
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = await _customerService.DeleteCustomerAsync(id, cancellationToken);

            if (!result.IsSuccess)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
    }
}