using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;


namespace Gui.Filters
{
    public class ValidateToken : System.Web.Http.Filters.ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var request = actionContext.Request;
            if (request.Headers.Authorization != null)
            {
                string header = request.Headers.GetValues("Authorization").First();

                if (string.IsNullOrEmpty(header))
                {
                    actionContext.Response = actionContext
                    .Request
                    .CreateErrorResponse(HttpStatusCode.Unauthorized, "Authorization header is empty");
                }
            }
            else
            {
                actionContext.Response = actionContext
                    .Request
                    .CreateErrorResponse(HttpStatusCode.Unauthorized, "The authorization header was not sent");
            }

            base.OnActionExecuting(actionContext);
        }
    }
}