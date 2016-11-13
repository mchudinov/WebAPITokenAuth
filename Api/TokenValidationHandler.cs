using System;
using System.Collections.ObjectModel;
using System.IdentityModel.Services;
using System.IdentityModel.Tokens;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace Api
{
    public class TokenValidationHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            if (request.Headers.Authorization != null)
            {
                string header = request.Headers.GetValues("Authorization").First();

                // Check that a value is there
                if (string.IsNullOrEmpty(header))
                {
                    return Task.Factory.StartNew(() => new HttpResponseMessage(HttpStatusCode.Unauthorized)
                    {
                        Content = new StringContent("Authorization header is empty")
                    }, cancellationToken);
                }

                string encoded = header;

                if (encoded.StartsWith("Bearer"))
                    encoded = encoded.Replace("Bearer", "");

                string tokenXml = Base64Decode(encoded); 
                
                Saml2SecurityToken token = GetSamlToken(tokenXml);

                if (null == token)
                {
                    return Task.Factory.StartNew(() => new HttpResponseMessage(HttpStatusCode.Unauthorized)
                    {
                        Content = new StringContent("Invalid token")
                    }, cancellationToken);
                }

                var handlers = FederatedAuthentication.FederationConfiguration.IdentityConfiguration.SecurityTokenHandlers;
                ClaimsIdentity claimsIdentity = handlers.ValidateToken(token).FirstOrDefault();
                ClaimsPrincipal principal = new ClaimsPrincipal(claimsIdentity);
                SetPrincipal(principal);
            }
            else
            {
                return Task.Factory.StartNew(() => new HttpResponseMessage(HttpStatusCode.Unauthorized)
                {
                    Content = new StringContent("The authorization header was not sent")
                }, cancellationToken);
            }

            return base.SendAsync(request, cancellationToken);
        }

        private void SetPrincipal(IPrincipal principal)
        {
            Thread.CurrentPrincipal = principal;
            if (HttpContext.Current != null)
            {
                HttpContext.Current.User = principal;
            }
        }

        private static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }

        private static Saml2SecurityToken GetSamlToken(string tokenXml)
        {
            var handlers = FederatedAuthentication.FederationConfiguration.IdentityConfiguration.SecurityTokenHandlers;
            Saml2SecurityToken token;
            using (StringReader stringReader = new StringReader(tokenXml))
            using (XmlTextReader xmlReader = new XmlTextReader(stringReader))
            {
                token = (Saml2SecurityToken)handlers.ReadToken(xmlReader);
            }
            return token;
        }
    }
}