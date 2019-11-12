using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
//Import the WCF namespace
using System.ServiceModel;

namespace WCF_CallServiceFromWebPart.WCF_CallServiceFromWebPart
{
    [ToolboxItemAttribute(false)]
    public class WCF_CallServiceFromWebPart : WebPart
    {
        //This sample demonstrates how to call a WCF service 
        //from a web part. Note that this is only possible in
        //server-side code when the Web Part is deployed OUTSIDE
        //the sandbox. The WCF Service is in the WCF_ExampleService
        //project in this solution.

        //To use this sample, configure the WCF_CallServiceFromWebPart
        //project to deploy to your SharePoint site (the default is
        //http://intranet.contoso.com) then start that project for 
        //debugging. Add the custom web part to any page. Before 
        //you click the"Today" button, run the WCF_ExampleService
        //project and wait until the prompt tells you the service is
        //ready. 

        //UI Controls
        Label instructionsLabel;
        Label resultLabel;
        Button buttonToday;

        protected override void CreateChildControls()
        {
            //Set up the instructions label
            instructionsLabel = new Label();
            instructionsLabel.Text = "Make sure the WCF Service is running, then push the Today button to call it.";
            this.Controls.Add(instructionsLabel);
            //Set up the button
            buttonToday = new Button();
            buttonToday.Text = "Today";
            buttonToday.Click += new EventHandler(buttonToday_Click);
            this.Controls.Add(buttonToday);
            //Set up the results label
            resultLabel = new Label();
            resultLabel.Text = String.Empty;
            this.Controls.Add(resultLabel);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            //Render the controls
            instructionsLabel.RenderControl(writer);
            writer.Write("<br />");
            buttonToday.RenderControl(writer);
            writer.Write("<br />");
            resultLabel.RenderControl(writer);
        }

        //This method fires when someone clicks the button
        private void buttonToday_Click(object sender, EventArgs args)
        {
            //I used svcutil.exe to generate the proxy class for the service
            //in the generatedDayNamerProxy.cs file. I'm going to configure this
            //in code by using a channel factory.
            
            //Create the channel factory with a Uri, binding and endpoint
            Uri serviceUri = new Uri("http://localhost:8088/WCF_ExampleService/Service/DayNamerService");
            WSHttpBinding serviceBinding = new WSHttpBinding();
            EndpointAddress dayNamerEndPoint = new EndpointAddress(serviceUri);
            ChannelFactory<httpWCF_ExampleService> channelFactory = new ChannelFactory<httpWCF_ExampleService>(serviceBinding, dayNamerEndPoint);
            //Create a channel
            httpWCF_ExampleService dayNamer = channelFactory.CreateChannel();
            //Now we can call the TodayIs method
            string today = dayNamer.TodayIs();
            resultLabel.Text= "Today is: " + today;
            //close the factory with all its channels
            channelFactory.Close();
        }
    }
}
