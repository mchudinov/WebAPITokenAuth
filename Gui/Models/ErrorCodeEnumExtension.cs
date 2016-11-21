using System;
using System.Net;

namespace Gui.Models
{
    public static class ErrorCodeEnumExtension
    {
        public static T GetAttribute<T>(this ErrorCode value) where T : Attribute
        {
            var type = value.GetType();
            var memberInfo = type.GetMember(value.ToString());
            var attributes = memberInfo[0].GetCustomAttributes(typeof(T), false);
            return (T)attributes[0];
        }

        // This method creates a specific call to the above method, requesting the
        // Description MetaData attribute.
        public static string GetDescription(this ErrorCode value)
        {
            var attribute = value.GetAttribute<DescriptionAttribute>();
            return attribute.Description;
        }

        public static HttpStatusCode GetStatusCode(this ErrorCode value)
        {
            var attribute = value.GetAttribute<StatusCodeAttribute>();
            return attribute.StatusCode;
        }
    }
}