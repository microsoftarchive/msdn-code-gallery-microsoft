<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Default.aspx.vb" Inherits="VBASPNETRatingControlSelectCurrentValue._Default" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <link href="Rate.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
        </asp:ToolkitScriptManager>
        <asp:GridView ID="gdvBooks" runat="server" AutoGenerateColumns="False" DataKeyNames="Id"
            OnRowCommand="gdvBooks_RowCommand">
            <Columns>
                <asp:BoundField DataField="Id" HeaderText="Id" InsertVisible="False" ReadOnly="True"
                    SortExpression="Id" />
                <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" />
                <asp:BoundField DataField="Author" HeaderText="Author" SortExpression="Author" />
                <asp:BoundField DataField="PublishDate" HeaderText="PublishDate" SortExpression="PublishDate" />
                <asp:TemplateField HeaderText="Rate">
                    <ItemTemplate>
                        <asp:LinkButton ID="LinkbtnSubmit" runat="server" CommandName="RateDetail" CommandArgument='<%# Eval("Rate") %>'>RateDetail</asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
        Rate for current book:
        <asp:Rating ID="Rating1" BehaviorID="Rating1" runat="server" CurrentRating="2" MaxRating="5"
            RatingAlign="Horizontal" RatingDirection="LeftToRightTopToBottom" StarCssClass="ratingStar"
            WaitingStarCssClass="savedRatingStar" FilledStarCssClass="filledRatingStar" EmptyStarCssClass="emptyRatingStar">
        </asp:Rating>
        <asp:Button ID="btnSubmit" runat="server" Text="Submit" OnClick="btnSubmit_Click"
            Style="visibility: hidden" />
        <br />
        <asp:Label ID="lbResponse" runat="server" Text=""></asp:Label>
    </div>

    <script src="Rate.js" type="text/javascript"></script>

    </form>
</body>
</html>
