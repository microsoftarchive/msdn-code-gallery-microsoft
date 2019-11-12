using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using Microsoft.SharePoint.Client;

namespace LicenseSPAppSampleWeb.Pages
{
    public partial class Default : System.Web.UI.Page
    {
     

        protected void Page_Load(object sender, EventArgs e)
        {
            string contextToken = Session["contexToken"].ToString();          
        }

       

        protected void ImportLicense_Click(object sender, EventArgs e)
        {

            statusMessage.Text = "";
       
            TokenHelper.TrustAllCertificates();
            ClientContext ctx = TokenHelper.GetClientContextWithContextToken(Session["SPHostUrl"].ToString(), Session["contexToken"].ToString(), Request.Url.Authority);

            string appIconUrl = Request.Url.AbsoluteUri.Substring(0, Request.Url.AbsoluteUri.LastIndexOf("/")) + "/AppIcon.png";
            SPAppLicenseType licenseType=SPAppLicenseHelper.GetLicenseTypeFromString(importLicenseType.SelectedValue);
            string token = SPAppLicenseHelper.GenerateTestToken(licenseType, importProductId.Text, importUserLimit.SelectedValue, int.Parse(importExpiration.SelectedValue), importPurchaserId.Text);

            SPAppLicenseHelper.ImportLicense(ctx, token, appIconUrl, importAppTitle.Text, importProviderName.Text);
            WebColorConverter converter=new WebColorConverter();

            statusMessage.Text = "License Imported Succesfully!  Time="+ DateTime.Now;
            //importTokenGenerated.Text = token;
        }

        protected void importLicenseType_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Validate Fields
            if (importLicenseType.SelectedValue.Equals("Free"))
            {
                importUserLimit.SelectedIndex = 0;
                importExpiration.SelectedIndex = 0;
                importExpiration.Enabled = false;
                importUserLimit.Enabled = false;
            }
            else if (importLicenseType.SelectedValue.Equals("Paid"))
            {
                importUserLimit.SelectedIndex = 0;
                importExpiration.SelectedIndex = 0;
                importExpiration.Enabled = false;
                importUserLimit.Enabled = true;
            }

            else if (importLicenseType.SelectedValue.Equals("Trial"))
            {
                importUserLimit.SelectedIndex = 0;
                importExpiration.SelectedIndex = 1;
                importExpiration.Enabled = true;
                importUserLimit.Enabled = true;
           
            }
        }

        protected void importExpiration_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(importLicenseType.SelectedValue.Equals("Trial") && importExpiration.SelectedValue.Equals("0"))
            {
                //Non expiring trials aren't supported hence resetting this to 30
                importExpiration.SelectedIndex = 1;
               
            }
        }

        protected void generateCustomerId_Click(object sender, EventArgs e)
        {
            Random random = new Random();

            importPurchaserId.Text = String.Format("{0}59FDE73E", random.Next().ToString("X"));
            
        }

        
    }
}