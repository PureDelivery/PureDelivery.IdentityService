namespace PureDelivery.IdentityService.Http.impl
{
    public static class HttpHelper
    {
        public static string? GetSessionIdFromRequest(HttpRequest request)
        {
            if (request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                var token = authHeader.FirstOrDefault();
                if (!string.IsNullOrEmpty(token) && token.StartsWith("Bearer "))
                {
                    return token.Substring(7);
                }
            }

            return null;
        }

        public static string GetUserAgent(HttpRequest request)
        {
            return request.Headers["User-Agent"].FirstOrDefault() ??
                   request.Headers["X-Gateway-UserAgent"].FirstOrDefault() ??
                   "Unknown";
        }

        public static string GetClientIpAddress(HttpContext httpContext)
        {
            var request = httpContext.Request;

            var gatewayIp = request.Headers["X-Gateway-IP"].FirstOrDefault();
            if (!string.IsNullOrEmpty(gatewayIp))
            {
                return gatewayIp;
            }

            var forwardedFor = request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwardedFor))
            {
                return forwardedFor.Split(',')[0].Trim();
            }

            var realIp = request.Headers["X-Real-IP"].FirstOrDefault();
            if (!string.IsNullOrEmpty(realIp))
            {
                return realIp;
            }

            return httpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        }
    }
}
