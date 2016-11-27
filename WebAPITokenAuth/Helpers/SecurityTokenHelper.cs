using System;
using System.IdentityModel.Tokens;
using System.Net.Http;
using System.Web;
using Gui.Models;

namespace Gui.Helpers
{
    public class SecurityTokenHelper
    {
        private static bool IsTokenExpired(SecurityToken token)
        {
            return token != null && (DateTime.Now > token.ValidFrom && DateTime.Now < token.ValidTo);
        }

        public static void SaveTokenInCache(SecurityToken token, string key)
        {
            HttpRuntime.Cache.Add(key, token, null, System.Web.Caching.Cache.NoAbsoluteExpiration, new TimeSpan(0, 30, 0), System.Web.Caching.CacheItemPriority.Normal, null);
        }

        private static SecurityToken GetTokenFromCache(string key)
        {
            return HttpRuntime.Cache[key.Trim()] as SecurityToken;
        }

        public static string GetKey()
        {
            return Guid.NewGuid().ToString();
        }

        private static string GetKeyFromRequest(HttpRequestMessage request, out ErrorCode? error)
        {
            var authHeader = request?.Headers?.Authorization;
            if (string.IsNullOrWhiteSpace(authHeader?.Parameter))
            {
                error = ErrorCode.SECURITY_TOKEN_MISSING;
                return string.Empty;
            }

            if (authHeader.Scheme != "Bearer")
            {
                error = ErrorCode.SECURITY_TOKEN_INVALID;
                return string.Empty;
            }

            error = null;
            return authHeader.Parameter;
        }

        public static SecurityToken GetTokenFromRequestMessage(HttpRequestMessage request, out ErrorCode? error)
        {
            SecurityToken token = null;
            string key = GetKeyFromRequest(request, out error);

            if (string.IsNullOrEmpty(key))
            {
                error = ErrorCode.SECURITY_TOKEN_INVALID;
            }

            try
            {
                token = GetTokenFromCache(key);
            }
            catch
            {
                error = ErrorCode.SECURITY_TOKEN_VALIDATION_ERROR;
            }

            if (null == token)
            {
                error = ErrorCode.SECURITY_TOKEN_INVALID;
            }

            if (IsTokenExpired(token))
            {
                error = ErrorCode.SECURITY_TOKEN_EXPIRED;
            }

            return token;
        }
    }
}
