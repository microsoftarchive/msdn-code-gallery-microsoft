<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProductClothesInfo.aspx.cs" Inherits="CSAzureReceiveSalesMessageViaTopic.ProductClothesInfo" MasterPageFile="~/WebMaster.Master" %>

<asp:Content ContentPlaceHolderID="head" runat="server" ID="cph_SalesOrderHead">
    <link href="CSS/OrderStyle.css" rel="stylesheet" />
    <script src="Scripts/jquery-1.10.2.min.js"></script>
    <style type="text/css">
        .TableRow {
            height: 30px;
            font-size: 11pt;
            color: black;
            text-align: center;
            margin: 0px 0px 0px 0px;
        }
         .DetailsButton {
            background-image: url('image/00Details.png');
            width: 60px;
            height: 22px;
        }
        .TableCell1 {
            border-bottom-color: #d7d7d7;
            border-bottom-width: 2px;
            border-bottom-style: solid;
            margin: 0px 0px 0px 0px;
            width: 12.5%;
            text-align: center;
            height: 26px;
            font-size:10pt;
        }
    </style>
    <script type="text/javascript">

        var CheckInput = function () {
            var retFlag = true;
            var strValue = $("#ContentPlaceHolder_Content_txt_FromDate").val();
            if (strValue.length > 0) {
                strValue = $("#ContentPlaceHolder_Content_txt_ToDate").val();
                if (strValue.length > 0) {
                        }
                        else {
                            alert("To date cant not be null!");
                            retFlag = false;
                        }
                   
            } else {
                            alert("From date cant not be null!");
                            retFlag = false;
                        }

            return retFlag;
        }

        $(document).ready(function () {


        });

    </script>
