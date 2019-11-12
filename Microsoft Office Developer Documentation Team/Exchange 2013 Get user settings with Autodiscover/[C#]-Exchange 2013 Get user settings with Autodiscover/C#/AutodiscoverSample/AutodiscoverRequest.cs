using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Microsoft.Exchange.Samples.Autodiscover
{
    // This sample is for demonstration purposes only. Before you run this sample, make sure 
    // that the code meets the coding requirements of your organization. 
    class AutodiscoverRequest
    {
        #region Private members
        // The maximum number of times to restart when
        // a redirect to a new email address is received.
        private const int MaxAddressRedirects = 2;
        // The maximum number of times to follow a
        // redirect to a new URL.
        private const int MaxUrlRedirects = 3;

        // The email address being used for Autodiscover.
        private string EmailAddress = null;
        // The credentials used to authenticate to the Autodiscover server.
        private NetworkCredential Credentials = null;
        // Used to generate and manage the list of potential Autodiscover URL.
        private AutodiscoverUrlList UrlList = null;
        // The current number of times you have restarted for an email
        // address redirect.
        private int AddressRedirects = 0;
        // The number of times you have followed a URL redirect.
        private int UrlRedirects = 0;
        // A list of URLs you have already attempted.
        private List<string> visitedUrls = new List<string>();
        #endregion

        public AutodiscoverRequest(string email, NetworkCredential creds)
        {
            EmailAddress = email;
            Credentials = creds;
            UrlList = new AutodiscoverUrlList();
        }

        // DoAutodiscover
        //   This function initializes the URL list and starts
        //   the Autodiscover process.
        //
        // Parameters:
        //   useSoapEndpoints: Indicates whether SOAP endpoints should be tried.
        //
        // Returns:
        //   A Dictionary object that contains the settings returned by the 
        //   Autodiscover process. If null, Autodiscover failed.
        //
        public Dictionary<string,string> DoAutodiscover(bool useSoapEndpoints)
        {
            Tracing.WriteLine("Generating endpoint list for " + EmailAddress);
            UrlList.GenerateList(EmailAddress);

            return TryAutodiscoverUrls(useSoapEndpoints);
        }

        // TryAutodiscoverUrls
        //   This function loops through the URL list and tries
        //   each URL. If none of the URLs work, it checks a nonsecured
        //   endpoint for a 302 redirect response.
        //
        // Parameters:
        //   useSoapEndpoints: Indicates whether SOAP endpoints should be tried.
        //
        // Returns:
        //   A Dictionary object that contains the settings returned by the 
        //   Autodiscover process. If null, the URLs failed.
        //
        public Dictionary<string, string> TryAutodiscoverUrls(bool useSoapEndpoints)
        {
            Dictionary<string, string> settings = null;

            // Loop through the list of URLs.
            foreach (string url in UrlList)
            {
                settings = TryAutodiscoverUrl(url, useSoapEndpoints);
                if (settings != null)
                    break;
            }

            if (settings == null)
            {
                // No settings were returned, so none of the URLs worked.
                // Check the nonsecured endpoint for a 302 redirect.
                settings = CheckNonSecuredEndpointForRedirect(useSoapEndpoints);
            }

            return settings;
        }

        // TryAutodiscoverUrl
        //   This function trys a URL as a SOAP endpoint (if applicable), then as a POX
        //   endpoint.
        //
        // Parameters:
        //   url: The URL to try. Note that this value is a "generic" URL, meaning
        //        it has no file extension. ("https://contoso.com/autodiscover/autodiscover")
        //   useSoapEndpoints: Indicates whether SOAP endpoints should be tried.
        //
        // Returns:
        //   A Dictionary object that contains the settings returned by the 
        //   Autodiscover process. If null, the URLs failed.
        //
        public Dictionary<string, string> TryAutodiscoverUrl(string url, bool useSoapEndpoints)
        {
            // If you have already tried this URL, don't try again.
            if (visitedUrls.Contains(url))
            {
                Console.WriteLine("Already attempted to use " + url + ". Skipping.");
                return null;
            }

            visitedUrls.Add(url);

            Dictionary<string, string> settings = null;

            // Try SOAP first, unless instructed not to.
            if (useSoapEndpoints)
            {
                settings = TrySoapAutodiscoverUrl(url + ".svc", useSoapEndpoints);
            }

            // Try POX if SOAP failed or was skipped.
            if (settings == null)
            {
                settings = TryPoxAutodiscoverUrl(url + ".xml", useSoapEndpoints);
            }

            return settings;
        }

        #region SOAP

        // TrySoapAutodiscoverUrl
        //   This function trys a URL as a SOAP endpoint.
        //   NOTE: The SOAP Autodiscover service is only available in versions of Exchange starting
        //   with Exchange 2010, including Exchange Online.
        //
        // Parameters:
        //   url: The URL to try. Note that this value is a SOAP URL, meaning
        //        it has a .svc file extension. ("https://contoso.com/autodiscover/autodiscover.svc")
        //   useSoapEndpoints: Indicates whether SOAP endpoints should be tried.
        //
        // Returns:
        //   A Dictionary object that contains the settings returned by the 
        //   Autodiscover process. If null, the URL failed.
        //
        private Dictionary<string,string> TrySoapAutodiscoverUrl(string url, bool useSoapEndpoints)
        {
            Tracing.WriteLine("Trying " + url);

            Dictionary<string, string> settingsDictionary = null;

            // Generate the SOAP request.
            XElement envelope = new XElement(SoapXmlStrings.Envelope,
                new XElement(SoapXmlStrings.Header,
                    new XElement(SoapXmlStrings.RequestedServerVersion, SoapXmlStrings.MinServerVersion),
                    new XElement(SoapXmlStrings.Action, "http://schemas.microsoft.com/exchange/2010/Autodiscover/Autodiscover/GetUserSettings"),
                    new XElement(SoapXmlStrings.To, url)
                ),
                new XElement(SoapXmlStrings.Body,
                    new XElement(SoapXmlStrings.GetUserSettingsRequestMessage,
                        new XElement(SoapXmlStrings.Request,
                            new XElement(SoapXmlStrings.Users,
                                new XElement(SoapXmlStrings.User,
                                    new XElement(SoapXmlStrings.Mailbox, EmailAddress)
                                )
                            ),
                            new XElement(SoapXmlStrings.RequestedSettings,
                                new XElement(SoapXmlStrings.Setting, "InternalEwsUrl"),
                                new XElement(SoapXmlStrings.Setting, "ExternalEwsUrl"),
                                new XElement(SoapXmlStrings.Setting, "ExternalEwsVersion"),
                                new XElement(SoapXmlStrings.Setting, "EwsSupportedSchemas")
                            )
                        )
                    )
                )
            );

            Tracing.WriteLine("SOAP Request:");
            PrintElement(envelope);

            try
            {
                HttpWebRequest soapRequest = (HttpWebRequest)WebRequest.Create(url);
                soapRequest.AllowAutoRedirect = false;
                soapRequest.Credentials = this.Credentials;
                soapRequest.Method = "POST";
                soapRequest.ContentType = "text/xml";

                Stream requestStream = soapRequest.GetRequestStream();
                envelope.Save(requestStream);
                requestStream.Close();

                HttpWebResponse soapResponse = (HttpWebResponse)soapRequest.GetResponse();
                if (soapResponse.StatusCode == HttpStatusCode.OK)
                {
                    // Successful response.
                    Stream responseStream = soapResponse.GetResponseStream();

                    XElement responseEnvelope = XElement.Load(responseStream);
                    if (responseEnvelope != null)
                    {
                        Tracing.WriteLine("Response:");
                        PrintElement(responseEnvelope);

                        // If no errors, it is safe to proceed.
                        string error = CheckSoapResponseForError(responseEnvelope);
                        if (error == "NoError")
                        {
                            settingsDictionary = new Dictionary<string, string>();

                            // Load settings.
                            IEnumerable<XElement> userSettings = from userSetting in responseEnvelope.Descendants
                                                                    (SoapXmlStrings.UserSetting)
                                                                 select userSetting;

                            foreach (XElement userSetting in userSettings)
                            {
                                XElement name = userSetting.Element(SoapXmlStrings.Name);
                                XElement value = userSetting.Element(SoapXmlStrings.Value);

                                if (name != null && value != null)
                                {
                                    settingsDictionary.Add(name.Value, value.Value);
                                }
                            }
                        }
                        else if (error == "RedirectAddress")
                        {
                            // The server has given you a better email
                            // address to use.
                            Tracing.WriteLine("Server response contains a RedirectAddress error.");

                            XElement redirectTarget = FindFirstDescendant(responseEnvelope,
                                SoapXmlStrings.RedirectTarget);

                            if (redirectTarget != null &&
                                IsValidRedirectAddress(redirectTarget.Value, EmailAddress))
                            {
                                Tracing.WriteLine("Restarting Autodiscover with email address: " +
                                    redirectTarget.Value);

                                EmailAddress = redirectTarget.Value;
                                UrlList.Clear();
                                UrlList.GenerateList(this.EmailAddress);

                                visitedUrls.Clear();
                                AddressRedirects++;
                                return TryAutodiscoverUrls(useSoapEndpoints);
                            }
                            Tracing.WriteLine("Invalid or missing RedirectTarget element, continuing...");
                        }
                        else if (error == "RedirectUrl")
                        {
                            // The server has given you a better URL
                            // to use. Validate that it is https and
                            // try it.
                            Tracing.Write("Server response contains a redirect to URL: ");

                            XElement redirectTarget = FindFirstDescendant(responseEnvelope,
                                SoapXmlStrings.RedirectTarget);

                            if (redirectTarget != null && IsValidRedirectUrl(redirectTarget.Value.ToLower()))
                            {
                                Tracing.WriteLine(redirectTarget.Value);

                                UrlRedirects++;

                                return TryAutodiscoverUrl(
                                    AutodiscoverUrlList.NormalizeAutodiscoverUrl(redirectTarget.Value),
                                    useSoapEndpoints);
                            }
                            Tracing.WriteLine("Invalid or missing RedirectTarget element, continuing...");
                            return null;
                        }
                    }
                }
                else if (soapResponse.StatusCode == HttpStatusCode.Redirect ||
                         soapResponse.StatusCode == HttpStatusCode.Moved ||
                         soapResponse.StatusCode == HttpStatusCode.RedirectKeepVerb ||
                         soapResponse.StatusCode == HttpStatusCode.RedirectMethod)
                {
                    // Redirect HTTP status scenario.
                    Tracing.WriteLine("Received a redirect status: " + soapResponse.StatusCode.ToString());
                    string redirectUrl = soapResponse.Headers["Location"].ToString();
                    Tracing.WriteLine("Location header: " +
                        (string.IsNullOrEmpty(redirectUrl) ? "MISSING" : redirectUrl));
                    if (IsValidRedirectUrl(redirectUrl.ToLower()))
                    {
                        UrlRedirects++;
                        return TryAutodiscoverUrl(
                            AutodiscoverUrlList.NormalizeAutodiscoverUrl(redirectUrl),
                            useSoapEndpoints);
                    }
                    Tracing.WriteLine("Invalid (non-https) redirect URL returned. Unable to proceed.");
                    return null;
                }

            }
            catch (WebException e)
            {
                // Some errors will be exposed as WebExceptions.
                // For example, 401 (Unauthorized)
                // 302 should not generate a WebException and is handled above.
                Tracing.WriteLine("Error connecting:");
                Tracing.WriteLine(e.ToString());
            }

            return settingsDictionary;
        }

        // CheckSoapResponseForError
        //   This function searches an XML response for an ErrorCode element.
        //
        // Parameters:
        //   responseEnvelope: An XElement object that contains the Envelope element of the
        //   SOAP response.
        //
        // Returns:
        //   A string that contains the text of the first ErrorCode element, or "NoError"
        //   if no ErrorCode element is present.
        //
        private string CheckSoapResponseForError(XElement responseEnvelope)
        {
            // Get error codes and check for errors.
            IEnumerable<XElement> errorCodes = from errorCode in responseEnvelope.Descendants
                                                   (SoapXmlStrings.ErrorCode)
                                               select errorCode;

            foreach (XElement errorCode in errorCodes)
            {
                if (errorCode.Value != "NoError")
                {
                    switch (errorCode.Parent.Name.LocalName.ToString())
                    {
                        case "Response":
                            Tracing.WriteLine("Response-level error: " + errorCode.Value);
                            return errorCode.Value;
                        case "UserResponse":
                            Tracing.WriteLine("User-level error: " + errorCode.Value);
                            return errorCode.Value;
                    }
                }
            }

            return "NoError";
        }
        #endregion

        #region POX
        // TryPoxAutodiscoverUrl
        //   This function trys a URL as a POX endpoint.
        //
        // Parameters:
        //   url: The URL to try. Note that this value is a POX URL, meaning
        //        it has a .xml file extension. ("https://contoso.com/autodiscover/autodiscover.xml")
        //   useSoapEndpoints: Indicates whether SOAP endpoints should be tried.
        //
        // Returns:
        //   A Dictionary object that contains the settings returned by the 
        //   Autodiscover process. If null, the URL failed.
        //
        private Dictionary<string, string> TryPoxAutodiscoverUrl(string url, bool useSoapEndpoints)
        {
            Tracing.WriteLine("Trying " + url);

            Dictionary<string, string> settingsDictionary = null;

            // Generate the POX request.
            XElement autodiscover = new XElement(PoxXmlStrings.Autodiscover,
                new XElement(PoxXmlStrings.Request,
                    new XElement(PoxXmlStrings.EMailAddress, EmailAddress),
                    new XElement(PoxXmlStrings.AcceptableResponseSchema, "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a")
                )
            );

            Tracing.WriteLine("POX Request:");
            PrintElement(autodiscover);

            try
            {
                HttpWebRequest poxRequest = (HttpWebRequest)WebRequest.Create(url);
                poxRequest.AllowAutoRedirect = false;
                poxRequest.Credentials = this.Credentials;
                poxRequest.Method = "POST";
                poxRequest.ContentType = "text/xml";

                Stream requestStream = poxRequest.GetRequestStream();
                autodiscover.Save(requestStream);
                requestStream.Close();

                HttpWebResponse poxResponse = (HttpWebResponse)poxRequest.GetResponse();
                if (poxResponse.StatusCode == HttpStatusCode.OK)
                {
                    // Successful response.
                    Stream responseStream = poxResponse.GetResponseStream();

                    XElement responseAutodiscover = XElement.Load(responseStream);
                    if (responseAutodiscover != null)
                    {
                        Tracing.WriteLine("Response:");
                        PrintElement(responseAutodiscover);

                        // If no errors, it is safe to proceed.
                        string error = CheckPoxResponseForError(responseAutodiscover);
                        if (error == "NoError")
                        {
                            // Check Action value.
                            XElement action = FindFirstDescendant(responseAutodiscover, PoxXmlStrings.Action);
                            if (action != null)
                            {
                                if (action.Value == "settings")
                                {
                                    settingsDictionary = new Dictionary<string, string>();

                                    IEnumerable<XElement> protocols = from protocol in responseAutodiscover.Descendants
                                                                            (PoxXmlStrings.Protocol)
                                                                      select protocol;
                                    foreach (XElement protocol in protocols)
                                    {
                                        XElement type = protocol.Element(PoxXmlStrings.Type);
                                        if (type != null && type.Value == "EXCH")
                                        {
                                            // You found what you are looking for.
                                            AddPoxSettingToDictionary(settingsDictionary, protocol, PoxXmlStrings.EwsUrl);
                                            break;
                                        }
                                    }
                                }
                                else if (action.Value == "redirectAddr")
                                {
                                    // The server has given you a better email address to use.
                                    Tracing.WriteLine("Server response contains a RedirectAddr element.");
                                    XElement redirectAddr = FindFirstDescendant(responseAutodiscover,
                                        PoxXmlStrings.RedirectAddr);

                                    if (redirectAddr != null &&
                                        IsValidRedirectAddress(redirectAddr.Value, EmailAddress))
                                    {
                                        Tracing.WriteLine("Restarting Autodiscover with email address: " +
                                            redirectAddr.Value);

                                        EmailAddress = redirectAddr.Value;
                                        UrlList.Clear();
                                        UrlList.GenerateList(this.EmailAddress);

                                        visitedUrls.Clear();
                                        AddressRedirects++;
                                        return TryAutodiscoverUrls(useSoapEndpoints);
                                    }
                                    Tracing.WriteLine("Invalid or missing redirectAddr element, continuing...");
                                }
                                else if (action.Value == "redirectUrl")
                                {
                                    // The server has given you a better URL
                                    // to use. Validate that it is https and
                                    // try it.
                                    Tracing.Write("Server response contains a RedirectUrl element for url: ");

                                    XElement redirectUrl = FindFirstDescendant(responseAutodiscover,
                                        PoxXmlStrings.RedirectUrl);

                                    if (redirectUrl != null && IsValidRedirectUrl(redirectUrl.Value.ToLower()))
                                    {
                                        Tracing.WriteLine(redirectUrl.Value);

                                        UrlRedirects++;
                                        return TryAutodiscoverUrl(
                                            AutodiscoverUrlList.NormalizeAutodiscoverUrl(redirectUrl.Value),
                                            useSoapEndpoints);
                                    }
                                    else
                                    {
                                        Tracing.WriteLine("Missing or invalid RedirectUrl element, continuing...");
                                    }
                                }
                            }
                        }
                    }
                }
                else if (poxResponse.StatusCode == HttpStatusCode.Redirect ||
                            poxResponse.StatusCode == HttpStatusCode.Moved ||
                            poxResponse.StatusCode == HttpStatusCode.RedirectKeepVerb ||
                            poxResponse.StatusCode == HttpStatusCode.RedirectMethod)
                {
                    // Redirect HTTP status scenario.
                    Tracing.WriteLine("Received a redirect status: " + poxResponse.StatusCode.ToString());
                    string redirectUrl = poxResponse.Headers["Location"].ToString();
                    Tracing.WriteLine("Location header: " +
                        (string.IsNullOrEmpty(redirectUrl) ? "MISSING" : redirectUrl));
                    if (IsValidRedirectUrl(redirectUrl.ToLower()))
                    {
                        UrlRedirects++;
                        return TryAutodiscoverUrl(AutodiscoverUrlList.NormalizeAutodiscoverUrl(redirectUrl),
                            useSoapEndpoints);
                    }

                    Tracing.WriteLine("Invalid or missing redirect URL returned. Unable to proceed.");
                    return null;
                }
            }
            catch (WebException e)
            {
                Tracing.WriteLine("Error connection:");
                Tracing.WriteLine(e.ToString());
            }

            return settingsDictionary;
        }

        // CheckPoxResponseForError
        //   This function searches an XML response for an Error element.
        //
        // Parameters:
        //   responseAutodiscover: An XElement that contains the Autodiscover element
        //   of the POX response.
        //
        // Returns:
        //   A string that contains the text of the ErrorCode element combined with
        //   the text of the Message element.
        //
        private string CheckPoxResponseForError(XElement responseAutodiscover)
        {
            string returnValue = "NoError";

            // Get error nodes and check for errors.
            XElement error = FindFirstDescendant(responseAutodiscover, PoxXmlStrings.Error);

            if (error != null)
            {
                XElement errorCode = error.Element(PoxXmlStrings.ErrorCode);
                XElement message = error.Element(PoxXmlStrings.Message);

                if (errorCode != null || message != null)
                {
                    returnValue = (errorCode != null ? errorCode.Value + ": " : "") +
                        (message != null ? message.Value : "");
                }
            }

            return returnValue;
        }

        // AddPoxSettingToDictionary
        //   This function searches a Protocol element for a setting with a specific
        //   name and, if found, adds it to the dictionary.
        //
        // Parameters:
        //   dictionary: The Dictionary object to add the setting to.
        //   protocol: An XElement object that contains the Protocol element of the
        //             Autodiscover resopnse.
        //   settingName: The name of the setting to search for.
        //
        // Returns:
        //   None.
        //
        private void AddPoxSettingToDictionary(Dictionary<string, string> dictionary, 
            XElement protocol, string settingName)
        {
            XElement setting = FindFirstDescendant(protocol, settingName);
            if (setting != null)
            {
                dictionary.Add(setting.Name.LocalName, setting.Value);
            }
        }
        #endregion

        #region Redirects
        // CheckNonSecuredEndpointForRedirect
        //   This function does an unauthenticated GET request to a non-https
        //   URL to see if the server will respond with a 302 redirect. If so, it will
        //   attempt to use the value of the Location header to perform Autodiscover.
        //
        // Parameters:
        //   useSoapEndpoints: Indicates whether SOAP endpoints should be tried.
        //
        // Returns:
        //   A Dictionary object that contains the settings returned by the 
        //   Autodiscover process. If null, either the redirect failed or the URL failed.
        //
        private Dictionary<string, string> CheckNonSecuredEndpointForRedirect(bool useSoapEndpoints)
        {
            // Get the non-https URL, or "last ditch" URL.
            string nonHttpsEndpoint = UrlList.GetLastDitchUrl(EmailAddress);
            Tracing.WriteLine("Sending non-authenticated GET to " + nonHttpsEndpoint);

            HttpWebRequest getRequest = (HttpWebRequest)WebRequest.Create(nonHttpsEndpoint);

            getRequest.Method = "GET";
            getRequest.AllowAutoRedirect = false;
            getRequest.PreAuthenticate = false;

            try
            {
                HttpWebResponse getResponse = (HttpWebResponse)getRequest.GetResponse();
                if (getResponse != null)
                {
                    if (getResponse.StatusCode == HttpStatusCode.Redirect ||
                        getResponse.StatusCode == HttpStatusCode.Moved ||
                        getResponse.StatusCode == HttpStatusCode.RedirectKeepVerb ||
                        getResponse.StatusCode == HttpStatusCode.RedirectMethod)
                    {
                        Tracing.WriteLine("Received a redirect status: " + getResponse.StatusCode.ToString());
                        string redirectUrl = getResponse.Headers["Location"].ToString();
                        Tracing.WriteLine("Location header: " +
                            (string.IsNullOrEmpty(redirectUrl) ? "MISSING" : redirectUrl));
                        if (IsValidRedirectUrl(redirectUrl))
                        {
                            // You got a valid redirect; try it.
                            UrlRedirects++;
                            return TryAutodiscoverUrl(AutodiscoverUrlList.NormalizeAutodiscoverUrl(redirectUrl),
                                useSoapEndpoints);
                        }
                        else
                        {
                            Tracing.WriteLine("Redirect returned missing or invalid URL, unable to proceed.");
                        }
                    }
                    else
                    {
                        Tracing.WriteLine("Received a non-redirect status: " + getResponse.StatusCode.ToString());
                    }
                }
            }
            catch (WebException e)
            {
                Tracing.WriteLine("Unable to connect.");
                Tracing.WriteLine(e.ToString());
            }

            return null;
        }

        // IsValidRedirectAddress
        //   This function checks that the value isn't empty or null, and that it does
        //   not match the current email address.
        //
        // Parameters:
        //   redirectEmailAddress: The redirected email address to validate.
        //   currentEmailAddress: The current email address.
        //
        // Returns:
        //   A Boolean value that is true if the address is valid, false if not.
        //
        private bool IsValidRedirectAddress(string redirectEmailAddress, string currentEmailAddress)
        {
            if (!string.IsNullOrEmpty(redirectEmailAddress))
            {
                if (redirectEmailAddress.ToLower() != currentEmailAddress.ToLower())
                {
                    return true;
                }
                // If they do match, consider this invalid. The server
                // has returned the same address you sent it as a redirect, so
                // something is wrong.
                Tracing.WriteLine("Redirected address matches what was sent, redirect is invalid.");
            }

            return false;
        }

        // IsValidRedirectUrl
        //   This function checks that the value isn't null or empty, makes sure
        //   that the URL is an https URL, and checks that you have not already tried
        //   this URL.
        //
        // Parameters:
        //   redirectUrl: The URL to validate.
        //
        // Return:
        //   A Boolean value that is true if the URL is valid, false if not.
        //
        private bool IsValidRedirectUrl(string redirectUrl)
        {
            return (!string.IsNullOrEmpty(redirectUrl) &&
                    redirectUrl.ToLower().StartsWith("https://") &&
                    !visitedUrls.Contains(redirectUrl));
        }
        #endregion

        #region XML Helpers
        // FindFirstDescendant
        //   This function searches the children of a parent XML element for a child
        //   element of a given name. It returns the first child element found.
        //
        // Parameters:
        //   parentElement: An XElement object that contains the parent element.
        //   descendantName: The name of the child to search for.
        //
        // Returns:
        //   An XElement that contains the first child found. The value is null if
        //   no child with the given name is found.
        //
        private XElement FindFirstDescendant(XElement parentElement, string descendantName)
        {
            IEnumerable<XElement> descendants = from descendant in parentElement.Descendants
                                                    (descendantName)
                                                select descendant;
            foreach (XElement descendant in descendants)
            {
                return descendant;
            }

            return null;
        }

        // PrintElement
        //   This function prints the contents of an XML element with indents.
        //
        // Parameters:
        //   element: An XElement object that contains the element to print.
        //
        // Returns:
        //   None.
        //
        private void PrintElement(XElement element)
        {
            StringBuilder stringBuilder = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            XmlWriter writer = XmlWriter.Create(stringBuilder, settings);
            element.Save(writer);
            writer.Close();

            Tracing.WriteLine(stringBuilder.ToString());
        }
        #endregion
    }
}
