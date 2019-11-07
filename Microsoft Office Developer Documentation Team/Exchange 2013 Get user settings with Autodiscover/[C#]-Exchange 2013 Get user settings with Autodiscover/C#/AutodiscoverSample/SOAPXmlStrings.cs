namespace Microsoft.Exchange.Samples.Autodiscover
{
    // This sample is for demonstration purposes only. Before you run this sample, make sure 
    // that the code meets the coding requirements of your organization. 
    class SoapXmlStrings
    {
        #region Namespaces
        public static string SoapNamespace = "{http://schemas.xmlsoap.org/soap/envelope/}";
        public static string AutodiscoverNamespace = "{http://schemas.microsoft.com/exchange/2010/Autodiscover}";
        public static string AddressingNamespace = "{http://www.w3.org/2005/08/addressing}";
        #endregion

        #region Element Names
        public static string Envelope = SoapNamespace + "Envelope";
        public static string Header = SoapNamespace + "Header";
        public static string RequestedServerVersion = AutodiscoverNamespace + "RequestedServerVersion";
        public static string Action = AddressingNamespace + "Action";
        public static string To = AddressingNamespace + "To";
        public static string Body = SoapNamespace + "Body";
        public static string GetUserSettingsRequestMessage = AutodiscoverNamespace + "GetUserSettingsRequestMessage";
        public static string Request = AutodiscoverNamespace + "Request";
        public static string Users = AutodiscoverNamespace + "Users";
        public static string User = AutodiscoverNamespace + "User";
        public static string Mailbox = AutodiscoverNamespace + "Mailbox";
        public static string RequestedSettings = AutodiscoverNamespace + "RequestedSettings";
        public static string Setting = AutodiscoverNamespace + "Setting";
        public static string GetUserSettingsResponseMessage = AutodiscoverNamespace + "GetUserSettingsResponseMessage";
        public static string Response = AutodiscoverNamespace + "Response";
        public static string ErrorCode = AutodiscoverNamespace + "ErrorCode";
        public static string ErrorMessage = AutodiscoverNamespace + "ErrorMessage";
        public static string UserResponses = AutodiscoverNamespace + "UserResponses";
        public static string UserResponse = AutodiscoverNamespace + "UserResponse";
        public static string RedirectTarget = AutodiscoverNamespace + "RedirectTarget";
        public static string UserSettingErrors = AutodiscoverNamespace + "UserSettingErrors";
        public static string UserSettings = AutodiscoverNamespace + "UserSettings";
        public static string UserSetting = AutodiscoverNamespace + "UserSetting";
        public static string Name = AutodiscoverNamespace + "Name";
        public static string Value = AutodiscoverNamespace + "Value";
        #endregion

        public static string MinServerVersion = "Exchange2010";
    }
}
