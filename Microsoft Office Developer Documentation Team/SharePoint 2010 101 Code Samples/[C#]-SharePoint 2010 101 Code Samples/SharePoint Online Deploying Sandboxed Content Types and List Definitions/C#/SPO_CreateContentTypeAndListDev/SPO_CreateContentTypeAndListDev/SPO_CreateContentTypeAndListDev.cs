using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace SPO_CreateContentTypeAndListDev.SPO_CreateContentTypeAndListDev
{
    [ToolboxItemAttribute(false)]
    public class SPO_CreateContentTypeAndListDev : WebPart
    {
        //This Web Part demonstrates code to create a content type and 
        //custom list. This code can run in the sandbox so you can use it
        //in SharePoint Online as well as your on premise farms.
        
        //To test this solution before deployment, set the Site URL 
        //property of the project to match your test SharePoint farm, then
        //use F5

        //To deploy this project to your SharePoint Online site, upload
        //the SPO_SandboxedWebPart.wsp solution file from the bin/debug
        //folder to your solution gallery. Activate the solution and add
        //the web part to a page. 

        //Web Controls
        Button buttonCreateType;
        Label labelCreateTypeResult;

        protected override void CreateChildControls()
        {
            //Set up the label that tells the user what happened
            labelCreateTypeResult = new Label();
            this.Controls.Add(labelCreateTypeResult);
            //Set up the button that you click to create the objects
            buttonCreateType = new Button();
            buttonCreateType.Text = "Create Content Type and List";
            buttonCreateType.Click += new EventHandler(this.buttonCreateType_Click);
            this.Controls.Add(buttonCreateType);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            EnsureChildControls();
            buttonCreateType.RenderControl(writer);
            writer.Write("<br />");
            labelCreateTypeResult.RenderControl(writer);
        }

        protected void buttonCreateType_Click(object sender, EventArgs e)
        {
            //Here we will create a new content type and list
            //Start by getting the current web
            SPWeb web = SPContext.Current.Web;
            //Get the web's collection of content types
            SPContentTypeCollection contentTypes = web.ContentTypes;
            //Create a new content type
            SPContentType newType = new SPContentType(contentTypes[SPBuiltInContentTypeId.Announcement], contentTypes, "Contoso Announcements");
            //Add it to the web 
            try
            {
                contentTypes.Add(newType);
            }
            catch (SPException ex)
            {
                //This is probably because the content type already exists
                labelCreateTypeResult.Text = ex.Message;
            }
            //Now get the web's field collection and add a new field to it
            SPFieldCollection siteFields = web.Fields;
            try
            {
                siteFields.Add("Product", SPFieldType.Text, false);
                web.Update();
            }
            catch (SPException ex)
            {
                //This is probably because the field already exists
                labelCreateTypeResult.Text = ex.Message;
            }
            //Add the field to the new content type
            newType.FieldLinks.Add(new SPFieldLink(siteFields["Product"]));
            newType.Update();
            //Get the web's list collection
            SPListCollection lists = web.Lists;
            try
            {
                Guid newListGuid = lists.Add("Product Announcements", "Announcements about Contoso Products", SPListTemplateType.Announcements);
                SPList newList = lists[newListGuid];
                newList.ContentTypes.Add(newType);
                newList.Update();
            }
            catch (SPException ex)
            {
                //This is probably because the field already exists
                labelCreateTypeResult.Text = ex.Message;
            }
            labelCreateTypeResult.Text = "Contoso Announcement content type and Product Announcements list created successfully";
        }

    }
}
