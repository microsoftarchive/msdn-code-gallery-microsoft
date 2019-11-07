using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace UI_EditorWebPart
{
    /// <summary>
    /// This Web Part simply displays the items in a list. However, the list to display
    /// is configurable by the user when they edit the Web Part. To implement this, we 
    /// must create a second Web Part, in this case called ExampleEditorWebPart
    /// </summary>
    /// <remarks>
    /// This web part must implement the IWebEditable interface in order to work with an
    /// editor web part
    /// </remarks>
    [ToolboxItemAttribute(false)]
    public class ListDisplayWebPart : WebPart, IWebEditable
    {
        //Internal properties
        private string listName = String.Empty;
        private Guid listID = Guid.Empty;

        //Controls
        private Label instructionsLabel;
        private Label listNameLabel;
        private Label listItemsLabel;

        protected override void CreateChildControls()
        {
            //Create the Instructions label
            instructionsLabel = new Label();
            instructionsLabel.Text = "Edit this Web Part to select the list to display";
            this.Controls.Add(instructionsLabel);
            //Create the List Contents label
            listNameLabel = new Label();
            listNameLabel.Text = "List Name:";
            if (listName != String.Empty)
            {
                listNameLabel.Text = "ListName: " + listName;
            }
            this.Controls.Add(listNameLabel);
            //Create the label that displays the items
            listItemsLabel = new Label();
            //If the list has been set, get it and add lines for each item
            if (listID != Guid.Empty)
            {
                listItemsLabel.Text = fillListLabel(listID);
            }
            this.Controls.Add(listItemsLabel);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            instructionsLabel.RenderControl(writer);
            writer.Write("<br />");
            listNameLabel.RenderControl(writer);
            writer.Write("<br />");
            listItemsLabel.RenderControl(writer);
        }

        //If there is a ListID, this method returns HTML of all the item Titles.
        private string fillListLabel(Guid listIdent)
        {
            string labelHTML = String.Empty;

            using (SPWeb currentWeb = SPContext.Current.Web)
            {
                SPList list = currentWeb.Lists[listIdent];
                if (list != null)
                {
                    foreach (SPListItem item in list.Items)
                    {
                        labelHTML += item["Title"] + "<br />";
                    }
                }
            }

            return labelHTML;
        }

        //This is the name of the list to display
        [WebBrowsable(false)]
        [Personalizable(PersonalizationScope.Shared)]
        public string ListName
        {
            get
            {
                return listName;
            }
            set
            {
                listName = value;
            }
        }

        //This is the ID of the list to display
        [WebBrowsable(false)]
        [Personalizable(PersonalizationScope.Shared)]
        public Guid ListID
        {
            get
            {
                return listID;
            }
            set
            {
                listID = value;
            }
        }

        //IWebEditable Members
        EditorPartCollection IWebEditable.CreateEditorParts()
        {
            //This method creates the editor Web Part 
            List<EditorPart> editors = new List<EditorPart>();
            ExampleEditorWebPart editor = new ExampleEditorWebPart(this.ID);
            editors.Add(editor);
            return new EditorPartCollection(editors);
        }

        object IWebEditable.WebBrowsableObject
        {
            //For this property we must return the Web Part itself
            get { return this; }
        }

    }
}
