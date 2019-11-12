//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var nav = WinJS.Navigation;
    var utils = WinJS.Utilities;

    WinJS.Namespace.define("GameManager", {
        PageControlNavigator: WinJS.Class.define(
            function (element, options) {
                this.element = element || document.createElement("div");
                // Need to create the pageHost element.
                this.element.appendChild(this._createPageElement());

                this.home = options.home;

                nav.onnavigated = this._navigated.bind(this);
                window.addEventListener("resize", this._viewstatechanged.bind(this));

                document.body.onkeyup = this._fwdbackHandler.bind(this);
                nav.navigate(this.home);
            }, {

                _createPageElement: function () {
                    var element = document.createElement("div");
                    element.style.width = "100%";
                    element.style.height = "100%";
                    return element;
                },

                _fwdbackHandler: function (e) {
                    switch (e.keyCode) {
                        case e.altKey && utils.Key.leftArrow: nav.back(); break;
                        case e.altKey && utils.Key.rightArrow: nav.forward(); break;
                    }
                },

                _viewstatechanged: function (e) {
                    this._updateLayout(this.pageElement);

                    // Restore game area to new aspect ratio
                    var gC = document.getElementById("gameCanvas");
                    if (gC) {
                        gC.width = window.innerWidth;
                        gC.height = window.innerHeight;
                    }
                },

                _navigated: function (e) {
                    var newElement = this._createPageElement();
                    var parentedComplete;
                    var parented = new WinJS.Promise(function (c) { parentedComplete = c; });

                    var that = this;
                    WinJS.UI.Pages.render(e.detail.location, newElement, e.detail.state, parented).
                        done(function (control) {
                            that.pageElement.winControl && that.pageElement.winControl.unload && that.pageElement.winControl.unload();
                            that.element.appendChild(newElement);
                            that.element.removeChild(that.pageElement);
                            parentedComplete();

                            that._updateLayout(newElement);
                            that.navigated();
                        });
                },

                _updateLayout: {
                    get: function () { return (this.pageControl && this.pageControl.updateLayout) || function () { }; }
                },

                navigated: function () {
                    // Do application specific on-navigated work here
                    var backButton = this.pageElement.querySelector("header[role=banner] .win-backbutton");
                    if (backButton) {
                        backButton.onclick = function () { nav.back(); };

                        if (nav.canGoBack) {
                            backButton.removeAttribute("disabled");
                        } else {
                            backButton.setAttribute("disabled", "disabled");
                        }
                    }
                },

                pageControl: {
                    get: function () { return this.pageElement && this.pageElement.winControl; }
                },

                pageElement: {
                    get: function () { return this.element.firstElementChild; }
                }
            }
        ),

        navigateHome: function () {
            var home = document.querySelector("#contentHost").winControl.home;
            var loc = nav.location;
            if (loc !== "" && loc !== home) {
                nav.navigate(home);
            }
        }
    });
})();
