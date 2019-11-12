using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

// using statments that make our code below easier to read/write
using System.Collections;
using Microsoft.SharePoint.Client;
using System.Web.Script.Serialization;

namespace WD_SharePointOAuth_csWeb.Pages
{
    public partial class ListCreator : System.Web.UI.Page
    {

        // Variables used in events and to control the actual page content
        public string connectedSiteUrl = string.Empty;
        public string accessToken = string.Empty;
        public string refreshToken = string.Empty;
        public string listData = string.Empty;
        public string listName = string.Empty;
        public string listDescription = string.Empty;
        public string newListUrl = string.Empty;
        public bool success = false;
        public Exception exception = null;
        public ArrayList listArray = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Read the data that has been posted
            connectedSiteUrl = Request.Form["connectedSiteUrl"];
            accessToken = Request.Form["accessToken"];
            refreshToken = Request.Form["refreshToken"];
            
            // Note that the following form field was set dynamically by JavaScript
            // to represent the data that was extracted from the worksheet.
            // We need to replace the double quotes because we want to deserialize it below
            listData = Request.Form["listData"].Replace("\"","");
            listName = Request.Form["listName"];
            listDescription = Request.Form["listDescription"];

            try
            {
                List newList = CreateNewList(listName, listDescription, listData);
                Uri baseSiteUrl = new Uri(connectedSiteUrl);
                if (success)
                {
                    newListUrl = baseSiteUrl.Scheme + "://" + baseSiteUrl.Authority + newList.DefaultViewUrl;
                }
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
                Response.Write("\n" + ex.StackTrace);
                Response.End();
            }
        }

        public List CreateNewList(string newListName, string listDescription, string listData)
        {
            // Continue to use our OAuth token to actually create the list
            using (ClientContext context = TokenHelper.GetClientContextWithAccessToken(connectedSiteUrl, accessToken))
            {
                // Use standard CSOM approaches for creating a list
                context.Load(context.Web);
                context.ExecuteQuery();
                string listName = newListName;
                string fieldXml = String.Empty;
                List<string> headers = new List<string>();
                List list = null;
                try
                {

                    if (listData != null)
                    {
                        ListCreationInformation lc = new ListCreationInformation();
                        lc.Title = listName;
                        lc.Description = listDescription;
                        lc.TemplateType = (int)ListTemplateType.GenericList;
                        list = context.Web.Lists.Add(lc);
                        string[] listFields = listData.Split(';');
                        foreach (string field in listFields)
                        {
                            if (field.ToLower() != "title")
                            {
                                fieldXml = String.Format("<Field Name='{0}' DisplayName='{0}' Type='Text' Hidden='False' Description='{0}' />", field);
                                Field fld = list.Fields.AddFieldAsXml(fieldXml, true, AddFieldOptions.DefaultValue);
                                headers.Add(field.Replace(" ", "_x0020_"));
                            }
                            else
                            {
                                headers.Add("Title");
                            }
                        }
                    }
                    context.ExecuteQuery();
                    context.Load(list, l => l.DefaultViewUrl);
                    context.ExecuteQuery();
                    success = true;
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
                return list;
            }
        }
    }
}