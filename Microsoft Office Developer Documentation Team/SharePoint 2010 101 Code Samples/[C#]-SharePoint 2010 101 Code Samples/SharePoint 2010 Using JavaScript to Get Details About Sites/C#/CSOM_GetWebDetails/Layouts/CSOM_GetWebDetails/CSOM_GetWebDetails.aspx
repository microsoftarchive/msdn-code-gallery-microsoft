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
    <SharePoint:ScriptLink runat="server" Name="sp.js" Localizable="false" LoadAfterUI="true" />

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

    <!-- These are the main scripts that find out about the Web object -->
    <script type="text/javascript">
        //This variable will hold a reference to the web object
        var web = null;

        //This function loads and queries the current SP.Web object
        function getWebDetails() {
            //Get the current context
            var context = new SP.ClientContext();
            //Get the current web
            web = context.get_web();
            //Load the site object
            context.load(web);
            //Run the query asynchronously, passing the functions to call when a response arrives
            context.executeQueryAsync(onSucceededCallback, onFailedCallback);
        }

        //This function fires when the query completes successfully
        function onSucceededCallback(sender, args) {
            //Formulate HTML from the web details
            var markup = '<p>Web Details: <br>';
            markup += 'Title: ' + web.get_title() + '<br>';
            markup += 'ID: ' + web.get_id() + '<br>';
            markup += 'Description: ' + web.get_description() + '<br>';
            markup += 'Created: ' + web.get_created() + '<br>';
            //Display the formulated HTML in the displayDiv element
            displayDiv.innerHTML = markup;
        }

        //This function fires when the query fails
        function onFailedCallback(sender, args) {
            //Formulate HTML to display details of the error
            var markup = '<p>The request failed: <br>';
            markup += 'Message: ' + args.get_message() + '<br>';
            //Display the details
            displayDiv.innerHTML = markup;
        }

    </script>
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <!-- You must add a FormDigest control to the page before you use 
    JavaScript to call the CSOM. This inserts a security validation 
    token into the page that can be used to prevent malicious attacks -->
    <SharePoint:FormDigest ID="FormDigest1" runat="server" />
    <h2>
        <!-- This link is clicked to call the javascript -->
        <a href="javascript:getWebDetails()">Click Here for Web Details</a>
    </h2>
    <!-- This div displays the results -->
    <div id="displayDiv"></div>
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
    CSOM Demonstration
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
    Using CSOM to Get Web Details
</asp:Content>
