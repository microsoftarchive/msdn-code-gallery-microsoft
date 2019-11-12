//Global Variables used in different functions.
var widthSelected=null;
var senderId;
var hostUrl = null;

// Extracts the width, selected by the user from the drop down menu.
function selectWidth() {

    widthSelected = document.getElementById("widthDropDownMenu").value;
    if (widthSelected != "null")
    {
        // display the selected width on the app part content area
        document.getElementById("UserSelectedWidthOption").innerText = widthSelected;
    }
}

//Main function to change the size dynamically.
function ResizeAppPart() {
    if (window.parent == null)
        return;

    selectWidth();

    // Extracts the host url and sender Id  values from the query string.
    var params = document.URL.split("?")[1].split("&");
    for (var i = 0; i < params.length; i = i + 1) {
        var param = params[i].split("=");
        if (hostUrl == null)
        {
            hostUrl = decodeURIComponent(param[1]);
        }

        if (i == (params.length - 1))
            senderId = decodeURIComponent(param[1]);
    }
    
    var height = 150; // Keeping the height of the app part constant.

    //use postmessage to resize the app part.
    var message = "<Message senderId=" + senderId + " >"
            + "resize(" + widthSelected + "," + height + ")</Message>";
    window.parent.postMessage(message, hostUrl);
}