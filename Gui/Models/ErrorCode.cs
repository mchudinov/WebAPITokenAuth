using System;
using System.Net;

namespace Gui.Models
{
    public enum ErrorCode
    {
        #region AccessToken errors
        [Description("Access token required")]
        [StatusCode(HttpStatusCode.Unauthorized)]
        ACCESS_TOKEN_MISSING,

        [Description("Access token is empty")]
        [StatusCode(HttpStatusCode.Unauthorized)]
        ACCESS_TOKEN_EMPTY,

        [Description("Invalid token")]
        [StatusCode(HttpStatusCode.Unauthorized)]
        ACCESS_TOKEN_INVALID,

        [Description("Expired token")]
        [StatusCode(HttpStatusCode.Unauthorized)]
        ACCESS_TOKEN_EXPIRED,

        [Description("Unexpected error while validating token")]
        [StatusCode(HttpStatusCode.InternalServerError)]
        ACCESS_TOKEN_VALIDATION_ERROR,
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