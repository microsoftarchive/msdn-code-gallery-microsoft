<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BuildSalesOrder.aspx.cs" Inherits="SalesSendMessagesViaTopic.BuildSalesOrder" MasterPageFile="~/WebMaster.Master" EnableViewState="true" ViewStateMode="Enabled" %>

<asp:Content ContentPlaceHolderID="head" runat="server" ID="cph_SalesOrderHead">
    <link href="CSS/SalesOrderStyle.css" rel="stylesheet" />
    <script src="Scripts/jquery-1.10.2.min.js"></script>
    <style type="text/css">
        .TableRow {
            height: 30px;
            font-size: 11pt;
            color: black;
            text-align: center;
            margin: 0px 0px 0px 0px;
        }

        .TableCellTextBox1 {
            border-bottom-color: #d7d7d7;
            border-bottom-width: 2px;
            border-bottom-style: solid;
            width: 100px;
            text-align: center;
            height: 24px;
        }

        .TableCell1 {
            border-bottom-color: #d7d7d7;
            border-bottom-width: 2px;
            border-bottom-style: solid;
            margin: 0px 0px 0px 0px;
            width: 12.5%;
            text-align: center;
            height: 26px;
        }
    </style>
    <script type="text/javascript">
        var AddNewRow = function () {
            var Table = document.getElementById("ContentPlaceHolder_Content_tbl_Production");
            var rowLength = Table.rows.length;
            for (i = 2; i < rowLength; i++) {
                var displayValue = $("#ContentPlaceHolder_Content_tbl_Production tbody tr").eq(i).css("display");
                if (displayValue == "none") {
                    $("#ContentPlaceHolder_Content_tbl_Production tbody tr").eq(i).css("display", "table-row");
                    break;
                }
            }
            var indexCount = 1;
            for (i = 2; i < rowLength; i++) {
                var displayValue = $("#ContentPlaceHolder_Content_tbl_Production tbody tr").eq(i).css("display");
                if (displayValue == "table-row") {
                    $("#ContentPlaceHolder_Content_tbl_Production tbody tr").eq(i).find("td").eq(1).html(indexCount);
                    indexCount++;
                }
            }
            return false;
        }

        var RemoveTableRow=function(obj){
            var indexCount = obj + 1;
            var rowLength = $("#ContentPlaceHolder_Content_tbl_Production tr").length;
            $("#ContentPlaceHolder_Content_tbl_Production tbody tr").eq(indexCount).find("input[type='text']").val("");
            if (indexCount >= 2)
            { $("#ContentPlaceHolder_Content_tbl_Production tbody tr").eq(indexCount).css("display", "none"); }
           
            indexCount = 1;
            for (i = 2; i < rowLength; i++) {
                var displayValue = $("#ContentPlaceHolder_Content_tbl_Production tbody tr").eq(i).css("display");
                if(displayValue=="table-row")
                {
                    $("#ContentPlaceHolder_Content_tbl_Production tbody tr").eq(i).find("td").eq(1).html(indexCount);
                    indexCount++;
                }
            }
            return false;
        }
        
        var GetRowTable=function()
        {
            var RowStatus = "";
            var rowLength = $("#ContentPlaceHolder_Content_tbl_Production tr").length;
            for (i = 1; i < rowLength; i++) {
                var displayValue = $("#ContentPlaceHolder_Content_tbl_Production tbody tr").eq(i).css("display");
                RowStatus = RowStatus + "_" + displayValue;
            }
            $.post("SaveRowStatusInfo.aspx", { "RowStatus": RowStatus }, function (json, status) {
                if (json.length > 0) {
                    return true;
                }
            }

            );
        }

        var CheckInput = function () {
            var retFlag = false;
            var strValue = $("#ContentPlaceHolder_Content_txt_CustomerName").val();
            if(strValue.length>0){
                strValue = $("#ContentPlaceHolder_Content_txt_TelePhone").val();
                if (strValue.length > 0) {
                    strValue = $("#ContentPlaceHolder_Content_txt_DeliveryDate").val();
                    if (strValue.length > 0) {
                        strValue = $("#ContentPlaceHolder_Content_txt_ShippingAddress").val();
                        if (strValue.length > 0) {
                          
                            var rowLength = $("#ContentPlaceHolder_Content_tbl_Production tr").length;
                            for (i = 2; i < rowLength; i++) {
                                var displayValue = $("#ContentPlaceHolder_Content_tbl_Production tbody tr").eq(i).css("display");
                                if (displayValue == "table-row")
                                {
                                    var retRowFlag = true;
                                    var col = $("#ContentPlaceHolder_Content_tbl_Production tbody tr").eq(i).find("td").find("input[type='text']").length;
                                    for( j=0;j<col;j++)
                                    {
                                        var obj=$("#ContentPlaceHolder_Content_tbl_Production tbody tr").eq(i).find("td").find("input[type='text']").eq(j).val();
                                        if(obj.length<=0)
                                        {
                                            retRowFlag = false;
                                            alert("Product Infomation can not be null!");
                                            break;
                                        }
                                    }

                                }
                            }

                        }
                        else {
                            alert("ShippingAddress cant not be null!");
                            retFlag = false;
                        }
                    }
                    else {
                        alert("DeliveryDate cant not be null!");
                        retFlag = false;
                    }
                }
                else {
                    alert("TelePhone cant not be null!");
                    retFlag = false;
                }
            }
            else
            {
                alert("Custmer Name cant not be null!");
                retFlag = false;
            }
            if (retFlag == true)
            {
                retFlag = GetRowTable();
            }
            return retFlag;
        }

        var CancelInput = function () {
            $("#ContentPlaceHolder_Content_txt_CustomerName").val("");
            $("#ContentPlaceHolder_Content_txt_TelePhone").val("");
            $("#ContentPlaceHolder_Content_txt_DeliveryDate").val("");
            $("#ContentPlaceHolder_Content_txt_ShippingAddress").val("");
            $("#ContentPlaceHolder_Content_tbl_Production").find("input[type='text']").val("");
            var rowLength = $("#ContentPlaceHolder_Content_tbl_Production tr").length;
            for (i = 3; i < rowLength; i++) {
                var row = $("#ContentPlaceHolder_Content_tbl_Production tbody tr").eq(i).css("display");
                $("#ContentPlaceHolder_Content_tbl_Production tbody tr").eq(i).css("display", "none");
            }
            return false;
        }

        $(document).ready(function () {

            var retCustomerFlag = false;
            $("#ContentPlaceHolder_Content_txt_CustomerName").focusin(function () {
                retCustomerFlag = true;
            });

            $("#ContentPlaceHolder_Content_txt_CustomerName").focusout(function () {
                retCustomerFlag = false;
                var CustomerName = $("#ContentPlaceHolder_Content_txt_CustomerName").val();
                $.post("GetCustomer.aspx", { "strCustomerName": CustomerName }, function (json, status) {
                   
                    if (json.length > 0) {
                        $("#ContentPlaceHolder_Content_txt_ShippingAddress").val("");
                        $("#ContentPlaceHolder_Content_txt_TelePhone").val("");
                        var jsonObj = jQuery.parseJSON(json);
                        for (var i = 0; i < jsonObj.Customer.length; i++) {
                            $("#ContentPlaceHolder_Content_txt_ShippingAddress").val(jsonObj.Customer[0].Address);
                            $("#ContentPlaceHolder_Content_txt_TelePhone").val(jsonObj.Customer[0].TelePhone);
                        }
                    }
                    
                });
            });
           
            $("#ContentPlaceHolder_Content_txt_1_ProductNo").focusout(function () {
                var ProductNo = $("#ContentPlaceHolder_Content_txt_1_ProductNo").val();
                $.post("GetProduct.aspx", { "strProductNo": ProductNo }, function (json, status) {
                    $("#ContentPlaceHolder_Content_txt_1_ProductName").val("");
                    $("#ContentPlaceHolder_Content_txt_1_ProductSize").val("");
                    $("#ContentPlaceHolder_Content_txt_1_ProductColor").val("");
                    $("#ContentPlaceHolder_Content_txt_1_ProductPrice").val("");
                    $("#ContentPlaceHolder_Content_txt_1_ProductId").val("");
                    if (json.length > 0) {

                        var jsonObj = jQuery.parseJSON(json);
                        for (var i = 0; i < jsonObj.Product.length; i++) {
                            $("#ContentPlaceHolder_Content_txt_1_ProductName").val(jsonObj.Product[0].ProductName);
                            $("#ContentPlaceHolder_Content_txt_1_ProductSize").val(jsonObj.Product[0].ProductSize);
                            $("#ContentPlaceHolder_Content_txt_1_ProductColor").val(jsonObj.Product[0].ProductColor);
                            $("#ContentPlaceHolder_Content_txt_1_ProductPrice").val(jsonObj.Product[0].ProductPrice);
                            $("#ContentPlaceHolder_Content_txt_1_ProductId").val(jsonObj.Product[0].ProductId);
                            
                        }
                    }else
                    {
                        alert("There is no this product infomation!");
                    }

                });
            });

            $("#ContentPlaceHolder_Content_txt_2_ProductNo").focusout(function () {
                var ProductNo = $("#ContentPlaceHolder_Content_txt_2_ProductNo").val();
                $.post("GetProduct.aspx", { "strProductNo": ProductNo }, function (json, status) {
                    $("#ContentPlaceHolder_Content_txt_2_ProductName").val("");
                    $("#ContentPlaceHolder_Content_txt_2_ProductSize").val("");
                    $("#ContentPlaceHolder_Content_txt_2_ProductColor").val
                    $("#ContentPlaceHolder_Content_txt_2_ProductPrice").val("");
                    $("#ContentPlaceHolder_Content_txt_2_ProductId").val("");
                    if (json.length > 0) {
                        var jsonObj = jQuery.parseJSON(json);
                        for (var i = 0; i < jsonObj.Product.length; i++) {
                            $("#ContentPlaceHolder_Content_txt_2_ProductName").val(jsonObj.Product[0].ProductName);
                            $("#ContentPlaceHolder_Content_txt_2_ProductSize").val(jsonObj.Product[0].ProductSize);
                            $("#ContentPlaceHolder_Content_txt_2_ProductColor").val(jsonObj.Product[0].ProductColor);
                            $("#ContentPlaceHolder_Content_txt_2_ProductPrice").val(jsonObj.Product[0].ProductPrice);
                            $("#ContentPlaceHolder_Content_txt_2_ProductId").val(jsonObj.Product[0].ProductId);
                        }
                    } else {
                        alert("There is no this product infomation!");
                    }

                });
            });
            $("#ContentPlaceHolder_Content_txt_3_ProductNo").focusout(function () {
                var ProductNo = $("#ContentPlaceHolder_Content_txt_3_ProductNo").val();
                $.post("GetProduct.aspx", { "strProductNo": ProductNo }, function (json, status) {
                    $("#ContentPlaceHolder_Content_txt_3_ProductName").val("");
                    $("#ContentPlaceHolder_Content_txt_3_ProductSize").val("");
                    $("#ContentPlaceHolder_Content_txt_3_ProductColor").val("");
                    $("#ContentPlaceHolder_Content_txt_3_ProductPrice").val("");
                    $("#ContentPlaceHolder_Content_txt_3_ProductId").val("");
                    if (json.length > 0) {
                        var jsonObj = jQuery.parseJSON(json);
                        for (var i = 0; i < jsonObj.Product.length; i++) {
                            $("#ContentPlaceHolder_Content_txt_3_ProductName").val(jsonObj.Product[0].ProductName);
                            $("#ContentPlaceHolder_Content_txt_3_ProductSize").val(jsonObj.Product[0].ProductSize);
                            $("#ContentPlaceHolder_Content_txt_3_ProductColor").val(jsonObj.Product[0].ProductColor);
                            $("#ContentPlaceHolder_Content_txt_3_ProductPrice").val(jsonObj.Product[0].ProductPrice);
                            $("#ContentPlaceHolder_Content_txt_3_ProductId").val(jsonObj.Product[0].ProductId);
                        }
                    } else {
                        alert("There is no this product infomation!");
                    }

                });
            });
          
            $("#ContentPlaceHolder_Content_txt_4_ProductNo").focusout(function () {
                var ProductNo = $("#ContentPlaceHolder_Content_txt_4_ProductNo").val();
                $.post("GetProduct.aspx", { "strProductNo": ProductNo }, function (json, status) {
                    $("#ContentPlaceHolder_Content_txt_4_ProductName").val("");
                    $("#ContentPlaceHolder_Content_txt_4_ProductSize").val("");
                    $("#ContentPlaceHolder_Content_txt_4_ProductColor").val("");
                    $("#ContentPlaceHolder_Content_txt_4_ProductPrice").val("");
                    $("#ContentPlaceHolder_Content_txt_4_ProductId").val("")
                    if (json.length > 0) {
                        var jsonObj = jQuery.parseJSON(json);
                        for (var i = 0; i < jsonObj.Product.length; i++) {
                            $("#ContentPlaceHolder_Content_txt_4_ProductName").val(jsonObj.Product[0].ProductName);
                            $("#ContentPlaceHolder_Content_txt_4_ProductSize").val(jsonObj.Product[0].ProductSize);
                            $("#ContentPlaceHolder_Content_txt_4_ProductColor").val(jsonObj.Product[0].ProductColor);
                            $("#ContentPlaceHolder_Content_txt_4_ProductPrice").val(jsonObj.Product[0].ProductPrice);
                            $("#ContentPlaceHolder_Content_txt_4_ProductId").val(jsonObj.Product[0].ProductId);
                        }
                    } else {
                        alert("There is no this product infomation!");
                    }

                });
            });

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
                <div class="DivRowLableStyle">Sales Order No</div>
                <div class="DivRowTextBoxStyle">
                    <asp:TextBox ID="txt_SalesOrderNo" runat="server" Width="100%"></asp:TextBox>
                </div>
            </div>
            <div class="DivRowStyle">
                <div class="DivRowLableStyle">Sales Order Type</div>
                <div class="DivRowTextBoxStyle">
                    <asp:DropDownList ID="dpd_SalesOrderType" runat="server" Width="100%">
                        <asp:ListItem Value="Clothes"></asp:ListItem>
                        <asp:ListItem Value="Footwear"></asp:ListItem>
                    </asp:DropDownList>
                </div>
            </div>
            <%--<div class="DivRowStyle">
                <div class="DivRowLableStyle">Sales Order Status</div>
                <div class="DivRowTextBoxStyle">
                    <asp:DropDownList ID="dpd_SalesOrderStatus" runat="server" Width="100%">
                        <asp:ListItem Value="Added"></asp:ListItem>
                        <asp:ListItem Value="producing"></asp:ListItem>
                        <asp:ListItem Value="Delivering"></asp:ListItem>
                    </asp:DropDownList>
                </div>
            </div>--%>
        <div class="DivRowStyle">
                <div class="DivRowLableStyle">Salesman</div>
                <div class="DivRowTextBoxStyle">
                    <asp:DropDownList ID="dpd_SalesMan" runat="server" Width="100%">
                        
                    </asp:DropDownList>
                </div>
            </div>
             </div>
        <div style="float: right">
            
            <div class="DivRowStyle">
                <div id="div_Customer" class="DivRowLableStyle">Customer</div>
                <div class="DivRowTextBoxStyle">
                    <asp:TextBox ID="txt_CustomerName" runat="server" Width="100%" ></asp:TextBox>
                </div>
            </div>
            <div class="DivRowStyle">
                <div class="DivRowLableStyle">Telephone</div>
                <div class="DivRowTextBoxStyle">
                    <asp:TextBox ID="txt_TelePhone" runat="server" Width="100%"></asp:TextBox>
                </div>
            </div>
            <div class="DivRowStyle">
                <div class="DivRowLableStyle">Delivery Date</div>
                    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                        <ContentTemplate>
                            <div class="DivRowTextBoxStyle">
                             <asp:TextBox ID="txt_DeliveryDate" runat="server" Style="margin: 0px 0px 0px 0px; float: left; width: 180px;" />
                            <asp:ImageButton ID="imageButton" runat="server" ImageUrl="Image/Calendar.png" AlternateText="calendar" OnClick="ImageButton_Click" CausesValidation="false" Style="margin: 0px 0px 0px 0px; float: right;" />                                                        
                            <div id="calendar" class="calendar" visible="false" runat="server" style="z-index: 1; margin: 0px 0px 0px 0px">
                                <asp:Calendar ID="requestedDeliveryDateCalendar" runat="server" OnSelectionChanged="RequestedDeliveryDateCalendar_SelectionChanged" Style="z-index: 1; position: relative; top: 0px; left: 0px;" BackColor="White" />
                           </div>
                          </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
               
            </div>
        </div>
        <div class="DivRowStyle" style="width: 600px">
            <div class="DivRowLableStyle">Shipping Address</div>
            <div class="DivRowTextBoxStyle" style="width: 450px; z-index: -1">
                <asp:TextBox ID="txt_ShippingAddress" runat="server" Width="440px" Style="z-index: -1; "></asp:TextBox>
            </div>
        </div>
       <%-- <div class="DivRowStyle" style="width: 600px; height: 55px">
            <div class="DivRowLableStyle" style="height: 50px;">
                <div style="margin: 15px 0px 10px 0px">Comment</div>
            </div>
            <div class="DivRowTextBoxStyle" style="width: 450px; height: 50px; z-index: -1">
                <asp:TextBox ID="txt_Coment" runat="server" Width="440px" Height="50" Style="z-index: -1; "></asp:TextBox>
            </div>
        </div>--%>
    </div>
    <div class="TableHeadTitle">
        Production Infomation
    </div>
    <div style="margin: 5px 0px 0px 20px; width: 100%">
        <asp:Table ID="tbl_Production" runat="server" CellPadding="0" Width="100%" CellSpacing="0" EnableViewState="true" ViewStateMode="Enabled">
            <asp:TableHeaderRow Font-Size="8pt" BackColor="White" Height="0" BorderStyle="None">
                <asp:TableHeaderCell ForeColor="White" Height="1px" Visible="false"></asp:TableHeaderCell>
            </asp:TableHeaderRow>
            <asp:TableRow ID="NameSpace_TableHead" CssClass="TableColHead">
                <asp:TableCell ID="cell_delete" CssClass="TableCell"></asp:TableCell>
                <asp:TableCell ID="cell_Ordinal" CssClass="TableCell">Ordinal</asp:TableCell>
                <asp:TableCell ID="cell_ProductNo" CssClass="TableCell">ProductNo</asp:TableCell>
                <asp:TableCell ID="cell_ProductName" CssClass="TableCell">ProductName</asp:TableCell>
                <asp:TableCell ID="cell_ProductColor" CssClass="TableCell">ProductColor</asp:TableCell>
                <asp:TableCell ID="cell_ProductSize" CssClass="TableCell">ProductSize</asp:TableCell>
                <asp:TableCell ID="cell_ProductPrice" CssClass="TableCell">ProductPrice</asp:TableCell>
                <asp:TableCell ID="cell_Number" CssClass="TableCell">Number</asp:TableCell>
                <asp:TableCell ID="cell_Unit" CssClass="TableCell">Unit</asp:TableCell>
                <asp:TableCell ID="cell_ID"  CssClass="TableCell" Style="display: none"></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell CssClass="TableCell">
                    <asp:ImageButton ID="img_1_delete" runat="server" ImageUrl="Image/delete.png"   OnClientClick="return RemoveTableRow(1)"/>
                </asp:TableCell>
                <asp:TableCell CssClass="TableCell">1</asp:TableCell>
                <asp:TableCell CssClass="TableCell">
                    <asp:TextBox ID="txt_1_ProductNo" runat="server" CssClass="TableCellTextBox"></asp:TextBox>
                </asp:TableCell>
                <asp:TableCell CssClass="TableCell">
                    <asp:TextBox ID="txt_1_ProductName" runat="server" CssClass="TableCellTextBox"></asp:TextBox>
                </asp:TableCell>
                <asp:TableCell CssClass="TableCell">
                    <asp:TextBox ID="txt_1_ProductColor" runat="server" CssClass="TableCellTextBox"></asp:TextBox>
                </asp:TableCell>
                <asp:TableCell CssClass="TableCell">
                    <asp:TextBox ID="txt_1_ProductSize" runat="server" CssClass="TableCellTextBox"></asp:TextBox>
                </asp:TableCell>
                <asp:TableCell CssClass="TableCell">
                    <asp:TextBox ID="txt_1_ProductPrice" runat="server" CssClass="TableCellTextBox"></asp:TextBox>
                </asp:TableCell>
                <asp:TableCell CssClass="TableCell">
                    <asp:TextBox ID="txt_1_Number" runat="server" CssClass="TableCellTextBox"></asp:TextBox>
                </asp:TableCell>
                <asp:TableCell CssClass="TableCell">
                    <asp:TextBox ID="txt_1_Unit" runat="server" CssClass="TableCellTextBox"></asp:TextBox>
                </asp:TableCell>
                  <asp:TableCell CssClass="TableCell" Style="display: none">
                    <asp:TextBox ID="txt_1_ProductId" runat="server" CssClass="TableCellTextBox"></asp:TextBox>
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow Style="display: none">
                <asp:TableCell CssClass="TableCell">
                    <asp:ImageButton ID="img_2_delete" runat="server" ImageUrl="Image/delete.png" OnClientClick="return RemoveTableRow(2)" />
                </asp:TableCell>
                <asp:TableCell CssClass="TableCell">2</asp:TableCell>
                <asp:TableCell CssClass="TableCell">
                    <asp:TextBox ID="txt_2_ProductNo" runat="server" CssClass="TableCellTextBox"></asp:TextBox>
                </asp:TableCell>
                <asp:TableCell CssClass="TableCell">
                    <asp:TextBox ID="txt_2_ProductName" runat="server" CssClass="TableCellTextBox"></asp:TextBox>
                </asp:TableCell>
                <asp:TableCell CssClass="TableCell">
                    <asp:TextBox ID="txt_2_ProductColor" runat="server" CssClass="TableCellTextBox"></asp:TextBox>
                </asp:TableCell>
                <asp:TableCell CssClass="TableCell">
                    <asp:TextBox ID="txt_2_ProductSize" runat="server" CssClass="TableCellTextBox"></asp:TextBox>
                </asp:TableCell>
                <asp:TableCell CssClass="TableCell">
                    <asp:TextBox ID="txt_2_ProductPrice" runat="server" CssClass="TableCellTextBox"></asp:TextBox>
                </asp:TableCell>
                <asp:TableCell CssClass="TableCell">
                    <asp:TextBox ID="txt_2_Number" runat="server" CssClass="TableCellTextBox"></asp:TextBox>
                </asp:TableCell>
                <asp:TableCell CssClass="TableCell">
                    <asp:TextBox ID="txt_2_Unit" runat="server" CssClass="TableCellTextBox"></asp:TextBox>
                </asp:TableCell>
                 <asp:TableCell CssClass="TableCell" Style="display: none">
                    <asp:TextBox ID="txt_2_ProductId" runat="server" CssClass="TableCellTextBox"></asp:TextBox>
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow Style="display: none">
                <asp:TableCell CssClass="TableCell">
                    <asp:ImageButton ID="img_3_delete" runat="server" ImageUrl="Image/delete.png" OnClientClick="return RemoveTableRow(3)" />
                </asp:TableCell>
                <asp:TableCell CssClass="TableCell">3</asp:TableCell>
                <asp:TableCell CssClass="TableCell">
                    <asp:TextBox ID="txt_3_ProductNo" runat="server" CssClass="TableCellTextBox"></asp:TextBox>
                </asp:TableCell>
                <asp:TableCell CssClass="TableCell">
                    <asp:TextBox ID="txt_3_ProductName" runat="server" CssClass="TableCellTextBox"></asp:TextBox>
                </asp:TableCell>
                <asp:TableCell CssClass="TableCell">
                    <asp:TextBox ID="txt_3_ProductColor" runat="server" CssClass="TableCellTextBox"></asp:TextBox>
                </asp:TableCell>
                <asp:TableCell CssClass="TableCell">
                    <asp:TextBox ID="txt_3_ProductSize" runat="server" CssClass="TableCellTextBox"></asp:TextBox>
                </asp:TableCell>
                <asp:TableCell CssClass="TableCell">
                    <asp:TextBox ID="txt_3_ProductPrice" runat="server" CssClass="TableCellTextBox"></asp:TextBox>
                </asp:TableCell>
                <asp:TableCell CssClass="TableCell">
                    <asp:TextBox ID="txt_3_Number" runat="server" CssClass="TableCellTextBox"></asp:TextBox>
                </asp:TableCell>
                <asp:TableCell CssClass="TableCell">
                    <asp:TextBox ID="txt_3_Unit" runat="server" CssClass="TableCellTextBox"></asp:TextBox>
                </asp:TableCell>
                 <asp:TableCell CssClass="TableCell" Style="display: none">
                    <asp:TextBox ID="txt_3_ProductId" runat="server" CssClass="TableCellTextBox"></asp:TextBox>
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow Style="display: none">
                <asp:TableCell CssClass="TableCell">
                    <asp:ImageButton ID="img_4_delete" runat="server" ImageUrl="Image/delete.png" OnClientClick="return RemoveTableRow(4)"/>
                </asp:TableCell>
                <asp:TableCell CssClass="TableCell">4</asp:TableCell>
                <asp:TableCell CssClass="TableCell">
                    <asp:TextBox ID="txt_4_ProductNo" runat="server" CssClass="TableCellTextBox"></asp:TextBox>
                </asp:TableCell>
                <asp:TableCell CssClass="TableCell">
                    <asp:TextBox ID="txt_4_ProductName" runat="server" CssClass="TableCellTextBox"></asp:TextBox>
                </asp:TableCell>
                <asp:TableCell CssClass="TableCell">
                    <asp:TextBox ID="txt_4_ProductColor" runat="server" CssClass="TableCellTextBox"></asp:TextBox>
                </asp:TableCell>
                <asp:TableCell CssClass="TableCell">
                    <asp:TextBox ID="txt_4_ProductSize" runat="server" CssClass="TableCellTextBox"></asp:TextBox>
                </asp:TableCell>
                <asp:TableCell CssClass="TableCell">
                    <asp:TextBox ID="txt_4_ProductPrice" runat="server" CssClass="TableCellTextBox"></asp:TextBox>
                </asp:TableCell>
                <asp:TableCell CssClass="TableCell">
                    <asp:TextBox ID="txt_4_Number" runat="server" CssClass="TableCellTextBox"></asp:TextBox>
                </asp:TableCell>
                <asp:TableCell CssClass="TableCell">
                    <asp:TextBox ID="txt_4_Unit" runat="server" CssClass="TableCellTextBox"></asp:TextBox>
                </asp:TableCell>
                 <asp:TableCell CssClass="TableCell" Style="display: none">
                    <asp:TextBox ID="txt_4_ProductId" runat="server" CssClass="TableCellTextBox"></asp:TextBox>
                </asp:TableCell>
            </asp:TableRow>
        </asp:Table>
        <asp:Button ID="btn_Add" runat="server" Text="" OnClientClick="return AddNewRow()" BorderStyle="None"  CssClass="buttonAdd" />
    </div>
    <div style="margin: 50px 0px 0px 20px; width: 100%;">
        <div style="float: left; width: 40%; text-align: right">
            <asp:Button ID="btn_Ok" runat="server" Text="Save" Width="100" Height="30px" OnClick="btn_Ok_Click"  OnClientClick="return CheckInput()"/>
        </div>
        <div style="float: right; width: 40%">
            <asp:Button ID="btn_Cancel" runat="server" Text="Cancel" Width="100" Height="30px"  OnClientClick="return CancelInput()"/>
        </div>
    </div>
</asp:Content>
