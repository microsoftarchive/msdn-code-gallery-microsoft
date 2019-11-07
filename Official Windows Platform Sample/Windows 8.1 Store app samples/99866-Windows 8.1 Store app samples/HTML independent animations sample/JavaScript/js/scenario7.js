//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario7.html", {
        ready: function (element, options) {
            document.getElementById("scenario7Open1").addEventListener("click", runscenario71, false);
            document.getElementById("scenario7Open2").addEventListener("click", runscenario72, false);

            document.getElementById("scenario7Open1").addEventListener("click", output71, false);
            document.getElementById("scenario7Open2").addEventListener("click", output72, false);
        }
    });

    // Button 1 Animation Event
    function runscenario71() {
        var styleSheet = document.styleSheets[1];
        var element1 = document.getElementById("ballcontainer");
        var animationString = '@keyframes bounce1 {'
            + 'from {'
            + 'transform: translateY(0)'
            + 'animation-timing-function: ease-out;'
            + '}'
            + '25% {'
            + 'transform: translateY(400px);'
            + '}'
            + '50% {'
            + 'transform: translateY(-100px);'
            + '}'
            + '75% {'
            + 'transform: translateY(400px);'
            + '}'
            + 'to { '
            + 'transform: translateY(0px)'
            + 'animation-timing-function: ease-in;'
            + '}';

        // Applying values to the animation
        styleSheet.insertRule(animationString, 0);
        element1.style.animationDuration = '2s';

        // Triggering the animation by setting the animation-name
        window.setImmediate(function () {
            element1.style.animationName = 'bounce1';
        });

        // Creating strings for output
        scenarioTransformValues = 'animation-name: bounce1;';
        scenarioDurationValues = 'animation-duation: 2s;';
        scenarioKeysValues = animationString;

        // Reseting the animation
        resetelement = element1;
        element1.addEventListener('animationend', resetPage, false);
    }

    // Button 2 Animation Event
    function runscenario72() {
        var styleSheet = document.styleSheets[1];
        var element2 = document.getElementById("ballcontainer");
        var animationString = '@keyframes bounce2 {'
            + 'from {'
            + 'transform: translateY(0)'
            + 'animation-timing-function: ease-out;'
            + '}'
            + '25% {'
            + 'transform: translateY(400px);'
            + '}'
            + '50% {'
            + 'transform: translateY(-100px);'
            + '}'
            + '75% {'
            + 'transform: translateY(400px);'
            + '}'
            + 'to { '
            + 'transform: translateY(0px)'
            + 'animation-timing-function: ease-in;'
            + '}';

        // Applying values to the animation
        styleSheet.insertRule(animationString, 0);
        element2.style.animationDuration = '2s';

        // Triggering the animation by setting the animation-name
        window.setImmediate(function () {
            element2.style.animationName = 'bounce2';
        });

        // Creating strings for output
        scenarioTransformValues = 'animation-name: bounce2;';
        scenarioDurationValues = 'animation-duation: 2s;';
        scenarioKeysValues = animationString;

        // Reseting the animation
        resetelement = element2;
        element2.addEventListener('animationend', setFinalAnimation, false);
    }

    // This function is called as a result of the animationend event from Button 2
    function setFinalAnimation() {
        var styleSheet = document.styleSheets[1];
        var element3 = document.getElementById("ballcontainer");
        element3.removeEventListener('animationend', setFinalAnimation, false);
        var animationString2 = '@keyframes scenario73 {'
            + 'from {'+ 'transform: rotate(0)'+ '}'
            + '50% {'
            + 'transform:rotate(90deg)'+ '}'
            + 'to { '+ 'transform: rotate(360deg)'+ '}';

        // Applying values to the animation
        styleSheet.insertRule(animationString2, 0);
        element3.style.animationDuration = '2s';

        // Triggering the animation by setting the animation-name
        window.setImmediate(function () {
            element3.style.animationName = 'scenario73';
        });

        // Creating strings for output
        animation2KeysValues = animationString2;

        // Reseting the animation
        resetelement = element3;
        element3.addEventListener('animationend', resetPage, false);
        return (animation2KeysValues);
    }

    // Output from clicking Button id scenario7Open1
    function output71() {
        var element = document.getElementById("output");
        element.innerHTML = "/*JavaScript*/<br />"
            + "function bounce() { <br />"
            + g_tab + "var element1 = document.getElementById('ballcontainer'); <br />"
            + g_tab + "element1.className = 'scenario7bounce'; <br />"
            + "} <br />"
            + "/*CSS*/<br />"
            + ".scenario7bounce { <br />"
            + g_tab + "animation-name: bounce; <br />"
            + g_tab + "animation-duration: 2s; <br />"
            + "} <br />"
            + "@keyframes bounce { <br />"
            + g_tab + "from { transform: translateY(0); <br />"
            + d_tab + "animation-timing-function: ease-out; } <br />"
            + g_tab + "25% { transform: translateY(400px);} <br />"
            + g_tab + "50% { transform: translateY(-100px);} <br />"
            + g_tab + "75% { transform: translateY(400px);} <br />"
            + g_tab + "to { transform: translateY(0px) <br />"
            + d_tab + " animation-timing-function: ease-in;} <br />"
            + "}; <br />";
    }

    // Output from clicking Button id scenario7Open2
    function output72() {
        var element = document.getElementById("output");
        element.innerHTML = "/*JavaScript for bounce animation*/<br />"
            + "function bounce() { <br />"
            + g_tab + "var element1 = document.getElementById('ballcontainer'); <br />"
            + g_tab + "element1.className = 'scenario7bounce'; <br />"
            + "<pre> element.addEventListener( 'animationend', spin, false ); } </pre>"
            + "} <br />"
            + "/*JavaScript spin animation*/<br />"
            + "function spin() { <br />"
            + g_tab + "var element1 = document.getElementById('ballcontainer'); <br />"
            + g_tab + "element1.className = 'scenario7spin'; <br />"
            + "} <br />"
            + "/*CSS for bounce animation*/<br />"
            + ".scenario7bounce { <br />"
            + g_tab + "animation-name: bounce; <br />"
            + g_tab + "animation-duration: 2s; <br />"
            + "} <br />"
            + "@keyframes bounce { <br />"
            + g_tab + "from { transform: translateY(0); <br />"
            + d_tab + "animation-timing-function: ease-out; } <br />"
            + g_tab + "25% { transform: translateY(400px);} <br />"
            + g_tab + "50% { transform: translateY(-100px);} <br />"
            + g_tab + "75% { transform: translateY(400px);} <br />"
            + g_tab + "to { transform: translateY(0px); <br />"
            + d_tab + " animation-timing-function: ease-in;}  <br />"
            + "}; <br />"

            + "/*CSS for spin animation*/<br />"
            + ".scenario7spin { <br />"
            + g_tab + "animation-name: spin; <br />"
            + g_tab + "animation-duration: 2s; <br />"
            + "} <br />"
            + "@keyframes spin { <br />"
            + g_tab + "from { transform: rotate(0);}<br />"
            + g_tab + "50% { transform:rotate(90deg);} <br />"
            + g_tab + "75% { transform: translateY(400px);} <br />"
            + g_tab + "to { transform: rotate(360deg);} <br />"
            +"};<br />";
    }
})();
