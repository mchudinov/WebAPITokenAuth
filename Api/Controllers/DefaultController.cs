using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http;

namespace Api.Controllers
{
    public class DefaultController : ApiController
    {
        [Authorize]
        public IEnumerable<string> Get()
        {
            return new [] { "value1", "value2" };

            //IEnumerable<string> headerValues = request.Headers.GetValues("X-My-Custom-Header");
            //if (null != headerValues)
            //{
            //    var encoding = Encoding.GetEncoding("iso-8859-1");
            //    string samlToken = encoding.GetString(Convert.FromBase64String(headerValues.FirstOrDefault()));
            //}

        }
    }
}
