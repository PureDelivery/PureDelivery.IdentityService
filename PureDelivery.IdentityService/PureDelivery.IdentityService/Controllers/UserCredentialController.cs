using Microsoft.AspNetCore.Mvc;
using PureDelivery.IdentityService.Core.Services;
using PureDelivery.Shared.Contracts.Domain.Models;
using PureDelivery.Shared.Contracts.DTOs.Identity.Requests;
using PureDelivery.Shared.Contracts.DTOs.Identity.Responses;

namespace PureDelivery.IdentityService.Controllers
{
    /// <summary>
    /// Внутренний endpoint для создания учётных данных Manager / Courier.
    /// Маршрут /api/internal/ намеренно НЕ проксируется через API Gateway —
    /// доступен только из внутренней сети (RestaurantService, CourierService).
    /// </summary>
    [ApiController]
    [Route("api/internal/v1/identity/[controller]")]
    [Produces("application/json")]
    public class UserCredentialController : ControllerBase
    {
        private readonly IUserCredentialService _credentialService;
        private readonly ILogger<UserCredentialController> _logger;

        public UserCredentialController(
            IUserCredentialService credentialService,
            ILogger<UserCredentialController> logger)
        {
            _credentialService = credentialService;
            _logger = logger;
        }

        /// <summary>
        /// Создать учётные данные для нового менеджера или курьера.
        /// Вызывается RestaurantService / CourierService после создания профиля.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(BaseResponse<RegisterUserCredentialResult>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(BaseResponse<RegisterUserCredentialResult>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<RegisterUserCredentialResult>), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<BaseResponse<RegisterUserCredentialResult>>> Register(
            [FromBody] RegisterUserCredentialRequest request,
            CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
                return BadRequest(BaseResponse<RegisterUserCredentialResult>.Failure("Invalid request data"));

            var result = await _credentialService.RegisterAsync(request, cancellationToken);

            if (!result.IsSuccess)
            {
                if (result.Error?.Contains("already exists") == true)
                    return Conflict(result);
                return BadRequest(result);
            }

            return CreatedAtAction(nameof(Register), result);
        }

        /// <summary>
        /// Деактивировать учётные данные пользователя (при увольнении менеджера / курьера).
        /// </summary>
        [HttpDelete("{userId:guid}")]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BaseResponse<bool>>> Deactivate(
            [FromRoute] Guid userId,
            CancellationToken cancellationToken = default)
        {
            var result = await _credentialService.DeactivateAsync(userId, cancellationToken);

            if (!result.IsSuccess)
                return NotFound(result);

            return Ok(result);
        }
    }
}
