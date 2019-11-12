//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/checkbox.html", {
        ready: function (element, options) {
            //check which stylesheet is currently loaded
            initStyleSheetRadioButton();

            toppingsCheck();    // set the initial status of "select all" in checkbox scenario

            likeDislikeSwitch();    // set the initial status of "like dislike" in checkbox scenario
        }
    });

})();

function toppingsCheck() {
    var options = document.getElementsByClassName("toppings");
    var selectedCount = 0;
    for (var i = 0; i < options.length; i++) {
        if (options[i].checked) {
            selectedCount++;
        }
    }

    // switch between 3 states using indeterminate and checked properties
    var selectAll = document.getElementById("toppingsAll");
    if (options.length === selectedCount) {
        selectAll.indeterminate = false;
        selectAll.checked = true;
    } else if (0 === selectedCount) {
        selectAll.indeterminate = false;
        selectAll.checked = false;
    } else {
        selectAll.indeterminate = true;
        selectAll.checked = false;
    }
}

function toppingsCheckAll() {
    var options = document.getElementsByClassName("toppings");
    for (var i = 0; i < options.length; i++) {
        options[i].checked = event.srcElement.checked;
    }
}

var likeDislikeStatus = 0;

function likeDislikeSwitch() {
    var likeDislike = document.getElementById("myLikeDislikeCheckbox");

    switch (likeDislikeStatus) {
        default:
        case 0:
            likeDislike.indeterminate = true;
            likeDislikeStatus = 1;          // neutral
            break;
        case 1:
            likeDislike.indeterminate = false;
            likeDislike.checked = true;
            likeDislikeStatus = 2;          // like
            break;
        case 2:
            likeDislike.indeterminate = false;
            likeDislike.checked = false;
            likeDislikeStatus = 3;          // dislike
            break;
        case 3:
            likeDislike.indeterminate = true;
            likeDislike.checked = false;
            likeDislikeStatus = 1;          // neutral
            break;
    }
}
