//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario1.html", {
        ready: function (element, options) {
            document.getElementById("scenario1Open").addEventListener("click", runscenario1, false);

            var range = document.getElementById('counterRange');
            range.value = uiThreadWorkHighRange;

            range.addEventListener("change", function () { uiThreadWorkHighRange = range.value; });
        }, unload: function () {
            stopUiThreadWork();
        }

        
    });

    var startButtonText = "Run Scenario 1", stopButtonText = "Stop";

    var uiThreadWorkHighRange = 0;

    var continueUiThreadWork = false;

    function runscenario1() {
        var button = document.getElementById("scenario1Open");

        if (button.innerText === startButtonText) {

            button.innerText = stopButtonText;

            startUiThreadWork();

            var IndependentAnimationelement = document.getElementById("IndependentAnimationBall");
            var DependentAnimation = document.getElementById("DependentAnimationBall");

            IndependentAnimationelement.className = 'ball';
            DependentAnimation.className = 'ball';
            IndependentAnimationelement.style.animationPlayState = 'running';
            DependentAnimation.style.animationPlayState = 'running';

        } else {

            button.innerText = startButtonText;
            resetScenario1();
        }
    }

    function startUiThreadWork() {

        continueUiThreadWork = true;

        window.requestAnimationFrame(doWork);
    }

    function stopUiThreadWork() {

        continueUiThreadWork = false;

    }

    function doWork() {

        var startTime = new Date().getTime();

        while (new Date().getTime() - startTime < uiThreadWorkHighRange) { }

        if (continueUiThreadWork) {
            var awakeTime = 250 - uiThreadWorkHighRange;
            setTimeout(doWork, awakeTime);
        }
    }

    function setDependencyLabel(targetLabel, elementClass) {

        var element = document.querySelector(elementClass);

        if (window.console.msIsIndependentlyComposed) {
            if (window.console.msIsIndependentlyComposed(element)) {
                targetLabel.textContent = "Independent";
            }
            else {
                targetLabel.textContent = "Dependent";
            }
        } else {
            targetLabel.textContent = "";
        }
    }


    function resetScenario1() {
        var IndependentAnimationelement = document.getElementById("IndependentAnimationBall");
        var DependentAnimation = document.getElementById("DependentAnimationBall");
        IndependentAnimationelement.className = 'ballstop';
        DependentAnimation.className = 'ballstop';
        stopUiThreadWork();
    }

})();
