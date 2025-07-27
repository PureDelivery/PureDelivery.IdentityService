using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureDelivery.IdentityService.Core.Configuration
{

    public class OtpSettings
    {
        public int ExpiryMinutes { get; set; } = 10;
        public int ResendCooldownMinutes { get; set; } = 1;
        public int MaxAttempts { get; set; } = 5;
        public int Length { get; set; } = 6;
    }
}
