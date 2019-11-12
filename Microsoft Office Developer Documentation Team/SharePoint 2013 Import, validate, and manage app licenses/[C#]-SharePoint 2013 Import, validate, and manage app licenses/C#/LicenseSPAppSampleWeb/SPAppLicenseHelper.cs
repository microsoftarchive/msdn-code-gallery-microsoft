using LicenseSPAppSampleWeb.LicenseVerificationService;
using Microsoft.SharePoint.Client;
using System;
using System.Web;
using System.Xml;

namespace LicenseSPAppSampleWeb
{
    /// <summary>
    /// Helper enum to categorize a license in Free, Paid, Trial or Uknown
    /// </summary>
    public enum SPAppLicenseType { Free, Paid, Trial, Uknown }
            
    /// <summary>
    /// Helper class to manipulate app licenses
    /// </summary>
    public class SPAppLicenseHelper
    {
        //Resource keys to pull strings
        public static string _trialUnlimitedStringKey = "trialUnlimitedLicenseWarning";
        public static string _freeStringKey = "freeLicenseWarning";
        public static string _paidStringKey = "paidLicenseMessage";
        public static string _trialStringKey = "trialLimitedLicenseWarning";
        public static string _trialExpiredStringKey = "trialExpiredWarning";
        public static string _noLicenseStringKey = "noLicenseWarning";
        public static string _invalidLicenseStringKey = "invalidLicenseWarning";

        /// <summary>
        /// Returns the corresponding license type given a verified license. Note that verified != valid
        /// </summary>
        /// <param name="verifiedLicense">A license object that has already been verified by the Store web service</param>
        public static SPAppLicenseType GetLicenseTypeFromLicense(VerifyEntitlementTokenResponse verifiedLicense)
        {
            string entitlementType = verifiedLicense.EntitlementType.ToLower();
            return GetLicenseTypeFromString(entitlementType);
        }

        /// <summary>
        /// Translates a string (e.g. "free") into the corresponding enum type. 
        /// </summary>
        /// <param name="licenseTypeString"></param>
        /// <returns></returns>
        public static SPAppLicenseType GetLicenseTypeFromString(String licenseTypeString)
        {

            switch (licenseTypeString.ToLower())
            {
                case "free":
                    return SPAppLicenseType.Free;
                case "paid":
                    return SPAppLicenseType.Paid;
                case "trial":
                    return SPAppLicenseType.Trial;
                default:
                    return SPAppLicenseType.Uknown;
            }
        }

        /// <summary>
        /// Returns the number of days this license is valid (can return negative if expired).
        /// If you want an extra level of granularity you shouldn't use days but instead look at the Time. 
        /// </summary>
        /// <param name="verifiedLicense"></param>
        public static int GetRemainingDays(VerifyEntitlementTokenResponse verifiedLicense)
        {
            DateTime licenseExpirationDate = (DateTime)verifiedLicense.EntitlementExpiryDate;
            if (licenseExpirationDate == DateTime.MaxValue)
            {
                //Unlimited trial, return max int
                return int.MaxValue;
            }
            else
            {
                int remainingDays = licenseExpirationDate.Subtract(DateTime.UtcNow).Days;
                return remainingDays;
            }
        }

        /// <summary>
        /// Returs a resource string given the supplied key
        /// </summary>
        /// <param name="key">Resource key</param>
        private static string GetLicenseStringFromResource(string  key)
        {
           return (String) HttpContext.GetGlobalResourceObject("LicensingResources", key);
        }

        /// <summary>
        /// Returns a ready to display UI string for a given license. Includes a link to the storefront if necessary
        /// </summary>
        /// <param name="verifiedLicense">A license object that has already been verified by the Store web service</param>
        /// <param name="hostWebUrl">The URL of the web where the app is installed. Get this from SP tokens</param>
        /// <param name="currentPageUrl">The URL of the current page (so users can return to it from the storefront); must be same domain as hostWeb</param>
        /// <param name="appDisplayName">The title of the breadcrum that storefront will display on the back link</param>
        /// <returns></returns>
        public static string GetUIStringText(VerifyEntitlementTokenResponse verifiedLicense, string hostWebUrl, string currentPageUrl, string appDisplayName, string additionalFeaturesFullVersion)
        {
            string uiWarning = String.Empty;     
      
            if (verifiedLicense == null)
            {

                uiWarning = String.Format(GetLicenseStringFromResource(_noLicenseStringKey), appDisplayName, "<a href='" +GetStoreSearchUrl(appDisplayName,hostWebUrl,currentPageUrl) + "'>" , "</a>", additionalFeaturesFullVersion );          
            }
            else
            {
                string storeFrontUrl = SPAppLicenseHelper.GetStorefrontUrl(verifiedLicense, hostWebUrl, currentPageUrl, appDisplayName);
                SPAppLicenseType licenseType = GetLicenseTypeFromLicense(verifiedLicense);

                switch (licenseType)
                {
                    case SPAppLicenseType.Free:
                        uiWarning = String.Format(GetLicenseStringFromResource(_freeStringKey), appDisplayName, "<a href='" + storeFrontUrl + "'>", "</a>");          
                        break;
                    case SPAppLicenseType.Paid:
                        uiWarning = String.Format(GetLicenseStringFromResource(_paidStringKey), appDisplayName,"<a href='" + storeFrontUrl + "'>", "</a>");
                        break;
                    case SPAppLicenseType.Trial:
                        int remainingDays = GetRemainingDays(verifiedLicense);
                        if (remainingDays == int.MaxValue)
                        {
                            //Unlimited time trial
                            uiWarning = String.Format(GetLicenseStringFromResource(_trialUnlimitedStringKey), appDisplayName, "<a href='" + storeFrontUrl + "'>", "</a>", additionalFeaturesFullVersion);
                        }
                        else
                        {
                            //Time limited trial
                            if (remainingDays > 0)
                            {
                                uiWarning = String.Format(GetLicenseStringFromResource(_trialStringKey), appDisplayName, remainingDays, "<a href='" + storeFrontUrl + "'>", "</a>", additionalFeaturesFullVersion);
                            }
                            else
                            {
                                //Expired trial
                                uiWarning = String.Format(GetLicenseStringFromResource(_trialExpiredStringKey), "<a href='" + storeFrontUrl + "'>", "</a>");
                            }        
                        }

                        break;
                }
            }
            
            return uiWarning;
        }



        /// <summary>
        /// Returns the URL (string) of the SharePoint storefront for a given app. It uses the license to extract the corresponding AssetId
        /// </summary>
        /// <param name="verifiedLicense">A license object that has already been verified by the Store web service</param>
        /// <param name="hostWebUrl">The URL of the web where the app is installed. Get this from SP tokens</param>
        /// <param name="currentPageUrl">The URL of the current page (so users can return to it from the storefront); must be same domain as hostWeb</param>
        /// <param name="appName">The title of the breadcrum that storefront will display on the back link</param>
        public static string GetStorefrontUrl(VerifyEntitlementTokenResponse verifiedLicense, string hostWebUrl, string currentPageUrl, string appName)
        {
            String storeTemplateString = "{0}/_layouts/15/storefront.aspx?source={1}&sname={2}&#vw=AppDetailsView,app={3},clg=0,cm=en-US";

            //Note: If you are using the hardcoded token provided in the sample this URL will always point to the Cheezburgers app. 
            return String.Format(storeTemplateString, hostWebUrl, currentPageUrl, appName,verifiedLicense.AssetId);
     
        }

        /// <summary>
        /// Returns the URL of the storefront for a given app name title.
        /// To get an exact match you must pass the exact name of the app as published on the store. 
        /// </summary>
        /// <param name="appName"></param>
        public static string GetStoreSearchUrl(string appName, string hostWebUrl, string currentPageUrl)
        {
            String storeSearchTemplateString ="{0}/_layouts/15/storefront.aspx?source={1}&sname={2}#qry={3}";
            return String.Format(storeSearchTemplateString, hostWebUrl, currentPageUrl, appName, appName);  
   
        }

        /// <summary>
        /// Retrieves a license from SP and calls the store verification service to process it. 
        /// Note that a verified license is NOT necessarily a valid license. You MUST still check the license properties to verify its validity
        /// </summary>
        /// <param name="productId">The ProductId of your app, get this from your AppManifest.xml</param>
        /// <param name="ctx"> An already wired up SP ClientContext object</param>
        public static VerifyEntitlementTokenResponse GetAndVerifyLicense(Guid productId, ClientContext ctx)
        {
            //Retrieve license from SharePoint
            string rawLicense = GetLicenseTokenFromSharePoint(productId, ctx);

            if (String.IsNullOrEmpty(rawLicense))
            {
                return null;// No license
            }

            //Validate license with the store
            VerifyEntitlementTokenResponse storeLicense = GetValidatedLicenseFromStore(rawLicense);
            return storeLicense;

        }

        /// <summary>
        /// Returns a license token from SharePoint. It is NOT recommended to read the token without verifying it first with the Store. 
        /// Use GetAndVerifyLicense instead. 
        /// </summary>
        /// <param name="productId">The ProductId of your app, get this from your AppManifest.xml</param>
        /// <param name="ctx"> An already wired up SP ClientContext object</param>
        private static string GetLicenseTokenFromSharePoint(Guid productId, ClientContext clientContext)
        {
            //Get the license from SP
            ClientResult<AppLicenseCollection> licenseCollection = Microsoft.SharePoint.Client.Utilities.Utility.GetAppLicenseInformation(clientContext, productId);
            clientContext.Load(clientContext.Web);
            clientContext.ExecuteQuery();

            string rawLicenseToken = null;

            foreach (AppLicense license in licenseCollection.Value)
            {
                //just get the first license; you could also traverse all licenses if required but usually the top one is enough because it the most 'relevant' 
                rawLicenseToken = license.RawXMLLicenseToken;
                break;
            }
            return (rawLicenseToken);
        }

        /// <summary>
        /// Takes a token from SP and verifies it with the Store using the verification web service. 
        /// </summary>
        /// <param name="rawLicenseToken">XML string containing a well-formed license token</param>
        private static VerifyEntitlementTokenResponse GetValidatedLicenseFromStore(string rawLicenseToken)
        {
            VerificationServiceClient service = null;
            VerifyEntitlementTokenResponse result = null;
            VerifyEntitlementTokenRequest request = new VerifyEntitlementTokenRequest();
            request.EntitlementToken = rawLicenseToken;

            service = new VerificationServiceClient();
            result = service.VerifyEntitlementToken(request);
            return result;
        }
    
        /// <summary>
        /// Utility method to add XML attributes to an XML document
        /// </summary>
        /// <param name="inputXml">XML document in string form</param>
        /// <param name="attributeName">Name of the attribute to add</param>
        /// <param name="attributeValue">Valud of the attribute</param>
        private static string AddAttributesToToken(String inputXml, String attributeName, String attributeValue)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(inputXml);

            XmlNodeList xmlNodeList = xmlDoc.GetElementsByTagName("t");

            foreach (XmlNode xmlNode in xmlNodeList)
            {
                try
                {
                    if (xmlNode.Attributes.GetNamedItem(attributeName).Value == null) { }
                }
                catch (Exception)
                {
                    XmlAttribute CountryAttr = xmlDoc.CreateAttribute(attributeName);
                    CountryAttr.Value = attributeValue;
                    xmlNode.Attributes.Append(CountryAttr);
                }
            }

            return xmlDoc.OuterXml;
        }


        /// <summary>
        /// Generates a TEST token. Note that the token returned will always contain a hardcoded AssetId that points to the Cheezburguers App
        /// </summary>
        /// <param name="licenseType">The type of license to generate</param>
        /// <param name="productId">The productId of the app</param>
        /// <param name="userLimit">Number of users for the license; 0 for unlimited</param>
        /// <param name="expirationDays">Number of days the license will be valid for. -1 for unlimited</param>
        /// <param name="purchaserId">Id of the purchaser; random GUID</param>
        public static string GenerateTestToken(SPAppLicenseType licenseType, String productId, String userLimit, int expirationDays, String purchaserId)
        {
            //Note that the AssetId matches that of the Cheezburgers app on the marketplace. 
            //This is just for TEST purposes so that the storefront URL takes you to a valid app page
            string hardCodedBaseToken = "<r v=\"0\"><t aid=\"WA103524926\"  did=\"{3F47392A-2308-4FC6-BF24-740626612B26}\"  ad=\"2012-06-19T21:48:56Z\"  te=\"2112-07-15T23:47:42Z\" sd=\"2012-02-01\" test=\"true\"/><d>449JFz+my0wNoCm0/h+Ci9DsF/W0Q8rqEBqjpe44KkY=</d></r>";
            
            string tokenXml = hardCodedBaseToken;
            tokenXml = AddAttributesToToken(tokenXml, "pid", productId);
            tokenXml = AddAttributesToToken(tokenXml, "et", UppercaseFirst(licenseType.ToString()));
            tokenXml = AddAttributesToToken(tokenXml, "cid", purchaserId);
            
            //Set user limit
            if (licenseType == SPAppLicenseType.Free)
            {
                tokenXml = AddAttributesToToken(tokenXml, "ts", "0");
            }
            else
            {
                tokenXml = AddAttributesToToken(tokenXml, "ts", userLimit);
            }

            //Set site license == unlimited users
            if (userLimit.Equals("0"))
            {
                tokenXml = AddAttributesToToken(tokenXml, "sl", "true");
            }
            else
            {
                tokenXml = AddAttributesToToken(tokenXml, "sl", "false");
            }    

            //Set expiration (only supported for Trials)
            if (licenseType == SPAppLicenseType.Trial)
            {
                DateTime expirationDate;
                if (expirationDays==-1)
                {
                    //expired token
                    expirationDate = DateTime.UtcNow.Subtract(TimeSpan.FromDays(10));
                }
                else if (expirationDays == 9999)
                {
                    //Unlimited trial
                    expirationDate = DateTime.MaxValue;
                }
                else
                {
                    //today + the selected number of days
                    expirationDate = DateTime.UtcNow.AddDays(expirationDays);
                }
                tokenXml = AddAttributesToToken(tokenXml, "ed", expirationDate.ToString("o"));

            }
            return tokenXml;
        }

        /// <summary>
        /// Imports a license token into SharePoint
        /// </summary>
        /// <param name="ctx">Already wired up ClientContext object</param>
        /// <param name="licenseToken">String representation of an XML license token</param>
        /// <param name="iconUrl">URL of the dummy icon to use; this will be displayed on licensing UI</param>
        /// <param name="appTitle">Title of the App</param>
        /// <param name="providerName">Author of the app</param>
        public static void ImportLicense(ClientContext ctx, string licenseToken, string iconUrl, string appTitle,string providerName)
        {
            Microsoft.SharePoint.Client.Utilities.Utility.ImportAppLicense(ctx,
                 licenseToken,
                 "en-US",
                 "US",
                 appTitle,
                 iconUrl,
                 providerName,
                 5);

            ctx.ExecuteQuery();
        }


        /// <summary>
        /// Utility method to uppercase first letter of a string
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private static string UppercaseFirst(string s)
        {
            // Check for empty string.
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            // Return char and concat substring.
            return char.ToUpper(s[0]) + s.Substring(1);
        }

    }
}