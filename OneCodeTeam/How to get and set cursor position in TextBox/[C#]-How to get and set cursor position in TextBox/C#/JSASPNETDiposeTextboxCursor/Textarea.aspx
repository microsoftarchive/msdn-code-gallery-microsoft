<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Textarea.aspx.cs" Inherits="JSASPNETDiposeTextboxCursor.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript">
        //Calculate the position of textarea cursor,it can execute in IE,FireFox and Chrome browsers
        function GetCursorPosition() {
            var getX = document.getElementById("getX");
            var getY = document.getElementById("getY");
            var targetText = document.getElementById("TextBox1");
            //IE  
            if (navigator.userAgent.indexOf("MSIE") > 0) {
                if (targetText != null) {
                    var targetRange = document.selection.createRange();
                    var boundingTop = (targetRange.offsetTop + targetText.scrollTop)
                   - targetText.__boundingTop;
                    var boundingLeft = (targetRange.offsetLeft + targetText.scrollLeft)
                   - targetText.__boundingLeft;
                    targetText.__Line = (boundingTop / targetText.__boundingHeight);
                    targetText.__Column = (boundingLeft / targetText.__boundingWidth);
                    getY.value = parseInt(targetText.__Line + 1);
                    getX.value = parseInt(targetText.__Column);
                }
            }
            //Other browsers
            else {
                var x = targetText.selectionStart;
                var y = targetText.selectionEnd;
                x = targetText.value.substring(0, x);
                y = targetText.value.substring(y);
                getX.value = x;
                getY.value = y;
                targetText.focus();
            }
      }

      //If IE,save the textarea distance from the Top,Left, and the height and width of textarea,
      //we need this data for GetCursorPosition() function.
      //If FF or Chrome,we need move the cursor position to the start of textarea.
      function LoadPostion(targetText) {
          if (navigator.userAgent.indexOf("MSIE") > 0) {
              var targetTextValue = targetText.value;
              targetText.value = "";
              targetText.select();
              var targetRange = document.selection.createRange();
              targetText.__boundingTop = targetRange.boundingTop;
              targetText.__boundingLeft = targetRange.boundingLeft;
              targetText.value = " ";
              targetText.select();
              targetRange = document.selection.createRange();
              targetText.__boundingWidth = targetRange.boundingWidth;
              targetText.__boundingHeight = targetRange.boundingHeight;
              targetText.value = targetTextValue;
          }
          else if (isFirefox = navigator.userAgent.indexOf("Firefox") > 0) {
              var targetText = document.getElementById("TextBox1");
              var getY = document.getElementById("getY");
              getY.value = targetText.value;
          }
          else {
              var targetText = document.getElementById("TextBox1");
              var getY = document.getElementById("getY");
              getY.value = targetText.value;
          }
      }

      //Button click event,it will be move cursor to correct position in textarea
      function JudgeInputAndSetPosition() {
          var setX = document.getElementById("setX");
          var targetText = document.getElementById("TextBox1");
          //IE 
          if (navigator.userAgent.indexOf("MSIE") > 0) {
              if (BaseNotInt(setX.value) && setX.value >= 0) {
                  var targetText = document.getElementById("TextBox1");
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
                          var getY = document.getElementById("getY");
                          targetText.focus();
                          targetText.setSelectionRange(step, step);
                          var x = targetText.value.substring(0, targetText.selectionStart);
                          var y = targetText.value.substring(targetText.selectionEnd);
                          getX.value = x;
                          getY.value = y;
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
          if(/^[-]?\d+$/.test(characters)) {
              return true;
          }
          return false;
      }

      //Remove the whitespace of this value function
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
            width: 423px;
        }
        .style2
        {
            width: 137px;
        }
    </style>
</head>
<body onload="LoadPostion(document.forms[0].TextBox1)">
    <form id="form1" runat="server">
    <div>
            <table style="width:100%;">
            <tr>
                <td class="style2">
                    <asp:Label ID="Label1" runat="server" Text="target text :"></asp:Label>
                    </td>
                <td class="style1">
                    <textarea id="TextBox1" name="TextBox1" cols="50" rows="10" 
                        onmouseup="GetCursorPosition()" onmousedown="GetCursorPosition()" 
                    onkeyup="GetCursorPosition()" onkeydown="GetCursorPosition()" 
                        onfocus="GetCursorPosition()">   China is one of the countries with the longest histories in the world. The people of all nationalities in China have jointly created a splendid culture and have a glorious revolutionary tradition.
                    </textarea></td>
                <td>
                    choose the position where you want by mouse or keyboard.
                </td>
            </tr>
            <tr>
                <td class="style2">
                    <asp:Label ID="Label2" runat="server" Text="get cursor position :"></asp:Label>
                    </td>
                <td class="style1">

                    <asp:Label ID="Label4" runat="server" Text="X :"></asp:Label>
                    &nbsp;
                    <input type="text" id="getX" />
                    <asp:Label ID="Label5" runat="server" Text="Y :"></asp:Label>
                    &nbsp;
                    <input type="text" id="getY" /></td>
                <td></td>
            </tr>
            <tr>
                <td class="style2">

                    <asp:Label ID="Label3" runat="server" Text="set cursor position :"></asp:Label>
                    </td>
                <td class="style1">
                    <asp:Label ID="Label6" runat="server" Text="number :"></asp:Label>
                    &nbsp;
                    <input type="text" id="setX" />
                    &nbsp;
                    <input type="button" value="set cursor position" onclick="JudgeInputAndSetPosition()" /></td>
                <td></td>
            </tr>
        </table>
        <br />
        </div>
    </form>
</body>
</html>
