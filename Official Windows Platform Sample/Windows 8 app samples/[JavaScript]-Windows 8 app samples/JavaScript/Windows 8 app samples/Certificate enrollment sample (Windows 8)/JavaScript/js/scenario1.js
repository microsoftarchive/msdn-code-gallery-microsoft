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
            document.getElementById("button_createReq").addEventListener("click", createCertificateRequest, false);
            document.getElementById("button_SubmitInstall").addEventListener("click", submitCertificateRequestAndInstallCertificate, false);
        }
    });


    var myRequest = "";

    //utility function to help display all fields of a CertificateRequestProperties object
    function convertCertificateRequestPropertiestoString(certificateRequestProperties) {
        var result = "";
        if (certificateRequestProperties === null) {
            result = "\ncertificateRequestProperties object is null";
        }
        else {
            for (var propertyName in certificateRequestProperties) {
                result = result + "  " + propertyName + " = ";
                if (certificateRequestProperties[propertyName] === null) {
                    result = result + "null";
                }
                else {
                    result = result + certificateRequestProperties[propertyName];
                }
                result = result + "\n";
            }

        }
        return result;
    }

    //utility function to help display all fields of an Error object
    function convertErrortoString(e) {
        var result = "\nError details:";
        result = result + "\nError Number = " + e.number;
        result = result + "\nError Name = " + e.name;
        result = result + "\nError Message = " + e.message;
        return result;
    }

    //Create a CertificateRequestProperties object and use it to create a certificate request
    function createCertificateRequest() {
        var myMessage = "";
        try {
            //call the default constructor of CertificateRequestProperties
            var myRequestProperties = new Windows.Security.Cryptography.Certificates.CertificateRequestProperties();
            myRequestProperties.subject = "Toby";
            myRequestProperties.friendlyName = "Toby's Cert";

            WinJS.log && WinJS.log("Calling createRequestAsync ...", "sample", "status");

            //call Certificate Enrollment function createRequest to create a certificate request
            Windows.Security.Cryptography.Certificates.CertificateEnrollmentManager.createRequestAsync(myRequestProperties)
            .done(function (req) {
                myRequest = req;

                WinJS.log && WinJS.log("createRequestAsync succeeded and request is created.", "sample", "status");

                var output = "Request propeties: \n";
                output = output + convertCertificateRequestPropertiestoString(myRequestProperties);
                output = output + "\nEncoded request String:\n" + myRequest;

                document.getElementById("ResultDisplay").innerText = output;
            },
            function (e) {
                WinJS.log && WinJS.log("createRequestAsync failed, error: " + convertErrortoString(e), "sample", "error");
            });
        }
        catch (e)
        {
            WinJS.log && WinJS.log("Creating request failed, error: " + convertErrortoString(e), "sample", "error");
        }
    }

    //Submit a certificate request to a Certificate Services and install the issued certificate
    function submitCertificateRequestAndInstallCertificate() {
        if (myRequest === "") {
            WinJS.log && WinJS.log("Please create certificate request first", "sample", "error");
            return;
        }

        var url = "";   // for this sample to work, this URL needs be a valid URL which can take certifiate request and issue certificate 
        // add code here to initialize url

        if (url === "") {
            WinJS.log && WinJS.log("Please update the code to have a valid URL for the request submittion.", "sample", "error");
            return;
        }

        WinJS.log && WinJS.log("Submitting certificate request to server ...", "sample", "status");

        var certificate = submitRequest(url, myRequest);

        //call Certificate Enrollment function installCertificate to install the certificate
        try {
            Windows.Security.Cryptography.Certificates.CertificateEnrollmentManager.installCertificateAsync(certificate,
                Windows.Security.Cryptography.Certificates.InstallOptions.None)
            .done(function () {
                var msg = "Certificate installation succeeded. The certificate is in the appcontainer Personal certificate store";
                WinJS.log && WinJS.log(msg, "sample", "status");
            },
            function (e) {
                WinJS.log && WinJS.log("installCertificateAsync failed, error: " + convertErrortoString(e), "sample", "error");
            });
        }
        catch (e) {
            WinJS.log && WinJS.log("Certificate installation failed, error: " + convertErrortoString(e), "sample", "error");
        }
    }


    // Use XMLHttpRequest object to communicate with a Certificate Service
    // The code of this function is specific to a test server implemented to test sample
    function submitRequest(url, request) {
        var xmlHttp = new XMLHttpRequest();
        var xmlResult;
        var endResult;
        var xmlElements;

        var body = '<SubmitRequest xmlns="http://tempuri.org/"><strRequest>' + request + '</strRequest></SubmitRequest>';

        xmlHttp.open("POST", url, false);
        xmlHttp.setRequestHeader("Content-type", "text/xml");
        xmlHttp.send(body);

        if (xmlHttp.status === 200) {
            xmlResult = xmlHttp.responseXML;

            xmlElements = xmlResult.getElementsByTagName("SubmitRequestResult");
            if (1 !== xmlElements.length) {
                endResult = "\tserver retunred more than 1 results";
            }
            else if (1 !== xmlElements[0].childNodes.length) {
                endResult = "\tSubmitRequestResult element has more than 1 child nodes";
            }
            else if (3 !== xmlElements[0].childNodes[0].nodeType) {
                endResult = "\tSubmitRequestResult element's child node type " + xmlElements[0].childNodes[0].nodeType.toString() + " != 3";
            }
            else {
                endResult = xmlElements[0].childNodes[0].nodeValue;
            }
        }
        else {
            var err = "An error occured while communicating with server. statis text is " + xmlHttp.statusText;
            throw err;
        }
        return endResult;
    }

})();
