using System.Collections.Generic;
using System.Web.Http;

namespace Api.Controllers
{
    public class RemoteDataController : ApiController
    {
        public IEnumerable<string> Get()
        {
            return new [] { "remote value1", "remote value2" };
        }
    }
}
