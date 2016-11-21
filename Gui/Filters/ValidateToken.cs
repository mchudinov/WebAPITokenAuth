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
        public override Task OnActionExecutingAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() => new HttpResponseMessage(HttpStatusCode.Unauthorized)
            {
                Content = new StringContent("Invalid token")
            }, cancellationToken);
        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            actionContext.Response = actionContext
                    .Request
                    .CreateErrorResponse(HttpStatusCode.BadRequest,"xcv");
        }
    }
}