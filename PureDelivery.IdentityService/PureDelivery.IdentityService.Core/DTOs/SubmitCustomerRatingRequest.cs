using System.ComponentModel.DataAnnotations;

namespace PureDelivery.IdentityService.Core.DTOs;

public class SubmitCustomerRatingRequest
{
    [Required]
    public Guid OrderId { get; set; }

    [Required]
    public Guid CourierId { get; set; }

    [Required, Range(1, 5)]
    public int Rating { get; set; }

    [MaxLength(500)]
    public string? Comment { get; set; }
}
