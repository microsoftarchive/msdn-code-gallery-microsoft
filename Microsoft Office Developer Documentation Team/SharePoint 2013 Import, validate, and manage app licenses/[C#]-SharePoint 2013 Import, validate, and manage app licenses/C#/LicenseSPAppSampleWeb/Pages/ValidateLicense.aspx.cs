using Microsoft.SharePoint.Client;
using System;
using LicenseSPAppSampleWeb.LicenseVerificationService;

namespace LicenseSPAppSampleWeb.Pages
{
    public partial class ValidateLicense : System.Web.UI.Page
    {
        bool _testMode = true;

        protected void Page_Load(object sender, EventArgs e)
        {
            //ProductId must match the one on the AppManifest.xml of the SharePoint App that is retrieving its license
            //An app can only retrieve its own licenses for security reasons (e.g. you cannot retrieve other Apps licenses)
            //The one hardcoded here matches this test app

            Guid ProductId = new Guid("7c617a53-6f45-4d23-ada0-28eabb744acb");
            string uiWarningMessage = null;

            //Get Context token to call SP; standard oAuth
            string contextToken = Session["contexToken"].ToString();

            TokenHelper.TrustAllCertificates();
            ClientContext ctx = TokenHelper.GetClientContextWithContextToken(Session["SPHostUrl"].ToString(), Session["contexToken"].ToString(), Request.Url.Authority);

            //Use helper method to retrieve a processed license object
            //It is recommended to CACHE the VerifyEntitlementTokenResponse result until the storeLicense.tokenExpirationDate
            VerifyEntitlementTokenResponse verifiedLicense = SPAppLicenseHelper.GetAndVerifyLicense(ProductId, ctx);

            //Get UI warning. 
            //Note that the name of the app is being hardcoded to Cheezburgers because is an app that already exists on the marketplace
            //You should use your exact app display name instead (make sure name matches with the metadata you submit to seller dashboard
            uiWarningMessage = SPAppLicenseHelper.GetUIStringText(verifiedLicense, Session["SPHostUrl"].ToString(), Request.Url.ToString(), "Cheezburgers", "Features X, Y, Z");

            if (verifiedLicense == null)
            {
                //No license found or the license was malformed
                //The UI string retrieved above will already contain the appropiate info
                //In real app code you could take additional steps (e.g. provide reduced functionality)
            }
            else
            {


                //There is a well-formed license; must look at properties to determine validity
                if (verifiedLicense.IsValid)
                {
                    //Valid production license
                    //For app sample purposes display 'Production License' + the corresponding warning text; replace this with your own logic. 
                    uiWarningMessage = "Production License: " + uiWarningMessage;
                    processLicense(verifiedLicense);
                }
                else if (verifiedLicense.IsTest && _testMode == true)
                {
                    //Test mode with valid test token
                    //For debug we just display 'Test License' plus the corresponding UI warning text; in a real world production scenario _testMode should be set to false and the test license should be rejected. 
                    uiWarningMessage = "Test License: " + uiWarningMessage;
                    processLicense(verifiedLicense);
                }
                else
                {
                    //Beep, production mode with invalid license
                    //Warn the user about missing/invalid license
                    uiWarningMessage = "Invalid License!!! " + uiWarningMessage;
                }
            }

            //Sets the text of the alert
            lblWarning.Text = uiWarningMessage;
        }

        protected void processLicense(VerifyEntitlementTokenResponse verifiedLicense)
        {
            SPAppLicenseType licenseType = SPAppLicenseHelper.GetLicenseTypeFromLicense(verifiedLicense);
            switch (licenseType)
            {
                case SPAppLicenseType.Trial:
                    //Do something for a valid trial besides presenting a message on the UI
                    if (SPAppLicenseHelper.GetRemainingDays(verifiedLicense) > 0)
                    {
                        //Valid trial
                        //The UI string retrieved above will already contain the appropiate info
                        //In real app code you could take additional steps (we encourage trials to be fully featured) 
                        //Helper code will return int.MaxValue for an unlimited trial
                    }
                    else
                    {
                        //Expired trial
                        //The UI string retrieved above will already contain the appropiate info
                        //In real app code you could take additional steps (e.g. provide reduced functionality) 
                    }
                    break;
                case SPAppLicenseType.Paid:
                    //Do something for a paid app
                    break;
                case SPAppLicenseType.Free:
                    //Do something for a free app
                    break;
                default:
                    throw new Exception("Unknown License Type");

            }

        }
    }
}