</asp:Content>
<asp:Content ContentPlaceHolderID="ContentPlaceHolder_Content" runat="server" ID="cph_SalesOrderContent">
    <asp:ScriptManager runat="server" ID="ScriptManager2">
    </asp:ScriptManager>
    <div class="PageTitle">
        General Information
    </div>
    <div class="SubmitSalesOrder">
        <div style="float: left">
            <div class="DivRowStyle">
                <div class="DivRowLableStyle">From :</div>
                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                    <ContentTemplate>
                        <div class="DivRowTextBoxStyle">
                            <asp:TextBox ID="txt_FromDate" runat="server" Style="margin: 0px 0px 0px 0px; float: left; width: 180px;" />
                            <asp:ImageButton ID="img_Btn_FromDate" runat="server" ImageUrl="Image/Calendar.png" AlternateText="calendar" OnClick="img_Btn_FromDate_Click" CausesValidation="false" Style="margin: 0px 0px 0px 0px; float: right;" />
                            <div id="calendarFrom" class="calendar" visible="false" runat="server" style="z-index: 1; margin: 0px 0px 0px 0px">
                                <asp:Calendar ID="RequestedFromDateCalendar" runat="server" OnSelectionChanged="RequestedFromDateCalendar_SelectionChanged" Style="z-index: 1; position: relative; top: 0px; left: 0px;" BackColor="White" />
                            </div>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>

            </div>

        </div>
        <div style="float: left">
            <div class="DivRowStyle">
                <div class="DivRowLableStyle">To:</div>
                <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                    <ContentTemplate>
                        <div class="DivRowTextBoxStyle">
                            <asp:TextBox ID="txt_ToDate" runat="server" Style="margin: 0px 0px 0px 0px; float: left; width: 180px;" />
                            <asp:ImageButton ID="img_Btn_ToDate" runat="server" ImageUrl="Image/Calendar.png" AlternateText="calendar" OnClick="img_Btn_ToDate_Click" CausesValidation="false" Style="margin: 0px 0px 0px 0px; float: right;" />
                            <div id="calendarTo" class="calendar" visible="false" runat="server" style="z-index: 1; margin: 0px 0px 0px 0px">
                                <asp:Calendar ID="requestedToDateCalendar" runat="server" OnSelectionChanged="RequestedToDateCalendar_SelectionChanged" Style="z-index: 1; position: relative; top: 0px; left: 0px;" BackColor="White" />
                            </div>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>

            </div>
        </div>
        <div style="float: right; width: 190px; text-align:center ;">
            <asp:Button ID="btn_Search" runat="server" Text="Search" Width="100" Height="30px" OnClientClick="return CheckInput()" OnClick="btn_Search_Click"/>

        </div>

    </div>
    <div class="TableHeadTitle">
       General Information 
    </div>
    <div style="margin: 5px 0px 0px 20px; width: 100%;overflow: scroll;height:300px; ">
        <asp:Table ID="tbl_ClothesOrder" runat="server" CellPadding="0" Width="100%" CellSpacing="0" EnableViewState="true" ViewStateMode="Enabled">
            <asp:TableHeaderRow Font-Size="8pt" BackColor="White" Height="0" BorderStyle="None">
                <asp:TableHeaderCell ForeColor="White" Height="1px" Visible="false"></asp:TableHeaderCell>
            </asp:TableHeaderRow>
            <asp:TableRow ID="NameSpace_TableHead1" CssClass="TableColHead">
                <asp:TableCell ID="cell_ProductOrderNo" CssClass="TableCell">ProductOrderNo</asp:TableCell>
                <asp:TableCell ID="cell_SalesOrderNo" CssClass="TableCell">SalesOrderNo</asp:TableCell>
                <asp:TableCell ID="cell_CreatedDate" CssClass="TableCell">CreatedDate</asp:TableCell>
                <asp:TableCell ID="cell_CustmerName" CssClass="TableCell">CustmerName</asp:TableCell>
                <asp:TableCell ID="cell_SalesManName" CssClass="TableCell">SalesManName</asp:TableCell>
                <asp:TableCell ID="cell_DeliveryDate" CssClass="TableCell">DeliveryDate</asp:TableCell>
                <asp:TableCell ID="cell_ShippingAddress" CssClass="TableCell">ShippingAddress</asp:TableCell>
                <asp:TableCell ID="cell_Details" CssClass="TableCell">Details</asp:TableCell>
                <%--<asp:TableCell ID="cell_Unit" CssClass="TableCell">Unit</asp:TableCell>--%>
            </asp:TableRow>
        </asp:Table>
    </div>
     <div class="TableHeadTitle">
       Product Infomation
    </div>
        <div style="margin: 5px 0px 0px 20px; width: 100%;overflow: scroll;height:50%;">
        <asp:Table ID="tbl_ProductionDetails" runat="server" CellPadding="0" Width="100%" CellSpacing="0" EnableViewState="true" ViewStateMode="Enabled">
            <asp:TableHeaderRow Font-Size="8pt" BackColor="White" Height="0" BorderStyle="None">
                <asp:TableHeaderCell ForeColor="White" Height="1px" Visible="false"></asp:TableHeaderCell>
            </asp:TableHeaderRow>
            <asp:TableRow ID="NameSpace_TableHead" CssClass="TableColHead">
                <asp:TableCell ID="cell_ProductClothesOrderId" CssClass="TableCell">DetailsOrderId</asp:TableCell>
                <asp:TableCell ID="cell_ProductNo" CssClass="TableCell">ProductNo</asp:TableCell>
                <asp:TableCell ID="cell_ProductName" CssClass="TableCell">ProductName</asp:TableCell>
                <asp:TableCell ID="cell_ProductColor" CssClass="TableCell">ProductColor</asp:TableCell>
                <asp:TableCell ID="cell_ProductSize" CssClass="TableCell">ProductSize</asp:TableCell>
                <asp:TableCell ID="cell_ProductPrice" CssClass="TableCell">ProductPrice</asp:TableCell>
                <asp:TableCell ID="cell_Number" CssClass="TableCell">Number</asp:TableCell>
                <asp:TableCell ID="cell_Unit" CssClass="TableCell">Unit</asp:TableCell>
            </asp:TableRow>
        </asp:Table>
    </div>
</asp:Content>
