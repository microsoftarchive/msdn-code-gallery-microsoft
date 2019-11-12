//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved



(function () {
    'use strict';
    var username;

    function ready(elements, options) {
        var /*@override*/ item = options && options.item ? options.item : getItem();
        //elements.querySelector('.pageTitle').textContent = item.group.title;

        WinJS.UI.processAll(elements)
            .then(function () {
                loadCredential();
                elements.querySelector('.title').textContent = item.title;
                elements.querySelector('.submitButton').addEventListener('click', enrollment, false);
                elements.querySelector('.customer').textContent = username;
                elements.querySelector('.image').style.backgroundColor = item.backgroundColor;
        });
    }

    // The getItem() function contains sample data.
    // TODO: Replace with custom data.
    function getItem() {
        var group = {
            key: 'group0',
            title: 'Collection title lorem 0'
        };

        return {
            group: group,
            title: 'testing',
            subtitle: 'sub testing',
            backgroundColor: 'rgba(142, 213, 87, 1)',
            content: (new Array(16)).join('<p>default</p>'),
        };
    }

    WinJS.UI.Pages.define('/html/enrollment.html', {
        ready: ready
    });

    // certificate enrollment
    // onClick() handler for the 'submitButton'
    /*@override*/
    function enrollment() {
        try {
            var enroll = document.getElementById("enroll").checked;
            var pfx = document.getElementById("pfx").checked;
     
            if (enroll === true) {
                if (username) {
                    if (pfx === true) {
                        doPFXEnrollment(username);
                    } else {
                        doPKCS10Enrollment(username);
                    };
                }
            }else {
                var /*@override*/ item = {
                    title: "Account Page",
                    content: "<strong>(Please upgrade to strong authentication if you need full access to your account.)",
                    backgroundColor: 'rgba(25,50,200,1)',
                    navigate: "groupeditemsPage"
                };
                WinJS.Navigation.navigate('/html/account.html', { item: item });
            }

        } catch (err) {
            var message = '';
            for (var f in err) {
                message += f;
                message += ':';
                message += err[f];
                message += ' ';
            }
            goError(message);
            return false;
        };

        return false;
    }

    // pfx certificate enrollment
    function doPFXEnrollment(user) {

        var exportable = Windows.Security.Cryptography.Certificates.ExportOption.exportable;
        var consent = Windows.Security.Cryptography.Certificates.KeyProtectionLevel.noConsent;
        var installOption = 0; //Windows.Security.Cryptography.Certificates.InstallOptions.none;
        var params = "username=" + user;

        WinJS.xhr({
            type: "POST",
            url: "Your URL",    //Please provide the server url here. For example:
                                //url: "https://WoodGrove-Bank/bankserver2/enrollment/getPFX",
            headers: { "Content-type": "application/x-www-form-urlencoded" },
            data: params
        }).done(
            function (request) {
                var obtainedData = window.JSON.parse(request.responseText);
                try {
                    // import pfx certificate file into the app container
                    Windows.Security.Cryptography.Certificates.CertificateEnrollmentManager.importPfxDataAsync(obtainedData.pfx, obtainedData.password, exportable, consent, installOption, obtainedData.friendlyName).done(
                        function () {
                    // set certificate enrollment mark to aviod redundant certificate enrollments
                    Windows.Storage.ApplicationData.current.localSettings.values["EnrollCertificate"] = true;
                    var /*@override*/ item = {
                        title: "Account Page",
                        content: "<strong>(Please sign out and re-launch the application so as to use your enrolled certificate.)</strong>",
                        backgroundColor: 'rgba(191, 84, 46, 1)',
                        navigate: "sign-out"
                    };
                    WinJS.Navigation.navigate('/html/account.html', { item: item });
                    return;
                        },
                        function(importError) {
                            goError("(The error was: <strong>" + requestError.message + "</strong>)");
                            return false;
                        }); 
                } catch (err) {
                    var message = '';
                    for (var f in err) {
                        message += f;
                        message += ':';
                        message += err[f];
                        message += ' ';
                    }
                    goError(message);
                    return false;
                }
            },
            function (request) {
                goError("(The error was: <strong>" + request.message + "</strong>) <br>" + "The server URL you are using may not be valid. <br>"
                      + "Please contact your bank server service, "
                      + "or refer to the bank server walk through document for instructions to setup your own server.");
                return false;
            }
        );
    }

    //pkcs10 certification enrollment

    function createRequestBlobAndEnroll(user)
    {
        var encoded;
        try {
            //WinRT APIs for creating a certficate request
            var request = new Windows.Security.Cryptography.Certificates.CertificateRequestProperties;
            request.subject = user;
            request.friendlyName = user + "'s WoodGrove Bank Certificate";
            request.keyProtectionLevel = Windows.Security.Cryptography.Certificates.KeyProtectionLevel.noConsent;
            Windows.Security.Cryptography.Certificates.CertificateEnrollmentManager.createRequestAsync(request).done(
                function (requestResult) {
                    encoded = requestResult;
		    var installOption = 0; //Windows.Security.Cryptography.Certificates.InstallOptions.none;
                    var params = "username=" + user;
                    params += "&request=" + encodeURIComponent(encoded);
		    
                    WinJS.xhr({
                        type: "POST",
                        url: "Your URL",    //Please provide the server url here. For example:
                                //url: "https://WoodGrove-Bank/bankserver2/enrollment/submit",
                        headers: { "Content-type": "application/x-www-form-urlencoded" },
                        data: params
                    }).done(
                        function (/*@override*/request) {
                            var obtainedData = window.JSON.parse(request.responseText);
                            try {
                                // WinRT API for certficate enrollment
                                Windows.Security.Cryptography.Certificates.CertificateEnrollmentManager.installCertificateAsync(obtainedData.certificate, installOption ).done(
                                function() {
                                    // set certificate enrollment mark to aviod redundant certificate enrollments
                                    Windows.Storage.ApplicationData.current.localSettings.values["EnrollCertificate"] = true;

                                    var /*@override*/ item = {
                                          title: "Account Page",
                                          content: "<strong>(Please sign out and re-launch the application so as to use your enrolled certificate.)<strong>",
                                          backgroundColor: 'rgba(191, 84, 46, 1)',
                                          navigate: "sign-out"
                                    };
                                    WinJS.Navigation.navigate('/html/account.html', { item: item });
                                    return false;
                                },
                                function(installError) {
                                    goError("(The error was: <strong>" + intallError.message + "</strong>)");
                                    return false;
                                });
                           } catch (err) {
                               var /*@override*/ message = '';
                               for (var /*@override*/ f in err) {
                               message += f;
                               message += ':';
                               message += err[f];
                               message += ' ';
                           }
                               goError(message);
                               return false;
                          }
                      },
                      function (/*@override*/request) {
                          goError("(The error was: <strong>" + request.message + "</strong>) <br>" + "The server URL you are using may not be valid. <br>"
                          + "Please contact your bank server service, "
                          + "or refer to the bank server walk through document for instructions to setup your own server.");
                          return false;
                     });
                },
                function (requestError) {
                    goError("(The error was: <strong>" + requestError.message + "</strong>)");
                    return false;
                });
        }
        catch (err) {
            var message = '';
            for (var f in err) {
                message += f;
                message += ':';
                message += err[f];
                message += ' ';
            }
            goError(message);
            return false;
        };
    }

    function doPKCS10Enrollment(user) {
        try {
            createRequestBlobAndEnroll(user);
        } catch (err) {
            var message = '';
            for (var f in err) {
                message += f;
                message += ':';
                message += err[f];
                message += ' ';
            }
            goError(message);
            return false;
        };
    }    

    //load user credentials from the vault component
    function loadCredential() {
        var vault = new Windows.Security.Credentials.PasswordVault();
        var creds = vault.retrieveAll();
        for (var i = 0; i < creds.size; i++) {
            if (creds.getAt(i).resource === "WoodGrove-Bank-usercred") {
                var cred = creds.getAt(i);
                username = cred.userName;
                break;
            }
        }
    }

    //error handler
    function goError(message) {
        var /*@override*/ item = {
            content: message
        };
        WinJS.Navigation.navigate('/html/error.html', { item: item });
    }
})();
