//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    window.addEventListener("message", receiveMessage, false);

    function receiveMessage(e) {
        if (e.origin === "ms-appx://" + document.location.host) {
            var items = JSON.parse(e.data),
                list = document.getElementById("feed");
            list.innerHTML = "";
            for (var i = 0; i < 5; i++) {
                var feedItem = items[i];
                var link = document.createElement("a");
                var newline = document.createElement("br");
                link.innerText = (i + 1) + ") " + feedItem.title;
                link.setAttribute("href", feedItem.link);
                link.setAttribute("target", "_blank");
                list.appendChild(link);
                list.appendChild(newline);
            }
        }
    }
})();
