//These lines enable Visual Studio to provide Intellisense for the JavaScript libraries
/// <reference name="MicrosoftAjax.js" />
/// <reference path="file://C:/Program Files/Common Files/Microsoft Shared/Web Server Extensions/14/TEMPLATE/LAYOUTS/SP.core.debug.js" />
/// <reference path="file://C:/Program Files/Common Files/Microsoft Shared/Web Server Extensions/14/TEMPLATE/LAYOUTS/SP.debug.js" />

//This function is called when the user click the Custom Action button
function demoStatusChange() {
    //Change the status bar text and make it green
    var statusHtml = "The user clicked the Custom Action button. <a href='#' onclick='javascript:closeStatus();return false;'>Close</a>";
    this.statusID = SP.UI.Status.addStatus("Demo Status", statusHtml, true);
    SP.UI.Status.setStatusPriColor(this.statusID, "green");
}

function closeStatus() {
    //Remove the status message
    SP.UI.Status.removeStatus(this.statusID);
}