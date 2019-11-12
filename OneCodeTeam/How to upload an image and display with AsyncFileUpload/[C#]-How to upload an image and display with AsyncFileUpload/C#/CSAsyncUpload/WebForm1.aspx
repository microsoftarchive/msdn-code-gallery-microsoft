<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="CSAsyncUpload.WebForm1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
     <script type="text/javascript">
         function uploadComplete(sender, args) {
             var imageName = args.get_fileName();
             $get("displayImage").src = "./upload/" + imageName;
         }
    </script>

</head>

<body>
   <form id="form1" runat="server">
    <ajaxToolkit:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
    </ajaxToolkit:ToolkitScriptManager>
    <ajaxToolkit:AsyncFileUpload ID="AsyncFileUpload1" runat="server" OnClientUploadComplete="uploadComplete" OnUploadedComplete="AsyncFileUpload1_UploadedComplete" />
    <br />
     <asp:Image ID="displayImage" runat="server" />
    </form>
</body>

</html>
