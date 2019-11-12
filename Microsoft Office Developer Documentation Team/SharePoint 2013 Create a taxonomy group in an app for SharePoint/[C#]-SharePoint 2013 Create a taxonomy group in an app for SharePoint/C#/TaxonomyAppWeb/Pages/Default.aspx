<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="TaxonomyAppWeb.Pages.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Taxonomy App</title>
    <script src="<%Response.Write(Request.QueryString["SPHostUrl"]); %>/_layouts/15/sp.ui.controls.js" type="text/javascript"></script>
    <script src="../Scripts/jquery-1.8.2.js" type="text/javascript"></script>
    <script type="text/javascript">
        function chromeLoaded() {
            $('body').show();
        }
    </script>
</head>
<body style="overflow: scroll; display: none">
    <form id="form1" runat="server">
        <div id="chrome_ctrl_container" data-ms-control="SP.UI.Controls.Navigation" data-ms-options='{
                "appHelpPageUrl" : "/?<%Response.Write(Request.QueryString); %>",
                "appIconUrl" : "/Content/SPAppIconHexagonBlue.png",
                "appTitle" : "Taxonomy CSOM",
                "onCssLoaded" : "chromeLoaded()",
                "settingsLinks" : [
                    {
                        "linkUrl" : "../?<%Response.Write(Request.QueryString); %>",
                        "displayName" : "Settings"
                    },
                    {
                        "linkUrl" : "../default.aspx?<%Response.Write(Request.QueryString); %>",
                        "displayName" : "Support"
                    }
                  ]
                }'>
        </div>

        <div style="width: 800px; padding-left: 20px">
            <h1>Taxonomy Creation Demonstration</h1>
            <p>
                The code behind for this application page demonstrates how to add groups,
        termsets and terms to a Term Store in the Managed Metadata service application.
        Before you click the button, ensure you have a Managed Metadata service 
        application running and available from the current site and 
        your user id is configured as taxonomy Admin. 
            </p>
            <div>
                <asp:Button ID="Button1" runat="server" Text="Create Plant Taxonomy" OnClick="Button1_Click" />
                <asp:Label ID="resultsLabel" runat="server" Text=""></asp:Label>
            </div>
            <br />
            <div>
                <asp:Button ID="Button2" runat="server" OnClick="Button2_Click" Text="Display Plant Taxonomy" />
                <div style="padding-left: 10px; padding-top: 20px">
                    <asp:Label ID="plantTaxonomy" runat="server" Text=""></asp:Label>
                </div>
            </div>
        </div>

    </form>
</body>
</html>
