//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario7.html", {
        ready: function (element, options) {
            document.getElementById("RunSample").addEventListener("click", RunSample, false);
        }
    });

    function RunSample() {
        var algName = AlgorithmNames.options[AlgorithmNames.selectedIndex].innerText;
        var KeySize;
        Scenario7Output.textContent = "";

        if (KeySizes.selectedIndex === 0) {
            KeySize = 512;
        }
        else if (KeySizes.selectedIndex === 1) {
            KeySize = 1024;
        }
        else if (KeySizes.selectedIndex === 2) {
            KeySize = 2048;
        }
        else if (KeySizes.selectedIndex === 3) {
            KeySize = 4096;
        }
        else {
            Scenario7Output.textContent += "An invalid key size was specified.\n";
            return;
        }

        var Data;
        var cookie = "Some cookie to encrypt";

        switch (AlgorithmNames.selectedIndex)
        {
            case 0:
                Data = Windows.Security.Cryptography.CryptographicBuffer.convertStringToBinary(cookie, Windows.Security.Cryptography.BinaryStringEncoding.utf16LE);
                break;

                // OAEP Padding depends on key size, message length and hash block length
                // 
                // The maximum plaintext length is KeyLength - 2*HashBlock - 2
                //
                // OEAP padding supports an optional label with the length is restricted by plaintext/key/hash sizes.
                // Here we just use a small label.
            case 1:
                Data = Windows.Security.Cryptography.CryptographicBuffer.generateRandom(1024 / 8 - 2 * 20 - 2);
                break;
            case 2:
                Data = Windows.Security.Cryptography.CryptographicBuffer.generateRandom(1024 / 8 - 2 * (256 / 8) - 2);
                break;
            case 3:
                Data = Windows.Security.Cryptography.CryptographicBuffer.generateRandom(2048 / 8 - 2 * (384 / 8) - 2);
                break;
            case 4:
                Data = Windows.Security.Cryptography.CryptographicBuffer.generateRandom(2048 / 8 - 2 * (512 / 8) - 2);
                break;
            default:
                Scenario7Output.textContent += "An invalid algorithm was selected";
                return;
        }

        var Encrypted;
        var Decrypted;
        var blobOfPublicKey;
        var blobOfKeyPair;

        // Crate an AsymmetricKeyAlgorithmProvider object for the algorithm specified on input.
        var Algorithm = Windows.Security.Cryptography.Core.AsymmetricKeyAlgorithmProvider.openAlgorithm(algName);

        Scenario7Output.textContent += "*** Sample Encryption Algorithm\n";
        Scenario7Output.textContent += "    Algorithm Name: " + Algorithm.algorithmName + "\n";
        Scenario7Output.textContent += "    Key Size: " + KeySize + "\n";

        // Generate a random key.
        var keyPair = Algorithm.createKeyPair(KeySize);

        // Encrypt the data.
        try
        {
            Encrypted = Windows.Security.Cryptography.Core.CryptographicEngine.encrypt(keyPair, Data, null);
        }
        catch (ex)
        {
            Scenario7Output.textContent += ex.message;
            Scenario7Output.textContent += "An invalid key size was selected for the given algorithm.\n";
            return;
        }

        Scenario7Output.textContent += "    Plain text: " + Data.length + " bytes\n";
        Scenario7Output.textContent += "    Encrypted: " + Encrypted.length + " bytes\n";

        // Export the public key.
        blobOfPublicKey = keyPair.exportPublicKey();
        blobOfKeyPair = keyPair.export();

        // Import the public key.
        var keyPublic = Algorithm.importPublicKey(blobOfPublicKey);
        if (keyPublic.keySize !== keyPair.keySize)
        {
            Scenario7Output.textContent += "ImportPublicKey failed!  The imported key's size did not match the original's!";
            return;
        }

        // Import the key pair.
        keyPair = Algorithm.importKeyPair(blobOfKeyPair);

        // Check the key size of the imported key.
        if (keyPublic.keySize !== keyPair.keySize)
        {
            Scenario7Output.textContent += "ImportKeyPair failed!  The imported key's size did not match the original's!";
            return;
        }

        // Decrypt the data.
        Decrypted = Windows.Security.Cryptography.Core.CryptographicEngine.decrypt(keyPair, Encrypted, null);

        if (!Windows.Security.Cryptography.CryptographicBuffer.compare(Decrypted, Data))
        {
            Scenario7Output.textContent += "Decrypted data does not match original!";
            return;
        }
    }
})();
