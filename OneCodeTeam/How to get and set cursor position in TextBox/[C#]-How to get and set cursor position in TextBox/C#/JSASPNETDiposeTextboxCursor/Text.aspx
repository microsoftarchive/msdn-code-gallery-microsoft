<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Text.aspx.cs" Inherits="JSASPNETDiposeTextboxCursor.Text" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript">
        //Calculate the position in text,it can execute in IE,FireFox and Chrome browsers
        function GetCursorPosition(targetText) {
            var getX = document.getElementById("getX");
            //IE 
            if (navigator.userAgent.indexOf("MSIE") > 0) {
                if (targetText != null) {
                    var targetRange = document.selection.createRange();
                    targetRange.moveStart("character", -event.srcElement.value.length);
                    getX.value = targetRange.text.length;
                }
            }
            //Other browsers
            else {
                getX.value = targetText.selectionStart;
            }
        }

        //Button click event,it will be move cursor to correct position in textarea
        function JudgeInputAndSetPosition() {
            var setX = document.getElementById("setX");
            var targetText = document.getElementById("TextBox1");
            //IE
            if (navigator.userAgent.indexOf("MSIE") > 0) {
                if (BaseNotInt(setX.value) && setX.value >= 0) {
                    if (targetText != null) {
                        var targetRange = targetText.createTextRange();
                        var step = parseInt(setX.value);
                        if (targetText.value.length >= step) {
                            targetRange.move("character", step);
                            targetRange.select();
                        }
                        else {
                            alert("sorry,your content is not so long enough,character is only " + targetText.value.length + ".");
                        }
                    }
                }
                else {
                    alert("please enter a positive integer ");
                }
            }
            //Other browsers
            else {
                if (BaseNotInt(setX.value) && setX.value >= 0) {
                    if (targetText != null) {
                        var step = parseInt(setX.value);
                        if (targetText.value.length >= step) {
                            var getX = document.getElementById("getX");
                            targetText.focus();
                            targetText.setSelectionRange(step, step);
                            getX.value = targetText.selectionStart;
                        }
                        else {
                            alert("sorry,your content is not so long enough,character is only " + targetText.value.length + ".");
                        }
                    }
                }
                else {
                    alert("please enter a positive integer ");
                }
            }
        }

        //Check value whether an integer function
        function BaseNotInt(character) {
            var characters = character;
            characters = Trim(characters);
            if (/^[-]?\d+$/.test(characters)) {
                return true;
            }
            return false;
        }

        //Remove the whitespace of a value function
        function Trim(s) {
            var reg = new RegExp("^[ \\t]*([^ \\t]+[.]*)?[ \\t]*$");
            var s1 = new String(s);
            reg.multiline = true;
            return s1.replace(reg, "$1");
        }
    </script>
    <style type="text/css">
        .style1
        {
            width: 310px;
        }
        .style2
        {
            width: 137px;
        }
    </style>
</head>
<body onload="">
    <form id="form1" runat="server">
    <div>
            <table style="width:100%;">
            <tr>
                <td class="style2">
                    <asp:Label ID="Label1" runat="server" Text="target text :"></asp:Label>
                    </td>
                <td class="style1">

                    <input type="Text" id="TextBox1" value="aaaa     " name="TextBox1" cols="50" rows="10"
                        onmouseup="GetCursorPosition(this)" onmousedown="GetCursorPosition(this)" 
                    onkeyup="GetCursorPosition(this)" onkeydown="GetCursorPosition(this)" 
                        onfocus="GetCursorPosition(this)" /></td>
                <td>
                    choose the position where you want by mouse or keyboard.&nbsp;
                </td>
            </tr>
            <tr>
                <td class="style2">
                    <asp:Label ID="Label2" runat="server" Text="get cursor position :"></asp:Label>

                    </td>
                <td class="style1">
                    <input type="text" ID="getX" value="0"/></td>
                <td>
                    &nbsp;</td>
            </tr>
            <tr>
                <td class="style2">

                    <asp:Label ID="Label3" runat="server" Text="set cursor position :"></asp:Label>
                    </td>
                <td class="style1">
                    <input type="text" ID="setX" />
                    &nbsp;
                    <input type="button" value="set cursor position" onclick="JudgeInputAndSetPosition()" ></input></td>
                <td>
                </td>
            </tr>
        </table>
        <br />
        </div>
    </form>
</body>
</html>
