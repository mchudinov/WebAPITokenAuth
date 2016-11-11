using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;
using System.Web.Http;
using System.Xml;

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

        //public ClaimsIdentity GetIdentityFromToken(string tokenBase64)
        //{
        //    if (string.IsNullOrEmpty(tokenBase64))
        //        return null;

        //    byte[] tokenByteArray = Convert.FromBase64String(tokenBase64);
        //    string decodedToken = Encoding.UTF8.GetString(tokenByteArray);

        //    if (string.IsNullOrWhiteSpace(decodedToken))
        //        return null;
        //    try
        //    {
        //        var handlers = FederatedAuthentication.FederationConfiguration.IdentityConfiguration.SecurityTokenHandlers;
        //        SecurityToken token;
        //        using (StringReader stringReader = new StringReader(decodedToken))
        //        {
        //            using (XmlTextReader xmlReader = new XmlTextReader(stringReader))
        //            {
        //                token = handlers.ReadToken(xmlReader);
        //            }
        //        }

        //        if (token == null)
        //            return null;

        //        return handlers.ValidateToken(token).FirstOrDefault();
        //    }
        //    catch (Exception e)
        //    {
        //        //logger.Error(new AuthenticationException("Error validating the token from ADFS", e));
        //        return null;
        //    }
        //}
    }
}
