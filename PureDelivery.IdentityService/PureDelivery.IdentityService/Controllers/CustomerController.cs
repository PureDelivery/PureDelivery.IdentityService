using Microsoft.AspNetCore.Mvc;
using PureDelivery.IdentityService.Core.Services;
using PureDelivery.Shared.Contracts.Domain.Models;
using PureDelivery.Shared.Contracts.DTOs.Identity;
using System.ComponentModel.DataAnnotations;

namespace PureDelivery.IdentityService.Controllers
{
    [ApiController]
    [Route("api/v1/identity/[controller]")]
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
        /// Получение минимальных данных клиента для главной страницы
        /// </summary>
        /// <param name="customerId">ID клиента</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Минимальные данные клиента</returns>
        [HttpGet("{customerId:guid}")]
        [ProducesResponseType(typeof(BaseResponse<CustomerSummaryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<CustomerSummaryDto>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BaseResponse<CustomerSummaryDto>>> GetCustomerSummary(
            [FromRoute] Guid customerId,
            CancellationToken cancellationToken = default)
        {
            var response = await _customerService.GetCustomerSummary(customerId, cancellationToken);

            if (!response.IsSuccess)
            {
                return NotFound(response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Получение данных для вкладки лояльности
        /// </summary>
        [HttpGet("{customerId:guid}/loyalty")]
        [ProducesResponseType(typeof(BaseResponse<CustomerLoyaltyDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<CustomerLoyaltyDto>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BaseResponse<CustomerLoyaltyDto>>> GetCustomerLoyalty(
            [FromRoute] Guid customerId,
            CancellationToken cancellationToken = default)
        {
            var response = await _customerService.GetCustomerLoyaltyAsync(customerId, cancellationToken);

            if (!response.IsSuccess)
            {
                return NotFound(response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Получение данных для вкладки профиля
        /// </summary>
        [HttpGet("{customerId:guid}/profile-info")]
        [ProducesResponseType(typeof(BaseResponse<CustomerProfileInfoDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<CustomerProfileInfoDto>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BaseResponse<CustomerProfileInfoDto>>> GetCustomerProfileInfo(
            [FromRoute] Guid customerId,
            CancellationToken cancellationToken = default)
        {
            var response = await _customerService.GetCustomerProfileInfoAsync(customerId, cancellationToken);

            if (!response.IsSuccess)
            {
                return NotFound(response);
            }

            return Ok(response);
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