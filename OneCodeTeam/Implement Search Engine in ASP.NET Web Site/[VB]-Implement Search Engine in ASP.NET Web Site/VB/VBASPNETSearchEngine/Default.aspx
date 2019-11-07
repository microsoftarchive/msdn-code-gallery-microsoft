<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Default.aspx.vb" Inherits="VBASPNETSearchEngine._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Search Engine</title>
</head>
<body>
    <form id="form1" runat="server" defaultfocus="txtKeyWords">

    <p>Please input one or more keywords (separated by spaces) and then click the search button.</p>

    <!-- Search Form -->
    <asp:Label ID="lbAlert" runat="server" ForeColor="Red"></asp:Label>
    <asp:TextBox ID="tbKeyWords" runat="server"></asp:TextBox>
    <asp:Button ID="btnSearch" runat="server" Text="Click to Search" onclick="btnSearch_Click" />

    <br /><br />

    <asp:Button ID="btnListAll" runat="server" Text="Click to show all data" onclick="btnListAll_Click" />

    <!-- Search Result -->
    <asp:Repeater ID="RepeaterSearchResult" runat="server">
        <HeaderTemplate>
            <h3>Search Result:</h3>
            <ol id="result">
        </HeaderTemplate>
        <ItemTemplate>
            <li>
                <h4><a href="<%# ResolveClientUrl("~/Show.aspx?id=" + Eval("ID").ToString()) %>" class="title"><%# Server.HtmlEncode(Eval("Title").ToString())%></a></h4>
                <p class="brief"><%# Server.HtmlEncode(Eval("Content").ToString())%></p>
            </li>
        </ItemTemplate>
        <FooterTemplate>
            </ol>
        </FooterTemplate>
    </asp:Repeater>

    <!-- use JavaScript to hightlight keywords. -->
    <script type="text/javascript">
        function HightLightKeywords() {
            var container = document.getElementById("result");
            var keywords = new Array();
            <%  
                ' This is VB.NET code runs in server side.
                For i As Integer = 0 To keywords.Count - 1
	                Response.Write(String.Format("keywords['{0}'] = '{1}';", i, keywords(i)))
                Next
            %>
            for (var i = 0; i < keywords.length; i++)
            {
                var a = new RegExp(keywords[i], "igm");
                container.innerHTML = container.innerHTML.replace(a, "<span style='background:#FF0;'>" + keywords[i] + "</span>");
            }
        }
        HightLightKeywords();
    </script>

    </form>
</body>
</html>
