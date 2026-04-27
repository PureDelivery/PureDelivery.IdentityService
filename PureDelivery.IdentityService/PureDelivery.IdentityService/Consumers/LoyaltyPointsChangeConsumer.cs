using MassTransit;
using PureDelivery.IdentityService.Core.Services;
using PureDelivery.Shared.Contracts.Events.Loyalty;
using PureDelivery.Shared.Contracts.Events.Orders;
using PureDelivery.Shared.Contracts.Interfaces;

namespace PureDelivery.IdentityService.Consumers
{
    public class LoyaltyPointsChangeConsumer(ICustomerProfileService customerProfileService) : IConsumer<LoyaltyPointsChangeEvent>
    {
        public async Task Consume(ConsumeContext<LoyaltyPointsChangeEvent> context)
        {
            var e = context.Message;

            await customerProfileService.AddLoyaltyPointsAsync(e.UserId, (decimal)e.PointsToChange, "Achievement");
        }
    }
}
