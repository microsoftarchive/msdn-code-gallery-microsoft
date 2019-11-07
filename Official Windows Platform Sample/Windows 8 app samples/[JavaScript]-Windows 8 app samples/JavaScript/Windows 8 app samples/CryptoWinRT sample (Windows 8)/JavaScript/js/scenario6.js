//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario6.html", {
        ready: function (element, options) {
            document.getElementById("RunSample").addEventListener("click", RunSample, false);
        }
    });

    function RunSample() {
        var algName = AlgorithmNames.options[AlgorithmNames.selectedIndex].innerText;
        var keySize;
        Scenario6Output.textContent = "";

        if (KeySizes.selectedIndex === 0) {
            keySize = 128;
        }
        else if (KeySizes.selectedIndex === 1) {
            keySize = 256;
        }
        else {
            Scenario6Output.textContent += "An invalid key size was specified.\n";
            return;
        }

        var Decrypted;
        var Data;
        var Cookie = "Some Cookie to Encrypt";

        // Data to encrypt.
        Data = Windows.Security.Cryptography.CryptographicBuffer.convertStringToBinary(Cookie, Windows.Security.Cryptography.BinaryStringEncoding.utf16LE);

        // Created a SymmetricKeyAlgorithmProvider object for the algorithm specified on input.
        var Algorithm = Windows.Security.Cryptography.Core.SymmetricKeyAlgorithmProvider.openAlgorithm(algName);

        Scenario6Output.textContent += "*** Sample Authenticated Encryption\n";
        Scenario6Output.textContent += "    Algorithm Name: " + Algorithm.algorithmName + "\n";
        Scenario6Output.textContent += "    Key Size: " + keySize + "\n";
        Scenario6Output.textContent += "    Block length: " + Algorithm.blockLength + "\n";

        // Generate a random key.
        var keymaterial = Windows.Security.Cryptography.CryptographicBuffer.generateRandom((keySize + 7) / 8);
        var key = Algorithm.createSymmetricKey(keymaterial);


        // Microsoft GCM implementation requires a 12 byte Nonce.
        // Microsoft CCM implementation requires a 7-13 byte Nonce.
        var Nonce = Windows.Security.Cryptography.CryptographicBuffer.generateRandom(12);

        // Encrypt and create an authenticated tag on the encrypted data.
        var Encrypted = Windows.Security.Cryptography.Core.CryptographicEngine.encryptAndAuthenticate(key, Data, Nonce, null);

        Scenario6Output.textContent += "    Plain text: " + Data.length + " bytes\n";
        Scenario6Output.textContent += "    Encrypted: " + Encrypted.encryptedData.length + " bytes\n";
        Scenario6Output.textContent += "    AuthTag: " + Encrypted.authenticationTag.length + " bytes\n";

        // Create another instance of the key from the same material.
        var key2 = Algorithm.createSymmetricKey(keymaterial);

        if (key.keySize !== key2.keySize)
        {
            Scenario6Output.textContent += "CreateSymmetricKey failed!  The imported key's size did not match the original's!";
            return;
        }

        // Decrypt and verify the authenticated tag.
        Decrypted = Windows.Security.Cryptography.Core.CryptographicEngine.decryptAndAuthenticate(key2, Encrypted.encryptedData, Nonce, Encrypted.authenticationTag, null);

        if (!Windows.Security.Cryptography.CryptographicBuffer.compare(Decrypted, Data))
        {
            Scenario6Output.textContent += "Decrypted does not match original!";
            return;
        }
    }
})();
