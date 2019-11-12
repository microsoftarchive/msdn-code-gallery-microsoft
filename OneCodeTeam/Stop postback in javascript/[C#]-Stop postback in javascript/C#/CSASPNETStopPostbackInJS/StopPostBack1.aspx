<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StopPostBack1.aspx.cs" Inherits="CSASPNETStopPostbackInJS.StopPostBack" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript" language="javascript">
        //Use ClientClick event to check stop postbacks event or not. 
        function onClientClickEvent() {
            var text = document.getElementById('textDisplay');
            var checkbox = document.getElementById('chkStopPostback');
            text.value = "This is a client click";
            if (checkbox.checked == true) {
                return false;
            }
            else {
                return true;
            }
        }
    </script>
    <style type="text/css">
        .style1
        {
            width: 105px;
        }
        #textDisplay
        {
            width: 271px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <table style="width:100%;">
            <tr>
                <td class="style1">
                    <asp:Label ID="Label1" runat="server" Text="Stop postbacks:"></asp:Label>
                </td>
                <td>
                    <input id="chkStopPostback" type="checkbox" />Stop postbacks?</td>
            </tr>
            <tr>
                <td class="style1">
                    <asp:Label ID="Label2" runat="server" Text="Cause postbacks:"></asp:Label>
                </td>
                <td>
                    <asp:Button ID="btnCausePostback" runat="server" Text="Click me to get info" 
                        OnClientClick="return onClientClickEvent()" onclick="btnCausePostback_Click" />
                </td>
            </tr>
            <tr>
                <td class="style1">
                    <asp:Label ID="Label3" runat="server" Text="Postbacks result:"></asp:Label>
                </td>
                <td>
                    <input id="textDisplay" readonly="readonly" type="text" runat="server" /></td>
            </tr>
        </table>  
    </div>
    </form>
</body>
</html>
