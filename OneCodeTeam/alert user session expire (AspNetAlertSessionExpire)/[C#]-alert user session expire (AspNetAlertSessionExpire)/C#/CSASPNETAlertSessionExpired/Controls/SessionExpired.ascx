<!--
/****************************** Module Header ******************************\
* Module Name:    SessionExpired.ascx
* Project:        CSASPNETAlertSessionExpired
* Copyright (c) Microsoft Corporation
*
* The project illustrates how to design a simple user control, which is used to 
* alert the user when the session is about to expired. 
* 
* In this file, Set the Timer to request the server by async method.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
*
\*****************************************************************************/
-->
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SessionExpired.ascx.cs" Inherits="CSASPNETAlertSessionExpired.Controls.SessionExpired" %>
<script type="text/javascript" language="javascript">

    function clientReceiveServerData(returnValue) {
             
    }

</script>

<asp:HiddenField ID="hfTimeOut" runat="server" />
<span id="spSessionExpired" style=" color:White; display:none;">
The session is expired.</span>
<table id="tbSessionExpired" style=" display:none;">
<tr>
<td >
<span style="color:White;">
Your session will be expired after  <span id="expiredTime"></span> seconds, would you like to extend the session time?
</span>
</td>
</tr>
<tr>
<td>
<input type="button" id="btnTimeextend" value="Yes"  onclick="SetTimeextend();"/>
<input type="button" id="btnTimeExpired" value="No" onclick="SetTimeExpired();" />
</td>
</tr>
</table>

<span style=" clear:both;"></span>
<script type="text/javascript" language="javascript">

    var i = 30;
    function SetTime() {

        
        $("#expiredTime").html(i.toString());
        if (i > 0) {
            i--;
        }
    }

   

</script>

<script type="text/javascript" language="javascript">

    $(document).ready(function () {
        var timeOut = $("input:hidden[id*='hfTimeOut']").val();
        
        //setInterval("TimeextendOrExpired()", (timeOut-1) * 60 * 1000);
        setInterval("TimeextendOrExpired()",30 * 1000);
    });

</script>