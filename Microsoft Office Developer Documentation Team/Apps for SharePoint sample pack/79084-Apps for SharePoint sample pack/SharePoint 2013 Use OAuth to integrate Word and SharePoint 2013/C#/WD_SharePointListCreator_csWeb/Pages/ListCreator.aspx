<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ListCreator.aspx.cs" Inherits="WD_SharePointOAuth_csWeb.Pages.ListCreator" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta charset="UTF-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=9" />
    <title>List Creator</title>
    <link rel="stylesheet" type="text/css" href="../Content/Office.css" />

    <!-- Add your CSS styles to the following file -->
    <link rel="stylesheet" type="text/css" href="../Content/App.css" />

</head>
<body>
    <!-- This page is built dynamically after the list has been created (or attempted).
        Note how the page variables are used to control the page content-->
    <%if (success)
      {%>
    <h1>List created successfully!</h1>
    
    <p>You can access the new list <a target="_blank" href="<%=newListUrl%>">here</a></p>
    <%}
      else
      { %>
    <h1>An error occurred:</h1>
    <p>Error: <%=exception.Message%></p>
    <p>Technical Details: <%=exception.Message%></p>
    <%} %>
    <p><a href="OAuth.aspx?siteUrl=<%=Uri.EscapeUriString(connectedSiteUrl)%>">Back</a></p>

</body>
</html>
