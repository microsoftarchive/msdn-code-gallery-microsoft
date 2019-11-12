using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace UI_ConnectedWebParts.ExampleConsumer
{
    /// <summary>
    /// This web part can connect to any Web Part that implements the ISimpleStringProvider
    /// interface. When such a Web Part sends a string, this Web Part displays it in
    /// the Received String textbox.
    /// </summary>
    [ToolboxItemAttribute(false)]
    public class ExampleConsumer : WebPart
    {
        private ISimpleStringExample myProvider;
        
        //Controls
        Label sentStringLabel;
        TextBox sentStringTextBox;

        protected override void CreateChildControls()
        {
            //Create the Received String label
            sentStringLabel = new Label();
            sentStringLabel.Text = "Received String:";
            this.Controls.Add(sentStringLabel);
            //Create the textbox
            sentStringTextBox = new TextBox();
            sentStringTextBox.ReadOnly = true;
            sentStringTextBox.Text = "No string received yet...";
            this.Controls.Add(sentStringTextBox);
        }

        //When a string is sent across the connection, we must ensure it is displayed.
        protected override void OnPreRender(EventArgs e)
        {
            EnsureChildControls();
            if (myProvider != null)
            {
                sentStringTextBox.Text = myProvider.SimpleString;
            }
        }

        //This method declares that this Web Part can connect to any provider
        //that implements the ISimpleStringProvider interface.
        [ConnectionConsumer("String Consumer", "StringConsumer")]
        public void SimpleStringConsumer(ISimpleStringExample Provider)
        {
            myProvider = Provider;
        }
    }
}
