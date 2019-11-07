using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.ServiceModel;

namespace AZURE_CallingWebPart.CallingWebPart
{
    //This web part calls the DayInfoService.svc WCF service in Windows Azure
    //Make sure you package and publish the service in your Window Azure account
    //before you run this web part.
    [ToolboxItemAttribute(false)]
    public class CallingWebPart : WebPart
    {
        Label instructionsLabel;
        Button getTodayButton;
        Label resultsLabel;

        protected override void CreateChildControls()
        {
            //Set up the instructions label
            instructionsLabel = new Label();
            instructionsLabel.Text = "Click the Get Today button to call the Azure service";
            this.Controls.Add(instructionsLabel);
            //Set up the GetToday button
            getTodayButton = new Button();
            getTodayButton.Text = "Get Today";
            getTodayButton.Click += new EventHandler(getTodayButton_Click);
            this.Controls.Add(getTodayButton);
            //Set up the results label
            resultsLabel = new Label();
            this.Controls.Add(resultsLabel);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            //Render the instructions label
            instructionsLabel.RenderControl(writer);
            writer.Write("<br />");
            //Render the button
            getTodayButton.RenderControl(writer);
            writer.Write("<br />");
            //Render the results label
            resultsLabel.RenderControl(writer);
        }

        private void getTodayButton_Click(object sender, EventArgs args)
        {
            //I used svcutil.exe to generate the proxy class for the service
            //in the generatedDayNamerProxy.cs file. I'm going to configure this
            //in code by using a channel factory.

            //Create the channel factory with a Uri, binding and endpoint
            //The Uri is to the hosted service in Windows Azure
            //Edit this Uri to match the location where you published the service.
            Uri serviceUri = new Uri("http://daynamercs.cloudapp.net/dayinfoservice.svc");
            BasicHttpBinding serviceBinding = new BasicHttpBinding();
            EndpointAddress dayNamerEndPoint = new EndpointAddress(serviceUri);
            ChannelFactory<IDayInfo> channelFactory = new ChannelFactory<IDayInfo>(serviceBinding, dayNamerEndPoint);
            //Create a channel
            IDayInfo dayNamer = channelFactory.CreateChannel();
            //Now we can call the TodayIs method
            string today = dayNamer.TodayIs();
            resultsLabel.Text = "Today is: " + today;
            //close the factory with all its channels
            channelFactory.Close();
        }
    }
}