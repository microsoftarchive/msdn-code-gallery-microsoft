//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/Scenario3.html", {
        ready: function (element, options) {
            document.getElementById("Scenario3Launch").addEventListener("click", launchScenario3, false);
            document.getElementById("Scenario3SignIn").addEventListener("click", signInScenario3, false);
            document.getElementById("Scenario3ChangeUser").addEventListener("click", changeUserScenario3, false);
            document.getElementById("Scenario3Reset").addEventListener("click", resetScenario3, false);
            document.getElementById("Scenario3Resources").addEventListener("click", selectResourceScenario3, false);
            document.getElementById("Scenario3Users").addEventListener("click", selectUserScenario3, false);
        }
    });

    var authzInProgress = false;

    function createResourceSelectList() {

        //Creating resource list based on saved vault credentials
        document.getElementById("Scenario3Resources").selectedIndex = 0;

        for (var i = document.getElementById("Scenario3Resources").length; i > 0; i--) {
            document.getElementById("Scenario3Resources").remove(0);
        }

        try {
            var vault = new Windows.Security.Credentials.PasswordVault();
            var creds = vault.retrieveAll();

            for (var j = 0; j < creds.size; j++) {
                var el = document.createElement('option');
                el.text = creds.getAt(i).resource;
                document.getElementById("Scenario3Resources").add(el, 0);
            }
        }
        catch (e) // If there are no stored credentials, no list to populate
        {
        }
        el = document.createElement('option');
        el.text = "Add new resource";
        document.getElementById("Scenario3Resources").add(el, 0);

        el = document.createElement('option');
        el.text = "";
        document.getElementById("Scenario3Resources").add(el, 0);

        document.getElementById("Scenario3Resources").selectedIndex = 0;

    }

    function createUserSelectList() {



        document.getElementById("Scenario3Users").selectedIndex = 0;

        for (var i = document.getElementById("Scenario3Users").length; i > 0; i--) {
            document.getElementById("Scenario3Users").remove(0);
        }

        try {
            var vault = new Windows.Security.Credentials.PasswordVault();
            var creds = vault.retrieveAll();

            for (var j = 0; j < creds.size; j++) {
                var el = document.createElement('option');
                el.text = creds.getAt(i).userName;
                document.getElementById("Scenario3Users").add(el, 0);
            }
        }
        catch (e) // If there are no stored credentials, no list to populate
        {
        }
        el = document.createElement('option');
        el.text = "Add new user";
        document.getElementById("Scenario3Users").add(el, 0);

        el = document.createElement('option');
        el.text = "";
        document.getElementById("Scenario3Users").add(el, 0);

        document.getElementById("Scenario3Users").selectedIndex = 0;

    }

    function launchScenario3() {

        resetScenario3();


        try {
            createResourceSelectList();

            if ((document.getElementById("Scenario3Resources").length) < 3) {
                document.getElementById("Scenario3WelcomeMessage").value = "The resource list is empty, please save some credentials by 'sign in' option";
            }
            else {
                document.getElementById("Scenario3WelcomeMessage").value = "The resource list is ready, please select one and do sign in";
            }
            createUserSelectList();

            if ((document.getElementById("Scenario3Users").length) < 3) {
                document.getElementById("Scenario3WelcomeMessage").value = "The user list is empty, please save some credentials by 'sign in' option";
            }
            else {
                document.getElementById("Scenario3WelcomeMessage").value = "The user list is ready, please select one and do sign in";
            }
            }
        catch (e) { // No stored credentials
            document.getElementById("Scenario3WelcomeMessage").value = "blocked";
        }

    }

    function selectResourceScenario3() {
        try{
            var i = document.getElementById("Scenario3Resources").selectedIndex;
            if (i <2 ) {
                document.getElementById("InputResourceValue").value  = "";
            }
            else {
                document.getElementById("InputResourceValue").value = document.getElementById("Scenario3Resources").options[document.getElementById("Scenario3Resources").selectedIndex].text;
                createUserSelectList();
            }
        }
        catch (e){
            launchScenario3();
        }

    }

    function selectUserScenario3() {

        var i = document.getElementById("Scenario3Users").selectedIndex;
        if (i < 2) {
            document.getElementById("InputUserNameValue3").value = "";
            document.getElementById("InputPasswordValue3").value = "";
        }
        else {
            var vault = new Windows.Security.Credentials.PasswordVault();
            try {
                var cred = vault.retrieve(document.getElementById("Scenario3Resources").options[document.getElementById("Scenario3Resources").selectedIndex].text,document.getElementById("Scenario3Users").options[document.getElementById("Scenario3Users").selectedIndex].text);
                document.getElementById("InputUserNameValue3").value = cred.userName;
                document.getElementById("InputPasswordValue3").value = cred.password;
            }
            catch (e) {
                launchScenario3();
                document.getElementById("Scenario3WelcomeMessage").value = "Can not find the credential, please reselect, make sure the username is matching to the resource";

            }
        }

    }

    function signInScenario3() {

        cleanMessageFieldScenario3();

        try {

            if (document.getElementById("InputUserNameValue3").value === "" || document.getElementById("InputPasswordValue3").value === "") {
                document.getElementById("Scenario3DebugAreaOutput").value = "Empty User Name and Blank Password is not allowed";
                throw new Error("Please enter a username and password.");     
            }

            var vault = new Windows.Security.Credentials.PasswordVault();
            var cred = new Windows.Security.Credentials.PasswordCredential(document.getElementById("InputResourceValue").value, document.getElementById("InputUserNameValue3").value, document.getElementById("InputPasswordValue3").value);

            if (!document.getElementById("Scenario3AuthToggle").checked) {

                if (document.getElementById("SaveCredCheck3").checked) {
                    vault.add(cred);
                    document.getElementById("Scenario3DebugAreaOutput").value = "Credential is saved to vault, You can check your credential in 'Control Panel->User Accounts>Credential Manager'";
                }
                document.getElementById("Scenario3WelcomeMessage").value = "Welcome to Scenario 3, " + cred.userName;
            } else { // Authentication failed
                document.getElementById("Scenario3WelcomeMessage").value = "Authentication failed";
            }

            cleanInputFieldScenario3();

            createResourceSelectList();

            createUserSelectList();

        }

        catch (e) {
            document.getElementById("Scenario3WelcomeMessage").value = "blocked";
            document.getElementById("Scenario3DebugAreaOutput").value = e.message;
        }

    }

    function changeUserScenario3() {


        cleanMessageFieldScenario3();

        var vault = new Windows.Security.Credentials.PasswordVault();

        try {
            var creds = vault.retrieveAll();
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
        resetScenario3();

        document.getElementById("Scenario3DebugAreaOutput").value = "User is removed, please try another user";

    }

    function cleanInputFieldScenario3() {

        try {

            document.getElementById("SaveCredCheck3").checked = false;
            document.getElementById("InputResourceValue").value = "";
            document.getElementById("InputUserNameValue3").value = "";
            document.getElementById("InputPasswordValue3").value = "";

        } catch (err) {
            return;
        }

    }

    function cleanMessageFieldScenario3() {

        try {

            document.getElementById("Scenario3DebugAreaOutput").value = false;
            document.getElementById("Scenario3WelcomeMessage").value = "";

        } catch (err) {
            return;
        }

    }


    function resetScenario3() {

        // Get all the result reset

        try {

            document.getElementById("SaveCredCheck3").checked = false;
            document.getElementById("Scenario3AuthToggle").checked = false;
            document.getElementById("InputUserNameValue3").value = "";
            document.getElementById("InputPasswordValue3").value = "";
            document.getElementById("InputResourceValue").value = "";
            document.getElementById("Scenario3DebugAreaOutput").value = "";
            document.getElementById("Scenario3WelcomeMessage").value = "";
            document.getElementById("Scenario3Resources").selectedIndex = 0;
            document.getElementById("Scenario3Users").selectedIndex = 0;


        } catch (err) {
            return;
        }

    }


})();
