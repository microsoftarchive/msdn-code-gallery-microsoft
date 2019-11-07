
@Code
    Layout = Nothing
End Code

<!DOCTYPE html>

<html>
<head>
    <meta name="viewport" content="width=device-width" />
    <title>Index</title>
    <script src="~/Scripts/jquery-2.1.1.min.js"></script>
    <script src="~/Scripts/jquery.cookie.js"></script>
</head>
<body>
    <div>
        <label>Your Client ID:</label><label id="lbClientId"></label>
        <br />
        <label>The latest time you open this page:</label><label id="lbActiveTime"></label>
        <br />
        <label>The latest time when the server detects your page is still open:</label><label id="lbRefreshTime"></label>
    </div>
    <script type="text/javascript">
        $(document).ready(function () {
            window.addEventListener('beforeunload', recordeCloseTime);
            recordeActiveTime();
            setTimeout(getRefreshTime, 5000)
            setInterval(getRefreshTime, 20000);
        });

        function generateGuid() {
            var result, i, j;
            result = '';
            for (j = 0; j < 32; j++) {
                if (j == 8 || j == 12 || j == 16 || j == 20)
                    result = result + '-';
                i = Math.floor(Math.random() * 16).toString(16).toUpperCase();
                result = result + i;
            }
            return result;
        }

        function getRefreshTime()
        {
            var targetUrl = "@Url.Action("GetRefreshTime", "Default")" + "?clientId=" + getClientID();
            $.ajax({
                type: "GET",
                url: targetUrl,
                cache:false,
                success: function (data) {
                    if (data != "") {
                        var clientInfo = JSON.parse(data);
                        $("#lbClientId").text(clientInfo.ClientID);
                        $("#lbActiveTime").text(new Date(clientInfo.ActiveTime));
                        $("#lbRefreshTime").text(new Date(clientInfo.RefreshTime));
                    }
                }
            });
        }

        function recordeActiveTime()
        {
            var targetUrl = "@Url.Action("RecordActiveTime", "Default")" + "?clientId=" + getClientID();
            $.ajax({
                type: "HEAD",
                url: targetUrl,
            });
        }

        function recordeCloseTime()
        {
            var targetUrl = "@Url.Action("RecordCloseTime", "Default")" + "?clientId=" + getClientID();
            $.ajax({
                type: "HEAD",
                url: targetUrl,
            });
        }

        function getClientID()
        {
            var clientId;
            if ($.cookie("ClientID")) {
                clientId = $.cookie("ClientID");
            }
            else {
                $.cookie("ClientID", generateGuid());
                clientId = $.cookie("ClientID");
            }
            return clientId;
        }
    </script>
</body>
</html>
