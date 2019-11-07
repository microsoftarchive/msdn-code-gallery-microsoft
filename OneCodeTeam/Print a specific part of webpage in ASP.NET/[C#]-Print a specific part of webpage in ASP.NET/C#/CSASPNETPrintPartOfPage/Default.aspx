<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="CSASPNETPrintPartOfPage.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css" media="print">  
      .nonPrintable
      {
        display: none;
      }
     </style>
     <script type="text/javascript">
         function print_page() {
             window.print();
         }
     </script>
</head>
<body>
<form id="Form1" method="post" runat="server">
<center>

<font face="Verdana" size="4">ASP.NET MVC Sample Code</font><br />
<table border="0" width="90%">
<tr><td>
<p /><font face="Verdana" size="1">
The Model-View-Controller (MVC) architectural pattern separates an application into three main components:
 the model, the view, and the controller. The ASP.NET MVC framework provides an alternative to 
 the ASP.NET Web Forms pattern for creating Web applications. The ASP.NET MVC framework is a lightweight, 
 highly testable presentation framework that (as with Web Forms-based applications) is integrated with existing ASP.NET features, 
 such as master pages and membership-based authentication. The MVC framework is defined in the System.Web.Mvc assembly.
</font>
</td></tr>
</table>
<br />

<%=PrintImageBegin %>
<table><tr><td>
    <asp:Image ID="Image1" runat="server" ImageUrl="~/image/microsoft.jpg" /></td></tr></table>
    <%=PrintImageEnd %><%=PrintListBegin %>
<table border="0" width="90%">
<tr>
<td>Title</td>
<td>Name</td>
<td>Date</td>
</tr>
<tr>
<td>sample title1</td>
<td>sample name1</td>
<td>2011-03-05</td>
</tr>
<tr>
<td>sample title2</td>
<td>sample name2</td>
<td>2011-03-06</td>
</tr>
<tr>
<td>sample title3</td>
<td>sample name3</td>
<td>2011-03-07</td>
</tr>
<tr>
<td>sample title4</td>
<td>sample name4</td>
<td>2011-03-08</td>
</tr>
<tr>
<td>sample title5</td>
<td>sample name5</td>
<td>2011-03-09</td>
</tr>
<tr>
<td>sample title6</td>
<td>sample name6</td>
<td>2011-03-10</td>
</tr>
<tr>
<td>sample title7</td>
<td>sample name7</td>
<td>2011-03-11</td>
</tr>
<tr>
<td>sample title8</td>
<td>sample name8</td>
<td>2011-03-12</td>
</tr>
</table>
    <%=PrintListEnd%><%=PrintButtonBegin %>
<table>
<tr>
<td>
<font face="Arial" size="3" color="white">
  <asp:Button ID="btnPrint" runat="server" Text="Print this page" 
        onclick="btnPrint_Click"/></font>
    <asp:CheckBox ID="chkDisplayImage" runat="server" Text="Display Image" />
    <asp:CheckBox ID="chkDisplayList" runat="server" Text="Display List" />
    <asp:CheckBox ID="chkDisplayButton" runat="server" Text="Display Button" />
    <asp:CheckBox ID="chkDisplaySearch" runat="server" Text="Display Search" />
</td>
</tr>
</table>
    <%=PrintButtonEnd %><%=PrintSearchBegin %>
<table>
<tr>
<td valign="top" bgcolor="#bfa57d" width="230">
<font face="Arial" size="3" color="white"><b>MVC Sample Code</b><br /><br />mvc<br /><br />
<font size="1"> Search for code<br /></font>
<asp:TextBox id="tbSearch" runat="server" width="100"/>
<asp:Button id="btnSearch" runat="server" height="22" Text="Search"/>
</font>
</td>
</tr>
</table>
    <%=PrintSearchEnd %>
</center>
</form>
</body>
</html>
