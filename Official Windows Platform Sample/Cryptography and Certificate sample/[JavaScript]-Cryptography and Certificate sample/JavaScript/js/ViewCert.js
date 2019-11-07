//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/ViewCert.html", {
        ready: function (element, options) {
           document.getElementById("RunSample").addEventListener("click", RunSample_Click, false);
           var verifycert = document.getElementById("VerifyCert");
           verifycert.options.length = 0;
            var verifyobject = {
                'verifycert': 'Verify Certificate',
                'signverify': 'Sign/Verify using certificate key',
                'signverifycms': 'Sign/Verify using CMS based format',
                'getcertdetails': 'Get certificate and show details'
            };
            WinJS.log && WinJS.log("", "sample", "status");
            for (var index in verifyobject) {
                verifycert.options[verifycert.options.length] = new Option(verifyobject[index], index);
            }
            var certlist = document.getElementById("CertificateList");
            certlist.options.length = 0;
            var certobject;
            Windows.Security.Cryptography.Certificates.CertificateStores.findAllAsync().done(function (cert) {
                certinfo=cert;
                for (var i = 0; i < cert.length; i++) {
                    certlist.options[certlist.options.length]=new Option(cert[i].subject,i);
                    }
                if (certlist.options.length === 0) {

                    output = "No certificates found.\n";
                    WinJS.log && WinJS.log(output, "sample", "status");
                    document.getElementById("VerifyCert").disabled = true;
                    document.getElementById("RunSample").disabled = true;
                }
                else {
                    document.getElementById("VerifyCert").disabled = false;
                    document.getElementById("RunSample").disabled = false;
                }
                
            });
            
        }
    });
    var output = "";
    var  certinfo="";

    function RunSample_Click() {
        WinJS.log && WinJS.log("", "sample", "status");
        output = "";
        var verifycert = document.getElementById("VerifyCert");
        var verifyselection = verifycert.options[verifycert.selectedIndex].text;
        var certlist = document.getElementById("CertificateList");
        var selectedCertificate;
        //get the selected certificate
        if (certlist.selectedIndex >= 0 && certlist.selectedIndex < certinfo.length) {
            selectedCertificate = certinfo[certlist.selectedIndex];
        }
        if (selectedCertificate === null) {
            output += "Please select a certificate first.";
            WinJS.log && WinJS.log(output, "sample", "error");
            return;
        }

        //a certificate was selected, do the desired operation
        if (verifyselection === "Verify Certificate") {
            output = "";
            //Build the chain

            selectedCertificate.buildChainAsync(null, null).done(function (chain) {
                var result = chain.validate();
                if (result !== Windows.Security.Cryptography.Certificates.ChainValidationResult.success) {
                    output += "\n Certificate validation failed ";
                    WinJS.log && WinJS.log(output, "sample", "error");
                }
                else {
                    output += "\n Certificate validation passed!";
                    WinJS.log && WinJS.log(output, "sample", "status");
                }
            });
        }

        else if (verifyselection === "Sign/Verify using certificate key") {

            output = "";
            //get the private key
            Windows.Security.Cryptography.Core.PersistedKeyProvider.openKeyPairFromCertificateAsync(selectedCertificate,
                                                                                                    Windows.Security.Cryptography.Core.HashAlgorithmNames.sha1,
                                                                                                    Windows.Security.Cryptography.Core.CryptographicPadding.rsaPkcs1V15).done(function (keyPair) {
            var cookie = "Some data to sign";
            var data = Windows.Security.Cryptography.CryptographicBuffer.convertStringToBinary(cookie, Windows.Security.Cryptography.BinaryStringEncoding.utf16BE);
              try {
                    //sign the data by using the key
                    var signed = Windows.Security.Cryptography.Core.CryptographicEngine.sign(keyPair, data);
                    var bresult = Windows.Security.Cryptography.Core.CryptographicEngine.verifySignature(keyPair, data, signed);
                    if (bresult === true) {

                        output += "\n Successfully signed and verified signature.";
                        WinJS.log && WinJS.log(output, "sample", "status");
                     }
                    else {

                        output += "\nVerify Signature failed.";
                        WinJS.log && WinJS.log(output, "sample", "error");
                      }
                   }

              catch (e) {

                     output += "\nVerification Failed. Exception Occurred:" + e.message;
                     WinJS.log && WinJS.log(output, "sample", "error");
                   }
           });

        }

        else if (verifyselection === "Sign/Verify using CMS based format") {
           
            var pdfInputstream;
            var originalData = new Windows.Storage.Streams.InMemoryRandomAccessStream();
            //Populate the new memory
            pdfInputstream = originalData.getInputStreamAt(0);

            var signer = new Windows.Security.Cryptography.Certificates.CmsSignerInfo();
            signer.certificate = selectedCertificate;
            signer.hashAlgorithmName = Windows.Security.Cryptography.Core.HashAlgorithmNames.sha1;
            try {
                Windows.Security.Cryptography.Certificates.CmsDetachedSignature.generateSignatureAsync(pdfInputstream, signer, null).done(function (signature) {
                    var cmsSignedData = new Windows.Security.Cryptography.Certificates.CmsDetachedSignature(signature);
                    pdfInputstream = originalData.getInputStreamAt(0);
                    cmsSignedData.verifySignatureAsync(pdfInputstream).done(function (validationResult) {
                        if (validationResult === Windows.Security.Cryptography.Certificates.SignatureValidationResult.success) {
                            output += "Successfully signed and verified Signature";
                            WinJS.log && WinJS.log(output, "sample", "status");
                        }
                        else {
                            output += "Verify Signature using CMS based format failed.";
                            WinJS.log && WinJS.log(output, "sample", "error");
                        }
                    });
                });
            }
            catch (e) {
                output += "Verification Failed.Exception Occurred :" + e.message;
                WinJS.log && WinJS.log(output, "sample", "error");
            }
        }
        else if(verifyselection === "Get certificate and show details"){

            output = "";
            output += "\n Certificate Selected:";
            output += "\n";
            output += "\n Subject : " + selectedCertificate.subject;
            output += "\n Issuer : " + selectedCertificate.issuer;
            output += "\n Friendly Name : " + selectedCertificate.friendlyName;
            output+="\n Thumbprint : "+ Windows.Security.Cryptography.CryptographicBuffer.encodeToHexString(Windows.Security.Cryptography.CryptographicBuffer.createFromByteArray(selectedCertificate.getHashValue()));
            output += "\n Serial Number : " + Windows.Security.Cryptography.CryptographicBuffer.encodeToHexString(Windows.Security.Cryptography.CryptographicBuffer.createFromByteArray(selectedCertificate.serialNumber));
            output += "\n Valid From : " + selectedCertificate.validFrom;
            output += "\n Validto : " + selectedCertificate.validTo;
            WinJS.log && WinJS.log(output, "sample", "status");

        }
           

    }
})();
