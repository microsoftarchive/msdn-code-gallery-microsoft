using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace UI_SandBoxedWebPart.UI_SandboxedWebPart
{
    [ToolboxItemAttribute(false)]
    public class UI_SandboxedWebPart : WebPart
    {
        //The Visual Web Part project template cannot run within the SharePoint 2010
        //sandbox. Therefore, if you use it to create a Web Part, administrators must 
        //deploy your project to the production SharePoint farm. By creating non-visual 
        //Web Parts, users can deploy your project as a user solution without involving 
        //administrators. The web part can also be used in SharePoint Online. This is 
        //the recommended approach wherever possible. 

        //To create a non-visual Web Part, create an empty SharePoint project, deployed 
        //in the Sandbox. Then add a new item to the project and choose Web Part, not Visual
        //Web Part!

        //In a non-visual Web Part there is no ASP.NET markup. Instead you must create your UI
        //in code.

        //Start by declaring ASP.NET controls in the class scope
        Panel hiddenPanel;
        Button showPanelButton;
        RadioButtonList exampleRadioButtonList;

        protected override void CreateChildControls()
        {
            //Set up the panel control
            hiddenPanel = new Panel();
            hiddenPanel.GroupingText = "Some Radio Buttons";
            //We'll hide it by default.
            hiddenPanel.Visible = false;
            this.Controls.Add(hiddenPanel);
            //Set up the radio button list
            exampleRadioButtonList = new RadioButtonList();
            exampleRadioButtonList.Items.Add("Item 1");
            exampleRadioButtonList.Items.Add("Item 2");
            exampleRadioButtonList.Items.Add("Item 3");
            //Because we want the list within the hidden panel, we add it to the panel's
            //controls collection, not the Web Part's controls collection
            hiddenPanel.Controls.Add(exampleRadioButtonList);
            //Set up the Show Panel button
            showPanelButton = new Button();
            showPanelButton.Text = "Show Panel";
            //handle the button's click event
            showPanelButton.Click += new EventHandler(showPanelButton_Click);
            this.Controls.Add(showPanelButton);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            //In the Render method, you can use the writer object to render any HTML.
            //In this example, we'll style the controls with a <div> tag
            writer.WriteBeginTag("div");
            //Add a simple style
            writer.Write(" style" + HtmlTextWriter.EqualsDoubleQuoteString);
            writer.WriteStyleAttribute("color", "blue");
            writer.Write(HtmlTextWriter.DoubleQuoteChar + ">");
            //Add simple HTML
            writer.Write("<p>You can use writer.Write() to render any HTML. Also note the inline style.</p>");
            //Render the controls
            showPanelButton.RenderControl(writer);
            writer.WriteBreak();
            hiddenPanel.RenderControl(writer);
            //N.B. You don't have to call RenderControl for the exampleRadioButtonList
            //because it is within the hiddenPanel.
            //Complete the <div> tag
            writer.WriteEndTag("div");
        }

        void showPanelButton_Click(object sender, EventArgs e)
        {
            //Show the hidden panel
            hiddenPanel.Visible = true;
        }
    }
}
