//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario4.html", {
        ready: function (element, options) {
            document.getElementById("scenario4Open1").addEventListener("click", runscenario41, false);
            document.getElementById("scenario4Open2").addEventListener("click", runscenario42, false);
            document.getElementById("scenario4Open3").addEventListener("click", runscenario43, false);
            document.getElementById("scenario4Open1").addEventListener("click", output41, false);
            document.getElementById("scenario4Open2").addEventListener("click", output42, false);
            document.getElementById("scenario4Open3").addEventListener("click", output43, false);
        }
    });

    // Scenario4 Button 1: Transition Events
    function runscenario41() {
        var element3 = document.getElementById("scenario4InputChild");
        element3.style.transitionProperty = 'transform';
        element3.style.transitionDuration = '2s';
        element3.style.transformOrigin = '90%';
        element3.style.transform = 'rotateY(90deg)';
        resetelement = element3;
        element3.addEventListener('transitionend', resetPage2, false);
    }

    // Scenario 4 Button 2: Transition Events
    function runscenario42() {
        var element3 = document.getElementById("scenario4InputChild");
        element3.removeEventListener('transitionend', runscenario42, false);
        element3.style.transitionProperty = 'transform';
        element3.style.transitionDuration = '5s';
        element3.style.transformOrigin = '50%';
        element3.style.transform = 'scale(2)';
        resetelement = element3;
        element3.addEventListener('transitionend', resetPage2, false);
    }

    // Scenario 4 Button 3: Using Events
    function runscenario43() {
        var element4 = document.getElementById("scenario4InputChild");
        element4.style.transitionProperty = 'transform';
        element4.style.transitionDuration = '2s';
        element4.style.transformOrigin = '90%';
        element4.style.transform = 'rotateY(90deg)';
        resetelement = element4;
        element4.addEventListener('transitionend', runscenario42, false);
    }

    //Output from clicking button id scenario4Open1
    function output41() {
        var element = document.getElementById("output");
        element.innerHTML = "/*Javascript*/ <br />"
            + "function button1() { <br />"
            + g_tab + "var element = document.getElementById(scenario4); <br />"
            + g_tab + "element.style.transitionProperty = 'transform';	<br />"
            + g_tab + "element.style.transitionDuration = '2s'; <br />"
            + g_tab + "element.style.transformOrigin = '90%'; <br />"
            + "<pre>    element.style.transform = 'rotateY(90deg)'; </pre>"
            + "} <br />";
    }

    //Output from clicking button id scenario4Open2
    function output42() {
        var element = document.getElementById("output");
        element.innerHTML = "*Javascript*/<br />"
            + "function button2() { <br />"
            + g_tab + "var element = document.getElementById(scenario4);<br />"
            + g_tab + "element.style.transitionProperty = 'transform';	<br />"
            + g_tab + "element.style.transitionDuration = '2s';<br />"
            + g_tab + "element.style.transformOrigin = '90%';<br />"
            + "<pre>    element.style.transform = 'scale(2)'; </pre>"
            	+ "} <br />";
    }

    //Output from clicking button id scenario4Open3
    function output43() {
        var element = document.getElementById("output");
        element.innerHTML = "/*Javascript*/<br />"
            + "function button1() {<br />"
            + g_tab + "var element = document.getElementById(scenario4);<br />"
            + g_tab + "element.style.transitionProperty = 'transform';	<br />"
            + g_tab + "element.style.transitionDuration = '2s';<br />"
            + g_tab + "element.style.transformOrigin = '90%';<br />"
            + g_tab + "element.style.transform = 'rotateY(90deg)'; <br />"
            + "<pre>    element.addEventListener( 'transitionend', button2, false );</pre>"
            + "} <br />"
            + "function button2() { <br />"
            + g_tab + "var element = document.getElementById(scenario4);<br />"
            + g_tab + "element.style.transitionProperty = 'transform';	<br />"
            + g_tab + "element.style.transitionDuration = '2s';<br />"
            + g_tab + "element.style.transformOrigin = '90%';<br />"
            + g_tab + "element.style.transform = 'scale(2)'; <br />"
            + "}<br />";
    }
})();
