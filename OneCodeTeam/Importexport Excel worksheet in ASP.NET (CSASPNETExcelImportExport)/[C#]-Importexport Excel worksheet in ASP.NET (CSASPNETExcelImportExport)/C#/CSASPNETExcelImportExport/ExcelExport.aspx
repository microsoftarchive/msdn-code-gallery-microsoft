<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ExcelExport.aspx.cs" Inherits="CSASPNETExcelImportExport.ExcelExport" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        Please choose the Excel version:<br />
        <asp:RadioButtonList ID="rblExtension" runat="server">
            <asp:ListItem Selected="True" Value="2003">Excel 97-2003</asp:ListItem>
            <asp:ListItem Value="2007">Excel 2007</asp:ListItem>
        </asp:RadioButtonList>
        <br />
        Generate the download link when finished:
        <asp:RadioButtonList 
            ID="rblDownload" runat="server">
            <asp:ListItem Selected="True" Value="Yes">Yes</asp:ListItem>
            <asp:ListItem Value="No">No</asp:ListItem>
        </asp:RadioButtonList>
        <br />
        <asp:Button ID="btnExport" runat="server" Text="Export" 
            onclick="btnExport_Click" />
        <br />
        <br />
        <asp:HyperLink ID="hlDownload" runat="server"></asp:HyperLink>        
    </div>
    </form>
</body>
</html>
