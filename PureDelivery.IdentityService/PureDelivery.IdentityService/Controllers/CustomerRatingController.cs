using Microsoft.AspNetCore.Mvc;
using PureDelivery.IdentityService.Core.DTOs;
using PureDelivery.IdentityService.Core.Services;
using PureDelivery.Shared.Contracts.Domain.Models;

namespace PureDelivery.IdentityService.Controllers;

[ApiController]
[Route("api/v1/identity/customer-rating")]
[Produces("application/json")]
public class CustomerRatingController(
    ICustomerRatingService ratingService,
    ILogger<CustomerRatingController> logger) : ControllerBase
{
    /// <summary>
    /// Get paginated ratings for a customer.
    /// </summary>
    [HttpGet("{customerId:guid}")]
    [ProducesResponseType(typeof(BaseResponse<PagedCustomerRatingsDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<PagedCustomerRatingsDto>>> GetRatings(
        [FromRoute] Guid customerId,
        [FromQuery] int page     = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct     = default)
    {
        var result = await ratingService.GetRatingsAsync(customerId, page, pageSize, ct);
        return Ok(result);
    }

    /// <summary>
    /// Courier submits a rating for a customer after delivery.
    /// </summary>
    [HttpPost("{customerId:guid}")]
    [ProducesResponseType(typeof(BaseResponse<CustomerRatingDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(BaseResponse<CustomerRatingDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<CustomerRatingDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(BaseResponse<CustomerRatingDto>), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<BaseResponse<CustomerRatingDto>>> SubmitRating(
        [FromRoute] Guid customerId,
        [FromBody] SubmitCustomerRatingRequest request,
        CancellationToken ct = default)
    {
        if (!ModelState.IsValid)
            return BadRequest(BaseResponse<CustomerRatingDto>.Failure("Invalid request data."));

        var result = await ratingService.SubmitRatingAsync(customerId, request, ct);

        if (!result.IsSuccess)
        {
            if (result.Error?.Contains("already rated") == true) return Conflict(result);
            if (result.Error?.Contains("not found") == true)     return NotFound(result);
            return BadRequest(result);
        }

        return StatusCode(StatusCodes.Status201Created, result);
    }
}
