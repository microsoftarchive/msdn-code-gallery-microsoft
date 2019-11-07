//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/keyderivation.html", {
        ready: function (element, options) {
            document.getElementById("RunSample").addEventListener("click", RunSample_Click, false);
        }
    });
    var output = "";
  

    function RunSample_Click() {
        WinJS.log && WinJS.log("", "sample", "status");
        output = "";
        var selectalgo = document.getElementById("AlgorithmNames");
        var algname = selectalgo.options[selectalgo.selectedIndex].text;
        var secret = Windows.Security.Cryptography.CryptographicBuffer.convertStringToBinary("Master key to derive from", Windows.Security.Cryptography.BinaryStringEncoding.utf8);
        var keysize = document.getElementById("KeySizes");
        var targetsize = parseInt(keysize.options[keysize.selectedIndex].text);
        var params;
        if (algname.indexOf("PBKDF2") !== -1) {
            //Password based key derivation function (PBKDF2).
            params = Windows.Security.Cryptography.Core.KeyDerivationParameters.buildForPbkdf2(Windows.Security.Cryptography.CryptographicBuffer.generateRandom(16), 10000);

        }
        else if (algname.indexOf("SP800_108") !== -1) {
            //SP800_108_CTR_HMAC key derivation function.
            params = Windows.Security.Cryptography.Core.KeyDerivationParameters.buildForSP800108(Windows.Security.Cryptography.CryptographicBuffer.convertStringToBinary("Label", Windows.Security.Cryptography.BinaryStringEncoding.utf8),
                                                                                                 Windows.Security.Cryptography.CryptographicBuffer.decodeFromHexString("303132333435363738"));
        }
        else if (algname.indexOf("SP800_56A") !== -1) {
            params = Windows.Security.Cryptography.Core.KeyDerivationParameters.buildForSP80056a(Windows.Security.Cryptography.CryptographicBuffer.convertStringToBinary("AlgorithmId", Windows.Security.Cryptography.BinaryStringEncoding.utf8),
                                                                                                 Windows.Security.Cryptography.CryptographicBuffer.convertStringToBinary("VParty", Windows.Security.Cryptography.BinaryStringEncoding.utf8),
                                                                                                 Windows.Security.Cryptography.CryptographicBuffer.convertStringToBinary("UParty", Windows.Security.Cryptography.BinaryStringEncoding.utf8),
                                                                                                 Windows.Security.Cryptography.CryptographicBuffer.convertStringToBinary("SubPubInfo", Windows.Security.Cryptography.BinaryStringEncoding.utf8),
                                                                                                 Windows.Security.Cryptography.CryptographicBuffer.convertStringToBinary("SubPrivInfo", Windows.Security.Cryptography.BinaryStringEncoding.utf8)
                                                                                                 );

        }
        else {
            output += "  An invalid algorithm was specified.\n";
            WinJS.log && WinJS.log(output, "sample", "error");
            return;
        }

        //Create a KeyDerivationAlgorithmProvider object for the algorithm specified on input.
        var Algorithm = Windows.Security.Cryptography.Core.KeyDerivationAlgorithmProvider.openAlgorithm(algname);

        output += "*** Sample Kdf Algorithm: " + Algorithm.algorithmName + "\n";
        output += "    Secrect Size: " + secret.length + "\n";
        output += "    Target Size: " + targetsize + "\n";

        //Create a key
        var key = Algorithm.createKey(secret);

        //Derive a key from the created key.
        var derived = Windows.Security.Cryptography.Core.CryptographicEngine.deriveKeyMaterial(key, params, targetsize);
        output += "    Derived  " + derived.length + " bytes\n";
        output += "    Derived: " + Windows.Security.Cryptography.CryptographicBuffer.encodeToHexString(derived) + "\n";
        WinJS.log && WinJS.log(output, "sample", "status");
    }

})();
