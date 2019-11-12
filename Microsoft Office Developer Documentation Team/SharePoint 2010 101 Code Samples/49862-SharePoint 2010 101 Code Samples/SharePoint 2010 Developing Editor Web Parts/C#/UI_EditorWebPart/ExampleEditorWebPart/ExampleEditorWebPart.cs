using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace UI_EditorWebPart
{
    /// <summary>
    /// This is the Editor Web Part that enables the user to set the list to 
    /// display in the ListDisplayWebPart
    /// </summary>
    /// <remarks>
    /// This web part inherits the EditorPart class, not the WebPart class
    /// </remarks>
    [ToolboxItemAttribute(false)]
    public class ExampleEditorWebPart : EditorPart
    {

        //Controls
        private DropDownList listOfSiteLists;

        //Constructor
        public ExampleEditorWebPart(string webPartID)
        {
            //In the constructor, ensure a unique ID for the editor web part
            this.ID = "ExampleEditorWebPart" + webPartID;
        }


        protected override void CreateChildControls()
        {
            //Create the list selector
            listOfSiteLists = new DropDownList();
            ListItem newItem;
            //Get the current SPWeb and find all the lists
            using (SPWeb currentWeb = SPContext.Current.Web)
            {
                foreach (SPList currentList in currentWeb.Lists)
                {
                    //Add each SharePoint list to the drop-down list
                    newItem = new ListItem();
                    newItem.Text = currentList.Title;
                    newItem.Value = currentList.ID.ToString();
                    listOfSiteLists.Items.Add(newItem);
                }
            }
            this.Controls.Add(listOfSiteLists);
        }

        protected override void RenderContents(HtmlTextWriter writer)
        {
            writer.WriteLine("List to Display: ");
            listOfSiteLists.RenderControl(writer);
            writer.WriteLine("<br/>");
        }

        //You must override this method to save configuration changes
        public override bool ApplyChanges()
        {
            EnsureChildControls();
            //Get a reference to the web part we're configuring
            ListDisplayWebPart webPart = this.WebPartToEdit as ListDisplayWebPart;
            if (webPart != null)
            {
                //Set it's properties
                webPart.ListName = listOfSiteLists.SelectedItem.Text;
                webPart.ListID = new Guid(listOfSiteLists.SelectedValue);
            }
            return true;
        }

        //You must override this method to ensure the editor displays the current configuration
        public override void SyncChanges()
        {
            EnsureChildControls();
            //Get a reference to the web part we're configuring
            ListDisplayWebPart webPart = this.WebPartToEdit as ListDisplayWebPart;
            if (webPart != null)
            {
                //Make sure the right item in the drop-down list is selected
                listOfSiteLists.SelectedValue = webPart.ListID.ToString();
            }
        }
    }
}
