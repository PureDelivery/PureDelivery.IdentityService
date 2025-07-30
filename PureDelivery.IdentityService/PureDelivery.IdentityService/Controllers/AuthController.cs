using Microsoft.AspNetCore.Mvc;
using PureDelivery.IdentityService.Core.Services;
using PureDelivery.IdentityService.Http.impl;
using PureDelivery.Shared.Contracts.Domain.Models;
using PureDelivery.Shared.Contracts.DTOs.Identity;
using PureDelivery.Shared.Contracts.DTOs.Identity.Requests;

namespace PureDelivery.IdentityService.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            ICustomerService customerService,
            ILogger<AuthController> logger)
        {
            _customerService = customerService;
            _logger = logger;
        }

        /// <summary>
        /// Регистрация нового клиента
        /// </summary>
        /// <param name="request">Данные для регистрации</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Результат регистрации</returns>
        [HttpPost("register")]
        [ProducesResponseType(typeof(BaseResponse<CreateCustomerResultDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(BaseResponse<CreateCustomerResultDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<CreateCustomerResultDto>), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<BaseResponse<CreateCustomerResultDto>>> Register(
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
                if (result.Error?.Contains("EmailAlreadyExists") == true)
                {
                    return Conflict(result);
                }
                return BadRequest(result);
            }

            return CreatedAtAction(nameof(Register), result);
        }

        /// <summary>
        /// Аутентификация клиента
        /// </summary>
        /// <param name="request">Данные для аутентификации</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Данные аутентификации</returns>
        [HttpPost("login")]
        [ProducesResponseType(typeof(BaseResponse<AuthDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<AuthDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<AuthDto>), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<BaseResponse<AuthDto>>> Login(
            [FromBody] AuthenticateRequest request,
            CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(BaseResponse<AuthDto>.Failure("Invalid request data"));
            }

            // Заполняем данные о клиенте
            request.UserAgent = HttpHelper.GetUserAgent(HttpContext.Request);
            request.UserIP = HttpHelper.GetClientIpAddress(HttpContext);

            var result = await _customerService.AuthenticateAsync(request, cancellationToken);

            if (!result.IsSuccess)
            {
                return Unauthorized(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Выход из системы
        /// </summary>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Результат выхода</returns>
        [HttpPost("logout")]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<BaseResponse<bool>>> Logout(CancellationToken cancellationToken = default)
        {
            var sessionId = HttpHelper.GetSessionIdFromRequest(HttpContext.Request);

            if (string.IsNullOrEmpty(sessionId))
            {
                return Unauthorized(BaseResponse<bool>.Failure("Session not found"));
            }

            var result = await _customerService.LogoutAsync(sessionId, cancellationToken);

            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

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
                request,
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
        /// Запрос на смену пароля (отправка OTP на email)
        /// </summary>
        [HttpPost("request-forgot-password")]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BaseResponse<bool>>> RequestPasswordChange(
            [FromBody] PasswordChangeRequest request,
            CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(BaseResponse<bool>.Failure("Invalid request data"));
            }

            var result = await _customerService.RequestForgotPasswordAsync(request.Email, cancellationToken);

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
        /// Смена пароля с OTP кодом
        /// </summary>
        [HttpPut("change-password-with-otp")]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BaseResponse<bool>>> ChangePasswordWithOtp(
            [FromBody] ChangePasswordWithOtpRequest request,
            CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(BaseResponse<bool>.Failure("Invalid request data"));
            }

            var result = await _customerService.ChangePasswordWithOtpAsync(request, cancellationToken);

            if (!result.IsSuccess)
            {
                return BadRequest(result);
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
            [FromRoute] string email,
            CancellationToken cancellationToken = default)
        {
            var result = await _customerService.IsEmailAvailableAsync(email, cancellationToken);
            return Ok(result);
        }
    }
}