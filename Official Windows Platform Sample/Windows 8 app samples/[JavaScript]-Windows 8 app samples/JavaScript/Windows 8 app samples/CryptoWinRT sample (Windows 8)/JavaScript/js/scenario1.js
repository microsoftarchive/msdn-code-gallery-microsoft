//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario1.html", {
        ready: function (element, options) {
            document.getElementById("RunSample").addEventListener("click", RunSample, false);
        }
    });

    function RunSample() {
        var buffer;
        Scenario1Output.textContent = "";

        // Initialize example data.
        var base64String = "uiwyeroiugfyqcajkds897945234==";
        var hexString = "30313233";
        var inputString = "Input string";
        var byteString = "123456789";

        // Generate random bytes.
        buffer = Windows.Security.Cryptography.CryptographicBuffer.generateRandom(32);
        Scenario1Output.textContent += "GenerateRandom\n";
        Scenario1Output.textContent += "  Buffer: " + Windows.Security.Cryptography.CryptographicBuffer.encodeToHexString(buffer) + "\n\n";

        // Convert from a byte array.
        buffer = Windows.Security.Cryptography.CryptographicBuffer.convertStringToBinary(inputString, Windows.Security.Cryptography.BinaryStringEncoding.utf8);
        var ByteArray = Windows.Security.Cryptography.CryptographicBuffer.copyToByteArray(buffer);
        buffer = Windows.Security.Cryptography.CryptographicBuffer.createFromByteArray(ByteArray);
        Scenario1Output.textContent += "CreateFromByteArray\n";
        Scenario1Output.textContent += "  Buffer: " + Windows.Security.Cryptography.CryptographicBuffer.encodeToHexString(buffer) + "\n\n";

        // Decode a Base64 encoded string to binary.
        buffer = Windows.Security.Cryptography.CryptographicBuffer.decodeFromBase64String(base64String);
        Scenario1Output.textContent += "DecodeFromBase64String\n";
        Scenario1Output.textContent += "  Base64 String: " + base64String + "\n";
        Scenario1Output.textContent += "  Buffer:        " + Windows.Security.Cryptography.CryptographicBuffer.encodeToHexString(buffer) + "\n\n";

        // Decode a hexadecimal string to binary.
        buffer = Windows.Security.Cryptography.CryptographicBuffer.decodeFromHexString(hexString);
        Scenario1Output.textContent += "DecodeFromHexString\n";
        Scenario1Output.textContent += "  Hex String: " + hexString + "\n";
        Scenario1Output.textContent += "  Buffer:     " + Windows.Security.Cryptography.CryptographicBuffer.encodeToHexString(buffer) + "\n\n";

        // Convert a string to UTF16BE binary data.
        buffer = Windows.Security.Cryptography.CryptographicBuffer.convertStringToBinary(inputString, Windows.Security.Cryptography.BinaryStringEncoding.utf16BE);
        Scenario1Output.textContent += "ConvertStringToBinary (Utf16BE)\n";
        Scenario1Output.textContent += "  String: " + inputString + "\n";
        Scenario1Output.textContent += "  Buffer: " + Windows.Security.Cryptography.CryptographicBuffer.encodeToHexString(buffer) + "\n\n";

        // Convert a string to UTF16LE binary data.
        buffer = Windows.Security.Cryptography.CryptographicBuffer.convertStringToBinary(inputString, Windows.Security.Cryptography.BinaryStringEncoding.utf16LE);
        Scenario1Output.textContent += "ConvertStringToBinary (Utf16LE)\n";
        Scenario1Output.textContent += "  String: " + inputString + "\n";
        Scenario1Output.textContent += "  Buffer: " + Windows.Security.Cryptography.CryptographicBuffer.encodeToHexString(buffer) + "\n\n";

        // Convert a string to UTF8 binary data.
        buffer = Windows.Security.Cryptography.CryptographicBuffer.convertStringToBinary(inputString, Windows.Security.Cryptography.BinaryStringEncoding.utf8);
        Scenario1Output.textContent += "ConvertStringToBinary (Utf8)\n";
        Scenario1Output.textContent += "  String: " + inputString + "\n";
        Scenario1Output.textContent += "  Buffer: " + Windows.Security.Cryptography.CryptographicBuffer.encodeToHexString(buffer) + "\n\n";

        // Decode from a Base64 encoded string.
        buffer = Windows.Security.Cryptography.CryptographicBuffer.decodeFromBase64String(base64String);
        Scenario1Output.textContent += "DecodeFromBase64String \n";
        Scenario1Output.textContent += "  String: " + base64String + "\n";
        Scenario1Output.textContent += "  Buffer (Hex): " + Windows.Security.Cryptography.CryptographicBuffer.encodeToHexString(buffer) + "\n\n";
        Scenario1Output.textContent += "  Buffer (Base64): " + Windows.Security.Cryptography.CryptographicBuffer.encodeToBase64String(buffer) + "\n\n";

        // Decode from a hexadecimal encoded string.
        buffer = Windows.Security.Cryptography.CryptographicBuffer.decodeFromHexString(hexString);
        Scenario1Output.textContent += "DecodeFromHexString \n";
        Scenario1Output.textContent += "  String: " + hexString + "\n";
        Scenario1Output.textContent += "  Buffer: " + Windows.Security.Cryptography.CryptographicBuffer.encodeToHexString(buffer) + "\n\n";
    }

})();
