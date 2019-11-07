using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Publishing;
using Microsoft.SharePoint.WebControls;

namespace WCM_CustomFieldControl
{
    /// <summary>
    /// This custom field control is a simple textbox control for a custom site column
    /// attached to the Publishing Page content type
    /// </summary>
    /// <remarks>
    /// Before you can use this project you must enable the Publishing features at both
    /// the site and site collection levels. You must also have created a custom site column
    /// called "DemoCustomColumn" and added it to the Page content type. Then add a Page
    /// to the Pages library. 
    /// </remarks>
    class CustomFieldControl : Control
    {
        protected override void OnInit(EventArgs e)
        {
            //Check whether there in a SPContext, and if this is a List Item
            if (SPContext.Current != null && SPContext.Current.ListItem != null)
            {
                //Check whether the current content type is or inherits from the Page content type
                SPContentTypeId pageContentTypeId = SPContext.Current.Web.AvailableContentTypes["Page"].Id;
                SPContentTypeId currentItemTypeId = SPContext.Current.ListItem.ContentTypeId;
                if (currentItemTypeId.Equals(pageContentTypeId) || currentItemTypeId.IsChildOf(pageContentTypeId))
                {
                    //Get the PageHolderMain content place holder control
                    ContentPlaceHolder placeHolderMain = (ContentPlaceHolder)this.Page.Master.FindControl("PlaceHolderMain");
                    if (placeHolderMain != null)
                    {

                        //Check whether the current item is a publishing page and is a New item or in Edit mode
                        if ((PublishingPage.IsPublishingPage(SPContext.Current.ListItem)) &&
                            (SPContext.Current.FormContext.FormMode == SPControlMode.Edit ||
                                SPContext.Current.FormContext.FormMode == SPControlMode.New))
                        {
                            //Get the custom column
                            SPField demoCustomColumn;
                            try
                            {
                                demoCustomColumn = SPContext.Current.ListItem.Fields["DemoCustomColumn"];
                            }
                            catch
                            {
                                demoCustomColumn = null;
                            }
                            if (demoCustomColumn != null)
                            {
                                //We have a page, in edit or new mode, with the demoCustomColumn.
                                //So render the custom field control
                                BaseFieldControl demoCustomColumnControl = demoCustomColumn.FieldRenderingControl;
                                demoCustomColumnControl.ID = demoCustomColumn.InternalName;
                                placeHolderMain.Controls.Add(new LiteralControl("<div class=\"edit-mode-panel\">"));
                                placeHolderMain.Controls.Add(demoCustomColumnControl);
                                placeHolderMain.Controls.Add(new LiteralControl("</div>"));
                            }
                        }
                        else if ((PublishingPage.IsPublishingPage(SPContext.Current.ListItem)) &&
                        (SPContext.Current.FormContext.FormMode == SPControlMode.Display))
                        {
                            //Get the custom column
                            SPField demoCustomColumn;
                            try
                            {
                                demoCustomColumn = (SPFieldText)SPContext.Current.ListItem.Fields["DemoCustomColumn"];
                            }
                            catch
                            {
                                demoCustomColumn = null;
                            }
                            if (demoCustomColumn != null)
                            {
                                //We have a page, in display mode, with the demoCustomColumn.
                                //So render the value of the field. You can add custom rendering markup
                                //here. In this case, the <div> tag renders the text in red.
                                placeHolderMain.Controls.Add(new LiteralControl("<div style=\"color: red;\">"));
                                placeHolderMain.Controls.Add(new LiteralControl(SPContext.Current.ListItem["DemoCustomColumn"].ToString()));
                                placeHolderMain.Controls.Add(new LiteralControl("</div>"));
                            }
                        }
                    }
                    
                }
            }
            base.OnInit(e);
        }
    }
}
