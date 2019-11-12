<%@ Page Title="Home Page" Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs"
    Inherits="CSASPNETInfiniteLoading._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Infinite loading</title>
    <link rel="stylesheet" href="Styles/Site.css" type="text/css" />
    <script type="text/javascript" src="Scripts/jquery-1.4.1.min.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {

            function lastPostFunc() {
                $('#divPostsLoader').html('<img src="images/bigLoader.gif">');

                //send a query to server side to present new content
                $.ajax({
                    type: "POST",
                    url: "Default.aspx/Foo",
                    data: "{}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data) {

                        if (data != "") {
                            $('.divLoadData:last').after(data.d);
                        }
                        $('#divPostsLoader').empty();
                    }

                })
            };

            //When scroll down, the scroller is at the bottom with the function below and fire the lastPostFunc function
            $(window).scroll(function () {
                if ($(window).scrollTop() == $(document).height() - $(window).height()) {
                    lastPostFunc();
                }
            });

        });
    </script>
</head>
<body>
    <form id="Form1" runat="server">
    <div style="height: 900px">
        <h1>
            Simply scroll down to see new content loading...</h1>
    </div>
    <div class="divLoadData">
    </div>
    <div id="divPostsLoader">
    </div>
    </form>
</body>
</html>
