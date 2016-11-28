using System.IdentityModel.Tokens;
using System.Web.Http;

namespace Gui.Controllers
{
    public abstract class BaseApiController : ApiController
    {
        public string TokenKey { get; set; }
        public SecurityToken SecurityToken { get; set; }
    }
}
