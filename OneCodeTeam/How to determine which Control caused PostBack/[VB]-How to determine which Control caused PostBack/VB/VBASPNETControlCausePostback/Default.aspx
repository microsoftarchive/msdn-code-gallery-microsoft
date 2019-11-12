<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Default.aspx.vb" Inherits="VBASPNETControlCausePostback._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title></title>
        <script type="text/javascript">
            function load() {
                var checkbox = document.getElementById("chkC");
                var hiddenCheckBox = document.getElementById("checkbox");
                if (hiddenCheckBox.value == "true") {
                    checkbox.checked = true;
                }
                else if (hiddenCheckBox.value == "false") {
                    checkbox.checked = false;
                }
                else {
                    checkbox.checked = false;
                }
                var dropdownlist = document.getElementById("ddlD");
                var hiddenDropdownlist = document.getElementById("dropdownlist");
                var index = parseInt(hiddenDropdownlist.value);
                dropdownlist.selectedIndex = index;
            }
        </script>
</head>
<body onload="load();">
    <form id="form1" runat="server">
    <div style="border-color:Blue; border-width:1px">
        <input type="button" name="btnA" value="Button A" onclick="PostBack('Button A', 'The Button A fires the post back event.')" />
        <br />
        <br />
        <input type="button" name="btnB" value="Button B" onclick="PostBack('Button B', 'The Button B fires the post back event.')" />
        <br />
        <br />
        <input type="checkbox" id="chkC" value="CheckBox C" onclick="PostBack('CheckBox C', 'The CheckBox C  fires the post back event.')" />CheckBox C
        <br />
        <br />
        <select ID="ddlD">
            <option>Please select----</option>
            <option onclick="PostBack('Select D', 'The Select D  fires the post back event.')">DropDownlist D</option>
        </select>
        <br />
        <br />
        <asp:Label ID="lbMessage" runat="server" ></asp:Label>
        <br />
        <input type="hidden" name="__EVENTTARGET"  id="__EVENTTARGET" value="" />
        <input type="hidden" name="__EVENTARGUMENT" id="__EVENTARGUMENT" value="" />
        <input type="hidden" name="checkbox" id="checkbox" runat="server" value="" />
        <input type="hidden" name="dropdownlist" id="dropdownlist" runat="server" value="" />
    </div>
    </form>
</body>
    <script type="text/javascript">
        var theForm = document.forms['form1'];
        if (!theForm) {
            theForm = document.form1;

            function PostBack(controlName, postbackData) {
                var name = controlName;
                var data = postbackData;
                var checkbox = document.getElementById("chkC");
                var hiddenCheckBox = document.getElementById("checkbox");
                var dropdownlist = document.getElementById("ddlD");
                var hiddenDropdownlist = document.getElementById("dropdownlist");
                if (checkbox.checked) {
                    hiddenCheckBox.value = "true";
                }
                else {
                    hiddenCheckBox.value = "false";
                }
                hiddenDropdownlist.value = dropdownlist.selectedIndex;
                __doPostBack(name, data);
            }


        }
        function __doPostBack(eventTarget, eventArgument) {
            theForm.__EVENTTARGET.value = eventTarget;
            theForm.__EVENTARGUMENT.value = eventArgument;
            theForm.submit();
        }
    </script>
</html>
