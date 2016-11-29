using System.Collections.Generic;
using System.Web.Http;
using Gui.Filters;

namespace Gui.Controllers
{
    public class DataController : BaseApiController
    {
        [TokenAuthorize]
        public IEnumerable<string> Get()
        {
            return new[] { "value1", "value2" };
        }
    }
}
