using System;
using System.Net;

namespace Gui.Models
{
    public enum ErrorCode
    {
        #region AccessToken errors
        [Description("Authorization header was not sent or empty")]
        [StatusCode(HttpStatusCode.Unauthorized)]
        SECURITY_TOKEN_MISSING,

        [Description("Invalid security token")]
        [StatusCode(HttpStatusCode.Unauthorized)]
        SECURITY_TOKEN_INVALID,

        [Description("Expired securty token")]
        [StatusCode(HttpStatusCode.Unauthorized)]
        SECURITY_TOKEN_EXPIRED,

        [Description("Unexpected error while validating security token")]
        [StatusCode(HttpStatusCode.InternalServerError)]
        SECURITY_TOKEN_VALIDATION_ERROR,
        #endregion
    }

    public class StatusCodeAttribute : Attribute
    {
        public HttpStatusCode StatusCode { get; private set; }

        public StatusCodeAttribute(HttpStatusCode statusCode)
        {
            StatusCode = statusCode;
        }
    }

    public class DescriptionAttribute : Attribute
    {
        public string Description { get; private set; }

        public DescriptionAttribute(string description)
        {
            Description = description;
        }
    }
}