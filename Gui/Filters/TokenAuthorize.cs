﻿using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Gui.Models;

namespace Gui.Filters
{
    public class TokenAuthorize : AuthorizationFilterAttribute
    {
        public override Task OnAuthorizationAsync(HttpActionContext actionContext, System.Threading.CancellationToken cancellationToken)
        {
            var request = actionContext.Request;
            if (request.Headers.Authorization != null)
            {
                string header = request.Headers.GetValues("Authorization").First();

                if (string.IsNullOrEmpty(header))
                {
                    actionContext.Response = request.CreateResponse(ErrorCode.SECURITY_TOKEN_EMPTY.GetStatusCode(), new Error(ErrorCode.SECURITY_TOKEN_EMPTY));
                    return Task.FromResult<object>(null);
                }

                string encoded = header;

                if (encoded.StartsWith("Bearer"))
                    encoded = encoded.Replace("Bearer", "");

                if (encoded.StartsWith("SAML"))
                    encoded = encoded.Replace("SAML", "");

                string tokenXml = Base64Decode(encoded);
            }
            else
            {
                actionContext.Response = request.CreateResponse(ErrorCode.SECURITY_TOKEN_MISSING.GetStatusCode(), new Error(ErrorCode.SECURITY_TOKEN_MISSING));
                return Task.FromResult<object>(null);
            }

            return Task.FromResult<object>(null);
        }

        private static string Base64Decode(string base64EncodedData)
        {
            try
            {
                var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
                return Encoding.UTF8.GetString(base64EncodedBytes);
            }
            catch
            {
                return null;
            }
        }
    }
}