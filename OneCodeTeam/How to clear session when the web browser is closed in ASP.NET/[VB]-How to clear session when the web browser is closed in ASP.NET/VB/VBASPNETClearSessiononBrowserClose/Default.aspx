<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Default.aspx.vb" Inherits="VBASPNETClearSessiononBrowserClose._Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>ClearSession</title>
    <script src="Scripts/jquery-2.1.0.min.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            window.addEventListener('beforeunload', recordeCloseTime);
        });

        function recordeCloseTime() {

            $.ajax({
                type: "POST",
                url: "ServiceToClearSession.asmx/RecordCloseTime",
            });
        }

    </script>
</head>    
<body>
    <form id="DefaultForm" runat="server">
    <div>
    </div>
    </form>
</body>
</html>
