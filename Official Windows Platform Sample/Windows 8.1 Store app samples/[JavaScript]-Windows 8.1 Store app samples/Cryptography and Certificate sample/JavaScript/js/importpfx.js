//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/importpfx.html", {
        ready: function (element, options) {
            document.getElementById("Browse").addEventListener("click", Browse_Click, false);
            document.getElementById("RunSample").addEventListener("click", RunSample_Click, false);
        }
    });
    var output = "";
    var pfxCertificate="";
    var pfxPassword="";
    

    function RunSample_Click() {
        output = "";
        WinJS.log && WinJS.log("", "sample", "status");
        var storeSelectionCheckbox = document.getElementById("UserStoreCheckBox");
        var pfxPasswordBox = document.getElementById("PfxPasswordBox");
        if (pfxCertificate === null || pfxCertificate === "") {

            output = "Please select a valid PFX file\n";
            WinJS.log && WinJS.log(output, "sample", "error");
            return;

        }

        try {

            output += "\n\n Importing PFX certificate ...";
            WinJS.log && WinJS.log(output, "sample", "status");
            var friendlyName = "test pfx certificate";
            pfxPassword = PfxPasswordBox.value;
            if (storeSelectionCheckbox.checked === true) {
                // target store is Shared User Certificate Store
                // call User Certificate Enrollment function importPfxData to insatll the certificate
                Windows.Security.Cryptography.Certificates.CertificateEnrollmentManager.userCertificateEnrollmentManager.importPfxDataAsync(pfxCertificate,
                    pfxPassword,
                    Windows.Security.Cryptography.Certificates.ExportOption.notExportable,
                    Windows.Security.Cryptography.Certificates.KeyProtectionLevel.noConsent,
                     Windows.Security.Cryptography.Certificates.InstallOptions.none,
                     friendlyName).done(function () {
                         output += "\nCertificate installation succeeded. The certificate is in the User's certificate store";
                         WinJS.log && WinJS.log(output, "sample", "status");
                     },
                      function(e)
                      {
                          WinJS.log && WinJS.log("ImportPfxDataAsync failed, error: " +e.message, "sample", "status");
                     });

            }

            else {
                // target store is app container Store
                // call Certificate Enrollment function importPFXData to install the certificate
                Windows.Security.Cryptography.Certificates.CertificateEnrollmentManager.userCertificateEnrollmentManager.importPfxDataAsync(pfxCertificate,
                    pfxPassword,
                    Windows.Security.Cryptography.Certificates.ExportOption.notExportable,
                     Windows.Security.Cryptography.Certificates.KeyProtectionLevel.noConsent,
                      Windows.Security.Cryptography.Certificates.InstallOptions.none,
                      friendlyName).done(function () {
                          output += "\nCertificate installation succeeded. The certificate is in the App's certificate store";
                          WinJS.log && WinJS.log(output, "sample", "status");
                      },
                      function(e)
                      {
                          WinJS.log && WinJS.log("ImportPfxDataAsync failed, error: " +e.message, "sample", "status");
                      });

            }

        }
        catch (ex) {
            output += "\nCertificate installation failed with error: " + ex.message;
        }
    }

   

    function Browse_Click() {
        WinJS.log && WinJS.log("", "sample", "status");
        //create fileopen picker with filter.pfx
        var filepicker = new Windows.Storage.Pickers.FileOpenPicker();
        filepicker.fileTypeFilter.replaceAll([".pfx"]);
        filepicker.commitButtonText = "Open";

        try {
            filepicker.pickSingleFileAsync().done(function (file) {
                if (file !== null) {
                    //file was picked and iavailable for read
                    //try to read the file content
                    Windows.Storage.FileIO.readBufferAsync(file).done(function (buffer) {
                        var dataReader = Windows.Storage.Streams.DataReader.fromBuffer(buffer);
                        pfxCertificate = Windows.Security.Cryptography.CryptographicBuffer.encodeToBase64String(buffer);
                        output += "Selected PFX file:" + file.path;
                        document.getElementById("PfxFileName").innerText = "File:" + file.name;
                        document.getElementById("passwordlabel").style.visibility = 'visible';
                        document.getElementById("PfxPasswordBox").style.visibility = 'visible';
                        document.getElementById("buttoncolumn").style.visibility = 'visible';
                    });
                   
                }
                
            });
            WinJS.log && WinJS.log(output, "sample", "status");
        }
       
        catch (e) {
            output+="\nPFX file selection failed with error: " + e.message;
            WinJS.log && WinJS.log(output, "sample", "status");
        }

    }
})();
