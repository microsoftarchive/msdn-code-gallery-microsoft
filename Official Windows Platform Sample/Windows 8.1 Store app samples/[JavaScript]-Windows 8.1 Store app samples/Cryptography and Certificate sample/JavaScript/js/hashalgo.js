//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/hashalgo.html", {
        ready: function (element, options) {
            document.getElementById("bHash").addEventListener("click", bHash_Checked, false);
            document.getElementById("bHmac").addEventListener("click", bHmac_Checked, false);
            document.getElementById("RunSample").addEventListener("click", RunSample_Click, false);
        }
    });

    var output = "";
    var digest = "";
    function bHash_Checked() {
        WinJS.log && WinJS.log("", "sample", "status");
        var algoname = document.getElementById("AlgorithmNames");
        algoname.options.length = 0;
        var option = document.createElement("OPTION");
        var algobject = {
            'Md5': Windows.Security.Cryptography.Core.HashAlgorithmNames.md5,
            'Sha1': Windows.Security.Cryptography.Core.HashAlgorithmNames.sha1,
            'Sha256': Windows.Security.Cryptography.Core.HashAlgorithmNames.sha256,
            'Sha384': Windows.Security.Cryptography.Core.HashAlgorithmNames.sha384,
            'Sha512': Windows.Security.Cryptography.Core.HashAlgorithmNames.sha512
        };
        for (var index in algobject) {
            algoname.options[algoname.options.length] = new Option(algobject[index], index);
        }

    }

    function bHmac_Checked() {
        WinJS.log && WinJS.log("", "sample", "status");
        var algoname = document.getElementById("AlgorithmNames");
        algoname.options.length = 0;
        var option = document.createElement("OPTION");
        var algobject = {
            'aesCmac': Windows.Security.Cryptography.Core.MacAlgorithmNames.aesCmac,
            'Hmacmd5': Windows.Security.Cryptography.Core.MacAlgorithmNames.hmacMd5,
            'hmacSha1': Windows.Security.Cryptography.Core.MacAlgorithmNames.hmacSha1,
            'hmacSha256': Windows.Security.Cryptography.Core.MacAlgorithmNames.hmacSha256,
            'hmacSha384': Windows.Security.Cryptography.Core.MacAlgorithmNames.hmacSha384,
            'hmacSha512': Windows.Security.Cryptography.Core.MacAlgorithmNames.hmacSha512

        };
        for (var index in algobject) {
            algoname.options[algoname.options.length] = new Option(algobject[index], index);
        }


    }

    function createHmacCryptographicHash() {
        var algName = AlgorithmNames.options[AlgorithmNames.selectedIndex].innerText;

        // Create a sample message.
        var Message = "Some message to authenticate";
        var vector = Windows.Security.Cryptography.CryptographicBuffer.decodeFromBase64String("uiwyeroiugfyqcajkds897945234==");
        // Created a MacAlgorithmProvider object for the algorithm specified on input.
        var Algorithm = Windows.Security.Cryptography.Core.MacAlgorithmProvider.openAlgorithm(algName);

        output += "\n*** Sample Hash Algorithm: " + Algorithm.algorithmName + "\n";
        output += "    Initial vector:  uiwyeroiugfyqcajkds897945234==\n";

        // Create a key.
        var keymaterial = Windows.Security.Cryptography.CryptographicBuffer.generateRandom(Algorithm.macLength);
        var hash = Algorithm.createHash(keymaterial);
        hash.append(vector);
        digest = hash.getValueAndReset();
        output += "    Hash:  " + Windows.Security.Cryptography.CryptographicBuffer.encodeToHexString(digest) + "\n";
        var reusableHash = Algorithm.createHash(keymaterial);
        reusableHash.append(vector);
       
        // Note that calling GetValue resets the data that has been appended to the
        // CryptographicHash object.
        var digest2 = reusableHash.getValueAndReset();

        if (!Windows.Security.Cryptography.CryptographicBuffer.compare(digest, digest2)) {
            output += "CryptographicHash failed to generate the same hash data!\n";
            WinJS.log && WinJS.log(output, "sample", "error");
            return;
        }

        reusableHash.append(vector);
        digest2 = reusableHash.getValueAndReset();

        if (!Windows.Security.Cryptography.CryptographicBuffer.compare(digest, digest2)) {
            output += "Reusable CryptographicHash failed to generate the same hash data!\n";
            WinJS.log && WinJS.log(output, "sample", "error");
            return;
        }
        
 }
   
    function createHashCryptographicHash() {
        var algName = AlgorithmNames.options[AlgorithmNames.selectedIndex].innerText;

        // Create a HashAlgorithmProvider object.
        var Algorithm = Windows.Security.Cryptography.Core.HashAlgorithmProvider.openAlgorithm(algName);
        var vector = Windows.Security.Cryptography.CryptographicBuffer.decodeFromBase64String("uiwyeroiugfyqcajkds897945234==");

        output += "\n*** Sample Hash Algorithm: " + Algorithm.algorithmName + "\n";
        output += "    Initial vector:  uiwyeroiugfyqcajkds897945234==\n";

        // Compute the hash in one call.
         digest = Algorithm.hashData(vector);

        if (digest.length !== Algorithm.hashLength) {
            output += "HashAlgorithmProvider failed to generate a hash of proper length!\n";
            return;
        }

        output += "    Hash:  " + Windows.Security.Cryptography.CryptographicBuffer.encodeToHexString(digest) + "\n";

        // Use a reusable hash object to hash the data by using multiple calls.
        var reusableHash = Algorithm.createHash();

        reusableHash.append(vector);

        // Note that calling GetValue resets the data that has been appended to the
        // CryptographicHash object.
        var digest2 = reusableHash.getValueAndReset();

        if (!Windows.Security.Cryptography.CryptographicBuffer.compare(digest, digest2)) {
            output += "CryptographicHash failed to generate the same hash data!\n";
            WinJS.log && WinJS.log(output, "sample", "error");
            return;
        }

        reusableHash.append(vector);
        digest2 = reusableHash.getValueAndReset();

        if (!Windows.Security.Cryptography.CryptographicBuffer.compare(digest, digest2)) {
            output += "Reusable CryptographicHash failed to generate the same hash data!\n";
            WinJS.log && WinJS.log(output, "sample", "error");
            return;
        }
    }
    function RunSample_Click() {
        output = "";
        if (document.getElementById("bHmac").checked === true) {
            createHmacCryptographicHash();
        }
        else
        {
            createHashCryptographicHash();
        }

        WinJS.log && WinJS.log(output, "sample", "status");
    }

})();
