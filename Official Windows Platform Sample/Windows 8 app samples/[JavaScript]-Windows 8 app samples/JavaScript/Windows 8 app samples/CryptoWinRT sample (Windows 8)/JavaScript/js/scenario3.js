//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario3.html", {
        ready: function (element, options) {
            document.getElementById("RunSample").addEventListener("click", RunSample, false);
        }
    });

    function RunSample() {
        var algName = AlgorithmNames.options[AlgorithmNames.selectedIndex].innerText;
        Scenario3Output.textContent = "";

        // Create a sample message.
        var Message = "Some message to authenticate";

        // Created a MacAlgorithmProvider object for the algorithm specified on input.
        var Algorithm = Windows.Security.Cryptography.Core.MacAlgorithmProvider.openAlgorithm(algName);

        Scenario3Output.textContent += "*** Sample Hmac Algorithm: " + Algorithm.algorithmName + "\n";

        // Create a key.
        var keymaterial = Windows.Security.Cryptography.CryptographicBuffer.generateRandom(Algorithm.macLength);
        var hmacKey = Algorithm.createKey(keymaterial);

        // Sign the message by using the key.
        var signature = Windows.Security.Cryptography.Core.CryptographicEngine.sign(
                                        hmacKey,
                                        Windows.Security.Cryptography.CryptographicBuffer.convertStringToBinary(Message, Windows.Security.Cryptography.BinaryStringEncoding.utf8)
                                        );

        Scenario3Output.textContent += "    Signature:  " + Windows.Security.Cryptography.CryptographicBuffer.encodeToHexString(signature) + "\n";

        // Verify the signature.
        hmacKey = Algorithm.createKey(keymaterial);

        var IsAuthenticated = Windows.Security.Cryptography.Core.CryptographicEngine.verifySignature(
                                        hmacKey,
                                        Windows.Security.Cryptography.CryptographicBuffer.convertStringToBinary(Message, Windows.Security.Cryptography.BinaryStringEncoding.utf8),
                                        signature
                                        );

        if (!IsAuthenticated)
        {
            Scenario3Output.textContent += "HashAlgorithmProvider failed to generate a hash of proper length!\n";
            return;
        }
    }
})();
