using System;
using System.IdentityModel.Selectors;
using System.IdentityModel.Services;
using System.IdentityModel.Tokens;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using Common;

namespace Api
{
    public class TokenValidationHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
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

                if (encoded.StartsWith("SAML"))
                    encoded = encoded.Replace("SAML", "");
                
                string tokenXml = Base64.Decode(encoded); 
                
                Saml2SecurityToken samlToken = GetSamlToken(tokenXml);

                ValidateSamlToken(samlToken);

                if (null == samlToken)
                {
                    return Task.Factory.StartNew(() => new HttpResponseMessage(HttpStatusCode.Unauthorized)
                    {
                        Content = new StringContent("Invalid token")
                    }, cancellationToken);
                }

                var handlers = FederatedAuthentication.FederationConfiguration.IdentityConfiguration.SecurityTokenHandlers;
                ClaimsIdentity claimsIdentity = handlers.ValidateToken(samlToken).FirstOrDefault();
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

        private static Saml2SecurityToken GetSamlToken(string tokenXml)
        {
            if (string.IsNullOrEmpty(tokenXml))
                return null;

            var handlers = FederatedAuthentication.FederationConfiguration.IdentityConfiguration.SecurityTokenHandlers;
            Saml2SecurityToken token;
            try
            {
                using (StringReader stringReader = new StringReader(tokenXml))
                using (XmlTextReader xmlReader = new XmlTextReader(stringReader))
                {
                    token = (Saml2SecurityToken)handlers.ReadToken(xmlReader);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            return token;
        }

        public static ClaimsIdentity ValidateSamlToken(SecurityToken securityToken)
        {
            var registry = new ConfigurationBasedIssuerNameRegistry();
            registry.AddTrustedIssuer("115E38C827ACE56638E32935604C52AF83DFA544", "https://mikael.accesscontrol.windows.net/");

            var configuration = new SecurityTokenHandlerConfiguration
            {
                AudienceRestriction = {AudienceMode = AudienceUriMode.Never},
                CertificateValidationMode = X509CertificateValidationMode.None,
                RevocationMode = X509RevocationMode.NoCheck,
                CertificateValidator = X509CertificateValidator.None,
                IssuerNameRegistry = registry
            };

            var handler = SecurityTokenHandlerCollection.CreateDefaultSecurityTokenHandlerCollection(configuration);
            var identity = handler.ValidateToken(securityToken).First();
            return identity;
        }

    }
}