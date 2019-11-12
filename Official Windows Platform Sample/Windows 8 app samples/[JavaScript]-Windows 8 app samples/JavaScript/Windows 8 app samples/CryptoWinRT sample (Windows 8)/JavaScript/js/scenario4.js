//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario4.html", {
        ready: function (element, options) {
            document.getElementById("RunSample").addEventListener("click", RunSample, false);
        }
    });

    function RunSample() {
        var algName = AlgorithmNames.options[AlgorithmNames.selectedIndex].innerText;
        var Secret = Windows.Security.Cryptography.CryptographicBuffer.convertStringToBinary("Master key to derive from", Windows.Security.Cryptography.BinaryStringEncoding.utf8);
        var TargetSize;
        var Params;
        Scenario4Output.textContent = "";

        if (KeySizes.selectedIndex === 0) {
            TargetSize = 64;
        }
        else if (KeySizes.selectedIndex === 1) {
            TargetSize = 256;
        }
        else {
            Scenario4Output.textContent += "An invalid target size was specified.\n";
            return;
        }


        if (algName.indexOf("PBKDF2",0) === 0)
        {
            // Password based key derivation function (PBKDF2).
            Params = Windows.Security.Cryptography.Core.KeyDerivationParameters.buildForPbkdf2(
                            Windows.Security.Cryptography.CryptographicBuffer.generateRandom(16),  // Salt
                            10000                       // PBKDF2 Iteration Count
                            );
        }
        else if (algName.indexOf("SP800_108", 0) === 0)
        {
            // SP800_108_CTR_HMAC key derivation function.
            Params = Windows.Security.Cryptography.Core.KeyDerivationParameters.buildForSP800108(
                             Windows.Security.Cryptography.CryptographicBuffer.convertStringToBinary("Label", Windows.Security.Cryptography.BinaryStringEncoding.utf8),  // Label
                             Windows.Security.Cryptography.CryptographicBuffer.decodeFromHexString("303132333435363738")                   // Context
                             );
        }
        else if (algName.indexOf("SP800_56A", 0) === 0)
        {
            Params = Windows.Security.Cryptography.Core.KeyDerivationParameters.buildForSP80056a(
                Windows.Security.Cryptography.CryptographicBuffer.convertStringToBinary("AlgorithmId", Windows.Security.Cryptography.BinaryStringEncoding.utf8),
                Windows.Security.Cryptography.CryptographicBuffer.convertStringToBinary("VParty", Windows.Security.Cryptography.BinaryStringEncoding.utf8),
                Windows.Security.Cryptography.CryptographicBuffer.convertStringToBinary("UParty", Windows.Security.Cryptography.BinaryStringEncoding.utf8),
                Windows.Security.Cryptography.CryptographicBuffer.convertStringToBinary("SubPubInfo", Windows.Security.Cryptography.BinaryStringEncoding.utf8),
                Windows.Security.Cryptography.CryptographicBuffer.convertStringToBinary("SubPrivInfo", Windows.Security.Cryptography.BinaryStringEncoding.utf8)
                );
        }
        else
        {
            Scenario4Output.textContent += "    An invalid algorithm was specified.\n";
            return;
        }

        // Create a KeyDerivationAlgorithmProvider object for the algorithm specified on input.
        var Algorithm = Windows.Security.Cryptography.Core.KeyDerivationAlgorithmProvider.openAlgorithm(algName);

        Scenario4Output.textContent += "*** Sample Kdf Algorithm: " + Algorithm.algorithmName + "\n";
        Scenario4Output.textContent += "    Secrect Size: " + Secret.length + "\n";
        Scenario4Output.textContent += "    Target Size: " + TargetSize + "\n";

        // Create a key.
        var key = Algorithm.createKey(Secret);

        // Derive a key from the created key.
        var derived = Windows.Security.Cryptography.Core.CryptographicEngine.deriveKeyMaterial(key, Params, TargetSize);
        Scenario4Output.textContent += "    Derived  " + derived.length + " bytes\n";
        Scenario4Output.textContent += "    Derived: " + Windows.Security.Cryptography.CryptographicBuffer.encodeToHexString(derived) + "\n";
    }
})();
