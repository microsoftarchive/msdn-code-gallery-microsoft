//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/styling.html", {
        ready: function (element, options) {
            //check which stylesheet is currently loaded
            initStyleSheetRadioButton();
        }
    });

})();

function stylingIFrameLoaded() {
    if (document.styleSheets[4].disabled === true) {
        setIFrameStyle("ui-light");     // set the page background of the nested iframe
    } else if (document.styleSheets[0].disabled === true) {
        setIFrameStyle("ui-dark");      // set the page background of the nested iframe
    }
}

function setIFrameStyle(colorStyle) {
    //send messages to pages in iframes to switch their styles
    var frame = document.getElementsByClassName("styleFrame");
    var domain = "*";
    if (window.Windows) {
        domain = "ms-appx://" + document.location.host;
    }

    for (var i = 0; i < frame.length; i++) {
        frame[i].contentWindow.postMessage(colorStyle, domain);
    }
}
