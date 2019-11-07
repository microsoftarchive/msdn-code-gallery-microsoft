<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <meta name="viewport" content="width=device-width" />
    <title>TestPage</title>
</head>
<body>
    <div>
        <%using (Html.BeginForm("SaveSession", "Home", FormMethod.Post))
          { %>
        <p><%=ViewData["sessionString"] %> </p>
        <fieldset>
            <div>
                The text will be stored: <%=Html.TextBox("sessionValue")%>
            </div>
            <div>
                <input type="submit" id="submitToMethod1" name="submit" value="submit" />
            </div>
        </fieldset>
        <%} %>

        <%using (Html.BeginForm("SaveSessionByExtensions", "Home", FormMethod.Post))
          { %>
        <p><%=ViewData["sessionStringByExtensions"] %> </p>
        <fieldset>
            <div>
                The text will be stored: <%=Html.TextBox("sessionValue")%>
            </div>
            <div>
                <input type="submit" id="submitToMethod2" name="submit" value="submit" />
            </div>
        </fieldset>
        <%} %>
    </div>
</body>
</html>
