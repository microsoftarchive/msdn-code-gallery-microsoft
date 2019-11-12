
@Code
    Layout = Nothing
End Code

<!DOCTYPE html>

<html>
<head>
    <meta name="viewport" content="width=device-width" />
    <title>Index</title>
    <script src="~/Scripts/jquery-2.1.1.min.js"></script>
    <script type="text/javascript">
    $(document).ready(function () {
        $("#SectionNames").append($('<option>', {
            value: "connectionStrings",
            text: "Connection Strings"
            })
        );

        $("#SectionNames").append($('<option>', {
            value: "appSettings",
            text: "Application Settings"
        })
        );

        $("#SectionNames").append($('<option>', {
            value: "system.web/machineKey",
            text: "Machine Key"
        })
        );

        $("#SectionNames").append($('<option>', {
            value: "system.web/sessionState",
            text: "Session State"
        })
        );



    });
    var EncryptConfig = function () {
        var url = "/Home/EncryptConfig";
        var sectionName = $("#SectionNames option:selected").text();
        $.ajax({
            url: url,
            type: "POST",
            data: "sectionName=" + $("#SectionNames option:selected").val(),
            success:function()
            {
                $("#lbresult").text("Encrypt the configuration file successfully. Please check it.");
            },
            error: function () {
                $("#lbresult").text("Failed to encrypt the configuration file..");
            }
        });
    }


    var DecryptConfig = function () {
        var url = "/Home/DecryptConfig";
        var sectionName = $("#SectionNames option:selected").text();
        $.ajax({
            url: url,
            type: "POST",
            data: "sectionName=" + $("#SectionNames option:selected").val(),
            success: function () {
                $("#lbresult").text("Decrypt the configuration file successfully. Please check it.");
            },
            error: function () {
                $("#lbresult").text("Failed to decrypt the configuration file..");
            }
        });
    }
    </script>

</head>
<body>
    <div>
        <p>Choose a section:</p>
        <select id="SectionNames"></select>
    </div>
    @*<br /><br /><br /><br />*@
    <div>
        <button id="btnEncrypt" onclick="EncryptConfig()">Encrypt it</button>
        <button id="btnDecrypt" onclick="DecryptConfig()">Decrypt it</button>
    </div>
    <p id="lbresult"></p>
</body>
</html>
