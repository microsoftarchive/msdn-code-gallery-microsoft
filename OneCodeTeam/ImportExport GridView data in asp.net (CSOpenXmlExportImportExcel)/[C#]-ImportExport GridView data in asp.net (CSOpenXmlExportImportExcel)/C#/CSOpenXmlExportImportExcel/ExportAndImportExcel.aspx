<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ExportAndImportExcel.aspx.cs" Inherits="CSOpenXmlExportImportExcel.ExportAndImportExcel" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <asp:GridView ID="gridViewTest" runat="server" 
            Height="145px" Width="369px">
        </asp:GridView>
    </div>
    <br/>
    <div>
        <asp:Button ID="btnExport" runat="server" Text="Export" 
            onclick="btnExport_Click" />&nbsp;&nbsp;
        <asp:FileUpload ID="FileUpload1"
            runat="server" />&nbsp;<asp:Button ID="btnImport" runat="server" Text="Import" 
            onclick="btnImport_Click" />
        </div>
    </form>
</body>
</html>
