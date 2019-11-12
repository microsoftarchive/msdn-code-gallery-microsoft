//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario2.html", {
        ready: function (element, options) {
            document.getElementById("button1").addEventListener("click", runscenario2, false);
            document.getElementById("button1").addEventListener("click", output2, false);
        }
    });

    function runscenario2() {
        WinJS.log && WinJS.log("Error message here", "sample", "error");
        var element1 = document.getElementById("scenario2Input");

        var duration1 = scenario2TansitionDuration1.options[scenario2TansitionDuration1.selectedIndex].value + 's';
        var duration2 = scenario2TansitionDuration2.options[scenario2TansitionDuration2.selectedIndex].value + 's';
        var transdelay1 = scenario2TransDelay1.options[scenario2TransDelay1.selectedIndex].value + 's';
        var transdelay2 = scenario2TransDelay2.options[scenario2TransDelay2.selectedIndex].value + 's';
        var timingfun = scenario2TransTimingFunc.options[scenario2TransTimingFunc.selectedIndex].value;

        var xPosition = scenario2XPosition.options[scenario2XPosition.selectedIndex].value + 'px';
        var yPosition = scenario2YPosition.options[scenario2YPosition.selectedIndex].value + 'px';
        var xyrotate = scenario2Rotate.options[scenario2Rotate.selectedIndex].value + 'deg';
        var xscale = scenario2ScaleX.options[scenario2ScaleX.selectedIndex].value;
        var yscale = scenario2ScaleY.options[scenario2ScaleY.selectedIndex].value;
        var xskew = scenario2SkewX.options[scenario2SkewX.selectedIndex].value + 'deg';
        var yskew = scenario2SkewY.options[scenario2SkewY.selectedIndex].value + 'deg';

        var trasformSting = 'translateX(' + xPosition + ') translateY(' + yPosition + ') rotate(' + xyrotate + ') scaleX(' + xscale + ') scaleY(' + yscale + ') skewX(' + xskew + ') skewY(' + yskew + ')';
        var durationString = duration1 + ' , ' + duration2;
        var delayString = transdelay1 + ' , ' + transdelay2;
        var timfunString = timingfun;

        element1.style.transitionProperty = 'transform, opacity';
        element1.style.transitionDelay = delayString;
        element1.style.transitionDuration = durationString;
        element1.style.transitionTimingFunction = timfunString;
        element1.style.transform = trasformSting;
        element1.style.opacity = '0';

        scenarioTransformValues = "transform: " + trasformSting + ";";
        scenarioDurationValues = "transition-duration: " + durationString + ";";
        scenarioDelayValues = "transition-delay: " + delayString + ";";
        scenarioTimFunValues = "transition-timing-function: " + timfunString + ";";
    
        resetelement = element1;
        element1.addEventListener('transitionend', resetPage2, false);
    }

    //Output from clicking button id scenario2Open
    function output2() {
        var element22 = document.getElementById("output");
        element22.innerHTML = "/*CSS*/ <br />"
            + ".scenario2 { <br />"
            + g_tab + "transition-property: transform, opacity; <br />"
            + g_tab + scenarioDurationValues + "<br />"
            + g_tab + scenarioDelayValues + "<br />"
            + g_tab + scenarioTimFunValues + "<br />"
            + g_tab + scenarioTransformValues + "<br />"
            + " } <br />";
    }
})();
