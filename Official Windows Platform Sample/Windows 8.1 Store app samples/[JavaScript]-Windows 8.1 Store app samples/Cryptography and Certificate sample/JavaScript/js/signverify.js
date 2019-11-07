//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/signverify.html", {
        ready: function (element, options) {
            document.getElementById("RunSample").addEventListener("click", RunSample_Click, false);
            document.getElementById("bAsymmetric").addEventListener("click", bAsymmetric_Checked, false);
            document.getElementById("bHmac").addEventListener("click", bHmac_Checked, false);
        }
    });
    var output = "";
  

    function RunSample_Click() {
        WinJS.log && WinJS.log("", "sample", "status");
        output = "";
        var cookie = "Some Data to sign";
        var data = Windows.Security.Cryptography.CryptographicBuffer.convertStringToBinary(cookie, Windows.Security.Cryptography.BinaryStringEncoding.utf16BE);
        var key = null;
  
        if (document.getElementById("bHmac").checked === true) {
            key = generateHMACKey();
        }
        else {
            key = generateAsymmetricKey();
        }

        if (key !== null) {
            //Sign the data using the generated key.
            var signature = Windows.Security.Cryptography.Core.CryptographicEngine.sign(key, data);
            output += "    Data was successfully signed.\n";

            //Verify the signature by using the public key.
            if (!Windows.Security.Cryptography.Core.CryptographicEngine.verifySignature(key, data, signature)) {
                output += "Signature verification failed!";
                WinJS.log && WinJS.log(output, "sample", "error");
                return;
            }
            else {
                output += "    Signature was successfully verified.\n";
                WinJS.log && WinJS.log(output, "sample", "status");
            }
        }
    }

    function bHmac_Checked() {
        WinJS.log && WinJS.log("", "sample", "status");
        var selectalgo = document.getElementById("AlgorithmNames");
        selectalgo.options.length = 0;
        var algobject = {
            'AesCmac': Windows.Security.Cryptography.Core.MacAlgorithmNames.aesCmac,
            'HmacMd5': Windows.Security.Cryptography.Core.MacAlgorithmNames.hmacMd5,
            'hmacsha1': Windows.Security.Cryptography.Core.MacAlgorithmNames.hmacSha1,
            'hmacsha256': Windows.Security.Cryptography.Core.MacAlgorithmNames.hmacSha256,
            'hmacsha384': Windows.Security.Cryptography.Core.MacAlgorithmNames.hmacSha384,
            'hmacsha512':Windows.Security.Cryptography.Core.MacAlgorithmNames.hmacSha512
        };
        for (var index in algobject) {
            selectalgo.options[selectalgo.options.length] = new Option(algobject[index], index);
        }

        document.getElementById("KeySizes").disabled = true;
    }

    function bAsymmetric_Checked() {

        WinJS.log && WinJS.log("", "sample", "status");
        var selectalgo = document.getElementById("AlgorithmNames");
        selectalgo.options.length = 0;
        var algobject = {
            'ecdsaP256Sha256': Windows.Security.Cryptography.Core.AsymmetricAlgorithmNames.ecdsaP256Sha256,
            'ecdsaP384Sha384': Windows.Security.Cryptography.Core.AsymmetricAlgorithmNames.ecdsaP384Sha384,
            'ecdsaP521Sha512': Windows.Security.Cryptography.Core.AsymmetricAlgorithmNames.ecdsaP521Sha512,
            'rsaSignPkcs1Sha1': Windows.Security.Cryptography.Core.AsymmetricAlgorithmNames.rsaSignPkcs1Sha1,
            'rsaSignPkcs1Sha256': Windows.Security.Cryptography.Core.AsymmetricAlgorithmNames.rsaSignPkcs1Sha256,
            'rsaSignPkcs1Sha384': Windows.Security.Cryptography.Core.AsymmetricAlgorithmNames.rsaSignPkcs1Sha384,
            'rsaSignPkcs1Sha512': Windows.Security.Cryptography.Core.AsymmetricAlgorithmNames.rsaSignPkcs1Sha512,
            'dsaSha1':Windows.Security.Cryptography.Core.AsymmetricAlgorithmNames.dsaSha1
        };
        for (var index in algobject) {
            selectalgo.options[selectalgo.options.length] = new Option(algobject[index], index);
        }
    }

    function generateHMACKey() {
        WinJS.log && WinJS.log("", "sample", "status");
        var selectalgo = document.getElementById("AlgorithmNames");
        var algname = selectalgo.options[selectalgo.selectedIndex].text;
        var algorithm = Windows.Security.Cryptography.Core.MacAlgorithmProvider.openAlgorithm(algname);
        output += "*** Sample Hmac Algorithm: " + algorithm.algorithmName + "\n";

        //Create a key
        var keymaterial = Windows.Security.Cryptography.CryptographicBuffer.generateRandom(algorithm.macLength);
        WinJS.log && WinJS.log(output, "sample", "status");
        return algorithm.createKey(keymaterial);

    }

    function generateAsymmetricKey() {
        WinJS.log && WinJS.log("", "sample", "status");
        var selectalgo = document.getElementById("AlgorithmNames");
        var algname = selectalgo.options[selectalgo.selectedIndex].text;
        var keyselect = document.getElementById("KeySizes");
        var keysize = parseInt(keyselect.options[keyselect.selectedIndex].text);
        var keypair;
        //Create an AsymmetricAlgorithmProvider object for the algorithm specified on input.
        var algorithm = Windows.Security.Cryptography.Core.AsymmetricKeyAlgorithmProvider.openAlgorithm(algname);
        output += "*** Sample Signature Algorithm\n";
        output += "    Algorithm Name: " + algorithm.algorithmName + "\n";
        output += "    Key Size: " + keysize + "\n";

        //Generate a key pair.
        try {

            keypair = algorithm.createKeyPair(keysize);
        }
        catch (ex) {

            output += ex.message + "\n";
            output += "An invalid key size was specified for the given algorithm.";
            WinJS.log && WinJS.log(output, "sample", "error");
            return;
        }

        WinJS.log && WinJS.log(output, "sample", "status");
        return keypair;

    }
})();
