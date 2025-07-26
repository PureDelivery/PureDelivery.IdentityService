using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PureDelivery.IdentityService.Core.Services.impl
{
    public class OtpService : IOtpService
    {
        public string GenerateOtp()
        {
            using var rng = RandomNumberGenerator.Create();
            var bytes = new byte[4];
            rng.GetBytes(bytes);

            // Convert to positive integer and get 6 digits
            var randomNumber = Math.Abs(BitConverter.ToInt32(bytes, 0));
            return (randomNumber % 1000000).ToString("D6");
        }

        public bool ValidateOtp(string providedOtp, string storedOtp, DateTime expiryTime)
        {
            if (string.IsNullOrWhiteSpace(providedOtp) || string.IsNullOrWhiteSpace(storedOtp))
                return false;

            if (IsOtpExpired(expiryTime))
                return false;

            return providedOtp.Equals(storedOtp, StringComparison.Ordinal);
        }

        public bool IsOtpExpired(DateTime expiryTime)
        {
            return DateTime.UtcNow > expiryTime;
        }

        public DateTime GetOtpExpiryTime(int expiryMinutes = 10)
        {
            return DateTime.UtcNow.AddMinutes(expiryMinutes);
        }
    }
}
