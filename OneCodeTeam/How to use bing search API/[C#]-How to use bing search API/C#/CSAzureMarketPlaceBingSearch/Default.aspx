<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WebRole1.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:TextBox ID="tbQueryString" runat="server" Height="43px" Width="584px"></asp:TextBox>
&nbsp;<br />
        <asp:Button ID="btnWebSearch" runat="server"  
            Text="Web Results" onclick="btnWebSearch_Click" />
        &nbsp;<asp:Button ID="btnImageSearch" runat="server" Text="Image Search" 
            onclick="btnImageSearch_Click" />
        &nbsp;<asp:Button ID="btnVideosSearch" runat="server" 
            Text="Videos Results" onclick="btnVideosSearch_Click" />
        &nbsp;<asp:Button ID="btnNewsSearch" runat="server" Text="News Search" 
            onclick="btnNewsSearch_Click" />
        &nbsp;<asp:Button ID="btnSpellingSuggestionSearch" runat="server" Text="Spelling Suggestion Search" onclick="btnSpellingSuggestionSearch_Click" 
             />
        &nbsp;<asp:Button ID="btnRelatedSearch" runat="server" Text="RelatedSearch" onclick="btnRelatedSearch_Click" 
             />
        &nbsp;<asp:Button ID="btnCompositeSearch" runat="server" Text="Composite Search" 
             Width="148px" onclick="btnCompositeSearch_Click" />
        &nbsp;<br />
        <asp:Panel ID="Panel1" runat="server">
        </asp:Panel>       
    </div>
    </form>
</body>
</html>
