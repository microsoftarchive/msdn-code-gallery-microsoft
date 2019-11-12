(function () {
    "use strict";

    function ready(element, options) {
        element.winControl = this;
        var that = this;
        WinJS.UI.process(element).then(function () {
            that.element = element;
            var template = element.querySelector(".currentTemplate").winControl;
            var e = element.querySelector(".currentContent");
            template.render(options.data).done(function (renderElement) {
                e.appendChild(renderElement);
            });
            if(document.body.clientWidth <= 499){
                element.style.opacity = "0";
            }
        });
    }

    function updateView(options) {
        var template = this.element.querySelector(".currentTemplate").winControl;
        var e = this.element.querySelector(".currentContent");
        template.render(options.data).done(function (element) {
            e.innerHTML = "";
            e.appendChild(element);
            WinJS.UI.Animation.enterPage(e);
        });
    }

    WinJS.UI.Pages.define("/html/current.html", {
        element: null, 
        ready: ready,
        updateView: updateView
    });
})();
