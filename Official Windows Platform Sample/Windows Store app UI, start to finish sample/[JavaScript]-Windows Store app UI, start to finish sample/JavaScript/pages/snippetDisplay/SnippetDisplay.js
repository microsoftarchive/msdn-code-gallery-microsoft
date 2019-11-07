// For an introduction to the Page Control template, see the following documentation:
// http://go.microsoft.com/fwlink/?LinkId=232511
(function () {
    "use strict";

    var controlConstructor = WinJS.UI.Pages.define("/pages/snippetDisplay/SnippetDisplay.html", {
        // This function is called whenever a user navigates to this page. It
        // populates the page elements with the app's data.
        ready: function (element, options) {
            // TODO: Initialize the page here.

            options = options || {};
            this._snippetDisplayElement = element.querySelector(".exampleDisplay");
            this._codeElementId = options.codeElementId;
            this._updateSnippetDisplay(); 
        },

        unload: function () {
            // TODO: Respond to navigations away from this page.
        },

        updateLayout: function (element) {
            /// <param name="element" domElement="true" />

            // TODO: Respond to changes in layout.
        },

        codeElementId: {
            get: function () { return this._codeElementId; },
            set: function(value) { this._codeElementId = value; }

        },

        _updateSnippetDisplay: function()
        {
            this._snippetDisplayElement.innerHTML = "";
            this._snippetDisplayElement.appendChild(document.createTextNode("\n"));
            this._snippetDisplayElement.appendChild(document.createTextNode(document.getElementById(this._codeElementId).outerHTML));
        }


    });

    WinJS.Namespace.define("SampleUtilities", {
        SnippetDisplay: controlConstructor
    });

})();


