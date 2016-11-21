using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Gui.Filters;

namespace Gui.Controllers
{
    public class LocalDataController : ApiController
    {
        //[ValidateToken]
        [TokenAuthorize]
        public IEnumerable<string> Get()
        {
            return new[] { "local value1", "local value2" };
        }
    }
}
