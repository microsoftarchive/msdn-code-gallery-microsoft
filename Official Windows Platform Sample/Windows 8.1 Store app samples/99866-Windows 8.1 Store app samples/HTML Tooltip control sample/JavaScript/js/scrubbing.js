//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scrubbing.html", {
        ready: function (element, options) {
            //check which stylesheet is currently loaded
            initStyleSheetRadioButton();

            initScrubbing();
        }
    });

})();



var toolTips = [];
var elements = [];
var maxItem = 5;
var curScrubGroup = null;
var centerLine = 0;
var isHorizontal = false;    // please set this option for horizontal / vertical scrubbing
var curPressedEle = null;
var tooltipType = "";

function initScrubbing() {
    for (var i = 0; i < maxItem; i++) {
        elements[i] = document.getElementById("scrubbingElement" + (i + 1));

        // tooltip content is taken from the "title" attribute of each element in HTML.
        toolTips[i] = new WinJS.UI.Tooltip(elements[i], { placement: "right" });
    }

    // add event listeners to scrubbing groups
    var grpList = document.getElementsByClassName("scrubbingGroup");
    for (var j = 0; j < grpList.length; j++) {
        grpList[j].addEventListener("pointerdown", scrubbingGrpDown, false);
        grpList[j].addEventListener("pointermove", scrubbingGrpMove, false);
        grpList[j].addEventListener("pointerup", scrubbingGrpUp, false);
    }
}

function activate(obj) {
    // please replace this with the activation code you want
    document.getElementById("scrubbingInfo").innerHTML = ("You've picked: " + obj.winControl.innerHTML);
}

function scrubbingGrpDown(ev) {
    // calculate a line that goes across the center of all the elements.
    // to simplify the design this only support horizontal and vertical scrubbing.
    // all elements should sit in the same row or column, although they could have different sizes and spacing.
    // when the contact ponit starts from within the group and then drags off it, the code will project it back to the
    // center point and do a hit testing to find the element underneath that point.

    calculateCenterLine();

    this.setPointerCapture(ev.pointerId);

    // different input should invoke different types of tooltip that have the appropriate show/hide timer for that input type.
    if ("touch" === ev.pointerType) {
        tooltipType = "touch";
    } else {
        // pen input also follows the same show/hide timer as mouse
        tooltipType = "mousemove";
    }

    curScrubGroup = this.id;

    scrubbingGrpMove(ev); // follow the same function to detect whether it's pressing on a scrubbing element
}

function scrubbingGrpUp(ev) {
    this.releasePointerCapture(ev.pointerId);

    var ele = document.elementFromPoint(ev.clientX, ev.clientY);
    if (curPressedEle) {
        curPressedEle.winControl.close();

        activate(curPressedEle);
        WinJS.Utilities.removeClass(curPressedEle, "scrubbing-pressed");
        curPressedEle = null;
    }

    curScrubGroup = null;
    tooltipType = "";
}

function scrubbingGrpMove(ev) {
    if (!curScrubGroup) {
        return;
    }

    var ele = null;

    // hit testing
    if (isHorizontal) {
        ele = document.elementFromPoint(ev.pageX - window.pageXOffset, centerLine - window.pageYOffset);
    } else {
        ele = document.elementFromPoint(centerLine - window.pageXOffset, ev.pageY - window.pageYOffset);
    }

    if ((ele) &&
         (WinJS.Utilities.hasClass(ele, "scrubbingElement")) &&
         (ele.parentNode) &&
         (ele.parentNode.id === curScrubGroup)) {
        if (ele !== curPressedEle) {
            // close the tooltip on the last element
            if (curPressedEle) {
                curPressedEle.winControl.close();
            }

            // since elements don't update :hover or :active if the user scrubs parallelly but off the track,
            // so here we need to programmatically add/remove classes to create the same effect.
            setPressed(ele);

            // open the tooltip on the current element
            curPressedEle.winControl.open(tooltipType);
        }
    }
}

function setPressed(ele) {
    if (curPressedEle) {
        WinJS.Utilities.removeClass(curPressedEle, "scrubbing-pressed");
    }
    curPressedEle = ele;
    WinJS.Utilities.addClass(curPressedEle, "scrubbing-pressed");
}

function calculateCenterLine() {
    var obj = elements[0];
    var offset = isHorizontal ? (obj.offsetHeight / 2) : (obj.offsetWidth / 2);
    do {
        offset += isHorizontal ? (obj.offsetTop) : (obj.offsetLeft);
        obj = obj.offsetParent;
    } while (obj);

    centerLine = offset;
}
