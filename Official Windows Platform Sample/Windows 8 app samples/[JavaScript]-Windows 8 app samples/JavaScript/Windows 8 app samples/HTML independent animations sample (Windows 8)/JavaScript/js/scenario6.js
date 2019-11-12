//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario6.html", {
        ready: function (element, options) {
            document.getElementById("scenario6Open").addEventListener("click", runScenario6, false);
            //3D Animation Keyframes
            document.getElementById("scenario6Keyframe1").addEventListener("click", keyframeflyout61, false);
            document.getElementById("scenario6Keyframe2").addEventListener("click", keyframeflyout62, false);
            document.getElementById("scenario6Keyframe3").addEventListener("click", keyframeflyout63, false);
            document.getElementById("scenario6Open").addEventListener("click", output6, false);
        }
    });

    function runScenario6() {
        var styleSheet = document.styleSheets[1];
        var element5 = document.getElementById("childscenario6Input");
        
        // Setting the selected values
        var duration1 = scenario6AnimationDuration.options[scenario6AnimationDuration.selectedIndex].value + 's';
        var transdelay1 = scenario6AnimationDelay.options[scenario6AnimationDelay.selectedIndex].value + 's';
        var timingfun = scenario6AnimationTiming.options[scenario6AnimationTiming.selectedIndex].value;
        var tranorigin = scenario6TransOrigin.options[scenario6TransOrigin.selectedIndex].value + '%';

        // Keyframe1 values
        var xPosition1 = scenario6XPosition.options[scenario6XPosition.selectedIndex].value + 'px';
        var yPosition1 = scenario6YPosition.options[scenario6YPosition.selectedIndex].value + 'px';
        var zPosition1 = scenario6ZPosition.options[scenario6ZPosition.selectedIndex].value + 'px';
        var xrotate1 = scenario6RotateX.options[scenario6RotateX.selectedIndex].value + 'deg';
        var yrotate1 = scenario6RotateY.options[scenario6RotateY.selectedIndex].value + 'deg';
        var xscale1 = scenario6ScaleX.options[scenario6ScaleX.selectedIndex].value;
        var yscale1 = scenario6ScaleY.options[scenario6ScaleY.selectedIndex].value;
        var xskew1 = scenario6SkewX.options[scenario6SkewX.selectedIndex].value + 'deg';
        var yskew1 = scenario6SkewY.options[scenario6SkewY.selectedIndex].value + 'deg';
        // Keyframe2 values
        var xPosition2 = scenario62XPosition.options[scenario62XPosition.selectedIndex].value + 'px';
        var yPosition2 = scenario62YPosition.options[scenario62YPosition.selectedIndex].value + 'px';
        var zPosition2 = scenario62ZPosition.options[scenario62ZPosition.selectedIndex].value + 'px';
        var xrotate2 = scenario62RotateX.options[scenario62RotateX.selectedIndex].value + 'deg';
        var yrotate2 = scenario62RotateY.options[scenario62RotateY.selectedIndex].value + 'deg';
        var xscale2 = scenario62ScaleX.options[scenario62ScaleX.selectedIndex].value;
        var yscale2 = scenario62ScaleY.options[scenario62ScaleY.selectedIndex].value;
        var xskew2 = scenario62SkewX.options[scenario62SkewX.selectedIndex].value + 'deg';
        var yskew2 = scenario62SkewY.options[scenario62SkewY.selectedIndex].value + 'deg';
        // Keyframe3 values
        var xPosition3 = scenario63XPosition.options[scenario63XPosition.selectedIndex].value + 'px';
        var yPosition3 = scenario63YPosition.options[scenario63YPosition.selectedIndex].value + 'px';
        var zPosition3 = scenario63ZPosition.options[scenario63ZPosition.selectedIndex].value + 'px';
        var xrotate3 = scenario63RotateX.options[scenario63RotateX.selectedIndex].value + 'deg';
        var yrotate3 = scenario63RotateY.options[scenario63RotateY.selectedIndex].value + 'deg';
        var xscale3 = scenario63ScaleX.options[scenario63ScaleX.selectedIndex].value;
        var yscale3 = scenario63ScaleY.options[scenario63ScaleY.selectedIndex].value;
        var xskew3 = scenario63SkewX.options[scenario63SkewX.selectedIndex].value + 'deg';
        var yskew3 = scenario63SkewY.options[scenario63SkewY.selectedIndex].value + 'deg';
        var animationString = '@keyframes animation3D {'
            + 'from { '
            + 'opacity: 1;'
            + 'transform: translateX(' + xPosition1 + ')'
            + 'translateY(' + yPosition1 + ')'
            + 'translateZ(' + zPosition1 + ')'
            + 'rotateX(' + xrotate1 + ')'
            + 'rotateY(' + yrotate1 + ')'
            + 'scaleX(' + xscale1 + ') scaleY(' + yscale1 + ')'
            + 'skewX(' + xskew1 + ') skewY(' + yskew1 + ');'
            + '}'
            + '50% { '
            + 'opacity: 0;'
            + 'transform: translateX(' + xPosition2 + ')'
            + 'translateY(' + yPosition2 + ')'
            + 'translateZ(' + zPosition2 + ')'
            + 'rotateX(' + xrotate2 + ')'
            + 'rotateY(' + yrotate2 + ')'
            + 'scaleX(' + xscale2 + ') scaleY(' + yscale2 + ')'
            + 'skewX(' + xskew2 + ') skewY(' + yskew2 + ');'
            + '}'
            + 'to {'
            + 'opacity: 1; '
            + 'transform: translateX(' + xPosition3 + ')'
            + 'translateY(' + yPosition3 + ')'
            + 'translateZ(' + zPosition3 + ')'
            + 'rotateX(' + xrotate3 + ')'
            + 'rotateY(' + yrotate3 + ')'
            + 'scaleX(' + xscale3 + ') scaleY(' + yscale3 + ')'
            + 'skewX(' + xskew3 + ') skewY(' + yskew3 + ');'
            + '}';

        // Taking the selected values and applying them to the animation
        styleSheet.insertRule(animationString, 0);
        element5.style.transformOrigin = tranorigin;
        element5.style.animationDuration = duration1;
        element5.style.animationDelay = transdelay1;
        element5.style.animationTimingFunction = timingfun;

        // Triggering the animation by applying the animation-name
        window.setImmediate(function () {
            element5.style.animationName = 'animation3D';
        });

        // Creating output strings with selected values
        scenarioTransformValues = "animation-name: animation3D; ";
        scenarioDurationValues = "transition-duration: " + duration1 + ";";
        scenarioDelayValues = "transition-delay: " + transdelay1 + ";";
        scenarioTimFunValues = "transition-timing-function: " + timingfun + ";";
        scenarioKeysValues = animationString;
        scenarioTransOrig = "transform-origin: " + tranorigin + ";";
        scenarioKeyfrom = "from { opacity: 1; transform: translateX(" + xPosition1 + ") translateY(" + yPosition1 + ") translateZ(" + zPosition1 + ") rotateX(" + xrotate1 + ") rotateY(" + yrotate1 + ") scaleX(" + xscale1 + ") scaleY(" + yscale1 + ") skewX(" + xskew1 + ") skewY(" + yskew1 + ");}";
        scenarioKey50 = "50% { opacity: 0; transform: translateX(" + xPosition2 + ") translateY(" + yPosition2 + ") translateZ(" + zPosition2 + ") rotateX(" + xrotate2 + ") rotateY(" + yrotate2 + ") scaleX(" + xscale2 + ") scaleY(" + yscale2 + ") skewX(" + xskew2 + ") skewY(" + yskew2 + ");}";
        scenarioKeyto = "to { opacity: 1;  transform: translateX(" + xPosition3 + ") translateY(" + yPosition3 + ") translateZ(" + zPosition3 + ") rotateX(" + xrotate3 + ") rotateY(" + yrotate3 + ") scaleX(" + xscale3 + ") scaleY(" + yscale3 + ") skewX(" + xskew3 + ") skewY(" + yskew3 + ");}";

        // Reseting the animation once it is completed
        resetelement = element5;
        element5.addEventListener('animationend', resetPage, false);

    }

    // Keyframe1 flyout
    function keyframeflyout61() {
        var elementkeyframe = document.getElementById("keyframe51");
        elementkeyframe.style.visibility = 'visible';
        elementkeyframe.style.transform = 'translateX(200px)';
        elementkeyframe.style.opacity = '1';

        // Hide other keyframes
        var p1 = document.getElementById("keyframe52");
        var p2 = document.getElementById("keyframe53");
        p1.style.visibility = 'hidden';
        p2.style.visibility = 'hidden';
    }

    // Keyframe2 flyout
    function keyframeflyout62() {
        var elementkeyframe = document.getElementById("keyframe52");
        elementkeyframe.style.visibility = 'visible';
        elementkeyframe.style.transform = 'translateX(200px)';
        elementkeyframe.style.opacity = '1';

        // Hide other keyframes
        var p1 = document.getElementById("keyframe51");
        var p2 = document.getElementById("keyframe53");
        p1.style.visibility = 'hidden';
        p2.style.visibility = 'hidden';
    }

    // Keyframe3 flyout
    function keyframeflyout63() {
        var elementkeyframe = document.getElementById("keyframe53");
        elementkeyframe.style.visibility = 'visible';
        elementkeyframe.style.transform = 'translateX(200px)';
        elementkeyframe.style.opacity = '1';

        // Hide other keyframes
        var p1 = document.getElementById("keyframe51");
        var p2 = document.getElementById("keyframe52");
        p1.style.visibility = 'hidden';
        p2.style.visibility = 'hidden';
    }

    // Output from clicking button id scenario6Open
    function output6() {
        var element = document.getElementById("output");
        element.innerHTML = "/*CSS*/ <br />"
            + ".scenario6 { <br />"
            + g_tab + scenarioTransformValues + "<br />"
            + g_tab + scenarioDurationValues + "<br />"
            + g_tab + scenarioDelayValues + "<br />"
            + g_tab + scenarioTimFunValues + "<br />"
            + g_tab + scenarioTransOrig + "<br />"
            + "}<br />"
            + "@keyframes animation3D { <br />"
            + d_tab + scenarioKeyfrom + "<br />"
            + d_tab + scenarioKey50 + "<br />"
            + d_tab + scenarioKeyto + "<br />"
            + g_tab + "} <br />";
    }
})();
