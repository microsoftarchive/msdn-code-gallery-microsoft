<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="CSASPNETShowSpinnerImage.Default" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %> 
<%@ Register src="UserControl/PopupProgress.ascx" tagname="PopupProgress" tagprefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
     <title></title>
       <style type="text/css"> 
        .modalBackground 
        { 
            background-color: Gray; 
            filter:alpha(opacity=70);
            opacity: 0.5; 
        } 
    </style> 
 </head>
 <body id="body1" runat="server" >
     <form id="form1" runat="server">
     <div>           
        <asp:ToolkitScriptManager ID="ToolkitScriptManagerPopup" runat="server" />      
        <asp:UpdatePanel ID="updatePanel" UpdateMode="Conditional" runat="server"> 
            <ContentTemplate>  
                <%=DateTime.Now.ToString() %> 
                <br /> 
                 <asp:Button ID="btnRefresh" runat="server" Text="Refresh Panel" OnClick="btnRefresh_Click" OnClientClick="document.getElementById('PopupProgressUserControl_btnLink').click();" /> 
                <br /> 
               <asp:GridView ID="gvwXMLData" runat="server">
            </asp:GridView>
            </ContentTemplate>           
        </asp:UpdatePanel> 
       
        
        
        <asp:UpdateProgress ID="updateProgress" runat="server" AssociatedUpdatePanelID="updatePanel"> 
            <ProgressTemplate> 
                <uc1:PopupProgress ID="PopupProgressUserControl" runat="server" />
            </ProgressTemplate>
        </asp:UpdateProgress>           
     </div>   
     </form>
 </body>
</html>
