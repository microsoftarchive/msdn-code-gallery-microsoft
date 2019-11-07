namespace Microsoft.Exchange.Samples.Autodiscover
{
    class PoxXmlStrings
    {
        // This sample is for demonstration purposes only. Before you run this sample, make sure 
        // that the code meets the coding requirements of your organization. 
        #region Namespaces
        public static string OutlookRequestSchema = "{http://schemas.microsoft.com/exchange/autodiscover/outlook/requestschema/2006}";
        public static string OutlookResponseSchema = "{http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a}";
        #endregion

        #region Element names
        public static string Autodiscover = OutlookRequestSchema + "Autodiscover";
        public static string Request = OutlookRequestSchema + "Request";
        public static string EMailAddress = OutlookRequestSchema + "EMailAddress";
        public static string LegacyDN = OutlookRequestSchema + "LegacyDN";
        public static string AcceptableResponseSchema = OutlookRequestSchema + "AcceptableResponseSchema";
        public static string Error = OutlookResponseSchema + "Error";
        public static string ErrorCode = OutlookResponseSchema + "ErrorCode";
        public static string Message = OutlookResponseSchema + "Message";
        public static string Action = OutlookResponseSchema + "Action";
        public static string Protocol = OutlookResponseSchema + "Protocol";
        public static string Type = OutlookResponseSchema + "Type";
        public static string ASUrl = OutlookResponseSchema + "ASUrl";
        public static string EwsUrl = OutlookResponseSchema + "EwsUrl";
        public static string RedirectAddr = OutlookResponseSchema + "RedirectAddr";
        public static string RedirectUrl = OutlookResponseSchema + "RedirectUrl";
        #endregion
    }
}
