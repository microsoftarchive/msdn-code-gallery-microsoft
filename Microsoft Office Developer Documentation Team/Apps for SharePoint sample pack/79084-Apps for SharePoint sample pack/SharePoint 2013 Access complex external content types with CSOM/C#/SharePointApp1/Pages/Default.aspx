<%-- The following 4 lines are ASP.NET directives needed when using SharePoint components --%>
<%@ Page Inherits="Microsoft.SharePoint.WebPartPages.WebPartPage, Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" MasterPageFile="~masterurl/default.master" language="C#" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<%-- The markup and script in the following Content element will be placed in the <head> of the page --%>
<asp:Content ContentPlaceHolderId="PlaceHolderAdditionalPageHead" runat="server">
    <script type="text/javascript" src="../Scripts/jquery-1.6.2.min.js"></script>

    <!-- Add your CSS styles to the following file -->
    <link rel="Stylesheet" type="text/css" href="../Content/App.css" />

    <!-- Add your JavaScript to the following file -->
    <script type="text/javascript" src="../Scripts/App.js"></script>

    <!-- The following script runs when the DOM is ready. The inline code uses a SharePoint feature to ensure -->
    <!-- The SharePoint script file sp.js is loaded and will then execute the sharePointReady() function in App.js -->
    <script type="text/javascript">
        $(document).ready(function () {
            SP.SOD.executeFunc('sp.js', 'SP.ClientContext', function () { sharePointReady(); });
        });
    </script>
</asp:Content>

<%-- The markup and script in the following Content element will be placed in the <body> of the page --%>
<asp:Content ContentPlaceHolderId="PlaceHolderMain" runat="server">

    <div>
        <p id="message">
            <!-- The following content will be replaced with the user name when you run the app - see App.js -->
            initializing...
        </p>
    </div>

    <div id="TopDiv">
        <input type="button" value="Employee List" onclick="javascript:window.location = '../lists/Employee'"/>
    </div>
    <div id="ContactCardTemplate" class="ContactCard" style="display:none">
		<div class="ContactCardTools">
			<img src="../Images/Delete.png" style="float:right; height:20px; margin-right:5px" onclick="DeleteItem(this.parentNode.parentNode);return false;"/>
			<img src="../Images/Edit.png" style="float:right; height:20px; margin-right:5px" onclick="ShowEditWindow(this.parentNode.parentNode);return false;"/>
		</div>
		<div class="icon"><img src="../Images/Person.jpg"/></div>
		<div class="ContactCardDetails">
			<div class="ContactCardFirstName"></div>
			<div class="ContactCardLastName"></div>
			<div class="ContactCardID"></div>
			<div class="ContactCardPhone"></div>
            <div class="BDCIdentity" style="display:none"></div>
		</div>
    </div>

    <div id="Container"></div>


</asp:Content>
