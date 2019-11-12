<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default2.aspx.cs" Inherits="CSASPNETControlCausePostback.Default2" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Button ID="btnA" runat="server" Text="Button A" OnClientClick="PostBack('Button A', 'The Button A fires the post back event.'); return false;" />
        <br />
        <br />
        <asp:Button ID="btnB" runat="server" Text="Button B" OnClientClick="PostBack('Button B', 'The Button B fires the post back event.'); return false;" />
        <br />
        <br />
        <asp:CheckBox ID="chkC" runat="server" Text="CheckBox C" onclick="PostBack('CheckBox C', 'The CheckBox C  fires the post back event.')" />
        <br />
        <br />
        <asp:DropDownList ID="ddlD" runat="server">
            <asp:ListItem Value="Please select----" Selected="True"></asp:ListItem>
            <asp:ListItem Value="DropDownlist D" onclick="PostBack('Select D', 'The Select D  fires the post back event.')"></asp:ListItem>
        </asp:DropDownList>
        <br />
        <br />
        <asp:Label ID="lbMessage" runat="server" ></asp:Label>
        <br />
        <input type="hidden" name="__EVENTTARGET"  id="__EVENTTARGET" value="" />
        <input type="hidden" name="__EVENTARGUMENT" id="__EVENTARGUMENT" value="" />
    </div>
    </form>
    <script type="text/javascript">
        var theForm = document.forms['form1'];
        if (!theForm) {
            theForm = document.form1;

            function PostBack(controlName, postbackData) {
                var name = controlName;
                var data = postbackData;
                __doPostBack(name, data);
            }


        }
        function __doPostBack(eventTarget, eventArgument) {
            theForm.__EVENTTARGET.value = eventTarget;
            theForm.__EVENTARGUMENT.value = eventArgument;
            theForm.submit();
        }
    </script>
</body>
</html>
