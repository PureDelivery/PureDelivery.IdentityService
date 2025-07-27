using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureDelivery.IdentityService.Core.Configuration
{
    public class CloudinarySettings
    {
        public string CloudName { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public string ApiSecret { get; set; } = string.Empty;
        public bool SecureUrl { get; set; } = true;
        public string UploadPreset { get; set; } = string.Empty;
        public TransformationOptions TransformationOptions { get; set; } = new();
    }

    public class TransformationOptions
    {
        public int Width { get; set; } = 200;
        public int Height { get; set; } = 200;
        public string Crop { get; set; } = "fill";
        public string Quality { get; set; } = "auto";
        public string Format { get; set; } = "auto";
    }
}
