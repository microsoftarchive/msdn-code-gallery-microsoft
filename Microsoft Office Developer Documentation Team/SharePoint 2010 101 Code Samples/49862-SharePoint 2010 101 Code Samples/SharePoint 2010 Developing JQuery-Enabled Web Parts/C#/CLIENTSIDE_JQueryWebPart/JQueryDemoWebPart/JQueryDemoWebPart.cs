using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace CLIENTSIDE_JQueryWebPart.JQueryDemoWebPart
{
    /// <summary>
    /// This Web Part renders client-side html and JavaScript that uses the jQuery
    /// library. The jQuery code queries the SharePoint List Data web service to find the
    /// items in the Announcements list in the intranet.contoso.com site. 
    /// </summary>
    /// <remarks>
    /// It would be easier to do this in a Visual Web Part, because instead writing code
    /// in the Render() method, you could just type the markup. However, Visual Web Parts
    /// cannot be used in sandbox. By building a non-visual Web Part, you can distibute
    /// your Web Part as a user control.
    /// </remarks>
    [ToolboxItemAttribute(false)]
    public class JQueryDemoWebPart : WebPart
    {

        protected override void CreateChildControls()
        {
        }

        protected override void Render(HtmlTextWriter writer)
        {
            //First, we must render a <script> tag to link to jQuery. Because this is 
            //a demo, I've linked to the full version. To optimise production code, link to
            //a minimised version such as jquery-1.6.3.min.js
            writer.AddAttribute(HtmlTextWriterAttribute.Src, "http://ajax.microsoft.com/ajax/jquery/jquery-1.6.3.js");
            writer.AddAttribute(HtmlTextWriterAttribute.Type, "text/javascript");
            writer.RenderBeginTag(HtmlTextWriterTag.Script);
            writer.RenderEndTag();

            //Render the javascript getListItems() function
            string functionJavaScript = @"
                                        function getListItems() {
                                            //Formulate a URL to the service to obtain the items in the Announcements list
                                            //You must ammend this URL to match your site and list name
                                            var Url = 'http://intranet.contoso.com/_vti_bin/ListData.svc/Announcements';
                                            //call the jQuery getJSON method to get the Announcements
                                            $.getJSON(Url, function (data) {
                                                //Fomulate HTML to display results
                                                var markup = 'Announcements:<br /><br />';
                                                //Call the jQuery each method to loop through the results
                                                $.each(data.d.results, function (i, result) {
                                                    //Display some properties
                                                    markup += 'Title: ' + result.Title + '<br />';
                                                    markup += 'ID: ' + result.Id + '<br />';
                                                    markup += 'Body: ' + result.Body + '<br />';
                                                });
                                                //Call the jQuery append method to display the HTML
                                                $('#JQueryDisplayDiv').append($(markup));
                                            });
                                        }";
            writer.AddAttribute(HtmlTextWriterAttribute.Type, "text/javascript");
            writer.RenderBeginTag(HtmlTextWriterTag.Script);
            writer.Write(functionJavaScript);
            writer.RenderEndTag();

            //Render the display html.
            //First an h2 tag
            writer.RenderBeginTag(HtmlTextWriterTag.H2);

            //Then a hyperlink that calls the JavaScript method
            writer.AddAttribute(HtmlTextWriterAttribute.Href, "javascript:getListItems();");
            writer.RenderBeginTag(HtmlTextWriterTag.A);
            writer.Write("Click Here to Obtain List Items");
            writer.RenderEndTag();

            //End the h2 tag
            writer.RenderEndTag();

            //Render a div to display results
            writer.AddAttribute(HtmlTextWriterAttribute.Id, "JQueryDisplayDiv");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            writer.RenderEndTag();
        }
    }
}
