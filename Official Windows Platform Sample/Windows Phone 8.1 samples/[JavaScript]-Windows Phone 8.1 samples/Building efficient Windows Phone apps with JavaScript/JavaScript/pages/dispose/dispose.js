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

    WinJS.UI.Pages.define("/pages/dispose/dispose.html", {
        ready: function (element, options) {

            $("#dispose").click(function () {
                var searchControl = $("#searchControl")[0];
                searchControl.winControl.dispose();
                searchControl.innerHTML = "";
            });

            var searchControlDiv = $('#searchControl')[0];
            var searchControl = new SearchLOCControl.Control(searchControlDiv);

        }
    });
})();
