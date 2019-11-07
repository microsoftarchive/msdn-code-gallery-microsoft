<%@ Page language="C#" MasterPageFile="~masterurl/default.master" Inherits="Microsoft.SharePoint.WebPartPages.WebPartPage, Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<asp:Content ContentPlaceHolderId="PlaceHolderAdditionalPageHead" runat="server">
    <SharePoint:ScriptLink name="sp.js" runat="server" OnDemand="true" LoadAfterUI="true" Localizable="false" />
</asp:Content>

<asp:Content ContentPlaceHolderId="PlaceHolderMain" runat="server">
    <WebPartPages:WebPartZone runat="server" FrameType="TitleBarOnly" ID="full" Title="loc:full" />

    <h1>Desktop List View Samples</h1>
    <h2>Office Web Widgets –Experimental</h2>
    <p>The following code samples show you how to use the Desktop List View control:</p>
    <ul>
        <li><a id="mus" href="#">MarkupSimple</a> - Learn how to declare the List View control in HTML markup.</li>
        <li><a id="jss" href="#">JSSimple</a> - Shows how to declare the List View control in JavaScript.</li>
    </ul>
    <p>Learn more about the <a href="http://msdn.microsoft.com/library/6ce01956-6bda-45bf-9b4a-cffc0687a913">Office Web Widgets –Experimental</a> at dev.office.com</p>

    <script type="text/javascript">
        //Build the URLs for the links above
        var remoteAppUrl;
        var link;
        var qs;

        qs = "?" + document.URL.split("?")[1];
        remoteAppUrl = decodeURIComponent(getQueryStringParameter("RemoteAppUrl"));

        link = document.getElementById("mus");
        link.href = remoteAppUrl + "/Pages/MarkupSimple.html" + qs;

        link = document.getElementById("jss");
        link.href = remoteAppUrl + "/Pages/JSSimple.html" + qs;

        function getQueryStringParameter(paramToRetrieve) {
            var params =
                document.URL.split("?")[1].split("&");
            var strParams = "";
            for (var i = 0; i < params.length; i = i + 1) {
                var singleParam = params[i].split("=");
                if (singleParam[0] == paramToRetrieve)
                    return singleParam[1];
            }
        }
    </script>
</asp:Content>
