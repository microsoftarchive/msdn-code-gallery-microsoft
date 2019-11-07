//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/encryptdecrypt.html", {
        ready: function (element, options) {
            document.getElementById("RunEncryption").addEventListener("click", runencryption_click, false);
            document.getElementById("RunDataProtection").addEventListener("click", rundataprotection_click, false);
            document.getElementById("bEncryption").addEventListener("click", bEncryption_Checked, false);
            document.getElementById("bDataProtection").addEventListener("click", bDataProtection_Checked, false);
            document.getElementById("bSymAlgs").addEventListener("click", bSymAlgs_Checked, false);
            document.getElementById("bAuthEncrypt").addEventListener("click", bAuthEncrypt_Checked, false);
            document.getElementById("bAsymAlgs").addEventListener("click", bAsymAlgs_Checked, false);
            document.getElementById("spDataProtection").style.visibility = 'hidden';
          
        }
    });

    var output="";
   
    function bEncryption_Checked() {
        WinJS.log && WinJS.log("", "sample", "status");
        document.getElementById("spEncryption").style.visibility = 'visible';
        document.getElementById("spDataProtection").style.visibility = 'hidden';
        document.getElementById("encryptdata").style.display = 'block';
        document.getElementById("protectdata").style.display = 'none';

    }

    function bDataProtection_Checked() {
        WinJS.log && WinJS.log("", "sample", "status");
        document.getElementById("spEncryption").style.visibility = 'hidden';
        document.getElementById("encryptdata").style.display = 'none';
        document.getElementById("protectdata").style.alignSelf = 'left';
        document.getElementById("protectdata").style.display = 'block';
        document.getElementById("spDataProtection").style.visibility = 'visible';
        
    }
   
    function bSymAlgs_Checked() {
        WinJS.log && WinJS.log("", "sample", "status");
        var selectalgo = document.getElementById("AlgorithmNames");
        selectalgo.options.length = 0;
        var option = document.createElement("OPTION");
        var algobject = {
            'aesCbc': Windows.Security.Cryptography.Core.SymmetricAlgorithmNames.aesCbc,
            'aesCbcPkcs7': Windows.Security.Cryptography.Core.SymmetricAlgorithmNames.aesCbcPkcs7,
            'aesEcb': Windows.Security.Cryptography.Core.SymmetricAlgorithmNames.aesEcb,
            'aesEcbPkcs7': Windows.Security.Cryptography.Core.SymmetricAlgorithmNames.aesEcbPkcs7,
            'desCbc': Windows.Security.Cryptography.Core.SymmetricAlgorithmNames.desCbc,
            'desCbcPkcs7': Windows.Security.Cryptography.Core.SymmetricAlgorithmNames.desCbcPkcs7,
            'desEcb': Windows.Security.Cryptography.Core.SymmetricAlgorithmNames.desEcb,
            'desEcbPkcs7': Windows.Security.Cryptography.Core.SymmetricAlgorithmNames.desEcbPkcs7,
            'rc2Cbc': Windows.Security.Cryptography.Core.SymmetricAlgorithmNames.rc2Cbc,
            'rc2CbcPkcs7': Windows.Security.Cryptography.Core.SymmetricAlgorithmNames.rc2CbcPkcs7,
            'rc2Ecb': Windows.Security.Cryptography.Core.SymmetricAlgorithmNames.rc2Ecb,
            'rc2EcbPkcs7': Windows.Security.Cryptography.Core.SymmetricAlgorithmNames.rc2EcbPkcs7,
            'rc4': Windows.Security.Cryptography.Core.SymmetricAlgorithmNames.rc4,
            'tripleDesCbc': Windows.Security.Cryptography.Core.SymmetricAlgorithmNames.tripleDesCbc,
            'tripleDesCbcPkcs7': Windows.Security.Cryptography.Core.SymmetricAlgorithmNames.tripleDesCbcPkcs7,
            'tripleDesEcb': Windows.Security.Cryptography.Core.SymmetricAlgorithmNames.tripleDesEcb,
            'tripleDesEcbPkcs7': Windows.Security.Cryptography.Core.SymmetricAlgorithmNames.tripleDesEcbPkcs7
        };
        for (var index in algobject) {
            selectalgo.options[selectalgo.options.length] = new Option(algobject[index], index);
        }
        
      
    }
    
    function bAuthEncrypt_Checked() {
        WinJS.log && WinJS.log("", "sample", "status");
        var selectalgo = document.getElementById("AlgorithmNames");
        selectalgo.options.length = 0;
        var algobject = {
            'aesCcm': Windows.Security.Cryptography.Core.SymmetricAlgorithmNames.aesCcm,
            'aesGcm': Windows.Security.Cryptography.Core.SymmetricAlgorithmNames.aesGcm
        };
        for (var index in algobject) {
            selectalgo.options[selectalgo.options.length] = new Option(algobject[index], index);
        }
    }

    function bAsymAlgs_Checked() {
        WinJS.log && WinJS.log("", "sample", "status");
        var selectalgo = document.getElementById("AlgorithmNames");
        selectalgo.options.length = 0;
        var algobject={
            'rsaPkcs1': Windows.Security.Cryptography.Core.AsymmetricAlgorithmNames.rsaPkcs1,
            'rsaOaepSha1': Windows.Security.Cryptography.Core.AsymmetricAlgorithmNames.rsaOaepSha1,
            'rsaOaepSha256': Windows.Security.Cryptography.Core.AsymmetricAlgorithmNames.rsaOaepSha256,
            'rsaOaepSha384': Windows.Security.Cryptography.Core.AsymmetricAlgorithmNames.rsaOaepSha384,
            'rsaOaepSha512': Windows.Security.Cryptography.Core.AsymmetricAlgorithmNames.rsaOaepSha512
        };
        for(var index in algobject){
            selectalgo.options[selectalgo.options.length]=new Option(algobject[index],index);
        }
  }

    function generateSymmetricKey() {

        var selectalgo = document.getElementById("AlgorithmNames");
        var algname = selectalgo.options[selectalgo.selectedIndex].text;
        var keylength=document.getElementById("KeySizes");
        var keySize = parseInt (keylength.options[keylength.selectedIndex].text);
        
        var key;
        // Create an SymmetricKeyAlgorithmProvider object for the algorithm specified on input.
        var Algorithm =Windows.Security.Cryptography.Core.SymmetricKeyAlgorithmProvider.openAlgorithm(algname);
       
        output+= "*** Sample Encryption Algorithm\n";
        output += "    Algorithm Name: " + Algorithm.algorithmName + "\n";
        output += "    Key Size: " + keySize + "\n";
        output += "    Block length: " + Algorithm.blockLength + "\n";
        WinJS.log && WinJS.log(output, "sample", "status");
        // Generate a symmetric key.
        var keymaterial = Windows.Security.Cryptography.CryptographicBuffer.generateRandom((keySize + 7) / 8);
        try
        {
            key = Algorithm.createSymmetricKey(keymaterial);
        }
        catch (ex)
        {
            output+= ex.Message + "\n";
            output += "An invalid key size was specified for the given algorithm.";
            WinJS.log && WinJS.log(output, "sample", "error");
            return null;
        }
        return key;

    }

    function generateAsymmetricKey() {

        var selectalgo = document.getElementById("AlgorithmNames");
        var algname = selectalgo.options[selectalgo.selectedIndex].text;
        var keylength = document.getElementById("KeySizes");
        var keySize = parseInt(keylength.options[keylength.selectedIndex].text);
        var keyPair;
        // Create an AsymmetricKeyAlgorithmProvider object for the algorithm specified on input.
        var Algorithm =Windows.Security.Cryptography.Core.AsymmetricKeyAlgorithmProvider.openAlgorithm(algname);

        output+= "*** Sample Encryption Algorithm\n";
        output += "    Algorithm Name: " + Algorithm.algorithmName + "\n";
        output += "    Key Size: " + keySize + "\n";
        WinJS.log && WinJS.log(output, "sample", "status");
        // Generate a key pair.
        try {
            keyPair = Algorithm.createKeyPair(keySize);
        }
        catch (ex)
        {
            output+= ex.Message + "\n";
            output += "An invalid key size was specified for the given algorithm.";
            WinJS.log && WinJS.log(output, "sample", "error");
            return null;
        }
        return keyPair;
    }

    function generateData() {
        var data;
        var cookie = "Data to encrypt ";
        if (document.getElementById("bSymAlgs").checked === true) {
            data = Windows.Security.Cryptography.CryptographicBuffer.convertStringToBinary(cookie, Windows.Security.Cryptography.BinaryStringEncoding.utf8);
        }
        else {
            switch (document.getElementById("AlgorithmNames").selectedIndex) {
                case 0:
                    data = Windows.Security.Cryptography.CryptographicBuffer.convertStringToBinary(cookie, Windows.Security.Cryptography.BinaryStringEncoding.Utf16LE);
                    break;

                    // OAEP Padding depends on key size, message length and hash block length
                    // 
                    // The maximum plaintext length is KeyLength - 2*HashBlock - 2
                    //
                    // OEAP padding supports an optional label with the length is restricted by plaintext/key/hash sizes.
                    // Here we just use a small label.
                case 1:
                    data = Windows.Security.Cryptography.CryptographicBuffer.generateRandom(1024 / 8 - 2 * 20 - 2);
                    break;
                case 2:
                    data = Windows.Security.Cryptography.CryptographicBuffer.generateRandom(1024 / 8 - 2 * (256 / 8) - 2);
                    break;
                case 3:
                    data = Windows.Security.Cryptography.CryptographicBuffer.generateRandom(2048 / 8 - 2 * (384 / 8) - 2);
                    break;
                case 4:
                    data = Windows.Security.Cryptography.CryptographicBuffer.generateRandom(2048 / 8 - 2 * (512 / 8) - 2);
                    break;
                default:
                    WinJS.log && WinJS.log("An invalid algorithm was selected", "sample", "error");
                    return null;
            }

        }
        return data;
    }
    //This utility function returns a nonce value for authenticated encryption modes
    function getNonce() {

        // NOTE: 
        // 
        // The security best practises require that the Encrypt operation
        // not be called more than once with the same nonce for the same key.
        // 
        // Nonce can be predictable, but must be unique per secure session.

        var carry = 1;
         var NonceBytes =new Array(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 );

        for (var i = 0; i < NonceBytes.Length && carry === 1; i++)
        {
            if (NonceBytes[i] === 255)
            {
                NonceBytes[i] = 0;
            }
            else
            {
                NonceBytes[i]++;
                carry = 0;
            }
        }

        return Windows.Security.Cryptography.CryptographicBuffer.createFromByteArray(NonceBytes);
    }
    //This is a click handler for 'RunSample' button.It is responsible for executing the sample code.
    function runencryption_click() {
        output = "";
        var selectalgo = document.getElementById("AlgorithmNames");
        var algname = selectalgo.options[selectalgo.selectedIndex].text;
        var key = null;
        var data;
        var nonce;
        var decrypted;
        var encrypted = null;
        var iv = null;
        if (document.getElementById("bSymAlgs").checked === true || document.getElementById("bAuthEncrypt").checked === true) {
            key = generateSymmetricKey();
        }
        else {
            key = generateAsymmetricKey();
        }

        data = generateData();
        if (document.getElementById("bAuthEncrypt").checked === true) {
            nonce = getNonce();
            var encrypteddata = Windows.Security.Cryptography.Core.CryptographicEngine.encryptAndAuthenticate(key, data, nonce, null);
            output += "    Plain text: " + data.length + " bytes\n";
            output += "    Encrypted: " + encrypteddata.encryptedData.length + " bytes\n";
            output += "    AuthTag: " + encrypteddata.authenticationTag.length + " bytes\n";

            decrypted = Windows.Security.Cryptography.Core.CryptographicEngine.decryptAndAuthenticate(key, encrypteddata.encryptedData, nonce, encrypteddata.authenticationTag, null);

            if (!Windows.Security.Cryptography.CryptographicBuffer.compare(decrypted, data)) {
                WinJS.log && WinJS.log("Decrypted does not match original!", "sample", "error");
                return;
            }
           
        }
        else {
            // CBC mode needs Initialization vector, here just random data.
            // IV property will be set on "Encrypted".
            if (algname.indexOf("CBC")!==-1)
            {
                var algorithm = Windows.Security.Cryptography.Core.SymmetricKeyAlgorithmProvider.openAlgorithm(algname);
                iv = Windows.Security.Cryptography.CryptographicBuffer.generateRandom(algorithm.blockLength);
            }

            // Encrypt the data.
            try
            {
                encrypted = Windows.Security.Cryptography.Core.CryptographicEngine.encrypt(key,data,iv);
                    
            }
            catch (ex)
            {
                output+= ex.Message + "\n";
                output += "An invalid key size was selected for the given algorithm.\n";
                WinJS.log && WinJS.log(output, "sample", "error");
                return;
            }

            output += "    Plain text: " + data.length + " bytes\n";
            output += "    Encrypted: " + encrypted.length + " bytes\n";
            WinJS.log && WinJS.log(output, "sample", "status");
            // Decrypt the data.
            decrypted = Windows.Security.Cryptography.Core.CryptographicEngine.decrypt(key, encrypted, iv);

            if (!Windows.Security.Cryptography.CryptographicBuffer.compare(decrypted, data))
            {
                output += "Decrypted data does not match original!";
                WinJS.log && WinJS.log(output, "sample", "error");
                return;
            }
        }
        WinJS.log && WinJS.log(output, "sample", "status");
    }
    //The descriptor string used to protect the data
    function sampleDataProtection(descriptor) {

        output += "*** Sample Data Protection for " + descriptor + " ***\n";
        var Provider = new Windows.Security.Cryptography.DataProtection.DataProtectionProvider(descriptor);
        output += "    DataProtectionProvider is Ready\n";

        //Create random data for protection

        var data = Windows.Security.Cryptography.CryptographicBuffer.generateRandom(73);
        output += "    Original Data: " + Windows.Security.Cryptography.CryptographicBuffer.encodeToHexString(data) + "\n";
        try {
            //Protect the random data
            Provider.protectAsync(data).done(function (protectedData) {
                output += "    Protected Data: " + Windows.Security.Cryptography.CryptographicBuffer.encodeToHexString(protectedData) + "\n";


                if (Windows.Security.Cryptography.CryptographicBuffer.compare(data, protectedData)) {

                    output += "ProtectAsync returned unprotected data";
                    WinJS.log && WinJS.log(output, "sample", "error");
                    return;
                }

                output += "    ProtectAsync succeeded\n";
                WinJS.log && WinJS.log(output, "sample", "status");

                //Unprotect

                var Provider2 = new Windows.Security.Cryptography.DataProtection.DataProtectionProvider();
                Provider2.unprotectAsync(protectedData).done(function (unprotectedData) {

                    if (!Windows.Security.Cryptography.CryptographicBuffer.compare(data, unprotectedData)) {

                        output += "UnprotectAsync returned invalid data";
                        WinJS.log && WinJS.log(output, "sample", "error");
                        return;
                    }
                    output += "    Unprotected Data: " + Windows.Security.Cryptography.CryptographicBuffer.encodeToHexString(unprotectedData) + "\n";
                    output += "*** Done!\n";
                    WinJS.log && WinJS.log(output, "sample", "status");
                });
            });
        }
        catch (ex) {
            output += "Function TestProtect failed :" + ex.message;
            WinJS.log && WinJS.log(output, "sample", "error");
            return;
        }
    }

    function sampleDataProtectionStream(descriptor) {

        output += "*** Sample Stream Data Protection for " + descriptor + " ***\n";

        var data = Windows.Security.Cryptography.CryptographicBuffer.generateRandom(10000);
        var reader1;
        var reader2;
        var buff1;
        var buff2;

        var Provider = new Windows.Security.Cryptography.DataProtection.DataProtectionProvider(descriptor);
        var originalData = new Windows.Storage.Streams.InMemoryRandomAccessStream();

        //Populate the new memory stream
        var outputStream = originalData.getOutputStreamAt(0);
        var writer = new Windows.Storage.Streams.DataWriter(outputStream);
        writer.writeBuffer(data);
        writer.storeAsync().done(function () {
            outputStream.flushAsync().done(function () {
                writer.close();
                //open new memory stream for read
                var source = originalData.getInputStreamAt(0);

                //Open the output memory stream
                var protectedData = new Windows.Storage.Streams.InMemoryRandomAccessStream();
                var dest = protectedData.getOutputStreamAt(0);

                // Protect
                Provider.protectStreamAsync(source, dest).done(function () {
                    //Flush the output
                    dest.flushAsync().done(function () {
                        output += "    Protected output was successfully flushed\n";

                        //Verify the protected data does not match the original
                        reader1 = new Windows.Storage.Streams.DataReader(originalData.getInputStreamAt(0));
                        reader2 = new Windows.Storage.Streams.DataReader(protectedData.getInputStreamAt(0));

                        reader1.loadAsync(originalData.size).done(function () {
                            reader2.loadAsync(protectedData.size).done(function () {
                                output += "    Size of original stream:  " + originalData.size + "\n";
                                output += "    Size of protected stream:  " + protectedData.size + "\n";

                                if (originalData.size === protectedData.size) {
                                    buff1 = reader1.readBuffer(originalData.size);
                                    buff2 = reader2.readBuffer(protectedData.size);
                                    if (CryptographicBuffer.compare(buff1, buff2)) {
                                        output += "ProtectStreamAsync returned unprotected data";
                                        WinJS.log && WinJS.log(output, "sample", "error");
                                        return;
                                    }
                                }

                                output += "    Stream Compare completed.  Streams did not match.\n";

                                source = protectedData.getInputStreamAt(0);

                                var unprotectedData = new Windows.Storage.Streams.InMemoryRandomAccessStream();
                                dest = unprotectedData.getOutputStreamAt(0);

                                // Unprotect
                                var Provider2 = new Windows.Security.Cryptography.DataProtection.DataProtectionProvider();
                                Provider2.unprotectStreamAsync(source, dest).done(function () {
                                    dest.flushAsync().done(function () {
                                        output += "    Unprotected output was successfully flushed\n";

                                        //Verify the unprotected data does match the original
                                        reader1 = new Windows.Storage.Streams.DataReader(originalData.getInputStreamAt(0));
                                        reader2 = new Windows.Storage.Streams.DataReader(unprotectedData.getInputStreamAt(0));

                                        reader1.loadAsync(originalData.size).done(function () {
                                            reader2.loadAsync(unprotectedData.size).done(function () {

                                                output += "    Size of original stream:  " + originalData.size + "\n";
                                                output += "    Size of unprotected stream:  " + unprotectedData.size + "\n";

                                                buff1 = reader1.readBuffer(originalData.size);
                                                buff2 = reader2.readBuffer(unprotectedData.size);
                                                if (!Windows.Security.Cryptography.CryptographicBuffer.compare(buff1, buff2)) {
                                                    output += "UnrotectStreamAsync did not return expected data";
                                                    WinJS.log && WinJS.log(output, "sample", "error");
                                                    return;
                                                }

                                                originalData.close();
                                                protectedData.close();
                                                unprotectedData.close();
                                                reader1.close();
                                                reader2.close();

                                                output += "*** Done!\n";
                                                WinJS.log && WinJS.log(output, "sample", "status");
                                            }, function () {
                                                originalData.close();
                                                protectedData.close();
                                                unprotectedData.close();
                                                reader1.close();
                                                reader2.close();
                                            });
                                        }, function () {
                                            originalData.close();
                                            protectedData.close();
                                            unprotectedData.close();
                                            reader1.close();
                                            reader2.close();
                                        });
                                    }, function () {
                                        originalData.close();
                                        protectedData.close();
                                        unprotectedData.close();
                                        reader1.close();
                                        reader2.close();
                                    });
                                }, function () {
                                    originalData.close();
                                    protectedData.close();
                                    unprotectedData.close();
                                    reader1.close();
                                    reader2.close();
                                });
                            }, function () {
                                originalData.close();
                                protectedData.close();
                                reader1.close();
                                reader2.close();
                            });
                        }, function () {
                            originalData.close();
                            protectedData.close();
                            reader1.close();
                            reader2.close();
                        });
                    }, function () {
                        originalData.close();
                        protectedData.close();
                    });
                }, function () {
                    originalData.close();
                    protectedData.close();
                });
            }, function () {
                originalData.close();
            });
        }, function () {
            originalData.close();
            writer.close();
        });
    }


      
    function rundataprotection_click() {

        output = "";
        var descriptor = document.getElementById("tbDescriptor").value;

        if (document.getElementById("bFixedInput").checked === true) {
            sampleDataProtection(descriptor);
        }
        if (document.getElementById("bStreamInput").checked === true) {
            sampleDataProtectionStream(descriptor);
        }
    }
})();
