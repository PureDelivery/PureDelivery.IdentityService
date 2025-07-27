using Microsoft.AspNetCore.Mvc;
using PureDelivery.IdentityService.Core.Models;
using PureDelivery.IdentityService.Core.ResponseConstants.Enums;
using PureDelivery.IdentityService.Core.Services;
using PureDelivery.Shared.Contracts.Domain.Models;
using PureDelivery.Shared.Contracts.DTOs.Identity;
using PureDelivery.Shared.Contracts.DTOs.Identity.Requests;
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
        /// Создание нового клиента
        /// </summary>
        /// <param name="request">Данные для создания клиента</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Результат создания клиента</returns>
        [HttpPost("register")]
        [ProducesResponseType(typeof(BaseResponse<CreateCustomerResultDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(BaseResponse<CreateCustomerResultDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<CreateCustomerResultDto>), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<BaseResponse<CreateCustomerResultDto>>> CreateCustomer(
            [FromBody] CreateCustomerRequest request,
            CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(BaseResponse<CreateCustomerResultDto>.Failure("Invalid request data"));
            }

            var result = await _customerService.CreateCustomerAsync(request, cancellationToken);

            if (!result.IsSuccess)
            {
                if (result.Error?.Contains(IdentityCoreErrors.EmailAlreadyExists.ToString()) == true)
                {
                    return Conflict(result);
                }
                return BadRequest(result);
            }

            return CreatedAtAction(nameof(GetCustomerById), new { id = result.Data.CustomerId }, result);
        }

        /// <summary>
        /// Аутентификация клиента
        /// </summary>
        /// <param name="request">Данные для аутентификации</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Данные аутентификации</returns>
        [HttpPost("authenticate")]
        [ProducesResponseType(typeof(BaseResponse<AuthDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<AuthDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<AuthDto>), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<BaseResponse<AuthDto>>> Authenticate(
            [FromBody] AuthenticateRequest request,
            CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(BaseResponse<AuthDto>.Failure("Invalid request data"));
            }

            request.UserAgent = GetUserAgent();
            request.UserIP = GetClientIpAddress();


            var result = await _customerService.AuthenticateAsync(request, cancellationToken);

            if (!result.IsSuccess)
            {
                return Unauthorized(result);
            }

            return Ok(result);
        }


        private string GetUserAgent()
        {
            return Request.Headers["User-Agent"].FirstOrDefault() ??
                   Request.Headers["X-Gateway-UserAgent"].FirstOrDefault() ??
                   "Unknown";
        }

        private string GetClientIpAddress()
        {
            // Сначала проверяем заголовки от Gateway
            var gatewayIp = Request.Headers["X-Gateway-IP"].FirstOrDefault();
            if (!string.IsNullOrEmpty(gatewayIp))
            {
                return gatewayIp;
            }

            // Проверяем стандартные заголовки прокси
            var forwardedFor = Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwardedFor))
            {
                // Берем первый IP из списка (оригинальный клиент)
                return forwardedFor.Split(',')[0].Trim();
            }

            var realIp = Request.Headers["X-Real-IP"].FirstOrDefault();
            if (!string.IsNullOrEmpty(realIp))
            {
                return realIp;
            }

            // Если ничего нет, берем удаленное соединение
            return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
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
        /// Изменение пароля клиента
        /// </summary>
        /// <param name="request">Данные для изменения пароля</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Результат изменения пароля</returns>
        [HttpPut("change-password")]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BaseResponse<bool>>> ChangePassword(
            [FromBody] ChangePasswordRequest request,
            CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(BaseResponse<bool>.Failure("Invalid request data"));
            }

            var result = await _customerService.ChangePasswordAsync(
                request.CustomerId,
                request.CurrentPassword,
                request.NewPassword,
                cancellationToken);

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

        /// <summary>
        /// Проверка доступности email
        /// </summary>
        /// <param name="email">Email для проверки</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Результат проверки доступности</returns>
        [HttpGet("email-availability/{email}")]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
        public async Task<ActionResult<BaseResponse<bool>>> CheckEmailAvailability(
            [FromRoute][EmailAddress] string email,
            CancellationToken cancellationToken = default)
        {
            var result = await _customerService.IsEmailAvailableAsync(email, cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Подтверждение email с помощью OTP кода
        /// </summary>
        /// <param name="request">Email и OTP код</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Результат подтверждения</returns>
        [HttpPost("confirm-email")]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BaseResponse<bool>>> ConfirmEmail(
            [FromBody] ConfirmEmailRequest request,
            CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(BaseResponse<bool>.Failure("Invalid request data"));
            }

            var result = await _customerService.ConfirmEmailAsync(request, cancellationToken);

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
        /// Повторная отправка OTP кода
        /// </summary>
        /// <param name="request">Email для повторной отправки</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Результат отправки</returns>
        [HttpPost("resend-otp")]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status429TooManyRequests)]
        public async Task<ActionResult<BaseResponse<bool>>> ResendOtp(
            [FromBody] ResendOtpRequest request,
            CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(BaseResponse<bool>.Failure("Invalid request data"));
            }

            var result = await _customerService.ResendOtpAsync(request, cancellationToken);

            if (!result.IsSuccess)
            {
                if (result.Message?.Contains("not found") == true)
                {
                    return NotFound(result);
                }
                if (result.Message?.Contains("wait") == true ||
                    result.Message?.Contains("Too many") == true)
                {
                    return StatusCode(StatusCodes.Status429TooManyRequests, result);
                }
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
