<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Default.aspx.vb" Inherits="VBASPNETStaticCodeByPageMethod._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>Using Page Methods to call server side function without web service </title>
</head>
<body>
    <form id="form1" runat="server">

    <script type="text/javascript">
        function getData() {
            var data = {
                Name: "TestUser",
                BirthDate: new Date(),
                Phone: ["13612345678", "02112345"]
            };
            PageMethods.getData(data, function(returnValue) {
                alert(returnValue.Name + ":" + returnValue.Value);
            });
        }
        
     
    </script>

    <hr />
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true">
        <Scripts>
            <asp:ScriptReference Path="script.js" />
        </Scripts>
    </asp:ScriptManager>
    <table>
        <tr>
            <td colspan="2">
                Type a name:<asp:TextBox ID="tbInput" runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr align="left">
            <td>
                Using Page Methods to call server side function:
            </td>
            <td>
                <input type="button" onclick="getSayHello(<%=tbInput.ClientID %>)" value="SayHello" />
                <input type="button" onclick="getData()" value="getData" />
            </td>
        </tr>
    </table>
    <hr />
    </form>
</body>
</html>
