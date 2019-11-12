<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StopPostBack2.aspx.cs" Inherits="CSASPNETStopPostbackInJS.StopPostBack2" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript" language="javascript">
        //Use ClientClick event to check stop postbacks event or not. 
        function onClientClickEvent() {
            var text = document.getElementById('textDisplay');
            var checkbox = document.getElementById('chkStopPostback');
            text.value = "this is a client click";
            if (checkbox.checked == false) {
                var button = document.getElementById('btnHiddenCausePostback');
                button.click();
            }
        }
    </script>
    <style type="text/css">
        .style1
        {
            width: 108px;
        }
        #textDisplay
        {
            width: 272px;
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
                    <input id="Button1" type="button" value="Click me to get info" onclick="onClientClickEvent()" />
                    <asp:Button ID="btnHiddenCausePostback" runat="server" Text="" onclick="btnCausePostback_Click" 
                         style="display:none" />
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
