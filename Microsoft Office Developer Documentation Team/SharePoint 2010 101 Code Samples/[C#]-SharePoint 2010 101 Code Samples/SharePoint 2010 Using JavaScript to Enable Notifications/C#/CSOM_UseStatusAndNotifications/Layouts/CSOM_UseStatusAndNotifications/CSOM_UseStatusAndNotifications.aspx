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
        //This variable holds an ID for a status so it can be modified or removed
        var demoStatusID

        //This function sets a Status
        function setStatus() {
            //Create strings for the title and status 
            var titleText = "Demonstration";
            var statusHtml = "<span>This status was set by <b>JavaScript</b> using <i>CSOM</i></span>";
            //Call the addStatus function
            demoStatusID = SP.UI.Status.addStatus(titleText, statusHtml, true);
        }

        //This function modifies a status that already exists
        function modifyStatus() {
            //Create strings for the title and status
            var statusHtml = "<span>This status has been updated</span>";
            //Call the updateStatus function
            SP.UI.Status.updateStatus(demoStatusID, statusHtml);
        }

        //This function modifies the color of a status that already exists
        function colorStatus() {
            //Call the setStatusPriColor function
            SP.UI.Status.setStatusPriColor(demoStatusID, 'red');
        }

        //This function clears all the statuses in the status bar
        function clearAllStatus() {
            //Call the removeAllStatus function
            SP.UI.Status.removeAllStatus(true);
        }

        //This function displays a notification
        function showNotification() {
            //Formulate the HTML for the notification
            var noteHtml = "<img src='../images/loading.gif'>This notification was shown by <b>JavaScript</b>";
            //Call the addNotification function
            var noteID = SP.UI.Notify.addNotification(noteHtml, false);
        }

    </script>
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <!-- You must add a FormDigest control to the page before you use 
    JavaScript to call the CSOM. This inserts a security validation 
    token into the page that can be used to prevent malicious attacks -->
    <SharePoint:FormDigest ID="FormDigest1" runat="server" />
    <h2>
        <a href="javascript:setStatus()">Click Here to Set the Status</a>
    </h2>
    <h2>
        <a href="javascript:modifyStatus()">Click Here to Update the Status</a>        
    </h2>
    <h2>
        <a href="javascript:colorStatus()">Click Here to Change the Status Color</a>        
    </h2>
    <h2>
        <a href="javascript:clearAllStatus()">Click Here to Clear the Status</a>        
    </h2>
    <h2>
        <a href="javascript:showNotification()">Click Here to Show a Notification</a>        
    </h2>
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
    CSOM Demonstration
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
    Using CSOM to Set Status and Notifications
</asp:Content>
