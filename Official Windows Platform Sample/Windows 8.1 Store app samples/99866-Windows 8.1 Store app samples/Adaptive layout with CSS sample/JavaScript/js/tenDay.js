(function () {
    "use strict";

    function ready(element, options) {
        element.winControl = this;
        var that = this;
        WinJS.UI.process(element).done(function () {
            that.listView = element.querySelector(".tenDayListView").winControl;
            var itemTemplate = element.querySelector(".tenDayTemplate");
            var listViewLayout = new WinJS.UI.GridLayout();

            if (document.body.clientWidth <= 499) {
                listViewLayout = new WinJS.UI.ListLayout();
            }
            else {
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

        var listview = document.querySelector(".tenDayListView").winControl;

        if (document.body.clientWidth <= 499) {
            if (!(listview.layout instanceof WinJS.UI.ListLayout)) {
                listview.layout = new WinJS.UI.ListLayout();
            }
        }
        else {
            if (listview.layout instanceof WinJS.UI.ListLayout) {
                listview.layout = new WinJS.UI.GridLayout();
            }
        }

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
