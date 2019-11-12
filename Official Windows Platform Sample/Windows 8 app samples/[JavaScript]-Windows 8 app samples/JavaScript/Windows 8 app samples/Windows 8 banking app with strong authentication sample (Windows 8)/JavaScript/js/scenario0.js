//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    'use strict';

    function ready(elements, options) {
        var /*@override*/ item = options && options.item ? options.item : getItem();

        WinJS.UI.processAll(elements)
            .then(function () {
            elements.querySelector('.title').textContent = item.title;
            elements.querySelector('.submitButton').addEventListener('click', logon, false);
            elements.querySelector('.image').style.backgroundColor = item.backgroundColor;
            elements.querySelector('.content').innerHTML = "You don't have a client certificate available to authenticate to your account.<br>"
                                                         + "Please upgrade to stronger authentication for full access to your account.";
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

    WinJS.UI.Pages.define('/html/scenario0.html', {
        ready: ready
    });

    //account logon
    // onClick() handler for the 'submitButton'
    function logon() {
        try {
            var username = document.getElementById("username").value;
            var password = document.getElementById("password").value;

            //store user credential in the vault component
            if (username && password) {
                var cred = new Windows.Security.Credentials.PasswordCredential("WoodGrove-Bank-usercred", username, password);
                var vault = new Windows.Security.Credentials.PasswordVault();
                vault.add(cred);
            }else {
                return false;
            }
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
        }

        var params = "UserName=" + username;
        params += "&Password=" + password;

        WinJS.xhr({
            type: "POST",
            url: "Your URL",    //Please provide the server url here. For example:
                                //url: "https://WoodGrove-Bank/bankserver2/account/simplelogon",
            headers: { "Content-type": "application/x-www-form-urlencoded" },
            data: params
        }).done(
            function (request) {
                var obtainedData = window.JSON.parse(request.responseText);
                var /*@override*/ item = {
                    title: "Account Page",
                    content: "<strong>(Please upgrade to strong authentication if you need full access to your account.)",
                    backgroundColor: 'rgba(25,50,200,1)',
                    navigate: "groupeditemsPage"
                };
                WinJS.Navigation.navigate('/html/account.html', { item: item });
            },
            function (request) {
                goError("(The error was: <strong>" + request.message + "</strong>) <br>" + "The server URL you are using may not be valid. <br>"
                      + "Please contact your bank server service, "
                      + "or refer to the bank server walk through document for instructions to setup your own server.");
                return false;
            }
        );
    };

    //error handler
    function goError(message) {
        var /*@override*/ item = {
            content: message
        };
        WinJS.Navigation.navigate('/html/error.html', { item: item });
    };

})();
