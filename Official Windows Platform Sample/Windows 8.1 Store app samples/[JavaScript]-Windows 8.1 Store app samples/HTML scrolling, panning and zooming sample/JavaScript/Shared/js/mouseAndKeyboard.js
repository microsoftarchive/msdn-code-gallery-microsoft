//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

var attachMouseKeyboard;
(function () {
    "use strict";

    attachMouseKeyboard = function (id) {

        var manipulationElement;
        var manipulationContent;
        var anchorElement;

        manipulationElement = document.getElementById(id);

        // Create a wrapper around the content so we have an element that we can apply
        // transforms and animations to.
        manipulationContent = document.createElement("div");
        manipulationContent.style["position"] = "relative";
        for (var i = manipulationElement.childNodes.length - 1; i >= 0; i--) {
            manipulationContent.appendChild(manipulationElement.childNodes[i]);
        }
        manipulationElement.appendChild(manipulationContent);
        

        // Create an element that we can position in order to use to scrollInToView
        // so that we can set scrollLeft and scrollTop atomically.
        anchorElement = document.createElement("div");
        anchorElement.className = "scrollAnchor";
        manipulationContent.appendChild(anchorElement);
        

        manipulationElement.addEventListener("keydown", handleKeydown);
        manipulationElement.addEventListener("wheel", handleWheel);
    };

    function getScroller(element) {
        var retValue = element;
        var currentStyle = window.getComputedStyle(retValue);
        while (!(currentStyle.getPropertyValue("overflow-x") === "scroll" || (currentStyle.getPropertyValue("overflow-x") === "auto" && retValue.scrollWidth > 0)) &&
            !(currentStyle.getPropertyValue("overflow-y") === "scroll" || (currentStyle.getPropertyValue("overflow-y")=== "auto" && retValue.scrollHeight > 0))) {
            
            retValue = retValue.parentNode;
            currentStyle = window.getComputedStyle(retValue);
        }
        return retValue;
    };

    // Handler is responsible determining the axis and the distance for a given
    // keystroke.
    function handleKeydown(eventObject) {

        var axis = -1;
        var distance;
        var Key = WinJS.Utilities.Key;
        switch (eventObject.keyCode) {
            case Key.leftArrow:
                axis = AXIS_ENUM.X;
                distance = -1;
                break;
            case Key.rightArrow:
                axis = AXIS_ENUM.X;
                distance = 1;
                break;
            case Key.upArrow:
                axis = AXIS_ENUM.Y;
                distance = 1;
                break;
            case Key.downArrow:
                axis = AXIS_ENUM.Y;
                distance = -1;
                break;
            case Key.add:
            case Key.equal:
                if (eventObject.ctrlKey) {
                    axis = AXIS_ENUM.Zoom;
                    distance = 1;
                } else {
                    return;
                }
                break;
            case Key.subtract:
            case Key.dash:
                if (eventObject.ctrlKey) {
                    axis = AXIS_ENUM.Zoom;
                    distance = -1;
                } else {
                    return;
                }
                break;
            default:
                return;
        }


        updateManipulationContext(getScroller(eventObject.srcElement));
        distance *= manipulationContext[axis].DistancePerKeystroke;



        if (axis === AXIS_ENUM.Zoom) {
            
            manipulationContext.manipulationType = MANIPULATION_TYPE_ENUM.SCALE;
        } else {
            manipulationContext.manipulationType = MANIPULATION_TYPE_ENUM.TRANSLATE;
        }

        // Since we are handling a key stroke, we need to set the origin for the transform to be the center 
        // of the zooming element.
        manipulationContext[AXIS_ENUM.X].transformOrigin = manipulationContext[AXIS_ENUM.X].size / 2;
        manipulationContext[AXIS_ENUM.Y].transformOrigin = manipulationContext[AXIS_ENUM.Y].size / 2;

        if (!manipulationContext[axis].useDefaultSupport) {
            nextPosition(distance, INPUT_TYPE_ENUM.Keyboard, axis);

            if (manipulationContext.hasPositionChanged()) {
                eventObject.preventDefault();
                eventObject.stopPropagation();
            }
        }
        saveManipulationContext();
    };

    // Handler is responsible for determining the axis and the distance for a given
    // wheel message.
    function handleWheel(eventObject) {

        updateManipulationContext(getScroller(eventObject.srcElement));

        var axis = -1;
        var distance;

        // since we have a number of cases to check that might handle the YDelta,
        // let us assume it gets handled and only set it to false if we don't
        var handledDelta = false;

        if (eventObject.deltaY !== 0) {

            // We need to look and see if we are in a situation that could cause us 
            // to want to handle the YDelta in different ways.  Those ways (and the
            // conditions for when they apply) are:
            //      
            //      Zooming: Control key needs to be depressed and zoom needs to 
            //               be possible.
            //
            //      Vertical Scrolling: Vertical scrolling needs to be possible and
            //               there needs to be vertical mandatory snap-points.
            //
            //      Horizontal Scrolling: Horizontal scrolling needs to be possible
            //               and there needs to be horizontal mandatory  snap-points.
            if (eventObject.ctrlKey && !manipulationContext[AXIS_ENUM.Zoom].useDefaultSupport) {
                axis = AXIS_ENUM.Zoom;
            } else if (!manipulationContext[AXIS_ENUM.Y].useDefaultSupport) {
                axis = AXIS_ENUM.Y;
            } else if (!manipulationContext[AXIS_ENUM.X].useDefaultSupport) {
                axis = AXIS_ENUM.X;
            }
        }

        if (axis !== -1) {
            distance = eventObject.deltaY * manipulationContext[axis].DistancePerWheelDelta;
        }

        // Look to see if wee need to handle the YDelta
        if (axis !== -1 && eventObject.deltaX !== 0) {
            axis = AXIS_ENUM.X;
            distance = eventObject.deltaX * manipulationContext[axis].DistancePerWheelDelta;
            handledDelta = true;
        }


        if (axis === -1) {
            return;
        }
        else if (axis === AXIS_ENUM.Zoom) {

            manipulationContext[AXIS_ENUM.X].transformOrigin = eventObject.offsetX;
            manipulationContext[AXIS_ENUM.Y].transformOrigin = eventObject.offsetY;
            var tempElement = eventObject.srcElement;

            // Walk up to accumulate the actual offset of the cursor.
            while (tempElement !== null && tempElement !== manipulationContext.AnimateElement) {
                manipulationContext[AXIS_ENUM.X].transformOrigin += tempElement.offsetLeft;
                manipulationContext[AXIS_ENUM.Y].transformOrigin += tempElement.offsetTop;
                tempElement = tempElement.parentNode;
            }

            manipulationContext.manipulationType = MANIPULATION_TYPE_ENUM.SCALE;
        } else {
            manipulationContext.manipulationType = MANIPULATION_TYPE_ENUM.TRANSLATE;
        }
        

        if (!manipulationContext[axis].useDefaultSupport) {
            nextPosition(distance, INPUT_TYPE_ENUM.MouseWheel, axis);
            if (manipulationContext.hasPositionChanged()) {
                eventObject.preventDefault();
                eventObject.stopPropagation();
            }
        }
        saveManipulationContext();
    };


    function nextPosition(distance, inputType, axis) {

        var currentTarget = manipulationContext[axis].targetPosition;

        if (manipulationContext[axis].hasMandatorySnapPoints) {
            manipulationContext[axis].targetPosition = getNextSnapPoint(axis, currentTarget, distance, manipulationContext);
        } else {
            manipulationContext[axis].targetPosition = currentTarget + distance;
        }
               

        if (manipulationContext[axis].targetPosition > manipulationContext[axis].max) {
            manipulationContext[axis].targetPosition = manipulationContext[axis].max;
        }
        else if (manipulationContext[axis].targetPosition < manipulationContext[axis].min) {
            manipulationContext[axis].targetPosition = manipulationContext[axis].min;
        }

        if (currentTarget !== manipulationContext[axis].targetPosition) {
            updatePosition();
        }
        saveManipulationContext();
    }




    // 'getNextSnapPoint'
    //
    //      Purpose:
    //              This function takes the axis, the current value, and a direction
    //              and determines the next snap-point location. 
    function getNextSnapPoint(axis, currentVal, distance) {

        var restPoint = distance - currentVal;
        var snapString = manipulationContext[axis].snapPoints;
        var nextSnap, percentages;
        var direction = 1;

        if (manipulationContext[axis].snapPoints.search("snapInterval") !== -1) {
            // Find the interval size and move in that direction.

            percentages = snapString.match(/\d+%/g);
            var startPosition = parsePercentage(percentages[0]) * manipulationContext[axis].size;
            var interval = parsePercentage(percentages[1]) * manipulationContext[axis].size;

            // Figure out which way we are moving.
            if (restPoint < startPosition) {
                direction = -1;
            }

            // Keep going to the next snap point until we find the closest one in the direction we are going.
            nextSnap = startPosition;
            while ( (distance > 0 ? nextSnap < restPoint : nextSnap + interval < restPoint) ) {
                nextSnap = nextSnap + direction * interval;
            }

        } else {
            // get the list of percentages
            percentages = snapString.match(/\d+%/g);

            // Figure out which way we are moving
            if (restPoint < startPosition) {
                direction = -1;
            }
            
            var index = 0;
            nextSnap = parsePercentage(commaList[index]) * manipulationContext[axis].size;
            while ((distance > 0 ? nextSnap < restPoint : nextSnap + manipulationContext[axis].size < restPoint)) {
                index++;
                if (index < percentages.length) {
                    nextSnap = manipulationContext[axis].size * parsePercentage(percentages[index]);
                } else {
                    return currentVal;
                }
            }
        }

        return -nextSnap;
    }


    function updatePosition() {
        if (manipulationContext.hasPositionChanged()) {

            // Determine if we are already transitioning a translate animation
            var transformString = window.getComputedStyle(manipulationContext.AnimateElement).getPropertyValue("transform");
            var alreadyTransitioning = (transformString !== "matrix(1, 0, 0, 1, 0, 0)" && transformString !== "none");

            if (manipulationContext.manipulationType === MANIPULATION_TYPE_ENUM.TRANSLATE) {

                // If we are already in the middle of a transition, cancel it
                // before we start the next one.  As an excerise for the reader,
                // velocity matching the existing animation to the next animation
                // would result in a much smoother experience.
                if (alreadyTransitioning) {
                    removeTransform();
                }

                // calculate the time of a scroll transition
                var xdistance = manipulationContext[AXIS_ENUM.X].targetPosition - manipulationContext[AXIS_ENUM.X].currentPosition;
                var ydistance = manipulationContext[AXIS_ENUM.Y].targetPosition - manipulationContext[AXIS_ENUM.Y].currentPosition;
                var distance = Math.sqrt(xdistance * xdistance + ydistance * ydistance);
                var scrollTime = (distance * manipulationContext[AXIS_ENUM.X].MilisecondsPerDistance);

                // calculate the time of a zoom transition
                var zoomDistance = Math.abs(manipulationContext[AXIS_ENUM.Zoom].targetPosition - manipulationContext[AXIS_ENUM.Zoom].currentPosition);
                var zoomTime = zoomDistance * manipulationContext[AXIS_ENUM.Zoom].MilisecondsPerDistance;

                // setup the actual transition
                var time = Math.min(Math.max(scrollTime, zoomTime), MaximumAnimationTime);
                manipulationContext.AnimateElement.style["transitionDuration"] = time + "ms";
                manipulationContext.AnimateElement.style["transition-property"] = "transform";

                // create the string for the new position of the content and create a translate transform
                var leftString = (manipulationContext[AXIS_ENUM.X].targetPosition + manipulationContext.element.scrollLeft) + "px";
                var topString = (manipulationContext[AXIS_ENUM.Y].targetPosition + manipulationContext.element.scrollTop) + "px";

                manipulationContext.AnimateElement.style["transform"] = "translate(" + leftString + ", " + topString + ")";
                manipulationContext.AnimateElement.style["transition-timing-function"] = manipulationContext[AXIS_ENUM.X].AnimationCurve;

                manipulationContext.AnimateElement.style["transform-origin"] = (manipulationContext[AXIS_ENUM.X].transformOrigin / manipulationContext[AXIS_ENUM.Zoom].currentPosition - manipulationContext[AXIS_ENUM.X].currentPosition) + "px "
                    + (manipulationContext[AXIS_ENUM.Y].transformOrigin / manipulationContext[AXIS_ENUM.Zoom].currentPosition - manipulationContext[AXIS_ENUM.Y].currentPosition) + "px";

                // wire up an event handler to remove the transform once the transition is complete.
                manipulationContext.AnimateElement.addEventListener("transitionend", completeTransition);
                manipulationContext.element.style["-ms-overflow-style"] = "none";
            } else {
                // We only want to update the zoom level if we are not in the middle of a transition.
                if (!alreadyTransitioning) {
                    // This block of code calculates how to zoom in on the centerpoint by updating scrollLeft, scrollTop, and msContentZoomFactor;
                    var deltaScale = manipulationContext[AXIS_ENUM.Zoom].targetPosition / manipulationContext.element.msContentZoomFactor;
                    var center = { x: 0, y: 0 };
                    center.x = (manipulationContext[AXIS_ENUM.X].transformOrigin / manipulationContext[AXIS_ENUM.Zoom].currentPosition - manipulationContext[AXIS_ENUM.X].currentPosition);
                    center.y = (manipulationContext[AXIS_ENUM.Y].transformOrigin / manipulationContext[AXIS_ENUM.Zoom].currentPosition - manipulationContext[AXIS_ENUM.Y].currentPosition);
                    var scaleMatrix = createScaleMatrix(deltaScale, deltaScale, center.x, center.y);

                    var position = getPositionFromTransform(scaleMatrix, center, manipulationContext.manipulationType);
                    manipulationContext.element.msContentZoomFactor = position.zoom;
                    setScrollPosition(position.left, position.top);
                }
            }
        }

    }

    function getPositionFromTransform(matrix, origin, manipulationType) {
        var retVal = { left: 0, top: 0, zoom: 0 };
        
        retVal.zoom = manipulationContext.element.msContentZoomFactor * matrix._11;
        if (manipulationType === MANIPULATION_TYPE_ENUM.SCALE) {
            
            var scaleMatrix = createScaleMatrix(matrix._11, matrix._22, origin.x, origin.y);
            var inverse = invertMatrix(scaleMatrix);
            var pt = transformPoint(scaleMatrix, 0, 0);
            
            retVal.left = manipulationContext.element.scrollLeft - pt.x / retVal.zoom;
            retVal.top = manipulationContext.element.scrollTop - pt.y / retVal.zoom;
            
        } else {
            retVal.left = manipulationContext.element.scrollLeft - matrix._31;
            retVal.top = manipulationContext.element.scrollTop - matrix._32;
        }
        return retVal;
    }


    // This function is used to set scrollLeft and scrollTop atomically.
    function setScrollPosition(leftPosition, topPosition) {
        manipulationContext.scrollAnchor.style.left = leftPosition + "px";
        manipulationContext.scrollAnchor.style.top = topPosition + "px";
        manipulationContext.scrollAnchor.scrollIntoView();
    }


    function removeTransform() {

        var currentStyle = window.getComputedStyle(manipulationContext.AnimateElement);
        var transformString = currentStyle.getPropertyValue("transform");

        if (currentStyle.getPropertyValue("transform") !== "matrix(1, 0, 0, 1, 0, 0)") {
            var matrix = parseMatrix(currentStyle.getPropertyValue("transform"));
            var origin = parseTransformOrigin(currentStyle.getPropertyValue("transform-origin"));

            var position = getPositionFromTransform(matrix, origin, manipulationContext.manipulationType);

            manipulationContext.AnimateElement.style["transition-property"] = "none";
            manipulationContext.AnimateElement.style["transitionDuration"] = 0;
            manipulationContext.AnimateElement.style["transform"] = "matrix(1,0,0,1,0,0)";
            manipulationContext.element.msContentZoomFactor = position.zoom;
            setScrollPosition(position.left, position.top);

        }
    }



    function completeTransition(eventObject) {
        updateManipulationContext(getScroller(eventObject.srcElement));
        removeTransform();
        manipulationContext.element.style["-ms-overflow-style"] = "auto";
        saveManipulationContext();
    }


    function initializeManipulationContext(element) {

        element.manipulationContextInitialized = true;

        element.targetPosition = new Array();
        element.targetPosition[AXIS_ENUM.X] = element.scrollLeft;
        element.targetPosition[AXIS_ENUM.Y] = element.scrollTop;
        element.targetPosition[AXIS_ENUM.Zoom] = element.msContentZoomFactor;

        element.manipulationType = MANIPULATION_TYPE_ENUM.NONE;
    }

    function saveManipulationContext() {

        for (var i = 0; i < AXIS_ENUM.values.length; i++) {
            var axis = AXIS_ENUM.values[i];
            manipulationContext.element.targetPosition[axis] = manipulationContext[axis].targetPosition;
        }
        manipulationContext.element.manipulationType = manipulationContext.manipulationType;
    }

    // 'updateManipulationContext()'
    //
    //      Purpose:    This function is responsible for extracting the information 
    //                  we need to know about the manipulationElement.
    function updateManipulationContext(element) {
        manipulationContext = new Array();

        if (!element.manipulationContextInitialized) {
            initializeManipulationContext(element);
        }

        manipulationContext.element = element;
        manipulationContext.AnimateElement = element.childNodes[0];

        manipulationContext.scrollAnchor;
        var anchors = manipulationContext.AnimateElement.querySelectorAll(".scrollAnchor");
        for (var i = 0; i < anchors.length; i++) {
            if (anchors[i].parentNode.parentNode === manipulationContext.element) {
                manipulationContext.scrollAnchor = anchors[i];
                break;
            }
        }

        manipulationContext.manipulationType = manipulationContext.element.manipulationType;

        manipulationContext.hasPositionChanged = function () {
            var positionsChanged = false;
            var floatThreshold = .001;
            var target;
            var tempDebug;
            var delta;
            if (manipulationContext.manipulationType === MANIPULATION_TYPE_ENUM.SCALE) {
                return Math.abs(this[AXIS_ENUM.Zoom].currentPosition - this[AXIS_ENUM.Zoom].targetPosition) > floatThreshold;
            } else if (manipulationContext.manipulationType === MANIPULATION_TYPE_ENUM.TRANSLATE) {
                return Math.abs(this[AXIS_ENUM.X].currentPosition - this[AXIS_ENUM.X].targetPosition) > floatThreshold ||
                       Math.abs(this[AXIS_ENUM.Y].currentPosition - this[AXIS_ENUM.Y].targetPosition) > floatThreshold;
            }
            for (var j = 0; j < AXIS_ENUM.values.length; j++) {
                var context = this[AXIS_ENUM.values[j]];
                positionsChanged = positionsChanged || (context.targetPosition + floatThreshold) < context.currentPosition || context.currentPosition < (context.targetPosition - floatThreshold);
            }
            return positionsChanged;
        };

        var currentStyle = window.getComputedStyle(element);
        manipulationContext[AXIS_ENUM.X] = {

            // Property determine whether intrinsic support should be used or if this module should
            // provide support for the axis.
            useDefaultSupport: !(currentStyle.getPropertyValue("overflow-x") === "scroll" || (currentStyle.getPropertyValue("overflow-x") === "auto" && element.scrollWidth > 0))
                || currentStyle.getPropertyValue("-ms-scroll-snap-type") !== "mandatory",

            // Next two properties represent the boundaries of the allowed values
            min: -(element.scrollWidth - element.clientWidth),
            max: 0,

            // Snap percentages are relative to this size.
            size: element.clientWidth,

            // Next two properties represent details around snap-points. String that represents what snap-points have been set.
            snapPoints: currentStyle.getPropertyValue("-ms-scroll-snap-points-x"),
            hasMandatorySnapPoints: currentStyle.getPropertyValue("-ms-scroll-snap-type") === "mandatory",
            
            // Next two properties define the multipliers for mapping input to a distance
            DistancePerKeystroke: element.clientWidth * .15,
            DistancePerWheelDelta: 1,

            // Following three properties are used for creating the animation
            MilisecondsPerDistance: 1.5,
            SnapPointMaximumTime: 400,
            AnimationCurve: "cubic-bezier(0.25, 1.0, 0.1, 0.0)",

            // currentPosition, which gets set shortly, properties tracks where we currently are and
            // targetPosition tracks where we are trying to go to.
            targetPosition: element.targetPosition[AXIS_ENUM.X]
        };

        manipulationContext[AXIS_ENUM.Y] = {

            // Property determine whether intrinsic support should be used or if this module should
            // provide support for the axis.
            useDefaultSupport: !(currentStyle.getPropertyValue("overflow-y") === "scroll" || (currentStyle.getPropertyValue("overflow-y") === "auto" && element.scrollHeight > 0))
                || currentStyle.getPropertyValue("-ms-scroll-snap-type") !== "mandatory",

            // Next two properties represent the boundaries of the allowed values
            min: -(element.scrollHeight - element.clientHeight),
            max: 0,

            // Snap percentages are relative to this size.
            size: element.clientHeight,

            // Next two properties represent details around snap-points. String that represents what snap-points have been set.
            snapPoints: currentStyle.getPropertyValue("-ms-scroll-snap-points-y"),
            hasMandatorySnapPoints: currentStyle.getPropertyValue("-ms-scroll-snap-type") === "mandatory",

            // Next two properties define the multipliers for mapping input to a distance
            DistancePerKeystroke: element.clientHeight * .15,
            DistancePerWheelDelta: 1,

            // Following three properties are constants used for controlling the animation
            MilisecondsPerDistance: manipulationContext[AXIS_ENUM.X].MilisecondsPerDistance,
            SnapPointMaximumTime: manipulationContext[AXIS_ENUM.X].SnapPointMaximumTime,
            AnimationCurve: manipulationContext[AXIS_ENUM.X].AnimationCurve,


            // currentPosition, which gets set shortly, properties tracks where we currently are and
            // targetPosition tracks where we are trying to go to.
            targetPosition: element.targetPosition[AXIS_ENUM.Y]
        };

        manipulationContext[AXIS_ENUM.Zoom] = {

            // Property determine whether intrinsic support should be used or if this module should
            // provide support for the axis.
            useDefaultSupport: !(currentStyle.getPropertyValue("-ms-content-zooming") === "zoom"),

            // Next two properties represent the boundaries of the allowed values
                min: parsePercentage(currentStyle.getPropertyValue("-ms-content-zoom-limit-min")),
                max: parsePercentage(currentStyle.getPropertyValue("-ms-content-zoom-limit-max")),

            // Snap percentages are relative to this size.
            size: 1,

            // Next two properties represent details around snap-points. String that represents what snap-points have been set.
            snapPoints: currentStyle.getPropertyValue("-ms-content-zoom-snap-points"),
            hasMandatorySnapPoints: currentStyle.getPropertyValue("-ms-content-zoom-snap-type") === "mandatory",

            // Next two properties define the multipliers for mapping input to a distance
            DistancePerKeystroke: .25,
            DistancePerWheelDelta: -.4 / (document.documentElement.clientHeight * .15),

            // Following three properties are constants used for controlling the animation
            MilisecondsPerDistance: 1500,
            SnapPointMaximumTime: 2000,
            AnimationCurve: "cubic-bezier(0.25, 1.0, 0.5, 0.0)",

            // currentPosition, which gets set shortly, properties tracks where we currently are and
            // targetPosition tracks where we are trying to go to.
            targetPosition: element.targetPosition[AXIS_ENUM.Zoom]
        };

        // Get the transform origin
        var origin = parseTransformOrigin(window.getComputedStyle(manipulationContext.AnimateElement).getPropertyValue("transform-origin"));
        manipulationContext[AXIS_ENUM.X].transformOrigin = origin.x;
        manipulationContext[AXIS_ENUM.Y].transformOrigin = origin.y;

        updateCurrentPosition(manipulationContext);
    }

    function parsePercentage(percentString) {
        var tempString = percentString.substr(0, percentString.length - 1);
        return tempString / 100;
    }

    function parseTransformOrigin(originString) {
        var retVal = { x: 0, y: 0 };
        var tempString = originString;
        var start = tempString.search("px");
        retVal.x = parseFloat(tempString.substr(0, start).trim());
        tempString = tempString.substr(start + 2).trim();
        retVal.y = parseFloat(tempString.substr(0, tempString.search("px")));
        return retVal;
    }

    function updateCurrentPosition() {
        var currentStyle = window.getComputedStyle(manipulationContext.AnimateElement);

        if (currentStyle.getPropertyValue("transform") !== "none" &&
            currentStyle.getPropertyValue("transform") !== "matrix(1, 0, 0, 1, 0, 0)") {

            var matrix = parseMatrix(currentStyle.getPropertyValue("transform"));
            var origin = parseTransformOrigin(currentStyle.getPropertyValue("transform-origin"));
            var position = getPositionFromTransform(matrix, origin, manipulationContext.manipulationType);


            manipulationContext[AXIS_ENUM.Zoom].currentPosition = position.zoom;
            manipulationContext[AXIS_ENUM.X].currentPosition = -position.left;
            manipulationContext[AXIS_ENUM.Y].currentPosition = -position.top;
        
        } else {
            // set to negative value as currentLeft denotes value in Matrix/Translate transform
            manipulationContext[AXIS_ENUM.X].currentPosition = -manipulationContext.element.scrollLeft;
            manipulationContext[AXIS_ENUM.Y].currentPosition = -manipulationContext.element.scrollTop;
            manipulationContext[AXIS_ENUM.Zoom].currentPosition = manipulationContext.element.msContentZoomFactor;
        }

    }


    var manipulationContext;

    var INPUT_TYPE_ENUM = { "MouseWheel": 1, "Keyboard": 2 };
    var DIRECTION_ENUM = { "Positive": 1, "Negative": -1 };

    var AXIS_ENUM = { "X": 0, "Y": 1, "Zoom": 2 };
    AXIS_ENUM.values = new Array();
    AXIS_ENUM.values[AXIS_ENUM.X] = AXIS_ENUM.X;
    AXIS_ENUM.values[AXIS_ENUM.Y] = AXIS_ENUM.Y;
    AXIS_ENUM.values[AXIS_ENUM.Zoom] = AXIS_ENUM.Zoom;

    var MANIPULATION_TYPE_ENUM = { "NONE": 0, "TRANSLATE": 1, "SCALE": 2 };

    var currentPosition = new Array();


    var MaximumAnimationTime = 400;

})();