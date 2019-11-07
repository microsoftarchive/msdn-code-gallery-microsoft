//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved


(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/Scenario1.html", {
        ready: function (element, options) {
            document.getElementById("Scenario1Launch").addEventListener("click", launchScenario1, false);
            document.getElementById("Scenario1SignIn").addEventListener("click", signInScenario1, false);
            document.getElementById("Scenario1ChangeUser").addEventListener("click", changeUserScenario1, false);
            document.getElementById("Scenario1Reset").addEventListener("click", resetScenario1, false);
            // Initialize the password vault, this may take less than a second
            // An optimistic initialization at this stage improves the UI performance
            // when any other call to the password vault may be made later on
            asyncVaultLoad();
        }
    });

    var authzInProgress = false;

    function asyncVaultLoad()
    {
        return new WinJS.Promise(function (complete, error, progress) {
            var vault = new Windows.Security.Credentials.PasswordVault();

            // any call to the password vault will load the vault
            var creds = vault.retrieveAll();
            complete();
        });
    }

    function launchScenario1() {

        resetScenario1();

        try {
            var vault = new Windows.Security.Credentials.PasswordVault();


            if (!document.getElementById("Scenario1AuthToggle").checked) {
                resetScenario1();
                var creds = vault.findAllByResource("Scenario 1");
                document.getElementById("Scenario1WelcomeMessage").value = "Welcome to Scenario 1, " + creds.getAt(0).userName;
            } else { // Authentication failed
                creds = vault.retrieveAll();
                for (var i = 0; i < creds.size; i++) {
                    try {
                        vault.remove(creds.getAt(i));
                    }
                    catch (e) {
                    }
                }
                document.getElementById("Scenario1WelcomeMessage").value = "blocked";
            }
        }
        catch (e) { // No stored credentials
            document.getElementById("Scenario1WelcomeMessage").value = "blocked";
        }

    }

    function signInScenario1() {

        cleanMessageFieldScenario1();

        try {

            if (document.getElementById("InputUserNameValue").value === "" || document.getElementById("InputPasswordValue").value === "") {
                document.getElementById("Scenario1DebugAreaOutput").value = "Empty User Name and Blank Password is not allowed";
                throw new Error("Please enter a username and password.");
                
            }

            var vault = new Windows.Security.Credentials.PasswordVault();
            var cred = new Windows.Security.Credentials.PasswordCredential("Scenario 1", document.getElementById("InputUserNameValue").value, document.getElementById("InputPasswordValue").value);

            if (!document.getElementById("Scenario1AuthToggle").checked) {

                if (document.getElementById("SaveCredCheck").checked) {
                    vault.add(cred);
                    document.getElementById("Scenario1DebugAreaOutput").value = "Credential is saved to vault, You can check your credential in 'Control Panel->User Accounts>Credential Manager'";
                }
                document.getElementById("Scenario1WelcomeMessage").value = "Welcome to Scenario 1, " + cred.userName;
            } else { // Authentication failed
                document.getElementById("Scenario1WelcomeMessage").value = "blocked";
            }

            cleanInputFieldScenario1();
        }

        catch (e) {
            document.getElementById("Scenario1WelcomeMessage").value = "blocked";
            document.getElementById("Scenario1DebugAreaOutput").value = e.message;
        }

    }

    function changeUserScenario1() {


        cleanMessageFieldScenario1();

        var vault = new Windows.Security.Credentials.PasswordVault();

        try {
            var creds = vault.findAllByResource("Scenario 1");
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
        resetScenario1();

        document.getElementById("Scenario1DebugAreaOutput").value = "User is removed, please try another user";
        
    }
   
    function cleanInputFieldScenario1() {

        try {

            document.getElementById("SaveCredCheck").checked = false;
            document.getElementById("InputUserNameValue").value = "";
            document.getElementById("InputPasswordValue").value = "";

        } catch (err) {
            return;
        }

    }

    function cleanMessageFieldScenario1() {

        try {

            document.getElementById("Scenario1DebugAreaOutput").value = false;
            document.getElementById("Scenario1WelcomeMessage").value = "";

        } catch (err) {
            return;
        }

    }


    function resetScenario1() {

        // Get all the result reset

        try {

            document.getElementById("SaveCredCheck").checked = false;
            document.getElementById("Scenario1AuthToggle").checked = false;
            document.getElementById("InputUserNameValue").value = "";
            document.getElementById("InputPasswordValue").value = "";
            document.getElementById("Scenario1DebugAreaOutput").value = "";
            document.getElementById("Scenario1WelcomeMessage").value = "";

        } catch (err) {
            return;
        }

    }

})();
