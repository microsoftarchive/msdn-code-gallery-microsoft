//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario8.html", {
        ready: function (element, options) {
            document.getElementById("RunSample").addEventListener("click", RunSample, false);
        }
    });

    function RunSample() {
        var algName = AlgorithmNames.options[AlgorithmNames.selectedIndex].innerText;
        var KeySize;
        Scenario8Output.textContent = "";

        if (KeySizes.selectedIndex === 0) {
            KeySize = 256;
        }
        else if (KeySizes.selectedIndex === 1) {
            KeySize = 384;
        }
        else if (KeySizes.selectedIndex === 2) {
            KeySize = 521;
        }
        else if (KeySizes.selectedIndex === 3) {
            KeySize = 1024;
        }
        else if (KeySizes.selectedIndex === 4) {
            KeySize = 2048;
        }
        else if (KeySizes.selectedIndex === 5) {
            KeySize = 3072;
        }
        else if (KeySizes.selectedIndex === 6) {
            KeySize = 4096;
        }
        else {
            Scenario8Output.textContent += "An invalid key size was specified.\n";
            return;
        }

        var keyPair;
        var blobOfPublicKey;
        var blobOfKeyPair;
        var cookie = "Some Data to sign";
        var Data = Windows.Security.Cryptography.CryptographicBuffer.convertStringToBinary(cookie, Windows.Security.Cryptography.BinaryStringEncoding.utf16BE);

        // Create an AsymmetricKeyAlgorithmProvider object for the algorithm specified on input.
        var Algorithm = Windows.Security.Cryptography.Core.AsymmetricKeyAlgorithmProvider.openAlgorithm(algName);

        Scenario8Output.textContent += "*** Sample Signature Algorithm\n";
        Scenario8Output.textContent += "    Algorithm Name: " + Algorithm.algorithmName + "\n";
        Scenario8Output.textContent += "    Key Size: " + KeySize + "\n";

        // Generate a key pair.
        try
        {
            keyPair = Algorithm.createKeyPair(KeySize);
        }
        catch (ex)
        {
            Scenario8Output.textContent += "An invalid key size was specified for the given algorithm.";
            return;
        }
        // Sign the data by using the generated key.
        var Signature = Windows.Security.Cryptography.Core.CryptographicEngine.sign(keyPair, Data);
        Scenario8Output.textContent += "    Data was successfully signed.\n";

        // Export the public key.
        blobOfPublicKey = keyPair.exportPublicKey();
        blobOfKeyPair = keyPair.export();
        Scenario8Output.textContent += "    Key pair was successfully exported.\n";

        // Import the public key.
        var keyPublic = Algorithm.importPublicKey(blobOfPublicKey);

        // Check the key size.
        if (keyPublic.keySize !== keyPair.keySize)
        {
            Scenario8Output.textContent += "ImportPublicKey failed!  The imported key's size did not match the original's!";
            return;
        }
        Scenario8Output.textContent += "    Public key was successfully imported.\n";

        // Import the key pair.
        keyPair = Algorithm.importKeyPair(blobOfKeyPair);

        // Check the key size.
        if (keyPublic.keySize !== keyPair.keySize)
        {
            Scenario8Output.textContent += "ImportKeyPair failed!  The imported key's size did not match the original's!";
            return;
        }
        Scenario8Output.textContent += "    Key pair was successfully imported.\n";

        // Verify the signature by using the public key.
        if (!Windows.Security.Cryptography.Core.CryptographicEngine.verifySignature(keyPublic, Data, Signature))
        {
            Scenario8Output.textContent += "Signature verification failed!";
            return;
        }
        Scenario8Output.textContent += "    Signature was successfully verified.\n";
    }
})();
