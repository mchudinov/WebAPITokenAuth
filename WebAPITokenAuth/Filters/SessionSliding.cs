using System.IdentityModel.Tokens;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Gui.Controllers;

namespace Gui.Filters
{
    public class SessionSliding : ActionFilterAttribute
    {
        public override Task OnActionExecutingAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            //await Trace.WriteAsync("Executing action named {0} for request {1}.",
            //    actionContext.ActionDescriptor.ActionName,
            //    actionContext.Request.GetCorrelationId());
            //var factory = new WSTrustChannelFactory()

            var controller = actionContext.ControllerContext.Controller;
            Saml2SecurityToken token = ((BaseApiController)controller).SecurityToken as Saml2SecurityToken;
            //((BaseApiController)controller).TokenKey = key;

            var newtoken = new Saml2SecurityToken(token.Assertion,token.SecurityKeys,token.IssuerToken);

            return Task.FromResult<object>(null);
        }
    }
}