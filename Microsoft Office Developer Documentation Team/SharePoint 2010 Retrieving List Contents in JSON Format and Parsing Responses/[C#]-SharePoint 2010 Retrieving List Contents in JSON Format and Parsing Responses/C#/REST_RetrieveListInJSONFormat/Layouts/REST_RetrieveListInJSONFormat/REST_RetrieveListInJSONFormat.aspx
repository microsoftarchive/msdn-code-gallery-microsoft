<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" Inherits="Microsoft.SharePoint.WebControls.LayoutsPageBase" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
    <!-- To deploy this example you change the Site URL property for the project so that it deploys to your SharePoint site
    The default value is "http://intranet.contoso.com" -->
    
    <!-- The following ScriptLink control loads the Client Side Object Model JavaScript files -->
    <SharePoint:ScriptLink ID="ScriptLink1" runat="server" Name="sp.js" Localizable="false" LoadAfterUI="true" />

    <!-- The following script tags enable Visual Studio to load the Client Side Object Model 
    script files, so that it can provide intellisense support. They are in a hidden placeholder
    so that they don't load twice at runtime. You must edit the src attibutes to point to your 
    14 hive location -->
    <asp:PlaceHolder runat="server" Visible="false">
        <script type="text/javascript" src="file://C:\Program%20Files\Common%20Files\Microsoft%20Shared\web%20server%20extensions\14\TEMPLATE\LAYOUTS\MicrosoftAjax.js" />
        <script type="text/javascript" src="file://C:\Program%20Files\Common%20Files\Microsoft%20Shared\web%20server%20extensions\14\TEMPLATE\LAYOUTS\SP.Runtime.debug.js" />
        <script type="text/javascript" src="file://C:\Program%20Files\Common%20Files\Microsoft%20Shared\web%20server%20extensions\14\TEMPLATE\LAYOUTS\SP.debug.js" />
        <script type="text/javascript" src="file://C:\Program%20Files\Common%20Files\Microsoft%20Shared\web%20server%20extensions\14\TEMPLATE\LAYOUTS\SP.Core.debug.js" />
        <script type="text/javascript" src="file://C:\Program%20Files\Common%20Files\Microsoft%20Shared\web%20server%20extensions\14\TEMPLATE\LAYOUTS\SP.Ribbon.debug.js" />
    </asp:PlaceHolder>

    <script type="text/javascript">
        //This functions makes the request to the RESTful ListData service
        function getListItems() {
            //Formulate a URL to the service to obtain the items in the Announcements list
            //You must ammend this URL to match your site and list name
            var Url = "http://intranet.contoso.com/_vti_bin/ListData.svc/Announcements";
            //Create a WebRequest object 
            var request = new Sys.Net.WebRequest();
            //Specify the verb
            request.set_httpVerb("GET");
            //Use the URL we already formulated
            request.set_url(Url);
            //Set the Accept header to ensure we get a JSON response
            request.get_headers()["Accept"] = "application/json";
            //Add a callback function that will execute when the request is completed
            request.add_completed(onCompletedCallback);
            //Run the web requests
            request.invoke();
        }

        //This function runs when the web request completes
        function onCompletedCallback(response, eventArgs) {
            //Parse the JSON reponse into a set of objects by using the JavaScript eval() function
            //NOTE: if you don't completely trust the web service, you should use a more robust method
            //to do this. E.g. use the parseJSON() function from the json.js library script. You can 
            //get json.js from http://www.json.org
            //The eval() function doesn't check for dangerous code as it evaluates the JSON text. 
            //Potentially, an attacker could insert malicious code in the response. In this case, the 
            //response comes from you own SharePoint server, so it's probably OK!
            var announcements = eval("(" + response.get_responseData() + ")");
            //Fomulate HTML to display results
            var markup = "Announcements:<br /><br />";
            for (var i = 0; i < announcements.d.results.length; i++) {
                //Display some properties
                markup += 'Title: ' + announcements.d.results[i].Title + '<br />';
                markup += 'ID: ' + announcements.d.results[i].Id + '<br />';
                markup += 'Body: ' + announcements.d.results[i].Body + '<br />';
            }
            //Display the raw JSON response as well, just for interest's sake
            markup += "Raw JSON response: <br/>" + response.get_responseData();
            //Update the displayDiv
            displayDiv.innerHTML = markup;    
        }
    </script>
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <h2>
        <!-- This link is clicked to call the javascript -->
        <a href="javascript:getListItems()">Click Here to Obtain List Items</a>
    </h2>
    <!-- This div displays the results -->
    <div id="displayDiv"></div>
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
    REST Demonstration
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
    Retrieving a List in JSON Format
</asp:Content>
