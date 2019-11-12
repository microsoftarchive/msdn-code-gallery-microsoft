<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OAuth.aspx.cs" Inherits="WD_SharePointTranslation_csWeb.Pages.OAuth" %>

<!DOCTYPE html>

<html>
<head id="Head1" runat="server">
    <meta charset="UTF-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=9" />
    <title>OAuth Word to SharePoint Online</title>

    <link rel="stylesheet" type="text/css" href="../Content/Office.css" />

    <!-- Add your CSS styles to the following file -->
    <link rel="stylesheet" type="text/css" href="../Content/App.css" />

    <script src="../Scripts/jquery-1.7.1.js"></script>

    <!-- Use the CDN reference to Office.js when deploying your app 
    <script src="https://appsforoffice.microsoft.com/lib/1.0/hosted/office.js"></script>-->

    <!-- Use the local script references for Office.js to enable offline debugging-->
    <script src="../Scripts/Office/1.0/MicrosoftAjax.js"></script>
    <script src="../Scripts/Office/1.0/office.js"></script>
    

    <!-- Custom Javascript has been added to these files -->
    <script src="../Scripts/OAuthHelper.js"></script>
    <script src="../Scripts/WD_SharePointTranslation_cs.js"></script>
</head>

<body>
    <!-- Various sections of this document are built depending on 
        variables in OAuth.aspx.cs -->
    <%if (!connected)
      { %>
    <h1>Welcome to the Word and SharePoint OAuth Sample</h1>
    Please enter the URL of the SharePoint site you want to connect to:
    <form id="config" runat="server">
        <asp:HiddenField ID="clientId" runat="server" />
        <asp:HiddenField ID="redirectUrl" runat="server" />

        Site Url:
        <input id="siteUrl" name="siteUrl" type="text" size="40" value="" />
        <br />
        Target Language:
        <select id="targetLanguage">
            <option value="ar-sa" selected>Arabic</option>
            <option value="da">Danish</option>
            <option value="nl">Dutch</option>
            <option value="fr">French</option>
            <option value="de">German</option>
            <option value="es">Spanish</option>
            <option value="hi">Hindi</option>
            <option value="ur">Urdu</option>
        </select>
        <br /><br />
    </form>
    <button onclick="startFlow($('#siteUrl').val(), true);">Connect</button>

    <input id="oAuthCode" name="oAuthCode" type="hidden" value="" /><br />
    <input id="state" name="state" type="hidden" value="" />
    <%}
      else
      { %>
    <h1>Word and SharePoint OAuth Sample</h1>
    
    <p>Connected to: <a href="<%=connectedSiteUrl%>"><%=siteTitle%> </a></p>
    <p>App is ready to translate your document!</p>
    <form id="translationForm" action="TextTranslator.aspx" method="post">
        <input type="hidden" name="connectedSiteUrl" id="connectedSiteUrl" value="<%=connectedSiteUrl %>" />
        <input type="hidden" name="refreshToken" id="refreshToken" value="<%=refreshToken%>" />
        <input type="hidden" name="accessToken" id="accessToken" value="<%=accessToken%>" />
        <textarea id="documentContent" name="documentContent" style="display:none"></textarea>
        <input type="hidden" name="documentLanguage" id="documentLanguage" value="" />
        <br />
    </form>
    <button onclick="translateNow();">Translate Now</button>
    <%} %>
    <div id="Status"></div>
</body>
</html>
