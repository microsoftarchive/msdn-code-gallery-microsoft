using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace CLIENTSIDE_AjaxWebPart.AjaxDemoWebPart
{

    /// <summary>
    /// This web part just displays the time. However it does so within an AJAX update
    /// panel so the entire page is not reloaded whenever the button is clicked. It
    /// also demonstrates how to use an UpdateProgress control to feedback to the user
    /// and enables the user to set the progress image and text when they configure the
    /// web part.
    /// </summary>
    /// <remarks>
    /// For this to work, there must be a <asp:ScriptManager> control on the page. In 
    /// SharePoint 2010, this is including in all master pages. Unless you are using
    /// a custom master page that doesn't include a script manager, you can use AJAX
    /// controls in Web Parts without creating it. 
    /// </remarks>
    [ToolboxItemAttribute(false)]
    public class AjaxDemoWebPart : WebPart
    {

        //This property enables the user to set the progress image path.
        [DefaultValue("_layouts/images/progress.gif")]
        [WebBrowsable(true)]
        [Category("ProgressTemplate")]
        [Personalizable(PersonalizationScope.Shared)]
        public string ImagePath { get; set; }

        //This property enables the user to set the progress feedback text.
        [DefaultValue("Checking...")]
        [WebBrowsable(true)]
        [Category("ProgressTemplate")]
        [Personalizable(PersonalizationScope.Shared)]
        public string DisplayText { get; set; }

        //Controls
        UpdatePanel mainUpdatePanel;
        UpdateProgress progressControl;
        Button checkTimeButton;
        Label timeDisplayLabel;

        protected override void CreateChildControls()
        {
            //Create the update panel
            mainUpdatePanel = new UpdatePanel();
            mainUpdatePanel.ID = "updateAjaxDemoWebPart";
            //Use conditional mode so that only controls within this panel cause an update
            mainUpdatePanel.UpdateMode = UpdatePanelUpdateMode.Conditional;

            //Create the update progress control
            progressControl = new UpdateProgress();
            progressControl.AssociatedUpdatePanelID = "updateAjaxDemoWebPart";
            //The class used for the progrss template is defined below in this code file
            progressControl.ProgressTemplate = new ProgressTemplate(ImagePath, DisplayText);

            //Create the Check Time button
            checkTimeButton = new Button();
            checkTimeButton.ID = "checkTimeButton";
            checkTimeButton.Text = "Check Time";
            checkTimeButton.Click += new EventHandler(checkTimeButton_Click);

            //Create the label that displays the time
            timeDisplayLabel = new Label();
            timeDisplayLabel.ID = "timeDisplayLabel";
            timeDisplayLabel.Text = string.Format("The time is: {0}", DateTime.Now.ToLongTimeString());

            //Add the button and label to the Update Panel
            mainUpdatePanel.ContentTemplateContainer.Controls.Add(timeDisplayLabel);
            mainUpdatePanel.ContentTemplateContainer.Controls.Add(checkTimeButton);

            //Add the Update Panel and the progress control to the Web Part controls
            this.Controls.Add(mainUpdatePanel);
            this.Controls.Add(progressControl);
        }

        private void checkTimeButton_Click(object sender, EventArgs e)
        {
            //This calls a server-side method, but because the button is in 
            //an update panel, only the update panel reloads.
            this.timeDisplayLabel.Text = string.Format("The time is: {0}", DateTime.Now.ToLongTimeString());
        }
    }


    //This template defines the contents of the Update Progress control
    public class ProgressTemplate : ITemplate
    {
        public string ImagePath { get; set; }
        public string DisplayText { get; set; }

        public ProgressTemplate(string imagePath, string displayText)
        {
            ImagePath = imagePath;
            DisplayText = displayText;
        }

        public void InstantiateIn(Control container)
        {
            Image img = new Image();
            img.ImageUrl = SPContext.Current.Site.Url + "/" + ImagePath;

            Label displayTextLabel = new Label();
            displayTextLabel.Text = DisplayText;

            container.Controls.Add(img);
            container.Controls.Add(displayTextLabel);
        }
    }
}
