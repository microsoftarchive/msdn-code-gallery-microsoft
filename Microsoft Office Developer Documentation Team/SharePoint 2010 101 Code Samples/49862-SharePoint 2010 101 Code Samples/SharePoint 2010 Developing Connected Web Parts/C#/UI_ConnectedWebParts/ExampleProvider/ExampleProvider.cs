using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace UI_ConnectedWebParts.ExampleProvider
{
    /// <summary>
    /// This Web Part demonstrates how to code connectable Web Parts. It sends a simple
    /// text property to the ExampleConsumer Web Part. To test the web parts, run the 
    /// project, then add both web parts to a SharePoint page. Edit the ExampleProvider
    /// web part, then click it's down arrow and, under Connections, select the
    /// Example Consumer web part. Save the changes to the page. When you click Send in the
    /// Example Provider web part, the string in its TextBox appears in the Example 
    /// Consumer Web Part.
    /// </summary>
    /// <remarks>
    /// The ISimpleStringExample interface that this web part implements, defines and string
    /// property only. This interface is how you define the information that web parts can
    /// share
    /// </remarks>
    [ToolboxItemAttribute(false)]
    public class ExampleProvider : WebPart, ISimpleStringExample
    {
        //Controls
        private Label stringToSendLabel;
        private TextBox sendingTextbox;
        private Button sendButton;

        //Internal properties
        private string stringToSend = String.Empty;

        protected override void CreateChildControls()
        {
            //Create the String To Send label
            stringToSendLabel = new Label();
            stringToSendLabel.Text = "String to Send:";
            this.Controls.Add(stringToSendLabel);
            //Create the textbox
            sendingTextbox = new TextBox();
            this.Controls.Add(sendingTextbox);
            //Create the Send button
            sendButton = new Button();
            sendButton.Text = "Send";
            sendButton.Click += new EventHandler(sendButton_Click);
            this.Controls.Add(sendButton);
        }

        void sendButton_Click(object sender, EventArgs e)
        {
            if (sendingTextbox.Text != String.Empty)
            {
                SimpleString = sendingTextbox.Text;
                sendingTextbox.Text = String.Empty;
            }
        }

        //Implement the ISimpleStringExample interface
        public string SimpleString
        {
            get
            {
                return stringToSend;
            }
            set
            {
                stringToSend = value;
            }
        }

        [ConnectionProvider("This provider sends a string from a text box.", "SimpleStringProvider")]
        public ISimpleStringExample SimpleStringProvider()
        {
            return this;
        }
    }
}
