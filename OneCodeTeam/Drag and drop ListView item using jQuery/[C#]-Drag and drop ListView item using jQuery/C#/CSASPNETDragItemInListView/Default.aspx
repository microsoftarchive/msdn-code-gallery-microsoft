<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="CSASPNETDragItemInListView.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title> 
    <link href="JQuery/jquery-ui.css" rel="stylesheet" type="text/css" />
    <script src="JQuery/jquery-1.4.4.min.js" type="text/javascript"></script>
    <script src="JQuery/jquery-ui.min.js" type="text/javascript"></script>
  <style type="text/css">
	#sortable1, #sortable2
	 {
	     list-style-type:none;
	     border-right: #669999 2px solid; 
	     padding-right: 5px; 
	     border-top: #669999 2px solid; 
	     padding-left: 5px; 
	     float: left; 
	     padding-bottom: 0px; 
	     margin: 3px; 
	     border-left: #669999 2px solid; 
	     width: 100px; 
	     padding-top: 5px; 
	     border-bottom: #669999 2px solid      
	}
	#sortable1 li, #sortable2 li
	 { 
	     border-right: #000 1px solid; 
	     padding-right: 2px; 
	     border-top: #000 1px solid; 
	     padding-left: 2px; 
	     font-size: 10px; 
	     margin-bottom: 5px; 
	     padding-bottom: 2px; 
	     border-left: #000 1px solid; 
	     width: 94px; 
	     cursor: pointer; 
	     padding-top: 2px; 
	     border-bottom: #000 1px solid; 
	     font-family: verdana, tahoma, arial; 
	     background-color: #eee
	 }
	</style>
	<script type="text/javascript">
	    $(function () {
	        $("#sortable1, #sortable2").sortable({
	            connectWith: ".connectedSortable"
	        }).disableSelection();
	    });

	    $(document).ready(function () {
	        $("li").dblclick(function () {
	            $(this).closest('li').remove();
	        });
	    });   
	</script>
</head>
<body>
    <form id="form1" runat="server">
       	<asp:Label ID="Label1" runat="server" 
        Text="Please drag items in ListView control to another, you can also sort items by drag item to right positon. Double click one item to drop it from the ListView control."></asp:Label><br/>
    <div>
        <asp:ListView ID="ListView1" runat="server">
        <LayoutTemplate>
        <ul id="sortable1" class="connectedSortable">
        <asp:PlaceHolder runat="server" id="itemPlaceholder"></asp:PlaceHolder>
        </ul>
        </LayoutTemplate>
        <ItemTemplate>    
	    <li class="ui-state-default" ><%# Eval("value") %></li>     
        </ItemTemplate>
        </asp:ListView>
    
        <asp:ListView ID="ListView2" runat="server">
        <LayoutTemplate>
        <ul id="sortable2" class="connectedSortable">
        <asp:PlaceHolder runat="server" id="itemPlaceholder"></asp:PlaceHolder>
        </ul>
        </LayoutTemplate>
        <ItemTemplate>    
	    <li class="ui-state-highlight" ><%# Eval("value2") %></li>     
        </ItemTemplate>
        </asp:ListView>
    
    </div>
    </form>
</body>
</html>
