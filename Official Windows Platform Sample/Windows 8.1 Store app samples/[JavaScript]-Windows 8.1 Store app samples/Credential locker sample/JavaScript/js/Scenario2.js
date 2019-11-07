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
            document.getElementById("ManageButton").addEventListener("click", manage, false);
        }
    });

    var settingsPane = Windows.UI.ApplicationSettings.SettingsPane.getForCurrentView();

    settingsPane.oncommandsrequested = function (args) {
        args.request.applicationCommands.append(
            Windows.UI.ApplicationSettings.SettingsCommand.accountsCommand);
    };

    var accountSettingsPane = Windows.UI.ApplicationSettings.AccountsSettingsPane.getForCurrentView();

    accountSettingsPane.onaccountcommandsrequested = function (args) {

        var passwordVault = Windows.Security.Credentials.PasswordVault();
        var passwordCredentials = passwordVault.retrieveAll();
        var credDeleteHandler = function (credcmd) {
            document.getElementById("ErrorMessage").value = "Credential is now removed";
            try {
                Windows.UI.ApplicationSettings.AccountsSettingsPane.show();
            }
            catch (e) { // No stored credentials
                document.getElementById("ErrorMessage").value = e.message;
            }

        };

        for (var i = 0; i < passwordCredentials.size; i++) {
            var credentialCmd = Windows.UI.ApplicationSettings.CredentialCommand(passwordCredentials.getAt(i), credDeleteHandler);
            args.credentialCommands.append(credentialCmd);
        }


        var commandInvokedHandler = function (command) {
            document.getElementById("ErrorMessage").value = "Please implement specified cmd";
            try {
                Windows.UI.ApplicationSettings.AccountsSettingsPane.show();
            }
            catch (e) { // No stored credentials
                document.getElementById("ErrorMessage").value = e.message;
            }
        };

        var appSpecifiedCmd = Windows.UI.ApplicationSettings.SettingsCommand(
                                                                    1,
                                                                    "App specified cmd label",
                                                                    commandInvokedHandler);

        args.commands.append(appSpecifiedCmd);

    };

    function manage() {
        try {
            Windows.UI.ApplicationSettings.AccountsSettingsPane.show();
            }
        catch (e) { // No stored credentials
            document.getElementById("ErrorMessage").value = e.message;
        }

    }


})();
