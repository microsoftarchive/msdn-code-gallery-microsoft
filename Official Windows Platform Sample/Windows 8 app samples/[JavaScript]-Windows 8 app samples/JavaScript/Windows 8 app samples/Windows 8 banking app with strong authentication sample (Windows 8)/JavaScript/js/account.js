//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    'use strict';

    var ui = WinJS.UI;
    var utils = WinJS.Utilities;

    function ready(elements, options) {
        var /*@override*/ item = options && options.item ? options.item : getItem();

        WinJS.UI.processAll(elements)
            .then(function () {
            elements.querySelector('.title').textContent = item.title;
            elements.querySelector('.submitButton').addEventListener('click', signOut, false);

            if (item.navigate === "groupeditemsPage") {
                elements.querySelector('.goBackButton').type = "Submit";
                elements.querySelector('.goBackButton').value = "Go to Scenario Page";
                elements.querySelector('.goBackButton').addEventListener('click', goLandingPage, false);
            }
            loadTemplate(elements, item);
        });
    };
 
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
    };

    //load account page dynamically
    function loadTemplate(elements, /*@override*/item) {
        var params = ""; 
        WinJS.xhr({
            type: "POST",
            url: "Your URL",    //Please provide the server url here. For example:
                                //url: "https://WoodGrove-Bank/bankserver2/account/GetAccountInfo",
            headers: { "Content-type": "application/x-www-form-urlencoded" },
            data: params
        }).done(
            function (request) {
                var obtainedData = window.JSON.parse(request.responseText);
                if (obtainedData.strongAuth === true) {
                    //certificate-based strong authentication
                    elements.querySelector('.content').innerHTML = "You have authenticated to your account using a certificate.<br>"
                                                                 + "This form of strong authentication enables you to perform all actions on your online banking account including external funds transfer and bill pay.";
                    elements.querySelector('.image').style.backgroundColor = "rgba(191, 84, 46, 1)";
                } else {
                    //weak authentication
                    elements.querySelector('.content').innerHTML = "You have authenticated to your account using a password.<br>"
                                                                 + "This form of authentication allows you limited access to your account such as viewing balance and transferring funds within internal accounts.<br>"
                                                                 + "If you need full access in order to transfer funds externally or pay bills, you are required to use stronger authentication.<br>"
                                                                 + item.content;
                    elements.querySelector('.image').style.backgroundColor = "rgba(191, 84, 46, 0.6)";
                }
                return false;
            },
            function (request) {
                goError("(The error was: <strong>" + request.message + "</strong>) <br>" + "The server URL you are using may not be valid. <br>"
                      + "Please contact your bank server service, "
                      + "or refer to the bank server walk through document for instructions to setup your own server.");
                return false;
            }
         );
    };

    WinJS.UI.Pages.define('/html/account.html', {
        ready: ready
    });

    function signOut() {
        try{
            //clean credential histories
            var vault = new Windows.Security.Credentials.PasswordVault();
            var creds = vault.retrieveAll();
            for (var i = 0; i < creds.size; i++) {
                vault.remove(creds.getAt(i));
            }

            // clean certificate enrollment mark
            if (Windows.Storage.ApplicationData.current.localSettings.values["EnrollCertificate"]) {
                Windows.Storage.ApplicationData.current.localSettings.values.remove("EnrollCertificate");
            }

            window.close();
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
    }

    //error handler
    function goError(message) {
        var /*@override*/ item = {
            content: message
        };
        WinJS.Navigation.navigate('/html/error.html', { item: item });
    };

    //go back to senario select page
    function goLandingPage() {
        WinJS.Navigation.navigate('/html/groupeditemsPage.html');
    }
})();
