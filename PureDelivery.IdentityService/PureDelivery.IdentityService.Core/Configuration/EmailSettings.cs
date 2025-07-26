using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureDelivery.IdentityService.Core.Configuration
{
    public class EmailSettings
    {
        public string SmtpHost { get; set; } = string.Empty;
        public int SmtpPort { get; set; }
        public string SmtpUsername { get; set; } = string.Empty;
        public string SmtpPassword { get; set; } = string.Empty;
        public string FromEmail { get; set; } = string.Empty;
        public string FromName { get; set; } = string.Empty;
        public bool EnableSsl { get; set; } = true;

        public EmailTemplates Templates { get; set; } = new();
        public EmailSettingsConfig Settings { get; set; } = new();
    }

    public class EmailTemplates
    {
        public string OtpSubject { get; set; } = "Email Confirmation";
        public string WelcomeSubject { get; set; } = "Welcome!";
        public string SupportEmail { get; set; } = "support@puredelivery.com";
    }

    public class EmailSettingsConfig
    {
        public int OtpExpiryMinutes { get; set; } = 10;
        public int MaxOtpAttempts { get; set; } = 5;
        public int ResendCooldownMinutes { get; set; } = 1;
    }
}
