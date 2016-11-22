using System.IdentityModel.Tokens;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Common;
using Gui.Models;

namespace Gui.Filters
{
    public class TokenAuthorize : AuthorizationFilterAttribute
    {
        public override Task OnAuthorizationAsync(HttpActionContext actionContext, System.Threading.CancellationToken cancellationToken)
        {
            var request = actionContext.Request;
            if (request.Headers.Authorization != null)
            {
                string header = request.Headers.GetValues("Authorization").First();

                if (string.IsNullOrEmpty(header))
                {
                    actionContext.Response = request.CreateResponse(ErrorCode.SECURITY_TOKEN_EMPTY.GetStatusCode(), new Error(ErrorCode.SECURITY_TOKEN_EMPTY));
                    return Task.FromResult<object>(null);
                }

                string encoded = header;

                if (encoded.StartsWith("Bearer"))
                    encoded = encoded.Replace("Bearer", "");

                if (encoded.StartsWith("SAML"))
                    encoded = encoded.Replace("SAML", "");

                string token = Base64.Decode(encoded);
                if (string.IsNullOrEmpty(header))
                {
                    actionContext.Response = request.CreateResponse(ErrorCode.SECURITY_TOKEN_INVALID.GetStatusCode(), new Error(ErrorCode.SECURITY_TOKEN_INVALID));
                    return Task.FromResult<object>(null);
                }

                SessionSecurityToken sessiontoken;
                try
                {
                    sessiontoken = System.Web.HttpContext.Current.Cache[token] as SessionSecurityToken;
                }
                catch
                {
                    actionContext.Response = request.CreateResponse(ErrorCode.SECURITY_TOKEN_VALIDATION_ERROR.GetStatusCode(), new Error(ErrorCode.SECURITY_TOKEN_VALIDATION_ERROR));
                    return Task.FromResult<object>(null);
                }

                if (null == sessiontoken)
                {
                    actionContext.Response = request.CreateResponse(ErrorCode.SECURITY_TOKEN_INVALID.GetStatusCode(), new Error(ErrorCode.SECURITY_TOKEN_INVALID));
                    return Task.FromResult<object>(null);
                }

                if (SecurityTokenHelper.IsTokenExpired(sessiontoken))
                {
                    actionContext.Response = request.CreateResponse(ErrorCode.SECURITY_TOKEN_EXPIRED.GetStatusCode(), new Error(ErrorCode.SECURITY_TOKEN_EXPIRED));
                    return Task.FromResult<object>(null);
                }
            }
            else
            {
                actionContext.Response = request.CreateResponse(ErrorCode.SECURITY_TOKEN_MISSING.GetStatusCode(), new Error(ErrorCode.SECURITY_TOKEN_MISSING));
                return Task.FromResult<object>(null);
            }

            return Task.FromResult<object>(null);
        }
    }
}