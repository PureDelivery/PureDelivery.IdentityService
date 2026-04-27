namespace PureDelivery.IdentityService.Core.DTOs;

public class CustomerRatingDto
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public Guid OrderId { get; set; }
    public Guid CourierId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class PagedCustomerRatingsDto
{
    public List<CustomerRatingDto> Items { get; set; } = [];
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public double AverageRating { get; set; }
}
