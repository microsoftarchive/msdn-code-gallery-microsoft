//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {

    // Track if the log in was successful
    var loggedIn;

    "use strict";
    var page = WinJS.UI.Pages.define("/html/collect-information.html", {
        ready: function (element, options) {
            document.getElementById("loginButton").addEventListener("click", showLoginFlyout, false);
            document.getElementById("submitLoginButton").addEventListener("click", submitLogin, false);
            document.getElementById("loginFlyout").addEventListener("afterhide", onDismiss, false);
        }
    });

    // Show the flyout
    function showLoginFlyout() {
        loggedIn = false;
        WinJS.log && WinJS.log("", "sample", "status", "status");

        var loginButton = document.getElementById("loginButton");
        document.getElementById("loginFlyout").winControl.show(loginButton);
    }

    // Show errors if any of the text fields are not filled out when the Login button is clicked
    function submitLogin() {
        var error = false;
        if (document.getElementById("password").value.trim() === "") {
            document.getElementById("passwordError").innerHTML = "Password cannot be blank";
            document.getElementById("password").focus();
            error = true;
        } else {
            document.getElementById("passwordError").innerHTML = "";
        }
        if (document.getElementById("username").value.trim() === "") {
            document.getElementById("usernameError").innerHTML = "Username cannot be blank";
            document.getElementById("username").focus();
            error = true;
        } else {
            document.getElementById("usernameError").innerHTML = "";
        }

        if (!error) {
            loggedIn = true;
            WinJS.log && WinJS.log("You have logged in.", "sample", "status");
            document.getElementById("loginFlyout").winControl.hide();
        }
    }

    // On dismiss of the flyout, reset the fields in the flyout
    function onDismiss() {

        // Clear fields on dismiss
        document.getElementById("username").value = "";
        document.getElementById("usernameError").innerHTML = "";
        document.getElementById("password").value = "";
        document.getElementById("passwordError").innerHTML = "";

        if (!loggedIn) {
            WinJS.log && WinJS.log("You have not logged in.", "sample", "status");
        }
    }

})();
