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

    <!-- These are the main scripts that open the current web and create a list -->
    <script type="text/javascript">
        //This example create a new list called Products in the current web

        //This variable holds a reference to the new list
        var newList = null

        //This function creates the new list, sets properties and fields, and executes the query
        function createNewList() {
            //Get the current context
            var context = new SP.ClientContext();
            //Get the current web
            var web = context.get_web();
            //Create a listCreationInformation object to store the required properties
            var listCreationInfo = new SP.ListCreationInformation();
            //Set the required Title property
            listCreationInfo.set_title('MyProducts');
            //Set the required TemplateType property
            listCreationInfo.set_templateType(SP.ListTemplateType.genericList);
            //Create and load the new list object
            newList = web.get_lists().add(listCreationInfo);
            context.load(newList);
            //Create some XML to describe custom fields for the list
            var descriptionXml = '<Field DisplayName="Description" Type="Text" />';
            var priceXml = '<Field DisplayName="Price" Type="Currency" />';
            var departmentXml = '<Field DisplayName="Department" Type="Text" />';
            //Add the custom fields to the new list object
            newList.get_fields().addFieldAsXml(descriptionXml, true, SP.AddFieldOptions.defaultValue);
            newList.get_fields().addFieldAsXml(priceXml, true, SP.AddFieldOptions.defaultValue);
            newList.get_fields().addFieldAsXml(departmentXml, true, SP.AddFieldOptions.defaultValue);
            //Update the list object
            newList.update();
            //Run the query asynchronously, passing the functions to call when a response arrives
            context.executeQueryAsync(onSucceededCallback, onFailedCallback);
        }

        //This function fires when the query completes successfully
        function onSucceededCallback(sender, args) {
            //Formulate HTML from the list items
            var markup = 'The new list was successfully create. <br><br>';
            markup += 'List Title: ' + newList.get_title() + '<br>';
            markup += 'List ID: ' + newList.get_id();
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
        <a href="javascript:createNewList()">Click Here to Create a New List</a>
    </h2>
    <!-- This div displays the results -->
    <div id="displayDiv"></div>
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
    CSOM Demonstration
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
    Using CSOM to Create a New List
</asp:Content>
