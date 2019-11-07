<%@ Page Inherits="Microsoft.SharePoint.WebPartPages.WebPartPage, Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" MasterPageFile="~masterurl/default.master" Language="C#" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<asp:Content ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
  <script type="text/javascript" src="../Scripts/jquery-1.8.3.min.js"></script>
  <script src="../Scripts/jquery-ui-1.9.2.min.js"></script>
  <link href="../Content/themes/base/jquery-ui.css" rel="stylesheet" />
  <script src="../Scripts/jsrender.js"></script>
  <script type="text/javascript" src="/_layouts/15/sp.runtime.debug.js"></script>
  <script type="text/javascript" src="/_layouts/15/sp.debug.js"></script>
  <link rel="Stylesheet" type="text/css" href="../Content/App.css" />
  <script type="text/javascript" src="../Scripts/App.js"></script>
</asp:Content>

<asp:Content ContentPlaceHolderID="PlaceHolderMain" runat="server">

  <div id="tabs">
    <ul>
      <li><a href="#beatles">The Beatles</a></li>
      <li><a href="#rolling_stones">The Rolling Stones</a></li>
      <li><a href="#who">The Who</a></li>
      <li><a href="#bob_dylan">Bob Dylan</a></li>
      <li><a href="#tom_petty">Tom Petty</a></li>
      <li><a href="#allman_brothers">The Allman Brothers</a></li>
      <li><a href="#cream">Cream</a></li>
    </ul>
    <div id="beatles"></div>
    <div id="rolling_stones"></div>
    <div id="who"></div>
    <div id="bob_dylan"></div>
    <div id="tom_petty"></div>
    <div id="allman_brothers"></div>
    <div id="cream"></div>
  </div>

</asp:Content>
