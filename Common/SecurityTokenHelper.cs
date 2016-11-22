using System;
using System.IdentityModel.Tokens;
using System.Text;
using System.Xml;

namespace Common
{
    public class SecurityTokenHelper
    {
        public static string GetSaml2TokenAsXml(BootstrapContext bootstrapContext)
        {
            if (!string.IsNullOrEmpty(bootstrapContext.Token))
                return bootstrapContext.Token;

            Saml2SecurityToken securityToken = bootstrapContext.SecurityToken as Saml2SecurityToken;
            var builder = new StringBuilder();

            try
            {
                using (var writer = XmlWriter.Create(builder))
                {
                    new Saml2SecurityTokenHandler(new SamlSecurityTokenRequirement()).WriteToken(writer, securityToken);
                }
            }
            catch 
            {
                return null;
            }

            return builder.ToString();
        }

        public static string GetSamlTokenAsXml(BootstrapContext bootstrapContext)
        {
            if (!string.IsNullOrEmpty(bootstrapContext.Token))
                return bootstrapContext.Token;

            SamlSecurityToken securityToken = bootstrapContext.SecurityToken as SamlSecurityToken;
            var builder = new StringBuilder();

            try
            {
                using (var writer = XmlWriter.Create(builder))
                {
                    new SamlSecurityTokenHandler(new SamlSecurityTokenRequirement()).WriteToken(writer, securityToken);
                }
            }
            catch
            {
                return null;
            }

            return builder.ToString();
        }


        public static bool IsTokenExpired(SecurityToken token)
        {
            return token != null && (DateTime.Now > token.ValidFrom && DateTime.Now < token.ValidTo);
        }
    }
}
