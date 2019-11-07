//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************
(function () {
    "use strict";

    self.addEventListener("message", function (message) {

        importScripts("//Microsoft.Phone.WinJS.2.1/js/base.js", "searchLoC.js");

        LOCPictures.getCollection(message.data).
            then(
                function (response) {
                    postMessage(response);
                });
    });
})();
