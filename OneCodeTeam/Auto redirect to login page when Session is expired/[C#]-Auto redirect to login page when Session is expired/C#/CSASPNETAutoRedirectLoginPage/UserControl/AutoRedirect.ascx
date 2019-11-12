<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AutoRedirect.ascx.cs" Inherits="CSASPNETAutoRedirectLoginPage.UserControl.AutoRedirect" %>
<script type="text/javascript">
    var timeRefresh;
    var timeInterval;
    var currentTime;
    var expressTime;

    expressTime = "<%=ExpressDate %>";
    currentTime = "<%=LoginDate %>";
    setCookie("express", expressTime);
    timeRefresh = setInterval("Refresh()", 1000);

    // Refresh this page to check session is expire or timeout.
    function Refresh() {
        var current = getCookie("express");
        var date = current.split(" ")[0];
        var time = current.split(" ")[1];
        var scriptDate = new Date();
        var year = scriptDate.getFullYear();
        var month = scriptDate.getMonth() + 1;
        var day = scriptDate.getDate();
        var hour = scriptDate.getHours();
        var min = scriptDate.getMinutes();
        var second = scriptDate.getSeconds();
        if (Date.UTC(year, month, day, hour, min, second) >=
           Date.UTC(date.split("-")[0], date.split("-")[1], date.split("-")[2],
           time.split(":")[0], time.split(":")[1], time.split(":")[2])) {
            clearInterval(timeRefresh);
            Redirect();
        }
    }

    function Redirect() {
        window.location.replace("LoginPage.aspx");
    }

    // Retrieve cookie by name.
    function getCookie(name) {
        var arg = name + "=";
        var aLen = arg.length;
        var cLen = document.cookie.length;
        var i = 0;
        while (i < cLen) {
            var j = i + aLen;
            if (document.cookie.substring(i, j) == arg) {
                return getCookieVal(j);
            }
            i = document.cookie.indexOf(" ", i) + 1;
            if (i == 0) break;
        }
        return;
    }

    function getCookieVal(offSet) {
        var endStr = document.cookie.indexOf(";", offSet);
        if (endStr == -1) {
            endStr = document.cookie.length;
        }
        return unescape(document.cookie.substring(offSet, endStr));
    }

    // Assign values to cookie variable.
    function setCookie(name, value) {
        document.cookie = name + "=" + escape(value);
    }
</script>