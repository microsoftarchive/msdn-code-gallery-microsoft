using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using WCF_CustomServiceApplication.Client;

namespace WCF_CustomServiceAppConsumer.WCF_SAConsumerWebPart
{
    //This simple web part calls the Day Namer service application
    //Make sure you have deployed the WCF_CustomServiceApplication
    //add created an instance of the service application, either
    //by using Central Administration or the PowerShell scripts,
    //before you try to use this Web Part

    [ToolboxItemAttribute(false)]
    public class WCF_SAConsumerWebPart : WebPart
    {
        //Controls
        Label instructionsLabel;
        Button getTodayButton;
        Label resultLabel;

        protected override void CreateChildControls()
        {
            //Setup instructions label
            instructionsLabel = new Label();
            instructionsLabel.Text = "Push the button to call the Day Namer service application";
            this.Controls.Add(instructionsLabel);
            //Setup the Get Today button
            getTodayButton = new Button();
            getTodayButton.Text = "Get Today";
            getTodayButton.Click += new EventHandler(getTodayButton_Click);
            this.Controls.Add(getTodayButton);
            //Setup the results label
            resultLabel = new Label();
            resultLabel.Text = string.Empty;
            this.Controls.Add(resultLabel);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            //Render all three controls
            instructionsLabel.RenderControl(writer);
            writer.Write("<br />");
            getTodayButton.RenderControl(writer);
            writer.Write("<br />");
            resultLabel.RenderControl(writer);
        }

        private void getTodayButton_Click(object sender, EventArgs args)
        {
            //This is where we call the custom service application
            //Notice that we only need the class of the client -
            //this is the portion of the service application that runs on
            //web front-end servers.
            DayNamerServiceClient dayNamerClient = new DayNamerServiceClient(SPServiceContext.Current);
            //Call a method and display the result
            string today = dayNamerClient.TodayIs();
            resultLabel.Text = today;
        }
    }
}
