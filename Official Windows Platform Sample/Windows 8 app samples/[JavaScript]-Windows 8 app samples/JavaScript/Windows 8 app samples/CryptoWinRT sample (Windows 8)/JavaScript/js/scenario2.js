//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario2.html", {
        ready: function (element, options) {
            document.getElementById("RunSample").addEventListener("click", RunSample, false);
        }
    });

    function RunSample() {
        Scenario2Output.textContent = "";
        var algName = AlgorithmNames.options[AlgorithmNames.selectedIndex].innerText;

        // Create a HashAlgorithmProvider object.
        var Algorithm = Windows.Security.Cryptography.Core.HashAlgorithmProvider.openAlgorithm(algName);
        var vector = Windows.Security.Cryptography.CryptographicBuffer.decodeFromBase64String("uiwyeroiugfyqcajkds897945234==");

        Scenario2Output.textContent += "\n*** Sample Hash Algorithm: " + Algorithm.algorithmName + "\n";
        Scenario2Output.textContent += "    Initial vector:  uiwyeroiugfyqcajkds897945234==\n";

        // Compute the hash in one call.
        var digest = Algorithm.hashData(vector);

        if (digest.length !== Algorithm.hashLength)
        {
            Scenario2Output.textContent += "HashAlgorithmProvider failed to generate a hash of proper length!\n";
            return;
        }

        Scenario2Output.textContent += "    Hash:  " + Windows.Security.Cryptography.CryptographicBuffer.encodeToHexString(digest) + "\n";

        // Use a reusable hash object to hash the data by using multiple calls.
        var reusableHash = Algorithm.createHash();

        reusableHash.append(vector);

        // Note that calling GetValue resets the data that has been appended to the
        // CryptographicHash object.
        var digest2 = reusableHash.getValueAndReset();

        if (!Windows.Security.Cryptography.CryptographicBuffer.compare(digest, digest2))
        {
            Scenario2Output.textContent += "CryptographicHash failed to generate the same hash data!\n";
            return;
        }

        reusableHash.append(vector);
        digest2 = reusableHash.getValueAndReset();

        if (!Windows.Security.Cryptography.CryptographicBuffer.compare(digest, digest2))
        {
            Scenario2Output.textContent += "Reusable CryptographicHash failed to generate the same hash data!\n";
            return;
        }
    }
})();
