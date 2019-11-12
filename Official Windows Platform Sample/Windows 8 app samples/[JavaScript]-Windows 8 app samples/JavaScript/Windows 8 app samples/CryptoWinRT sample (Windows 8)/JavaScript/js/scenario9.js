//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario9.html", {
        ready: function (element, options) {
            document.getElementById("RunSample").addEventListener("click", RunSample, false);
        }
    });

    function RunSample() {
        var descriptor = tbDescriptor.value;
        Scenario9Output.textContent = "";

        Scenario9Output.textContent += "*** Sample Data Protection for " + descriptor + " ***\n";

        var Provider = new Windows.Security.Cryptography.DataProtection.DataProtectionProvider(descriptor);
        Scenario9Output.textContent += "    DataProtectionProvider is Ready\n";

        // Create random data for protection
        var data = Windows.Security.Cryptography.CryptographicBuffer.generateRandom(73);
        Scenario9Output.textContent += "    Original Data: " + Windows.Security.Cryptography.CryptographicBuffer.encodeToHexString(data) + "\n";

        try {
            // Protect the random data
            Provider.protectAsync(data).done(function (protectedData) {
                Scenario9Output.textContent += "    Protected Data: " + Windows.Security.Cryptography.CryptographicBuffer.encodeToHexString(protectedData) + "\n";
                if (Windows.Security.Cryptography.CryptographicBuffer.compare(data, protectedData))
                {
                    Scenario9Output.textContent += "ProtectAsync returned unprotected data";
                    return;
                }
                Scenario9Output.textContent += "    ProtectAsync succeeded\n";

                // Unprotect
                var Provider2 = new Windows.Security.Cryptography.DataProtection.DataProtectionProvider();

                Provider2.unprotectAsync(protectedData).done(function (unprotectedData) {
                    if (!Windows.Security.Cryptography.CryptographicBuffer.compare(data, unprotectedData))
                    {
                        Scenario9Output.textContent += "UnprotectAsync returned invalid data";
                        return;
                    }

                    Scenario9Output.textContent += "    Unprotected Data: " + Windows.Security.Cryptography.CryptographicBuffer.encodeToHexString(unprotectedData) + "\n";
                    Scenario9Output.textContent += "*** Done!\n";
                });
            });


        }
        catch (ex) {
            Scenario9Output.textContent += "Function TestProtect failed :" + ex.message;
            return;
        }
    }
})();
