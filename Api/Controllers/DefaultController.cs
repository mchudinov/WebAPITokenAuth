using System.Collections.Generic;
using System.Web.Http;

namespace Api.Controllers
{
    public class DefaultController : ApiController
    {
        public IEnumerable<string> Get()
        {
            return new [] { "value1", "value2" };
        }
    }
}
