(function () {
    "use strict";

	var ui = WinJS.UI;
	var utils = WinJS.Utilities;
	var lightGray = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsQAAA7EAZUrDhsAAAANSURBVBhXY7h4+cp/AAhpA3h+ANDKAAAAAElFTkSuQmCC";

    function ready(element, options) {
        var /*@override*/ item = options && options.item ? options.item : data.items.getAt(0);

        element.querySelector(".titlearea .pagetitle").textContent = "Error Messages:";
        element.querySelector("article .item-title").textContent = item.title;
        element.querySelector("article .item-subtitle").textContent = item.subtitle;
        element.querySelector("article .item-image").src = lightGray;
        element.querySelector("article .item-content").innerHTML = item.content; 
    }

    ui.Pages.define("/html/error.html", {
        ready: ready
    });
})();
