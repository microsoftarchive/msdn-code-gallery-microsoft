//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario5.html", {
        ready: function (element, options) {
            document.getElementById("scenario5Open").addEventListener("click", runscenario5, false);
            document.getElementById("scenario5Open").addEventListener("click", output5, false);
            //2D Animation Keyframes
            document.getElementById("scenario5Keyframe1").addEventListener("click", keyframeflyout1, false);
            document.getElementById("scenario5Keyframe2").addEventListener("click", keyframeflyout2, false);
            document.getElementById("scenario5Keyframe3").addEventListener("click", keyframeflyout3, false);
        }
    });
    function runscenario5() {
        var styleSheet = document.styleSheets[1];
        var element5 = document.getElementById("scenario5Input");
        var duration1 = scenario5AnimationDuration.options[scenario5AnimationDuration.selectedIndex].value + 's';
        var transdelay1 = scenario5AnimationDelay.options[scenario5AnimationDelay.selectedIndex].value + 's';
        var timingfun = scenario5AnimationTiming.options[scenario5AnimationTiming.selectedIndex].value;
        // Keyframe1 values
        var xPosition1 = scenario5XPosition.options[scenario5XPosition.selectedIndex].value + 'px';
        var yPosition1 = scenario5YPosition.options[scenario5YPosition.selectedIndex].value + 'px';
        var xyrotate1 = scenario5Rotate.options[scenario5Rotate.selectedIndex].value + 'deg';
        var xscale1 = scenario5ScaleX.options[scenario5ScaleX.selectedIndex].value;
        var yscale1 = scenario5ScaleY.options[scenario5ScaleY.selectedIndex].value;
        var xskew1 = scenario5SkewX.options[scenario5SkewX.selectedIndex].value + 'deg';
        var yskew1 = scenario5SkewY.options[scenario5SkewY.selectedIndex].value + 'deg';
        // Keyframe2 values
        var xPosition2 = scenario52XPosition.options[scenario52XPosition.selectedIndex].value + 'px';
        var yPosition2 = scenario52YPosition.options[scenario52YPosition.selectedIndex].value + 'px';
        var xyrotate2 = scenario52Rotate.options[scenario52Rotate.selectedIndex].value + 'deg';
        var xscale2 = scenario52ScaleX.options[scenario52ScaleX.selectedIndex].value;
        var yscale2 = scenario52ScaleY.options[scenario52ScaleY.selectedIndex].value;
        var xskew2 = scenario52SkewX.options[scenario52SkewX.selectedIndex].value + 'deg';
        var yskew2 = scenario52SkewY.options[scenario52SkewY.selectedIndex].value + 'deg';
        // Keyframe3 values
        var xPosition3 = scenario53XPosition.options[scenario53XPosition.selectedIndex].value + 'px';
        var yPosition3 = scenario53YPosition.options[scenario53YPosition.selectedIndex].value + 'px';
        var xyrotate3 = scenario53Rotate.options[scenario53Rotate.selectedIndex].value + 'deg';
        var xscale3 = scenario53ScaleX.options[scenario53ScaleX.selectedIndex].value;
        var yscale3 = scenario53ScaleY.options[scenario53ScaleY.selectedIndex].value;
        var xskew3 = scenario53SkewX.options[scenario53SkewX.selectedIndex].value + 'deg';
        var yskew3 = scenario53SkewY.options[scenario53SkewY.selectedIndex].value + 'deg';
        var animationString = '@keyframes animation2D {'
            + 'from { opacity: 1;'
            + 'transform: translateX(' + xPosition1 + ')'
            + 'translateY(' + yPosition1 + ')'
            + 'rotate(' + xyrotate1 + ')'
            + 'scaleX(' + xscale1 + ') scaleY(' + yscale1 + ')'
            + 'skewX(' + xskew1 + ') skewY(' + yskew1 + ');'
            + '}'
            + '50% { opacity: 0;'
            + 'transform: translateX(' + xPosition2 + ')'
            + 'translateY(' + yPosition2 + ')'
            + 'rotate(' + xyrotate2 + ')'
            + 'scaleX(' + xscale2 + ') scaleY(' + yscale2 + ')'
            + 'skewX(' + xskew2 + ') skewY(' + yskew2 + ');'
            + '}'
            + 'to { opacity: 1; '
            + 'transform: translateX(' + xPosition3 + ')'
            + 'translateY(' + yPosition3 + ')'
            + 'rotate(' + xyrotate3 + ')'
            + 'scaleX(' + xscale3 + ') scaleY(' + yscale3 + ')'
            + 'skewX(' + xskew3 + ') skewY(' + yskew3 + ');'
            + '}';

        // Taking the selected values and applying them to the animation
        styleSheet.insertRule(animationString, 0);
        element5.style.animationDuration = duration1;
        element5.style.animationDelay = transdelay1;
        element5.style.animationTimingFunction = timingfun;

        // Triggering the animation by applying the animation-name
        window.setImmediate(function () {
            element5.style.animationName = 'animation2D';
        });

        // Creating output strings with selected values
        scenarioTransformValues = "animation-name: animation2D; ";
        scenarioDurationValues = "transition-duration: " + duration1 + ";";
        scenarioDelayValues = "transition-delay: " + transdelay1 + ";";
        scenarioTimFunValues = "transition-timing-function: " + timingfun + ";";
        scenarioKeyfrom = "from { opacity: 1; transform: translateX(" + xPosition1 + ") translateY(" + yPosition1 + ") rotate(" + xyrotate1 + ") scaleX(" + xscale1 + ") scaleY(" + yscale1 + ") skewX(" + xskew1 + ") skewY(" + yskew1 + ");}";
        scenarioKey50 = "50% { opacity: 0; transform: translateX(" + xPosition2 + ") translateY(" + yPosition2 + ") rotate(" + xyrotate2 + ") scaleX(" + xscale2 + ") scaleY(" + yscale2 + ") skewX(" + xskew2 + ") skewY(" + yskew2 + ");}";
        scenarioKeyto = "to { opacity: 1; transform: translateX(" + xPosition3 + ") translateY(" + yPosition3 + ") rotate(" + xyrotate3 + ") scaleX(" + xscale3 + ") scaleY(" + yscale3 + ") skewX(" + xskew3 + ") skewY" + yskew3 + ");}";
        resetelement = element5;
        element5.addEventListener('animationend', resetPage, false);
    }

    // Keyframe1 flyout
    function keyframeflyout1() {
        var elementkeyframe1 = document.getElementById("keyframe1");
        elementkeyframe1.style.visibility = 'visible';
        elementkeyframe1.style.transform = 'translateX(200px)';
        elementkeyframe1.style.opacity = '1';
        // Hide other keyframes
        var p1 = document.getElementById("keyframe2");
        var p2 = document.getElementById("keyframe3");
        p1.style.visibility = 'hidden';
        p2.style.visibility = 'hidden';
    }

    // Keyframe2 flyout
    function keyframeflyout2() {
        var elementkeyframe2 = document.getElementById("keyframe2");
        elementkeyframe2.style.visibility = 'visible';
        elementkeyframe2.style.transform = 'translateX(200px)';
        elementkeyframe2.style.opacity = '1';
        // Hide other keyframes
        var p1 = document.getElementById("keyframe1");
        var p2 = document.getElementById("keyframe3");
        p1.style.visibility = 'hidden';
        p2.style.visibility = 'hidden';
    }

    // Keyframe3 flyout
    function keyframeflyout3() {
        var elementkeyframe3 = document.getElementById("keyframe3");
        elementkeyframe3.style.visibility = 'visible';
        elementkeyframe3.style.transform = 'translateX(200px)';
        elementkeyframe3.style.opacity = '1';
        // Hide other keyframes
        var p1 = document.getElementById("keyframe1");
        var p2 = document.getElementById("keyframe2");
        p1.style.visibility = 'hidden';
        p2.style.visibility = 'hidden';
    }

    // Output from clicking button id scenario5Open
    function output5() {
        var element = document.getElementById("output");
        element.innerHTML = "/*CSS*/ <br />"
            + ".scenario5 { <br />"
            + g_tab + scenarioTransformValues + "<br />"
            + g_tab + scenarioDurationValues + "<br />"
            + g_tab + scenarioDelayValues + "<br />"
            + g_tab + scenarioTimFunValues + "<br />"
            + "}<br />"
            + "@keyframes animation2D { <br />"
            + d_tab + scenarioKeyfrom + "<br />"
            + d_tab + scenarioKey50 + "<br />"
            + d_tab + scenarioKeyto + "<br />"
            + g_tab + "} <br />";
    }
})();
