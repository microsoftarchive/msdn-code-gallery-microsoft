<!DOCTYPE html>
<html>
<head>
    <title>How to use RESTful WCF Service with HTTP requests like "Get/Post/Put/Delete".
    </title>
    <meta charset="utf-8" />
    <link href="@Url.Content("~/Static/Css/site.css")" rel="stylesheet" />
    <link href="@Url.Content("~/Static/Css/user.css")" rel="stylesheet" />
    <script src="@Url.Content("~/Static/Js/jquery-1.7.1.min.js")" type="text/javascript"></script>
    <script src="@Url.Content("~/Static/Js/jquery.validate.min.js")" type="text/javascript"></script>
    <script src="@Url.Content("~/Static/Js/jquery.validate.unobtrusive.min.js")" type="text/javascript"></script>
</head>
<body>
    <div id="header">
        <h1>How to use RESTful WCF Service with HTTP requests like "Get/Post/Put/Delete".</h1>
    </div>
    <div id="main">
        @RenderBody()
    </div>
    <div id="footer">
        <p class="pageinfo">
            @* Users count is <span id="datacount">0</span>; Page count is <span id="pagecount">0</span>;
            Current page index is <span id="pageindex">0</span>
            <br />*@

            @code
                Dim errorMsg = TempData("error")
                
                If Not IsNothing(errorMsg) Then
                @<span id="error" class="field-validation-error">
                Error: @Html.Encode(errorMsg) </span> 
                End If
            End Code
        </p>
    </div>

</body>
</html>
