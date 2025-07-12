using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureDelivery.IdentityService.Core.Configuration
{
    public class IdentityServiceConfig
    {
        public string ConnectionString { get; set; } = string.Empty;
        public DatabaseSettings Database { get; set; } = new();
    }

    public class DatabaseSettings
    {
        public int CommandTimeout { get; set; } = 30;
        public bool EnableRetryOnFailure { get; set; } = true;
        public int MaxRetryCount { get; set; } = 3;
        public int MaxRetryDelay { get; set; } = 5;
        public bool EnableSensitiveDataLogging { get; set; } = false;
    }
}
