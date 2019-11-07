<%-- The following 4 lines are ASP.NET directives needed when using SharePoint components --%>

<%@ Page Inherits="Microsoft.SharePoint.WebPartPages.WebPartPage, Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" MasterPageFile="~masterurl/default.master" Language="C#" %>

<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<%-- The markup and script in the following Content element will be placed in the <head> of the page --%>
<asp:Content ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
    <script type="text/javascript" src="../Scripts/jquery-1.7.1.min.js"></script>
    <script type="text/javascript" src="/_layouts/15/sp.runtime.debug.js"></script>
    <script type="text/javascript" src="/_layouts/15/sp.debug.js"></script>

    <!-- A few styles have been added to the following file -->
    <link rel="Stylesheet" type="text/css" href="../Content/App.css" />

    <!-- The entire game is implemented in JavaScript in the following file -->
    <script type="text/javascript" src="../Scripts/App.js"></script>
</asp:Content>

<%-- The markup and script in the following Content element will be placed in the <body> of the page --%>
<asp:Content ContentPlaceHolderID="PlaceHolderMain" runat="server">

    
    <h1>SharePoint vs. You!</h1>
    <h2>Tic-Tac-To...oh...oh...oh...oe!</h2>
    <!-- Scores will be written here -->   
    <div id="currentScores" style="float:none;"></div>
    
    <!-- The game is rendered as a 9x9 matrix of square <div> elements --> 
    <!-- The onclick events are handled in App.js -->     
    <div id="gameArea" style="width:300px;height:300px;float:none;">
        <div id="square1" onclick="square_click(1);" class="blank" style=""></div>
        <div id="square2" onclick="square_click(2);" class="blank"></div>
        <div id="square3" onclick="square_click(3);" class="blank"></div>
        <div id="square4" onclick="square_click(4);" class="blank"></div>
        <div id="square5" onclick="square_click(5);" class="blank"></div>
        <div id="square6" onclick="square_click(6);" class="blank"></div>
        <div id="square7" onclick="square_click(7);" class="blank"></div>
        <div id="square8" onclick="square_click(8);" class="blank"></div>
        <div id="square9" onclick="square_click(9);" class="blank"></div>
    </div>
    
    <!-- The button clears the matrix (but retains your current scores for the session) --> 
    <!-- The onclick event is handled in App.js -->     
    <input type="button" ID="resetGame" OnClick="resetNow();" runat="server" value="Restart Game" />
</asp:Content>
