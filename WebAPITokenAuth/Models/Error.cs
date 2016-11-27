namespace Gui.Models
{
    /// <summary>
    /// Example
    /// {
    ///     "ErrorCode": "CATALOG_NO_RESULT",
    ///     "Description": "Item does not exist"
    /// } 
    /// </summary>
    public class Error
    {
        public Error(ErrorCode? errorCode)
        {
            if (errorCode == null) return;
            Code = errorCode.ToString();
            Description = errorCode.GetDescription();
        }

        public string Code { get; set; }

        public string Description { get; set; }
    }
}