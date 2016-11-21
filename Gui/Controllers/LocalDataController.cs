using System.Collections.Generic;
using System.Web.Http;
using Gui.Filters;

namespace Gui.Controllers
{
    public class LocalDataController : ApiController
    {
        [TokenAuthorize]
        public IEnumerable<string> Get()
        {
            return new[] { "local value1", "local value2" };
        }
    }
}
