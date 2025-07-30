using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureDelivery.IdentityService.Core.Services
{
    public interface IEmailService
    {
        Task<bool> SendOtpEmailAsync(string email, string otpCode, CancellationToken cancellationToken = default);
        Task<bool> SendWelcomeEmailAsync(string email, string firstName, CancellationToken cancellationToken = default);
        Task<bool> SendPasswordChangeOtpEmailAsync(string email, string otpCode, CancellationToken cancellationToken = default);
    }

}
