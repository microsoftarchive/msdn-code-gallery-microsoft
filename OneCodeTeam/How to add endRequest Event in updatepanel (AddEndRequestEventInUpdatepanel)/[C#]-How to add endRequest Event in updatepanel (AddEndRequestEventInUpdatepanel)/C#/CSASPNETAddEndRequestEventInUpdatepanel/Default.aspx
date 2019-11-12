<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="CSASPNETAddEndRequestEventInUpdatepanel._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta http-equiv="content-type" content="text/html; charset=UTF-8" />
    <title>default</title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager" runat="server" LoadScriptsBeforeUI="false">
    </asp:ScriptManager>
    <div>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" EnableViewState="false">
            <ContentTemplate>
                <asp:Timer ID="Timer1" Interval="3000" runat="server">
                </asp:Timer>
                <asp:GridView ID="grid1" runat="server" AutoGenerateColumns="False" DataKeyNames="Id"
                    DataSourceID="SqlDataSource2">
                    <Columns>
                        <asp:BoundField DataField="Id" HeaderText="Id" InsertVisible="False" ReadOnly="True"
                            SortExpression="Id" />
                        <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" />
                        <asp:BoundField DataField="Score" HeaderText="Score" SortExpression="Score" />
                    </Columns>
                </asp:GridView>
                <asp:SqlDataSource ID="SqlDataSource2" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>"
                    SelectCommand="SELECT [Id], [Name], [Score] FROM [BooksScore]"></asp:SqlDataSource>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="LinkButton1" />
            </Triggers>
        </asp:UpdatePanel>
         <asp:LinkButton ID="LinkButton1" runat="server" onclick="LinkButton1_Click"></asp:LinkButton>
    </div>

    <script type="text/javascript">
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(endRequestHandler);

        function endRequestHandler(sender, args) {
            if (!Sys.WebForms.PageRequestManager.getInstance().get_isInAsyncPostBack())
    __doPostBack('LinkButton1', '');
                //alert test
//                var testGrid = $get('<%=grid1.ClientID %>');
//                alert(testGrid.rows[1].cells[0].innerHTML);
        }
    </script>

    </form>
</body>
</html>
