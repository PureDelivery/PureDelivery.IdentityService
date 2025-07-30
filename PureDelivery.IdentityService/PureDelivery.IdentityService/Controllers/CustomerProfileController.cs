using Microsoft.AspNetCore.Mvc;
using PureDelivery.IdentityService.Core.Models;
using PureDelivery.IdentityService.Core.Services;
using PureDelivery.Shared.Contracts.Domain.Models;
using PureDelivery.Shared.Contracts.DTOs.Identity.Requests;
using System.ComponentModel.DataAnnotations;

namespace PureDelivery.IdentityService.Controllers
{
    [ApiController]
    [Route("api/v1/identity/[controller]")]
    [Produces("application/json")]
    public class CustomerProfileController : ControllerBase
    {
        private readonly ICustomerProfileService _profileService;
        private readonly ILogger<CustomerProfileController> _logger;
        private readonly ICloudinaryService _cloudinaryService;

        public CustomerProfileController(
            ICustomerProfileService profileService,
            ILogger<CustomerProfileController> logger,
            ICloudinaryService cloudinaryService)
        {
            _profileService = profileService;
            _logger = logger;
            _cloudinaryService = cloudinaryService;
        }

        /// <summary>
        /// Обновление профиля клиента
        /// </summary>
        /// <param name="customerId">ID клиента</param>
        /// <param name="profile">Данные профиля</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Результат обновления</returns>
        [HttpPut("{customerId:guid}")]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BaseResponse<bool>>> UpdateProfile(
            [FromRoute] Guid customerId,
            [FromBody] UpdateProfileRequest profile,
            CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(BaseResponse<bool>.Failure("Invalid request data"));
            }

            var result = await _profileService.UpdateProfileAsync(customerId, profile, cancellationToken);

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

        [HttpPost("{customerId:guid}/upload-avatar")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BaseResponse<string>>> UploadAvatar(
            [FromRoute] Guid customerId,
            IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(BaseResponse<string>.Failure("No file provided"));

            var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/webp" };
            if (!allowedTypes.Contains(file.ContentType.ToLower()))
                return BadRequest(BaseResponse<string>.Failure("Only JPEG, PNG and WebP images are allowed"));

            if (file.Length > 5 * 1024 * 1024)
                return BadRequest(BaseResponse<string>.Failure("File size cannot exceed 5MB"));

            try
            {
                var fileName = $"user_{customerId}_{DateTime.UtcNow:yyyyMMdd_HHmmss}";

                using var stream = file.OpenReadStream();
                var avatarUrl = await _cloudinaryService.UploadAvatarAsync(stream, fileName);

                var result = await _profileService.UpdateAvatarAsync(customerId, avatarUrl);

                if (result.IsSuccess)
                    return Ok(BaseResponse<string>.Success(avatarUrl, "Avatar uploaded successfully"));

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading avatar for customer {CustomerId}", customerId);
                return StatusCode(500, BaseResponse<string>.Failure("Failed to upload avatar"));
            }
        }

        /// <summary>
        /// Добавление баллов лояльности
        /// </summary>
        /// <param name="customerId">ID клиента</param>
        /// <param name="request">Данные для добавления баллов</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Результат добавления</returns>
        [HttpPost("{customerId:guid}/loyalty-points/add")]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BaseResponse<bool>>> AddLoyaltyPoints(
            [FromRoute] Guid customerId,
            [FromBody] LoyaltyPointsRequest request,
            CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(BaseResponse<bool>.Failure("Invalid request data"));
            }

            var result = await _profileService.AddLoyaltyPointsAsync(customerId, request.Points, request.Reason, cancellationToken);

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
        /// Списание баллов лояльности
        /// </summary>
        /// <param name="customerId">ID клиента</param>
        /// <param name="request">Данные для списания баллов</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Результат списания</returns>
        [HttpPost("{customerId:guid}/loyalty-points/spend")]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BaseResponse<bool>>> SpendLoyaltyPoints(
            [FromRoute] Guid customerId,
            [FromBody] LoyaltyPointsRequest request,
            CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(BaseResponse<bool>.Failure("Invalid request data"));
            }

            var result = await _profileService.SpendLoyaltyPointsAsync(customerId, request.Points, request.Reason, cancellationToken);

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
        /// Получение баланса баллов лояльности
        /// </summary>
        /// <param name="customerId">ID клиента</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Баланс баллов</returns>
        [HttpGet("{customerId:guid}/loyalty-points/balance")]
        [ProducesResponseType(typeof(BaseResponse<decimal>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<decimal>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BaseResponse<decimal>>> GetLoyaltyPointsBalance(
            [FromRoute] Guid customerId,
            CancellationToken cancellationToken = default)
        {
            var result = await _profileService.GetLoyaltyPointsBalanceAsync(customerId, cancellationToken);

            if (!result.IsSuccess)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Обновление даты последнего заказа
        /// </summary>
        /// <param name="customerId">ID клиента</param>
        /// <param name="request">Дата заказа</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Результат обновления</returns>
        [HttpPut("{customerId:guid}/last-order-date")]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BaseResponse<bool>>> UpdateLastOrderDate(
            [FromRoute] Guid customerId,
            [FromBody] UpdateLastOrderDateRequest request,
            CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(BaseResponse<bool>.Failure("Invalid request data"));
            }

            var result = await _profileService.UpdateLastOrderDateAsync(customerId, request.OrderDate, cancellationToken);

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
        /// Оценка клиента
        /// </summary>
        /// <param name="customerId">ID клиента</param>
        /// <param name="request">Данные оценки</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Результат оценки</returns>
        [HttpPost("{customerId:guid}/grade")]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BaseResponse<bool>>> GradeCustomer(
            [FromRoute] Guid customerId,
            [FromBody] GradeCustomerRequest request,
            CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(BaseResponse<bool>.Failure("Invalid request data"));
            }

            var result = await _profileService.GradeUser(customerId, request.Grade, cancellationToken);

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
    }
}