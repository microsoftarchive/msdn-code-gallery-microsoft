<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="CSASPNETPagingGridViewMaintainCheckBox.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:GridView ID="gvData" runat="server" BackColor="White"
                BorderColor="#CC9966" BorderStyle="None" BorderWidth="1px" CellPadding="4"
                AllowPaging="True" OnPageIndexChanged="gvData_PageIndexChanged" OnPageIndexChanging="gvData_PageIndexChanging" OnRowDataBound="gvData_RowDataBound">
                <Columns>
                    <asp:TemplateField HeaderText="Choice">
                        <ItemTemplate>
                            <asp:CheckBox ID="chbChoice" runat="server" Checked="false" AutoPostBack="true" OnCheckedChanged="chbChoice_CheckedChanged" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </form>
</body>
</html>
