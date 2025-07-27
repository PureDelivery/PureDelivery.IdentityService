using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Logging;
using PureDelivery.Common.Configuration.Services;
using PureDelivery.IdentityService.Core.Configuration;
using System.Security.Principal;

namespace PureDelivery.IdentityService.Core.Services.impl
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;
        private readonly CloudinarySettings _settings;
        private readonly ILogger<CloudinaryService> _logger;

        public CloudinaryService(
            ILogger<CloudinaryService> logger,
            ICustomConfigurationProvider configProvider)
        {
            _logger = logger;
            _settings = configProvider.GetConfigurationAsync<CloudinarySettings>("Cloudinary").GetAwaiter().GetResult();

            var account = new Account(_settings.CloudName, _settings.ApiKey, _settings.ApiSecret);
            _cloudinary = new Cloudinary(account);
        }

        public async Task<string> UploadAvatarAsync(Stream imageStream, string fileName)
        {
            try
            {
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(fileName, imageStream),
                    Folder = "avatars",
                    PublicId = $"avatar_{fileName}_{Guid.NewGuid()}",
                    Transformation = new Transformation()
                        .Width(_settings.TransformationOptions.Width)
                        .Height(_settings.TransformationOptions.Height)
                        .Crop(_settings.TransformationOptions.Crop)
                        .Quality(_settings.TransformationOptions.Quality)
                        .FetchFormat(_settings.TransformationOptions.Format)
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    _logger.LogInformation("Avatar uploaded successfully: {PublicId}", uploadResult.PublicId);
                    return uploadResult.SecureUrl.ToString();
                }

                _logger.LogError("Avatar upload failed: {Error}", uploadResult.Error?.Message);
                throw new Exception($"Upload failed: {uploadResult.Error?.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading avatar for file: {FileName}", fileName);
                throw;
            }
        }

        public async Task<bool> DeleteAvatarAsync(string publicId)
        {
            try
            {
                var deleteParams = new DeletionParams(publicId);
                var result = await _cloudinary.DestroyAsync(deleteParams);

                return result.StatusCode == System.Net.HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting avatar: {PublicId}", publicId);
                return false;
            }
        }

        public string GetAvatarUrl(string publicId, int width = 200, int height = 200)
        {
            return _cloudinary.Api.UrlImgUp.Transform(new Transformation()
                .Width(width).Height(height).Crop("fill"))
                .BuildUrl(publicId);
        }
    }
}