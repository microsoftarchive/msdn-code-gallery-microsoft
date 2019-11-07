//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario5.html", {
        ready: function (element, options) {
            document.getElementById("RunSample").addEventListener("click", RunSample, false);
        }
    });

    function RunSample() {
        var algName = AlgorithmNames.options[AlgorithmNames.selectedIndex].innerText;
        var keySize;
        Scenario5Output.textContent = "";

        if (KeySizes.selectedIndex === 0) {
            keySize = 64;
        }
        else if (KeySizes.selectedIndex === 1) {
            keySize = 128;
        }
        else if (KeySizes.selectedIndex === 2) {
            keySize = 192;
        }
        else if (KeySizes.selectedIndex === 3) {
            keySize = 256;
        }
        else if (KeySizes.selectedIndex === 4) {
            keySize = 512;
        }
        else {
            Scenario5Output.textContent += "An invalid key size was specified.\n";
            return;
        }

        var encrypted;
        var decrypted;
        var buffer;
        var iv = null;
        var blockCookie = "1234567812345678"; // 16 bytes

        // Open the algorithm provider for the algorithm specified on input.
        var Algorithm = Windows.Security.Cryptography.Core.SymmetricKeyAlgorithmProvider.openAlgorithm(algName);

        Scenario5Output.textContent += "\n*** Sample Cipher Encryption\n";
        Scenario5Output.textContent += "    Algorithm Name: " + Algorithm.algorithmName + "\n";
        Scenario5Output.textContent += "    Key Size: " + keySize + "\n";
        Scenario5Output.textContent += "    Block length: " + Algorithm.blockLength + "\n";

        // Generate a symmetric key.
        var keymaterial = Windows.Security.Cryptography.CryptographicBuffer.generateRandom((keySize + 7) / 8);
        var key;
        try
        {
            key = Algorithm.createSymmetricKey(keymaterial);
        }
        catch (ex)
        {
            Scenario5Output.textContent += ex.message;
            Scenario5Output.textContent += "An invalid key size was selected for the given algorithm.\n";
            return;
        }

        // CBC mode needs Initialization vector, here just random data.
        // IV property will be set on "Encrypted".
        if (algName.indexOf("CBC", 0) !== -1)
        {
            iv = Windows.Security.Cryptography.CryptographicBuffer.generateRandom(Algorithm.blockLength);
        }

        // Set the data to encrypt. 
        buffer = Windows.Security.Cryptography.CryptographicBuffer.convertStringToBinary(blockCookie, Windows.Security.Cryptography.BinaryStringEncoding.utf8);

        // Encrypt and create an authenticated tag.
        encrypted = Windows.Security.Cryptography.Core.CryptographicEngine.encrypt(key, buffer, iv);

        Scenario5Output.textContent += "    Plain text: " + buffer.length + " bytes\n";
        Scenario5Output.textContent += "    Encrypted: " + encrypted.length + " bytes\n";

        // Create another instance of the key from the same material.
        var key2 = Algorithm.createSymmetricKey(keymaterial);

        if (key.keySize !== key2.keySize)
        {
            Scenario5Output.textContent += "CreateSymmetricKey failed!  The imported key's size did not match the original's!";
            return;
        }

        // Decrypt and verify the authenticated tag.
        decrypted = Windows.Security.Cryptography.Core.CryptographicEngine.decrypt(key2, encrypted, iv);

        if (!Windows.Security.Cryptography.CryptographicBuffer.compare(decrypted, buffer))
        {
            Scenario5Output.textContent += "Decrypted does not match original!";
            return;
        }
    }
})();
