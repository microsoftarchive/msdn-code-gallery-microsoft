<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true"
    CodeBehind="WebForm1.aspx.cs" Inherits="CSASPNETAddEndRequestEventInUpdatepanel.WebForm1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <script src="Script/jquery-1.4.1.min.js" type="text/javascript"></script>

    <div id="iBuy" style="display: none;">
        <asp:ScriptManagerProxy ID="iSmpSum" runat="server">
            <Services>
                <asp:ServiceReference Path="~/WebService2.asmx" />
            </Services>
        </asp:ScriptManagerProxy>
        Please enter the number you want to buy:<input type="text" id="iNumber" size="20" />
        Please enter the current price of books :<input id="iPrice" size="20" type="text" />
        <input id="Button1" type="button" value="Calculate" onclick="return OnbtCount_Click()" /><br />
        <div id="iSumMoney">
        </div>
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
        $(document).ready(function() {
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(endRequestHandle);

            function endRequestHandle(sender, Args) {
                if (!Sys.WebForms.PageRequestManager.getInstance().get_isInAsyncPostBack())
                    __doPostBack('LinkButton1', '');
                //alert test
                //                var testGrid = $get('<%=grid1.ClientID %>');
                //                alert(testGrid.rows[1].cells[0].innerHTML);
            }
        });

        function OnbtCount_Click() {
            var tNumber = document.getElementById("iNumber").value;
            var tPrice = tbook = document.getElementById("iPrice").value;
            var wsp = CSASPNETAddEndRequestEventInUpdatepanel.WebService2;
            wsp.oMoney(tNumber, tPrice, ShowMoney);
            return false;
        }

        function ShowMoney(result) {
            var sResult = result.toString();
            document.getElementById("iSumMoney").innerHTML = sResult;
        }
    </script>

</asp:Content>
