(function () {
    "use strict";

    var resizeListener = null;
    var smallState = false;

    function ready() {
        var backButton = document.querySelector(".win-backbutton");
        backButton.addEventListener("click", function (e) {
            var fs = document.getElementById("full-screen");
            window.removeEventListener("resize", resize, false);
            WinJS.UI.Animation.exitContent(fs, null).done(function () {
                document.getElementById("ten-day-page").winControl.clearTenDayResize();
                document.body.removeChild(fs);
            });
        });

        if (document.body.clientWidth <= 499) {
            smallState = true;
            document.getElementById("tenDay").winControl.selected = true;
            document.getElementById("current").winControl.selected = false;
        } else {
            smallState = false;
            document.getElementById("current").winControl.selected = true;
            document.getElementById("tenDay").winControl.selected = false;
        }

        window.addEventListener("resize", resize, false);
        initializeMenu();
    }

    function resize(e) {
        if (document.body.clientWidth <= 499 && !smallState) {
            smallState = true;
            transitionPivot();
        } else if (document.body.clientWidth > 499 && smallState) {
            smallState = false;
            transitionPivot();
        }
    }

    function initializeMenu() {
        var title = document.querySelector("header .titlearea");
        title.addEventListener("click", function _click(e) {
            var flyout = document.getElementById("headerFlyout").winControl;
            flyout.anchor = title;
            flyout.placement = "bottom";
            flyout.alignment = "left";
            flyout.show();
        });

        var headerFlyout = document.getElementById("headerFlyout");
        for (var i = 0; i < headerFlyout.children.length; i++) {
            headerFlyout.children[i].addEventListener("click", function () {
                var element = event.srcElement;
                document.querySelector(".pagesubtitle").textContent = element.innerText;
                headerFlyout.winControl.hide();
                loadView(element.innerText);
            });
        }
    }

    function loadView(view) {
        // Get the view data index
        var viewDataIndex = FluidAppLayout.Data.dataTable[view];

        // Set the currentView and currentViewData
        var currentView = FluidAppLayout.Data.mountains[viewDataIndex].name;
        var currentViewData = FluidAppLayout.Data.mountains[viewDataIndex].weatherData;

        // Rebind the data to the already loaded pages
        document.getElementById("current-page").winControl.updateView({ "data": currentViewData[0] });
        document.getElementById("ten-day-page").winControl.updateView({ "data": currentViewData });
    }

    function transitionPivot(e) {
        var isTenDayView = document.getElementById("tenDay").winControl.selected;

        if (isTenDayView) {
            document.getElementById("tenDay").winControl.selected = false;
            document.getElementById("current").winControl.selected = true;
            WinJS.UI.Animation.exitContent(document.getElementById("ten-day-page"), null).done(function () {
                WinJS.UI.Animation.enterContent(document.getElementById("current-page"), null).done();
            });
        } else {
            document.getElementById("current").winControl.selected = false;
            document.getElementById("tenDay").winControl.selected = true;
            WinJS.UI.Animation.exitContent(document.getElementById("current-page"), null).done(function () {
                WinJS.UI.Animation.enterContent(document.getElementById("ten-day-page"), null).done();
            });
        }
    }

    WinJS.Utilities.markSupportedForProcessing(transitionPivot);

    WinJS.Namespace.define('FluidAppLayout', {
        loadView: loadView,
        transitionPivot: transitionPivot
    });

    var page = WinJS.UI.Pages.define("/html/app.html", {
        ready: ready
    });
})();
