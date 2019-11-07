<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PopupProgress.ascx.cs" Inherits="CSASPNETShowSpinnerImage.PopupProgress" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %> 
<script language="javascript" type="text/javascript">
    <% =LoadImage() %>
    
    // The JavaScript function can shows loaded image in Image control.
    var imgStep = 0;
    function slide()
    {
        var img = document.getElementById("PopupProgressUserControl_imgProgress");      
        if (document.all)
        {
            img.filters.blendTrans.apply();
        } 
           
        img.title=imgMessage[imgStep];  
        img.src=imgUrl[imgStep]; 
                      
        if (document.all)
        {
            img.filters.blendTrans.play();
        }
        
        imgStep = (imgStep < (imgUrl.length-1)) ? (imgStep + 1) : 0;
        (new Image()).src = imgUrl[imgStep];
    }
    setInterval("slide()",1000);

</script>
<asp:Panel ID="pnlProgress" runat="server" CssClass="modalpopup"> 
    <div class="popupcontainerLoading"> 
        <div class="popupbody"> 
            <table width="100%"> 
                <tr> 
                    <td align="center"> 
                        <asp:Image ID="imgProgress" runat="server" style="filter: blendTrans(duration=0.618)"  ImageUrl="~/Image/0.jpg"/>                       
                    </td> 
                </tr> 
                <tr> 
                    <td>
                    <asp:Button ID="btnCancel" runat="server" Text="Cancel" /></td>
                </tr>
            </table> 
        </div> 
    </div> 
</asp:Panel> 
<asp:LinkButton ID="btnLink" runat="server" Text=""></asp:LinkButton> 
<asp:ModalPopupExtender ID="mpeProgress" runat="server" TargetControlID="btnLink"
    X="500" Y="0" PopupControlID="pnlProgress" BackgroundCssClass="modalBackground" DropShadow="true"  CancelControlID="btnCancel" > 
</asp:ModalPopupExtender>