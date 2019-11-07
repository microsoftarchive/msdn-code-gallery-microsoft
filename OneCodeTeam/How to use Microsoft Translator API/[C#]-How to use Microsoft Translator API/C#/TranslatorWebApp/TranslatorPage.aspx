<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TranslatorPage.aspx.cs" Inherits="TranslatorWebApp.TranslatorPage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <asp:Label runat="server" Text="Enter Text"></asp:Label>
        <br />
        <asp:TextBox runat="server" ID="txtUser" Height="79px" TextMode="MultiLine" Width="452px" OnTextChanged="txtUser_TextChanged"></asp:TextBox>
        <br />
        <asp:Button ID="Button1" runat="server" Text="Detect" OnClick="Button1_Click" />
        <br />
        Detected Language :<asp:Label runat="server" ID="lblDetectedText"></asp:Label>
        <br />
        <br />
        Select Language :
        <asp:DropDownList ID="drpAllLang" runat="server" Height="33px" Width="155px">
        </asp:DropDownList>
        <br />
        <br />
        <asp:Button ID="Button2" runat="server" OnClick="Button2_Click" Text="Translate" />
        <br />
        <br />
        Tranlated Text<br />
        <asp:TextBox runat="server" ID="txtTranslated" Height="79px" TextMode="MultiLine" Width="452px"></asp:TextBox>
        <br />
    </div>
    </form>
</body>
</html>
