//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/ApplyAsync.html", {
        ready: function (element, options) {
            document.getElementById("ApplyAsyncLaunch").addEventListener("click", applyPolicy, false);
            document.getElementById("ApplyAsyncReset").addEventListener("click", applyReset, false);
        }
    });

 
    
    function applyPolicy() {

        // Get all the parameters from the user
        var requireEncryptionValue = document.getElementById("RequireEncryptionValue2").checked;
        if (!requireEncryptionValue) {
            requireEncryptionValue = false;
        }

        var minPasswordLengthValue = document.getElementById("MinPasswordLengthValue2").value;
        if (minPasswordLengthValue === null || minPasswordLengthValue === "") {
            minPasswordLengthValue = 0;
        }

        var disallowConvenienceLogonValue = document.getElementById("DisallowConvenienceLogonValue2").checked;
        if (!disallowConvenienceLogonValue) {
            disallowConvenienceLogonValue = false;
        }


        var minPasswordComplexCharactersValue = document.getElementById("MinPasswordComplexCharactersValue2").value;
        if (minPasswordComplexCharactersValue === null || minPasswordComplexCharactersValue === "") {
            minPasswordComplexCharactersValue = 0;
        }

        var passwordExpirationValue = document.getElementById("PasswordExpirationValue2").value;
        if (passwordExpirationValue === null || passwordExpirationValue === "") {
            passwordExpirationValue = 0;
        }

        var passwordHistoryValue = document.getElementById("PasswordHistoryValue2").value;
        if (passwordHistoryValue === null || passwordHistoryValue === "") {
            passwordHistoryValue = 0;
        }

        var maxPasswordFailedAttemptsValue = document.getElementById("MaxPasswordFailedAttemptsValue2").value;
        if (maxPasswordFailedAttemptsValue === null || maxPasswordFailedAttemptsValue === "") {
            maxPasswordFailedAttemptsValue = 0;
        }

        var maxInactivityTimeLockValue = document.getElementById("MaxInactivityTimeLockValue2").value;
        if (maxInactivityTimeLockValue === null || maxInactivityTimeLockValue === "") {
            maxInactivityTimeLockValue = 0;
        }


        try {

            var requestedPolicy = new Windows.Security.ExchangeActiveSyncProvisioning.EasClientSecurityPolicy();
            requestedPolicy.requireEncryption = requireEncryptionValue;
            requestedPolicy.minPasswordLength = parseInt(minPasswordLengthValue);
            requestedPolicy.disallowConvenienceLogon = disallowConvenienceLogonValue;
            requestedPolicy.minPasswordComplexCharacters = parseInt(minPasswordComplexCharactersValue);
            requestedPolicy.passwordExpiration = parseInt(passwordExpirationValue) * 86400000;
            requestedPolicy.passwordHistory = parseInt(passwordHistoryValue);
            requestedPolicy.maxPasswordFailedAttempts = parseInt(maxPasswordFailedAttemptsValue);
            requestedPolicy.maxInactivityTimeLock = parseInt(maxInactivityTimeLockValue) * 1000;

            requestedPolicy.applyAsync().done(callbackApply, callbackApplyError);

        } catch (err) {
            sdkSample.displayError("Error CheckCompliance: " + err);
            return;
        }

    }

    function callbackApply(result) {

        document.getElementById("RequireEncryptionApplyResult").value = result.requireEncryptionResult;
        document.getElementById("MinPasswordLengthApplyResult").value = result.minPasswordLengthResult;
        document.getElementById("DisallowConvenienceLogonApplyResult").value = result.disallowConvenienceLogonResult;
        document.getElementById("MinPasswordComplexCharactersApplyResult").value = result.minPasswordComplexCharactersResult;
        document.getElementById("PasswordExpirationApplyResult").value = result.passwordExpirationResult;
        document.getElementById("PasswordHistoryApplyResult").value = result.passwordHistoryResult;
        document.getElementById("MaxPasswordFailedAttemptsApplyResult").value = result.maxPasswordFailedAttemptsResult;
        document.getElementById("MaxInactivityTimeLockApplyResult").value = result.maxInactivityTimeLockResult;
    }

    function callbackApplyError(err) {
        WinJS.log("Error returned by EAS ApplyAsync: " + err, "EAS SDK Sample", "error");
        document.getElementById("EASDebugArea").value += " Error Message: " + err.message + "\r\n";
    }


    function applyReset() {

        // Get all the result reset

        try {

            document.getElementById("RequireEncryptionValue2").checked = false;
            document.getElementById("MinPasswordLengthValue2").value = "";
            document.getElementById("DisallowConvenienceLogonValue2").checked = false;
            document.getElementById("MinPasswordComplexCharactersValue2").value = "";
            document.getElementById("PasswordExpirationValue2").value = "";
            document.getElementById("PasswordHistoryValue2").value = "";
            document.getElementById("MaxPasswordFailedAttemptsValue2").value = "";
            document.getElementById("MaxInactivityTimeLockValue2").value = "";

            document.getElementById("RequireEncryptionApplyResult").value = "";
            document.getElementById("MinPasswordLengthApplyResult").value = "";
            document.getElementById("DisallowConvenienceLogonApplyResult").value = "";
            document.getElementById("MinPasswordComplexCharactersApplyResult").value = "";
            document.getElementById("PasswordExpirationApplyResult").value = "";
            document.getElementById("PasswordHistoryApplyResult").value = "";
            document.getElementById("MaxPasswordFailedAttemptsApplyResult").value = "";
            document.getElementById("MaxInactivityTimeLockApplyResult").value = "";

        } catch (err) {
            sdkSample.displayError("Error reset CheckCompliance: " + err);
            return;
        }

    }

})();
