//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/Scenario2.html", {
        ready: function (element, options) {
            document.getElementById("Scenario2Launch").addEventListener("click", launchScenario2, false);
            document.getElementById("Scenario2SignIn").addEventListener("click", signInScenario2, false);
            document.getElementById("Scenario2ChangeUser").addEventListener("click", changeUserScenario2, false);
            document.getElementById("Scenario2Reset").addEventListener("click", resetScenario2, false);
            document.getElementById("Scenario2Users").addEventListener("click", selectScenario2, false);   
        }
    });

     function createUserList() {
        document.getElementById("Scenario2Users").selectedIndex = 0;

        for (var i = document.getElementById("Scenario2Users").length; i > 0; i--)
        {
            document.getElementById("Scenario2Users").remove(0);
        }

        try {
            var vault = new Windows.Security.Credentials.PasswordVault();
            var creds = vault.findAllByResource("Scenario 2");

            for (var j = 0; j < creds.size; j++) {
                var el = document.createElement('option');
                el.text = creds.getAt(i).userName;
                document.getElementById("Scenario2Users").add(el, 0);
            }
        }
        catch (e) // If there are no stored credentials, no list to populate
        {
        }
        el = document.createElement('option');
        el.text = "Add new user";
        document.getElementById("Scenario2Users").add(el, 0);

        el = document.createElement('option');
        el.text = "";
        document.getElementById("Scenario2Users").add(el, 0);

        document.getElementById("Scenario2Users").selectedIndex = 0;

    }

    function launchScenario2() {

        resetScenario2();


        try {
            createUserList();

            if (document.getElementById("Scenario2Users").length === 2) {
                document.getElementById("Scenario2WelcomeMessage").value = "The user list is empty, please save some credentials by 'sign in' option";
            }
            else {
                document.getElementById("Scenario2WelcomeMessage").value = "The user list is ready, please select one and do sign in";
            }
            }
        catch (e) { // No stored credentials
            document.getElementById("Scenario2WelcomeMessage").value = "blocked";
        }

    }

    function selectScenario2() {

        var i = document.getElementById("Scenario2Users").selectedIndex;
        if (i <2 ) {
            document.getElementById("InputUserNameValue").value = "";
            document.getElementById("InputPasswordValue").value = "";
        }
        else {
            var vault = new Windows.Security.Credentials.PasswordVault();
            try {
                var cred = vault.retrieve("Scenario 2", document.getElementById("Scenario2Users").options[document.getElementById("Scenario2Users").selectedIndex].text);
                document.getElementById("InputUserNameValue").value = cred.userName;
                document.getElementById("InputPasswordValue").value = cred.password;
            }
            catch (e) {
                launchScenario2();
            }
        }

    }

    function signInScenario2() {

        cleanMessageFieldScenario2();

        try {

            if (document.getElementById("InputUserNameValue").value === "" || document.getElementById("InputPasswordValue").value === "") {
                document.getElementById("Scenario2DebugAreaOutput").value = "Empty User Name and Blank Password is not allowed";
                throw new Error("Please enter a username and password.");
            }

            var vault = new Windows.Security.Credentials.PasswordVault();
            var cred = new Windows.Security.Credentials.PasswordCredential("Scenario 2", document.getElementById("InputUserNameValue").value, document.getElementById("InputPasswordValue").value);

            if (!document.getElementById("Scenario2AuthToggle").checked) {

                if (document.getElementById("SaveCredCheck").checked) {
                    vault.add(cred);
                    document.getElementById("Scenario2DebugAreaOutput").value = "Credential is saved to vault, You can check your credential in 'Control Panel->User Accounts>Credential Manager'";
                }
                document.getElementById("Scenario2WelcomeMessage").value = "Welcome to Scenario 2, " + cred.userName;
            } else { // Authentication failed
                document.getElementById("Scenario2WelcomeMessage").value = "Authentication failed";
            }

            cleanInputFieldScenario2();

            createUserList();
        }

        catch (e) {
            document.getElementById("Scenario2WelcomeMessage").value = "blocked";
            document.getElementById("Scenario2DebugAreaOutput").value = e.message;
        }

    }

    function changeUserScenario2() {


        cleanMessageFieldScenario2();

        var vault = new Windows.Security.Credentials.PasswordVault();

        try {
            var creds = vault.findAllByResource("Scenario 2");
            for (var i = 0; i < creds.size; i++) {
                try {
                    vault.remove(creds.getAt(i));
                }
                catch (e) { // Remove is best effort
                }
            }
        }
        catch (e) { // No credentials to remove
        }
        resetScenario2();

        document.getElementById("Scenario2DebugAreaOutput").value = "User is removed, please try another user";

    }

    function cleanInputFieldScenario2() {

        try {

            document.getElementById("SaveCredCheck").checked = false;
            document.getElementById("InputUserNameValue").value = "";
            document.getElementById("InputPasswordValue").value = "";

        } catch (err) {
            return;
        }

    }

    function cleanMessageFieldScenario2() {

        try {

            document.getElementById("Scenario2DebugAreaOutput").value = false;
            document.getElementById("Scenario2WelcomeMessage").value = "";

        } catch (err) {
            return;
        }

    }


    function resetScenario2() {

        // Get all the result reset

        try {

            document.getElementById("SaveCredCheck").checked = false;
            document.getElementById("Scenario2AuthToggle").checked = false;
            document.getElementById("InputUserNameValue").value = "";
            document.getElementById("InputPasswordValue").value = "";
            document.getElementById("Scenario2DebugAreaOutput").value = "";
            document.getElementById("Scenario2WelcomeMessage").value = "";


        } catch (err) {
            return;
        }

    }


})();
