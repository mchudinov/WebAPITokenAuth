using System.IdentityModel.Tokens;
using System.Web.Http;

namespace Gui.Controllers
{
    public abstract class BaseApiController : ApiController
    {
        public SecurityToken SecurityToken { get; set; }
    }
}
