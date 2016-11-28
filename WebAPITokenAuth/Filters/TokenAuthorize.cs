using System.IdentityModel.Tokens;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Gui.Controllers;
using Gui.Helpers;
using Gui.Models;

namespace Gui.Filters
{
    public class TokenAuthorize : AuthorizationFilterAttribute
    {
        public override Task OnAuthorizationAsync(HttpActionContext actionContext, System.Threading.CancellationToken cancellationToken)
        {
            ErrorCode? errorCode;
            SecurityToken token = SecurityTokenHelper.GetTokenFromRequestMessage(actionContext.Request, out errorCode);
            if (null != errorCode)
            {
                actionContext.Response = actionContext.Request.CreateResponse(errorCode.GetStatusCode(), new Error(errorCode));
                return Task.FromResult<object>(null);
            }

            var controller = actionContext.ControllerContext.Controller;
            ((BaseApiController)controller).SecurityToken = token;
            //((BaseApiController)controller).TokenKey = key;

            actionContext.Request.Properties.Add("token", token);
            return Task.FromResult<object>(null);
        }
    }
}