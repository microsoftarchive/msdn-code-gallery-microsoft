//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario3.html", {
        ready: function (element, options) {
            document.getElementById("scenario3Open").addEventListener("click", runscenario3, false);
            document.getElementById("scenario3Open").addEventListener("click", output3, false);
        }
    });

    function runscenario3() {
        var element2 = document.getElementById("scenario3InputChild");
        var duration1 = scenario3Duration1.options[scenario3Duration1.selectedIndex].value + 's';
        var duration2 = scenario3Duration2.options[scenario3Duration2.selectedIndex].value + 's';
        var transdelay1 = scenario3TransDelay1.options[scenario3TransDelay1.selectedIndex].value + 's';
        var transdelay2 = scenario3TransDelay2.options[scenario3TransDelay2.selectedIndex].value + 's';
        var timingfun = scenario3TransTimingFunc.options[scenario3TransTimingFunc.selectedIndex].value;
        var tranorigin = scenario3TransOrigin.options[scenario3TransOrigin.selectedIndex].value;
        element2.style.transitionProperty = 'all';

        // Setting the values from the user selection
        var xPosition = scenario3XPosition.options[scenario3XPosition.selectedIndex].value + 'px';
        var yPosition = scenario3YPosition.options[scenario3YPosition.selectedIndex].value + 'px';
        var zPosition = scenario3ZPosition.options[scenario3ZPosition.selectedIndex].value + 'px';
        var xrotate = scenario3RotateX.options[scenario3RotateX.selectedIndex].value + 'deg';
        var yrotate = scenario3RotateY.options[scenario3RotateY.selectedIndex].value + 'deg';
        var zrotate = scenario3RotateZ.options[scenario3RotateZ.selectedIndex].value + 'deg';
        var xscale = scenario3ScaleX.options[scenario3ScaleX.selectedIndex].value;
        var yscale = scenario3ScaleY.options[scenario3ScaleY.selectedIndex].value;
        var xskew = scenario3SkewX.options[scenario3SkewX.selectedIndex].value + 'deg';
        var yskew = scenario3SkewY.options[scenario3SkewY.selectedIndex].value + 'deg';

        var trasformSting = 'translateX(' + xPosition + ') translateY(' + yPosition + ') translateZ(' + zPosition + ') rotateX(' + xrotate + ') rotateY(' + yrotate + ') rotateZ(' + zrotate + ') scaleX(' + xscale + ') scaleY(' + yscale + ') skewX(' + xskew + ') skewY(' + yskew + ')';
        var durationString = duration1 + ' , ' + duration2;
        var delayString = transdelay1 + ' , ' + transdelay2;
        var timfunString = timingfun;
        var transorgString = tranorigin + '%';

        element2.style.transitionProperty = 'transform, opacity';
        element2.style.transitionDelay = delayString;
        element2.style.transitionDuration = durationString;
        element2.style.transitionTimingFunction = timfunString;
        element2.style.transformOrigin = transorgString;

        element2.style.transform = trasformSting;

        scenarioTransformValues = "transform: " + trasformSting + ";";
        scenarioDurationValues = "transition-duration: " + durationString + ";";
        scenarioDelayValues = "transition-delay: " + delayString + ";";
        scenarioTimFunValues = "transition-timing-function: " + timfunString + ";";
        scenarioTransOrig = "transform-origin: " + transorgString + ";";

        element2.style.transformOrigin = tranorigin + "%";
        element2.style.opacity = '0';
        resetelement = element2;
        element2.addEventListener('transitionend', resetPage2, false);
    }

    //Output from clicking button id scenario3Open
    function output3() {
        var element22 = document.getElementById("output");
        element22.innerHTML = "/*CSS*/ <br />"
        + ".scenario3 { <br />"
        + g_tab + "transition-property: transform, opacity; <br />"
        + g_tab + scenarioDurationValues + "<br />"
        + g_tab + scenarioDelayValues + "<br />"
        + g_tab + scenarioTimFunValues + "<br />"
        + g_tab + scenarioTransformValues + "<br />"
        + " } <br />";
    }
})();
