Imports System
Imports System.ComponentModel
Imports System.Text
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.WebControls.WebParts
Imports Microsoft.SharePoint
Imports Microsoft.SharePoint.WebControls

''' <summary>
''' This Web Part renders client-side html and JavaScript that uses the jQuery
''' library. The jQuery code queries the SharePoint List Data web service to find the
''' items in the Announcements list in the intranet.contoso.com site. 
''' </summary>
''' <remarks>
''' It would be easier to do this in a Visual Web Part, because instead writing code
''' in the Render() method, you could just type the markup. However, Visual Web Parts
''' cannot be used in sandbox. By building a non-visual Web Part, you can distibute
''' your Web Part as a user control.
''' </remarks>
<ToolboxItemAttribute(false)> _
Public Class JQueryDemoWebPart
    Inherits WebPart

    Protected Overrides Sub CreateChildControls()
    End Sub

    Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)
        'First, we must render a <script> tag to link to jQuery. Because this is 
        'a demo, I've linked to the full version. To optimise production code, link to
        'a minimised version such as jquery-1.6.3.min.js
        writer.AddAttribute(HtmlTextWriterAttribute.Src, "http://ajax.microsoft.com/ajax/jquery/jquery-1.6.3.js")
        writer.AddAttribute(HtmlTextWriterAttribute.Type, "text/javascript")
        writer.RenderBeginTag(HtmlTextWriterTag.Script)
        writer.RenderEndTag()

        'Render the javascript getListItems() function (Using a string builder optimises this)
        Dim functionJavaScript As StringBuilder = New StringBuilder()
        functionJavaScript.AppendLine("function getListItems() {")
        functionJavaScript.AppendLine(" //Formulate a URL to the service to obtain the items in the Announcements list")
        functionJavaScript.AppendLine(" //You must ammend this URL to match your site and list name")
        functionJavaScript.AppendLine(" var Url = 'http://intranet.contoso.com/_vti_bin/ListData.svc/Announcements';")
        functionJavaScript.AppendLine(" //call the jQuery getJSON method to get the Announcements")
        functionJavaScript.AppendLine(" $.getJSON(Url, function (data) {")
        functionJavaScript.AppendLine("     //Fomulate HTML to display results")
        functionJavaScript.AppendLine("     var markup = 'Announcements:<br /><br />';")
        functionJavaScript.AppendLine("     //Call the jQuery each method to loop through the results")
        functionJavaScript.AppendLine("     $.each(data.d.results, function (i, result) {")
        functionJavaScript.AppendLine("         //Display some properties")
        functionJavaScript.AppendLine("         markup += 'Title: ' + result.Title + '<br />';")
        functionJavaScript.AppendLine("         markup += 'ID: ' + result.Id + '<br />';")
        functionJavaScript.AppendLine("         markup += 'Body: ' + result.Body + '<br />';")
        functionJavaScript.AppendLine("     });")
        functionJavaScript.AppendLine("     //Call the jQuery append method to display the HTML")
        functionJavaScript.AppendLine("     $('#JQueryDisplayDiv').append($(markup));")
        functionJavaScript.AppendLine(" });")
        functionJavaScript.AppendLine("}")

        writer.AddAttribute(HtmlTextWriterAttribute.Type, "text/javascript")
        writer.RenderBeginTag(HtmlTextWriterTag.Script)
        writer.Write(functionJavaScript.ToString())
        writer.RenderEndTag()

        'Render the display html.
        'First an h2 tag
        writer.RenderBeginTag(HtmlTextWriterTag.H2)

        'Then a hyperlink that calls the JavaScript method
        writer.AddAttribute(HtmlTextWriterAttribute.Href, "javascript:getListItems();")
        writer.RenderBeginTag(HtmlTextWriterTag.A)
        writer.Write("Click Here to Obtain List Items")
        writer.RenderEndTag()

        'End the h2 tag
        writer.RenderEndTag()

        'Render a div to display results
        writer.AddAttribute(HtmlTextWriterAttribute.Id, "JQueryDisplayDiv")
        writer.RenderBeginTag(HtmlTextWriterTag.Div)
        writer.RenderEndTag()

    End Sub

End Class
