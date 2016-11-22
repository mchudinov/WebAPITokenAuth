using System;
using System.Text;

namespace Common
{
    public class Base64
    {
        public static string Encode(string plainText)
        {
            try
            {
                var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
                return Convert.ToBase64String(plainTextBytes);
            }
            catch
            {
                return null;
            }
        }

        public static string Decode(string base64EncodedData)
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
