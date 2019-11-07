(function () {
    "use strict";

    var scrollElement;
    var page = WinJS.UI.Pages.define("/html/scenario6_touchAction.html", {
        processed: function (element, options) {
                        
            document.getElementById("scaleElement").addEventListener("MSGestureChange", scaleHandler);
            document.getElementById("scaleElement").addEventListener("MSGestureEnd", endHandler);
            document.getElementById("scaleElement").addEventListener("pointerdown", downHandler);

            document.getElementById("horizontalElement").addEventListener("MSGestureChange", horizontalHandler);
            document.getElementById("horizontalElement").addEventListener("MSGestureEnd", endHandler);
            document.getElementById("horizontalElement").addEventListener("pointerdown", downHandler);

            document.getElementById("verticalElement").addEventListener("MSGestureChange", verticalHandler);
            document.getElementById("verticalElement").addEventListener("MSGestureEnd", endHandler);
            document.getElementById("verticalElement").addEventListener("pointerdown", downHandler);

            attachMouseKeyboard("TouchAction_Scroller");
        }
    });

    function getManipulationElement(element) {
        var retValue = element;
        while (!WinJS.Utilities.hasClass(retValue, "ManipulationElement")) {
            retValue = retValue.parentNode;
        }

        // Now that we found the correct element, ensure that it has 
        // been initialized.
        if (retValue.scale === null || typeof retValue.scale === "undefined") {
            retValue.scale = 1;
        }
        if (retValue.translationX === null || typeof retValue.translationX === "undefined") {
            retValue.translationX = 0;
        }
        if (retValue.translationY === null || typeof retValue.translationY === "undefined") {
            retValue.translationY = 0;
        }
        
        if (retValue.gestureObject === null || typeof retValue.gestureObject === "undefined") {
            retValue.gestureObject = new MSGesture();
            retValue.gestureObject.target = retValue;
        }
        return retValue;
    };

    
    // Function is responsible for responding to scale manipulations
    // and moving the object
    function scaleHandler(eventObject) {
        var target = getManipulationElement(eventObject.target);

        target.style.transitionDuration = "0ms";
        target.scale *= eventObject.scale;
        target.style.transform = "scale(" + target.scale + ")";
    }

    // Function is responsible for responding to horizontal swipe manipulations
    // and moving the object
    function horizontalHandler(eventObject) {
        var target = getManipulationElement(eventObject.target);

        target.style.transitionDuration = "0ms";
        target.translationX += eventObject.translationX;
        target.style.transform = "translate(" + target.translationX + "px, 0px)";
    }

    // Function is responsible for responding to vertical swipe manipulations
    // and moving the object
    function verticalHandler(eventObject) {
        var target = getManipulationElement(eventObject.target);

        target.style.transitionDuration = "0ms";
        target.translationY += eventObject.translationY;
        target.style.transform = "translate(0px," + target.translationY + "px)";
    }

    // Function is responsible for responding to the end of the manipulation and
    // moving the object back into its old position.
    function endHandler(eventObject) {
        var target = getManipulationElement(eventObject.target);

        target.style.transitionDuration = "500ms";
        target.style.transform = "matrix(1, 0, 0, 1, 0, 0)";
        target.scale = 1;
        target.translationX = 0;
        target.translationY = 0;

    }

    // Function is responsible for associating contacts with a gesture object.
    function downHandler(eventObject) {
        var target = getManipulationElement(eventObject.target);
        target.gestureObject.addPointer(eventObject.pointerId);
    }

})();


