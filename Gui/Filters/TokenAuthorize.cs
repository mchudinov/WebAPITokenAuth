using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
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
                    actionContext.Response = request.CreateResponse(ErrorCode.ACCESS_TOKEN_EMPTY.GetStatusCode(), new Error(ErrorCode.ACCESS_TOKEN_EMPTY));
                    return Task.FromResult<object>(null);
                }
            }
            else
            {
                actionContext.Response = request.CreateResponse(ErrorCode.ACCESS_TOKEN_MISSING.GetStatusCode(), new Error(ErrorCode.ACCESS_TOKEN_MISSING));
                return Task.FromResult<object>(null);
            }

            return Task.FromResult<object>(null);
        }
    }
}