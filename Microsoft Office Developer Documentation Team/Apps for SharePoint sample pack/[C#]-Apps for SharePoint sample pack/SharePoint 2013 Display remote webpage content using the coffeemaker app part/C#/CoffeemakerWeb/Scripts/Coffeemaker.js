"use strict";

var name;
var teaspoons;
var decaf;
var size;
var editmode;
var senderId;
var hostweburl;

// Extracts the property values from the query string
name        = decodeURIComponent(getQueryStringParameter("name"));
teaspoons   = parseInt(getQueryStringParameter("teaspoons"));
decaf       = (getQueryStringParameter("decaf") == "true");
size        = decodeURIComponent(getQueryStringParameter("size"));
editmode    = decodeURIComponent(getQueryStringParameter("editmode"));
senderId    = decodeURIComponent(getQueryStringParameter("SenderId"));
hostweburl  = decodeURIComponent(getQueryStringParameter("SPHostUrl"));

if (editmode == 1) {
    // While the web part is in edit mode
    //  hide the UI elements and display 
    //  an edit mode banner.
    document.getElementById("editmodehdr").style.display = "inline";
    document.getElementById("msghdr").style.display = "none";
    document.getElementById("size").style.display = "none";
    document.getElementById("sweet").style.display = "none";
    document.getElementById("partSize").style.display = "none";
}
else {
    // Uses the placeholders to customize the
    //  rendering process based on the property values
    document.getElementById("name").innerText = name;

    if (decaf)
        document.getElementById("decaf").innerText =
            "decaffeinated ";

    var img;
    if (size == "8 oz")
        img = "Images/short_coffee.jpg";
    else
        img = "Images/tall_coffee.jpg";
    document.getElementById("size").src = img;

    if (teaspoons > 2)
        document.getElementById("sweet").innerHTML =
            "<h1>You are so sweet!</h1>";

    // Hide the edit mode banner and display all
    //  other UI elements.
    document.getElementById("editmodehdr").style.display = "none";
    document.getElementById("msghdr").style.display = "inline";
    document.getElementById("size").style.display = "inline";
    document.getElementById("sweet").style.display = "inline";
    document.getElementById("partSize").style.display = "inline";
    
}

// Resize the app part (width)
function partWidthChanged() {
    var partWidth;
    
    //Get the value of the selected item in the drop-down menu
    partWidth = document.getElementById("partWidth").value;

    window.parent.postMessage("<message senderId=" + senderId + ">resize(" + partWidth + ", 530)</message>", hostweburl);
}