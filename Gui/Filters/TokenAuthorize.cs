using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Mvc;

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
                    actionContext.Response = request.CreateResponse(HttpStatusCode.Unauthorized, "Authorization header is empty");
                    return Task.FromResult<object>(null);
                }
            }
            else
            {
                actionContext.Response = request.CreateResponse(HttpStatusCode.Unauthorized, "The authorization header was not sent");
                return Task.FromResult<object>(null);
            }

            return Task.FromResult<object>(null);
        }
    }
}