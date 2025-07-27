using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureDelivery.IdentityService.Core.Services
{
    public interface ICloudinaryService
    {
        Task<string> UploadAvatarAsync(Stream imageStream, string fileName);
        Task<bool> DeleteAvatarAsync(string publicId);
        string GetAvatarUrl(string publicId, int width = 200, int height = 200);
    }
}
