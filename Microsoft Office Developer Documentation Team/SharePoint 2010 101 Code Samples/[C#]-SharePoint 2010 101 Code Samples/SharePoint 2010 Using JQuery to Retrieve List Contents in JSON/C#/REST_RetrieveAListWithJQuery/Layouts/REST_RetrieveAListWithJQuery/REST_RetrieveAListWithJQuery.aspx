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

    <!-- To use jQuery, we must link to the latest jQuery file. This link uses the Microsoft 
    CDN to get jQuery. There are alternatice CDNs or you can download the file and include it 
    in your solution. Also, because this is a demo, I've used the non-minimized version of the 
    file for more helpful debugging. For your production code, use a minimized version such as 
    jquery-1.6.3.min.js -->
    <script src="http://ajax.microsoft.com/ajax/jquery/jquery-1.6.3.js" type="text/javascript"></script>

    <script type="text/javascript">
        function getListItems() {
            //Formulate a URL to the service to obtain the items in the Announcements list
            //You must ammend this URL to match your site and list name
            var Url = "http://intranet.contoso.com/_vti_bin/ListData.svc/Announcements";
            //call the jQuery getJSON method to get the Announcements
            $.getJSON(Url, function (data) {
                //Fomulate HTML to display results
                var markup = "Announcements:<br /><br />";
                //Call the jQuery each method to loop through the results
                $.each(data.d.results, function (i, result) {
                    //Display some properties
                    markup += 'Title: ' + result.Title + '<br />';
                    markup += 'ID: ' + result.Id + '<br />';
                    markup += 'Body: ' + result.Body + '<br />';
                });
                //Call the jQuery append method to display the HTML
                $('#displayDiv').append($(markup));
            });
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
    Retrieving a List Using jQuery
</asp:Content>
