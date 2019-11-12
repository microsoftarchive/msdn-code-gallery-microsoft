<!DOCTYPE HTML>
<html>
    <body>
        <!-- Placeholder for edit mode message -->
        <h1 id="editmodehdr" style="display:none">
            The app part is in edit mode.
        </h1>

        <div id="content">
            <!-- Placeholders for properties -->
            String property: <span id="strProp"></span><br />
            Integer property: <span id="intProp"></span><br />
            Boolean property: <span id="boolProp"></span><br />
            Enumeration property: <span id="enumProp"></span><br />
        </div>

    <!-- Main JavaScript function, controls the rendering
         logic based on the custom property values -->
    <script lang="javascript">
        "use strict";

        var params = document.URL.split("?")[1].split("&");
        var strProp;
        var intProp;
        var boolProp;
        var enumProp;
        var editmode;

        // Extracts the property values from the query string
        for (var i = 0; i < params.length; i = i + 1) {
            var param = params[i].split("=");
            if (param[0] == "strProp")
                strProp = decodeURIComponent(param[1]);
            else if (param[0] == "intProp")
                intProp = parseInt(param[1]);
            else if (param[0] == "boolProp")
                boolProp = (param[1] == "true");
            else if (param[0] == "enumProp")
                enumProp = decodeURIComponent(param[1]);
            else if (param[0] == "editmode")
                editmode = decodeURIComponent(param[1]);
        }

        // If the app part is in edit mode.
        if (editmode == 1) {
            // Display an edit mode banner.
            document.getElementById("editmodehdr").style.display = "inline";
            document.getElementById("content").style.display = "none";
        }
        else {
            // Uses the placeholders to render the property values
            document.getElementById("strProp").innerText = strProp;
            document.getElementById("intProp").innerText = intProp;
            document.getElementById("boolProp").innerText = boolProp;
            document.getElementById("enumProp").innerText = enumProp;

            // Hide the edit mode banner and display the content.
            document.getElementById("editmodehdr").style.display = "none";
            document.getElementById("content").style.display = "inline";
        }
    </script>
	</body>
</html>