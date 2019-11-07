//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var reapplyColor = false;

    var page = WinJS.UI.Pages.define("/html/scenario1.html", {
        ready: function (element, options) {
            document.getElementById("applyColor").addEventListener("click", applyColors);
            document.getElementById("resetColor").addEventListener("click", resetColorValues);
            document.getElementById("baseTheme").addEventListener("change", setDefaultStyleSheet);
            
            // Prevent form from actually submitting
            document.getElementById("colorInputGrid").addEventListener("submit", function (e) {
                e.preventDefault();
            });

            // Wire up RGB channels to dynamically update color swatches
            var channels = document.querySelectorAll(".channelTextbox");
            for (var i = 0; i < channels.length; i++) {
                channels[i].addEventListener("change", function (e) {
                    swatchLookUp[e.srcElement.id]();
                });
            }
        }
    });

    // Check to make sure that channel values are between
    function checkColorValues() {
        if (this.validity.rangeOverflow) {
            this.value = "255";
        } else if (this.validity.rangeUnderflow) {
            this.value = "0";
        }
    }


    // Using our template CSS, replace placeholder values with RGB strings, apply to the HTML document, and output to text area
    function applyColors() {
        reapplyColor = true;
        var white = new Color("255", "255", "255");
        var primaryColor = new Color(document.getElementById('pcR').value, document.getElementById('pcG').value, document.getElementById('pcB').value);
        var primaryTextColor = new Color(document.getElementById('ptcR').value, document.getElementById('ptcG').value, document.getElementById('ptcB').value);
        var secondaryColor = new Color(document.getElementById('scR').value, document.getElementById('scG').value, document.getElementById('scB').value);
        var secondaryTextColor = new Color(document.getElementById('stcR').value, document.getElementById('stcG').value, document.getElementById('stcB').value);

        var primaryAlphaColor = primaryColor.blendColor(0.13);
        var secondaryAlphaColor = secondaryColor.blendColor(0.13);

        var template = document.getElementById("colorRulesTemplate");
        var apply = template.innerText;

        // Set up color swatches
        document.getElementById("primaryColorSwatch").style.backgroundColor = primaryColor.toString();
        document.getElementById("primaryTextColorSwatch").style.backgroundColor = primaryTextColor.toString();
        document.getElementById("secondaryColorSwatch").style.backgroundColor = secondaryColor.toString();
        document.getElementById("secondaryTextColorSwatch").style.backgroundColor = secondaryTextColor.toString();

        // Replace CSS template with color values
        apply = apply.replace(/\?PRIMARY\?/g, primaryColor.toString());
        apply = apply.replace(/\?PRIMARY_TEXT\?/g, primaryTextColor.toString());
        apply = apply.replace(/\?PRIMARY_ALPHA\?/g, primaryAlphaColor.toString());
        apply = apply.replace(/\?SECONDARY\?/g, secondaryColor.toString());
        apply = apply.replace(/\?SECONDARY_TEXT\?/g, secondaryTextColor.toString());
        apply = apply.replace(/\?SECONDARY_ALPHA\?/g, secondaryAlphaColor.toString());

        // Ensure button borders are the same color as the background if we are using ui-light.css
        if (document.getElementById("baseTheme").value === "Light") {
            apply = apply.replace(/\?SECONDARY_UI_LIGHT\?/g, secondaryColor.toString());
            apply = apply.replace(/\?SECONDARY_ALPHA_UI_LIGHT\?/g, secondaryAlphaColor.toString());
        }
        else {
            apply = apply.replace(/\?SECONDARY_UI_LIGHT\?/g, "currentColor");
            apply = apply.replace(/\?SECONDARY_ALPHA_UI_LIGHT\?/g, "currentColor");
        }

        document.getElementById("appliedColorRules").innerText = apply;
        document.getElementById("cssRulesOutput").innerText = apply;
    }

    // Reset the color values and CSS output
    function resetColorValues() {
        var inputs = document.querySelectorAll("input[type='number']");
        for (var i = 0; i < inputs.length; i++) {
            inputs[i].value = "0";
        }
        document.getElementById("appliedColorRules").innerText = "";
        document.getElementById("cssRulesOutput").innerText = "";

        // Set up color swatches
        document.getElementById("primaryColorSwatch").style.backgroundColor = "#000";
        document.getElementById("primaryTextColorSwatch").style.backgroundColor = "#000";
        document.getElementById("secondaryColorSwatch").style.backgroundColor = "#000";
        document.getElementById("secondaryTextColorSwatch").style.backgroundColor = "#000";

        reapplyColor = false;
    }

    // Sets the base default style sheet for the application
    function setDefaultStyleSheet(e) {
        var element = e.srcElement;
        if (element.value === "Dark") {
            swapStyleSheet("ui-light.css", "ui-dark.css");
        }
        else {
            swapStyleSheet("ui-dark.css", "ui-light.css");
        }

        if(reapplyColor){
            applyColors();
        }
    }

    // Replaces the base style sheet
    function swapStyleSheet(oldStyleSheet, newStyleSheet) {
        var linkTag = document.querySelector("link[href$='" + oldStyleSheet + "']");
        var index = linkTag.href.lastIndexOf("/");
        var baseURL = linkTag.href.slice(0, index + 1);
        linkTag.href = baseURL += newStyleSheet;
    }

    // Update color swatch
    function updateSwatch(r, g, b, swatch){
        var color = new Color(document.getElementById(r).value, document.getElementById(g).value, document.getElementById(b).value);
        document.getElementById(swatch).style.backgroundColor = color.toString();
    }

    // Color helper class
    var Color = WinJS.Class.define(function (r, g, b, a) {
        this._r = r;
        this._g = g;
        this._b = b;
        this._a = a;
    },
    {
        _r: null,
        _g: null,
        _b: null,
        _a: null,
        
        // Outputs a CSS color string
        toString: function () {
            var color = null;
            if (this._a !== undefined && this._a !== null) {
                color = "rgba(" + this._r + ", " + this._g + ", " + this._b + ", " + this._a + ")";
            } else {
                color = "rgb(" + this._r + ", " + this._g + ", " + this._b  + ")";
            }
            return color;
        },

        // Returns an alpha blended Color
        blendColor: function (alpha) {
            var fgR = 1;
            var fgG = 1;
            var fgB = 1;
            var bgR = this._r / 255;
            var bgG = this._g / 255;
            var bgB = this._b / 255;
            var resultR = Math.round(((alpha * fgR) + (1 - alpha) * bgR) * 255);
            var resultG = Math.round(((alpha * fgG) + (1 - alpha) * bgG) * 255);
            var resultB = Math.round(((alpha * fgB) + (1 - alpha) * bgB) * 255);
            return new Color(resultR, resultG, resultB);
        }
    });

    // Function table for dynamically updating color swatches
    var swatchLookUp = {
        "pcR": function () {
            updateSwatch("pcR", "pcG", "pcB", "primaryColorSwatch");
        },
        "pcG": function () {
            updateSwatch("pcR", "pcG", "pcB", "primaryColorSwatch");
        },
        "pcB": function () {
            updateSwatch("pcR", "pcG", "pcB", "primaryColorSwatch");
        },
        "ptcR": function () {
            updateSwatch("ptcR", "ptcG", "ptcB", "primaryTextColorSwatch");
        },
        "ptcG": function () {
            updateSwatch("ptcR", "ptcG", "ptcB", "primaryTextColorSwatch");
        },
        "ptcB": function () {
            updateSwatch("ptcR", "ptcG", "ptcB", "primaryTextColorSwatch");
        },
        "scR": function () {
            updateSwatch("scR", "scG", "scB", "secondaryColorSwatch");
        },
        "scG": function () {
            updateSwatch("scR", "scG", "scB", "secondaryColorSwatch");
        },
        "scB": function () {
            updateSwatch("scR", "scG", "scB", "secondaryColorSwatch");
        },
        "stcR": function () {
            updateSwatch("stcR", "stcG", "stcB", "secondaryTextColorSwatch");
        },
        "stcG": function () {
            updateSwatch("stcR", "stcG", "stcB", "secondaryTextColorSwatch");
        },
        "stcB": function () {
            updateSwatch("stcR", "stcG", "stcB", "secondaryTextColorSwatch");
        }
    };

})();
