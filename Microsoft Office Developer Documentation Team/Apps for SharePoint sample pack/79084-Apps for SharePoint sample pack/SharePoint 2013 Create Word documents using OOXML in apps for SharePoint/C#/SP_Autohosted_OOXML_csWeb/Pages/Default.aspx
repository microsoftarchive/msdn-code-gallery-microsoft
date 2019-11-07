<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="SP_Autohosted_OOXML_csWeb.Pages.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>OOXML Document Creator Sample</title>
    <!-- Custom style sheet used ot render links as tiles -->
    <link rel="Stylesheet" href="../CSS/point8020metro.css" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
        <!-- UI consists of labels, a dropdown list for displaying SharePoint libraries, a button and a hyperlink -->
        <!-- Code in default.aspx.cs controls this UI and enables the user to add a new file based on OOXML to SharePoint-->
        <asp:Label ID="SiteTitle" CssClass="heading" runat="server" Text="Label"></asp:Label>
        <br />
        <asp:Label ID="DropdownLabel" runat="server" Text="Target Library:"></asp:Label>
        <asp:DropDownList ID="OutputLibrary" runat="server" ToolTip="Select a library to store the new document">
            <asp:ListItem Selected="True">Select Target Library</asp:ListItem>  
        </asp:DropDownList>
        <br/>
        <asp:RequiredFieldValidator 
            ID="RequiredFieldValidatorJobTitle" 
            runat="server" 
            ControlToValidate="OutputLibrary"
            InitialValue="Select Target Library" 
            ForeColor="Red" 
            ErrorMessage="Target Library is required"></asp:RequiredFieldValidator>
        <br/>
        <asp:LinkButton ID="CreateDocumentLink" runat="server" Text="" OnClick="CreateDocumentLink_Click" />
        <br/>
        <asp:HyperLink ID="DocumentLink" runat="server" Target="_blank" CssClass="tile tileRed" Visible="false"></asp:HyperLink>

   </form>
</body>
</html>
