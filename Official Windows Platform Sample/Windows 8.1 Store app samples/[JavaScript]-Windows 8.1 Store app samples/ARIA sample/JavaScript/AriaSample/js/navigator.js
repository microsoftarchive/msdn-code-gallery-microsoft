(function () {
    "use strict";

    var nav = WinJS.Navigation;
    var ui = WinJS.UI;
    var utils = WinJS.Utilities;

    WinJS.Namespace.define("AriaSample", {
        PageControlNavigator: WinJS.Class.define(
            // Define the constructor function for the PageControlNavigator.
            function PageControlNavigator(element, options) {
                this.element = element || document.createElement("div");
                this.element.appendChild(this._createPageElement());

                this.home = options.home;

                nav.onnavigated = this._navigated.bind(this);
                window.onresize = this._resized.bind(this);

                document.body.onkeyup = this._keyupHandler.bind(this);
                document.body.onkeypress = this._keypressHandler.bind(this);
                nav.navigate(this.home);
            }, {
                /// <field domElement="true" />
                element: null,
                home: "",

                // This function creates a new container for each page.
                _createPageElement: function () {
                    var element = document.createElement("div");
                    element.style.width = "100%";
                    element.style.height = "100%";
                    return element;
                },

                // This function responds to keypresses to only navigate when
                // the backspace key is not used elsewhere.
                _keypressHandler: function (args) {
                    if (args.key === "Backspace"){
                        nav.back();
                    }
                },

                // This function responds to keyup to enable keyboard navigation.
                _keyupHandler: function (args) {
                    if ((args.key === "Left" && args.altKey) || (args.key === "BrowserBack")) {
                        nav.back();
                    } else if ((args.key === "Right" && args.altKey) || (args.key === "BrowserForward")) {
                        nav.forward();
                    }
                },

                // This function responds to navigation by adding new pages
                // to the DOM.
                _navigated: function (args) {
                    if (this.pageElement.winControl && this.pageElement.winControl.unload) {
                        this.pageElement.winControl.unload();
                    }

                    var newElement = this._createPageElement();
                    var parentedComplete;
                    var parented = new WinJS.Promise(function (c) { parentedComplete = c; });
                    var that = this;

                    WinJS.UI.Pages.render(args.detail.location, newElement, args.detail.state, parented).
                        done(function parentElement(control) {
                            that.element.appendChild(newElement);
                            that.element.removeChild(that.pageElement);
                            parentedComplete();
                            document.body.focus();
                            that.navigated();
                        });
                },

                _resized: function (args) {
                    if (this.pageControl && this.pageControl.updateLayout) {
                        this.pageControl.updateLayout.call(this.pageControl, this.pageElement);
                    }
                },

                // This function updates application controls once a navigation
                // has completed.
                navigated: function () {
                    // Do application specific on-navigated work here
                    var backButton = this.pageElement.querySelector("header[role=banner] .win-backbutton");
                    if (backButton) {
                        backButton.onclick = function () { nav.back(); };

                        if (nav.canGoBack) {
                            backButton.removeAttribute("disabled");
                        }
                        else {
                            backButton.setAttribute("disabled", "disabled");
                        }
                    }
                },

                // This is the PageControlNavigator object.
                pageControl: {
                    get: function () { return this.pageElement && this.pageElement.winControl; }
                },

                // This is the root element of the current page.
                pageElement: {
                    get: function () { return this.element.firstElementChild; }
                }
            }
        ),

        // This function navigates to the home page which is defined when the
        // control is created.
        navigateHome: function () {
            var home = document.querySelector("#contenthost").winControl.home;
            var loc = nav.location;
            if (loc !== "" && loc !== home) {
                nav.navigate(home);
            }
        },
    });
})();
