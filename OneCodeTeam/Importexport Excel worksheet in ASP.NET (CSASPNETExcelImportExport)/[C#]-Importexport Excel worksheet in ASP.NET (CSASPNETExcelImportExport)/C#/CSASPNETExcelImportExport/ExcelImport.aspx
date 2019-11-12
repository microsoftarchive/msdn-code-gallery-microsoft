<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ExcelImport.aspx.cs" Inherits="CSASPNETExcelImportExport.ExcelImport" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        Please select a Excel spreadsheet to import:<br />    
        <asp:FileUpload ID="fupExcel" runat="server" />
        <br />
        <br />
        Archive the Excel file on server after importing:
        <asp:RadioButtonList ID="rblArchive" runat="server">
            <asp:ListItem Value="Yes">Yes</asp:ListItem>
            <asp:ListItem Selected="True" Value="No">No</asp:ListItem>
        </asp:RadioButtonList>    
        <br />
        <asp:Button ID="btnImport" runat="server" 
            Text="Import" onclick="btnImport_Click" />    
        <br />
        <br />    
        <asp:Label ID="lblMessages" runat="server" Text=""></asp:Label>
    </div>
    </form>
</body>
</html>
