//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

// Array containing the comments for the detail page on Strawberry Sorbet
var myCommentData = new WinJS.Binding.List([
        { title: "The Smoothest", text: "I loved this ice cream. I thought it was maybe the smoothest ice cream that i have ever had!" },
        { title: "What a great flavor!", text: "Although the texture was a bit lacking, this one has the best flavor I have tasted!" },
        { title: "Didn't like the 'choco bits'", text: "The little bits of chocolate just weren't working for me" },
        { title: "Loved the peanut butter", text: "The peanut butter was the best part of this delicious snack" },
        { title: "Wish there was more sugar", text: "This wasn't sweet enough for me. I will have to try your other flavors, but maybe this is too healthy for me." },
        { title: "Texture was perfect", text: "This was the smoothest ice cream I have ever had" },
        { title: "Kept wishing there was more", text: "When I got to the end of each carton I kept wishing there was more ice cream. It was delicious!" }
    ]);

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario3.html", {
        ready: function (element, options) {
            element.querySelector("#listView3").winControl.forceLayout();
        }
    });
})();
