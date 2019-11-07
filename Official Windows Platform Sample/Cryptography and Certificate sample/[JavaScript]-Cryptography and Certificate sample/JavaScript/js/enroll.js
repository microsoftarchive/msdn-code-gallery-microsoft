//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/enroll.html", {
        ready: function (element, options) {
            document.getElementById("RunSample").addEventListener("click", RunSample_Click, false);
        }
    });


    var certificateRequest = "";
    var output = "";
    var response = "";
    var url = "";

    function RunSample_Click() {

        var urlTextBox = document.getElementById("UrlTextBox").value;
        var userStoreCheckBox = document.getElementById("UserStoreCheckBox");
        var urlpattern = /(ftp|http|https):\/\/(\w+:{0,1}\w*@)?(\S+)(:[0-9]+)?(\/|\/([\w#!:.?+=&%@!\-\/]))?/;
        output = "Creating certificate request...";
        WinJS.log && WinJS.log(output, "sample", "status");
        if (urlTextBox === null || urlTextBox === "" || urlpattern.test(urlTextBox) === false) {

            output = "A valid URL is not provided, so request will be created but will not be submitted.";
            WinJS.log && WinJS.log(output, "sample", "error");
        }
        else {
            url = urlTextBox.trim();
        }

        if (true === userStoreCheckBox.checked) {
            runSampleUserEnroll();
        }
        else {
            runSampleAppEnroll();
        }
    }

    function runSampleUserEnroll() {
        //call the default constructor of CertificateREquestProperties
        var reqProp = new Windows.Security.Cryptography.Certificates.CertificateRequestProperties();
        reqProp.subject = "Toby";
        reqProp.friendlyName = "Toby's Cert";
        var installOption = 0;
        var certresponse;
        var request;
        var data;

        // have to use User's Certificate Store
        // call User Certificate Enrollment function createRequest to create a certificate request
        Windows.Security.Cryptography.Certificates.CertificateEnrollmentManager.userCertificateEnrollmentManager.createRequestAsync(reqProp).then(function (req) {
            output += "\nRequest created for User's certificate store, content:\n" + req;
            WinJS.log && WinJS.log(output, "sample", "status");
            certificateRequest="<SubmitRequest xmlns=\"http://tempuri.org/\"><request>" + req + "</request>"
                                + "<attributes xmlns:a=\"http://schemas.microsoft.com/2003/10/Serialization/Arrays\" xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\"><a:KeyValueOfstringstring><a:Key>CertificateTemplate</a:Key><a:Value>WebServer</a:Value></a:KeyValueOfstringstring></attributes>"
                                + "</SubmitRequest>";
        }).done(function () {
            if (certificateRequest === "" || certificateRequest === null) {
                return;
            }
            if (url === null || url === "") {
                return;
            }
            output += "\n Submitting request to Enrollment URL ...";
            WinJS.log && WinJS.log(output, "sample", "status");

            try {
                var xmlHttp = new XMLHttpRequest();
                xmlHttp.onreadystatechange = function () {
                    if (xmlHttp.readyState === 4) {
                        if (xmlHttp.status === 200) {
                            data = xmlHttp.responseXML.querySelectorAll("SubmitRequestResult");
                            certresponse = data[0].childNodes[0].nodeValue;
                            output += "\n Response received from server:\n" + certresponse;
                            WinJS.log && WinJS.log(output, "sample", "status");
                            try {
                                output += "\nInstalling certificate ...";
                                WinJS.log && WinJS.log(output, "sample", "status");
                                Windows.Security.Cryptography.Certificates.CertificateEnrollmentManager.userCertificateEnrollmentManager.installCertificateAsync(certresponse.toString(),installOption);
                                output += "\nThe certificate response is installed successfully to User's certificate store.\n";
                                WinJS.log && WinJS.log(output, "sample", "status");
                            }
                            catch (e) {
                                output += "Error occured with installing certificate in User's store" + e.message;
                                WinJS.log && WinJS.log(output, "sample", "error");
                            }
                        }
                        else {
                            output += xmlHttp.status.toString();
                            WinJS.log && WinJS.log(output, "sample", "error");
                        }
                    }
                };
                xmlHttp.open("POST", url, true);
                xmlHttp.setRequestHeader("Content-type", "text/xml;charset=utf-8");

                xmlHttp.send(certificateRequest);
            }
            catch (e) {
                output += "\nCertificate Installation failed with error: " + e.Message + "\n";
                WinJS.log && WinJS.log(output, "sample", "error");
            }
        });
    }

    function runSampleAppEnroll() {
        //call the default constructor of CertificateREquestProperties
        var reqProp = new Windows.Security.Cryptography.Certificates.CertificateRequestProperties();
        reqProp.subject = "Toby";
        reqProp.friendlyName = "Toby's Cert";
        var installOption = 0;
        var certresponse;
        var data;

        // have to use User's Certificate Store
        // call User Certificate Enrollment function createRequest to create a certificate request

        Windows.Security.Cryptography.Certificates.CertificateEnrollmentManager.createRequestAsync(reqProp).then(function (req) {
            output += "\nRequest created for App's certificate store, content:\n" + req;
            WinJS.log && WinJS.log(output, "sample", "status");
            certificateRequest = "<SubmitRequest xmlns=\"http://tempuri.org/\"><request>" + req + "</request>"
                        + "<attributes xmlns:a=\"http://schemas.microsoft.com/2003/10/Serialization/Arrays\" xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\"><a:KeyValueOfstringstring><a:Key>CertificateTemplate</a:Key><a:Value>WebServer</a:Value></a:KeyValueOfstringstring></attributes>"
                        + "</SubmitRequest>";
        }).done(function () {
            if (certificateRequest === "") {
                return;
            }
            if (url === null || url === "") {
                return;
            }
            output += "\n Submitting request to Enrollment URL ...";
            WinJS.log && WinJS.log(output, "sample", "status");
            var request;
            try {
                var xmlHttp = new XMLHttpRequest();
                xmlHttp.onreadystatechange = function () {
                    if (xmlHttp.readyState === 4) {
                        if (xmlHttp.status === 200) {
                            data = xmlHttp.responseXML.querySelectorAll("SubmitRequestResult");
                            certresponse = data[0].childNodes[0].nodeValue;
                            output += "\n Response received from server:\n" + certresponse;
                            WinJS.log && WinJS.log(output, "sample", "status");
                            try {
                                output += "\nInstalling certificate ...";
                                WinJS.log && WinJS.log(output, "sample", "status");
                                Windows.Security.Cryptography.Certificates.CertificateEnrollmentManager.installCertificateAsync(certresponse.toString(), installOption);
                                output += "\nThe certificate response is installed successfully to App's certificate store.\n";
                                WinJS.log && WinJS.log(output, "sample", "status");
                            }
                            catch (e) {
                                output += "Error occured with installing certificate" + e.message;
                                WinJS.log && WinJS.log(output, "sample", "error");
                            }
                        }
                        else {
                            output += xmlHttp.status.toString();
                            WinJS.log && WinJS.log(output, "sample", "error");
                        }
                    }
                };
                xmlHttp.open("POST", url, true);
                xmlHttp.setRequestHeader("Content-type", "text/xml;charset=utf-8");

                xmlHttp.send(certificateRequest);
            }
            catch (e) {
                output += "\nCertificate Installation in App's store failed with error: " + e.message + "\n";
                WinJS.log && WinJS.log(output, "sample", "error");
            }
        });
    }



})();
