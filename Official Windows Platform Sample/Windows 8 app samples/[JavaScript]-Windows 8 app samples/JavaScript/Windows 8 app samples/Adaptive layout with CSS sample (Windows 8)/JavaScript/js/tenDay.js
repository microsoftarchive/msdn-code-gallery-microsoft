(function () {
    "use strict";

    var lastViewState = null;

    function ready(element, options) {
        element.winControl = this;
        var that = this;
        WinJS.UI.process(element).done(function () {
            that.listView = element.querySelector(".tenDayListView").winControl;
            var itemTemplate = element.querySelector(".tenDayTemplate");
            var listViewLayout = new WinJS.UI.GridLayout();
            lastViewState = Windows.UI.ViewManagement.ApplicationView.value;
            if (Windows.UI.ViewManagement.ApplicationView.value === Windows.UI.ViewManagement.ApplicationViewState.snapped) {
                listViewLayout = new WinJS.UI.ListLayout();
            } else {
                element.style.opacity = "0";
            }

            WinJS.UI.setOptions(that.listView, {
                itemTemplate: itemTemplate,
                itemDataSource: new WinJS.Binding.List(options.data).dataSource,
                layout: listViewLayout,
                selectionMode: 'none',
                tap: 'none',
                crossSlide: 'none'
            });

            window.addEventListener("resize", tenDayResize, false);
        });
    }

    function tenDayResize(e) {
        var currentViewState = Windows.UI.ViewManagement.ApplicationView.value;
        var snapped = Windows.UI.ViewManagement.ApplicationViewState.snapped;
        var listView = document.querySelector(".tenDayListView").winControl;
        
        if (currentViewState === snapped) {
            listView.layout = new WinJS.UI.ListLayout(); 
        }
        else if (lastViewState === snapped && currentViewState !== snapped) {
            listView.layout = new WinJS.UI.GridLayout();
        }

        lastViewState = currentViewState;
    }
    
    function clearTenDayResize() {
        window.removeEventListener("resize", tenDayResize, false);
    }
    
    function updateView(options) {
        this.listView.itemDataSource = new WinJS.Binding.List(options.data).dataSource;
    }

    WinJS.UI.Pages.define("/html/tenDay.html", {
        listView: null,
        ready: ready, 
        updateView: updateView,
        clearTenDayResize: clearTenDayResize
    });

})();
