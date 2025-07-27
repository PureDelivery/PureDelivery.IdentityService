using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureDelivery.IdentityService.Core.Services
{
    public interface IOtpService
    {
        string GenerateOtp();
        bool ValidateOtp(string providedOtp, string storedOtp, DateTime expiryTime);
        bool IsOtpExpired(DateTime expiryTime);
        DateTime GetOtpExpiryTime(int expiryMinutes = 10);
    }
}
