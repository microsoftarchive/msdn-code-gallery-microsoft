//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF 
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO 
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//// PARTICULAR PURPOSE. 
//// 
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    WinJS.Namespace.define("ItemMovers", {
        InputProcessor: WinJS.Class.define(function () {
            /// <summary> 
            /// Constructs a new InputProcessor object
            /// InputProcessor is a thin wrapper over GestureRecognizer which is the WinJS gesture
            /// detection component.
            /// </summary>
            this._gestureRecognizer = new Windows.UI.Input.GestureRecognizer();
            this._downPoint = null;
            this._lastState = null;
        }, {
            element: {
                /// <summary> 
                /// The element that is going to be moved
                /// </summary>
                get: function () {
                    if (!this._element) {
                        return null;
                    }
                    return this._element;
                },
                set: function (value) {
                    this._element = value;
                    this._setupElement();
                }
            },
            parent: {
                /// <summary> 
                /// The reference element that contains the coordinate space that is used
                /// for transformations during manipulation
                /// </summary>
                get: function () {
                    if (!this._parent) {
                        return null;
                    }
                    return this._parent;
                },
                set: function (value) {
                    this._parent = value;
                }
            },
            getRecognizer: function () {
                return this._gestureRecognizer;
            },
            getDown: function () {
                return this._downPoint;
            },
            _setupElement: function () {
                /// <summary> 
                /// Adds event listeners to the element
                /// </summary>
                var that = this;
                this._element.addEventListener("MSPointerDown",
                    function (evt) { ItemMovers.InputProcessor._handleDown(that, evt); },
                    false);
                this._element.addEventListener("MSPointerMove",
                    function (evt) { ItemMovers.InputProcessor._handleMove(that, evt); },
                    false);
                this._element.addEventListener("MSPointerUp",
                    function (evt) { ItemMovers.InputProcessor._handleUp(that, evt); },
                    false);
                this._element.addEventListener("MSPointerCancel",
                    function (evt) { ItemMovers.InputProcessor._handleCancel(that, evt); },
                    false);
                this._element.addEventListener("wheel",
                    function (evt) { ItemMovers.InputProcessor._handleMouse(that, evt); },
                    false);
            }
        }, {
            _handleDown: function (that, evt) {
                /// <summary> 
                /// Handles the MSPointerDown event
                /// </summary>
                /// <param name="that" type="Object">
                /// The InputProcessor object handling this event
                /// </param>
                /// <param name="evt" type="Event">
                /// The event object
                /// </param>
                var pp = evt.getCurrentPoint(that._parent);
                that._element.msSetPointerCapture(pp.pointerId);
                that._gestureRecognizer.processDownEvent(pp);
                // stopImmediatePropagation prevents the semantic zoom control from taking over this input
                evt.stopImmediatePropagation();

                that._downPoint = { x: pp.position.x, y: pp.position.y };
            },
            _handleMove: function (that, evt) {
                /// <summary> 
                /// Handles the MSPointerMove event
                /// </summary>
                /// <param name="that" type="Object">
                /// The InputProcessor object handling this event
                /// </param>
                /// <param name="evt" type="Event">
                /// The event object
                /// </param>
                var pps = evt.getIntermediatePoints(that._parent);
                that._gestureRecognizer.processMoveEvents(pps);
                evt.stopImmediatePropagation();
            },
            _handleUp: function (that, evt) {
                /// <summary> 
                /// Handles the MSPointerUp event
                /// </summary>
                /// <param name="that" type="Object">
                /// The InputProcessor object handling this event
                /// </param>
                /// <param name="evt" type="Event">
                /// The event object
                /// </param>
                var pp = evt.getCurrentPoint(that._parent);
                that._gestureRecognizer.processUpEvent(pp);
                evt.stopImmediatePropagation();
            },
            _handleCancel: function (that, evt) {
                /// <summary> 
                /// Handles the MSPointerCancel event
                /// </summary>
                /// <param name="that" type="Object">
                /// The InputProcessor object handling this event
                /// </param>
                /// <param name="evt" type="Event">
                /// The event object
                /// </param>
                that._gestureRecognizer.completeGesture();
                evt.stopImmediatePropagation();
            },
            _handleMouse: function (that, evt) {
                /// <summary> 
                /// Handles the mouse wheel event
                /// </summary>
                /// <param name="that" type="Object">
                /// The InputProcessor object handling this event
                /// </param>
                /// <param name="evt" type="Event">
                /// The event object
                /// </param>
                var pp = evt.getCurrentPoint(that._parent);
                that._gestureRecognizer.processMouseWheelEvent(pp, evt.shiftKey, evt.ctrlKey);
                evt.stopImmediatePropagation();
                evt.preventDefault();
            }
        }),

        Manipulable: WinJS.Class.define(function () {
            /// <summary> 
            /// Creates a new Manipulable object
            /// Manipulable is built on top of InputProcessor and configures it for manipulation. It can be
            /// configured to turn on and off different components of manipulation. Also moveHandlers and finishedHandlers
            /// can be registered which are called on any manipulation update and manipulation completion respectively.
            /// </summary>
            this._inputProcessor = new ItemMovers.InputProcessor();
            this._finishedHandler = null;
            this._moveHandler = null;
            this._currentTransform = new MSCSSMatrix();
            this._initialTransform = new MSCSSMatrix();
            this._crossSlidePos = null;

            this._initialTransformParams = {
                translation: { x: 0, y: 0 },
                rotation: 0,
                scale: 1
            };

            this._currentTransformParams = {
                translation: { x: 0, y: 0 },
                rotation: 0,
                scale: 1
            };
        }, {
            configure: function (scale, rotate, translate, inertia,
                                initialScale, initialRotate, initialTranslate) {
                /// <summary> 
                /// Configures the Manipulable object for use
                /// </summary>
                /// <param name="scale" type="Boolean">
                /// True if the manipulable object can be scaled.
                /// </param>
                /// <param name="rotate" type="Boolean">
                /// True if the manipulable object can be rotated.
                /// </param>
                /// <param name="translate" type="Boolean">
                /// True if the manipulable object can be translated (moved).
                /// </param>
                /// <param name="inertia" type="Boolean">
                /// True if the object should use inertia for the manipulations.
                /// </param>
                /// <param name="initialScale" type="Number">
                /// The object's initial scaling factor
                /// </param>
                /// <param name="initialRotate" type="Number">
                /// The objects initial rotation value
                /// </param>
                /// <param name="initialTranslate" type="Object">
                /// The object initial x,y translation values
                /// </param>

                var gr = this._inputProcessor.getRecognizer();
                if (!gr.isActive) {
                    var settings = 0;
                    if (scale) {
                        settings |= Windows.UI.Input.GestureSettings.manipulationScale;
                        if (inertia) {
                            settings |= Windows.UI.Input.GestureSettings.manipulationScaleInertia;
                        }
                    }
                    if (rotate) {
                        settings |= Windows.UI.Input.GestureSettings.manipulationRotate;
                        if (inertia) {
                            settings |= Windows.UI.Input.GestureSettings.manipulationRotateInertia;
                        }
                    }
                    if (translate) {
                        settings |= Windows.UI.Input.GestureSettings.manipulationTranslateX |
                            Windows.UI.Input.GestureSettings.manipulationTranslateY;
                        if (inertia) {
                            settings |= Windows.UI.Input.GestureSettings.manipulationTranslateInertia;
                        }
                    }

                    var that = this;

                    if (scale || rotate || translate) {
                        gr.addEventListener('manipulationstarted',
                            function (evt) { ItemMovers.Manipulable._manipulationStarted(that, evt); },
                            false);
                        gr.addEventListener('manipulationupdated',
                            function (evt) { ItemMovers.Manipulable._manipulationUpdated(that, evt); },
                            false);
                        gr.addEventListener('manipulationended',
                            function (evt) { ItemMovers.Manipulable._manipulationEnded(that, evt); },
                            false);
                    }

                    gr.gestureSettings = settings;

                    // set initial transform matrix to be used when resetting the transform
                    this._currentTransformParams.scale = initialScale;
                    this._currentTransformParams.rotation = initialRotate;
                    this._currentTransformParams.translation = initialTranslate;

                    this._initialTransformParams.scale = initialScale;
                    this._initialTransformParams.rotation = initialRotate;
                    this._initialTransformParams.translation = initialTranslate;

                    if (initialRotate) {
                        this._initialTransform = this._initialTransform.rotate(initialRotate);
                    }
                    else {
                        this._currentTransformParams.rotation = 0;
                        this._initialTransformParams.rotation = 0;
                    }
                    if (initialTranslate) {
                        this._initialTransform = this._initialTransform.translate(initialTranslate.x, initialTranslate.y);
                    }
                    else {
                        this._currentTransformParams.translation = { x: 0, y: 0 };
                        this._initialTransformParams.translation = { x: 0, y: 0 };
                    }
                    if (initialScale) {
                        this._initialTransform = this._initialTransform.scale(initialScale);
                    }
                    else {
                        this._currentTransformParams.scale = 1;
                        this._initialTransformParams.scale = 1;
                    }

                    this._currentTransform = this._initialTransform;
                }
            },
            setElement: function (elm) {
                /// <summary> 
                /// Sets the element that will be manipulated
                /// </summary>
                /// <param name="elm" type="Object">
                /// The HtmlElement object that will be manipulated
                /// </param>
                this._inputProcessor.element = elm;
                // Also set the default origin of manipulation
                this._inputProcessor.element.style.msTransformOrigin = "0 0";
            },
            setParent: function (elm) {
                /// <summary> 
                /// Store the parent of the manipulable element
                /// </summary>
                /// <param name="elm" type="Object">
                /// The HtmlElement object that will be manipulated
                /// </param>
                this._inputProcessor.parent = elm;
            },
            registerFinishedHandler: function (handler) {
                /// <summary> 
                /// Registers the handler function that will be called after the manipulation is completed
                /// </summary>
                /// <param name="handler" type="Function">
                /// The manipulation finished event handler
                /// </param>
                this._finishedHandler = handler;
            },
            registerMoveHandler: function (arg, handler) {
                /// <summary> 
                /// Registers a handler function that is called prior to applying the transform to the element
                /// </summary>
                /// <param name="args">
                /// Arguments to pass to the move handler function
                /// </param>
                /// <param name="handler" type="Function">
                /// The manipulation finished event handler
                /// </param>
                this._moveHandlerArg = arg;
                this._moveHandler = handler;
            },
            resetAllTransforms: function () {
                /// <summary> 
                /// Resets the manipulable object back to its initial state
                /// </summary>

                // check that the element has been registered before this is called
                if (this._inputProcessor.element) {

                    // Reapply the initial transform
                    this._inputProcessor.element.style.transform = this._initialTransform.toString();
                    this._currentTransform = this._initialTransform;

                    // Also reset the current transform parameters to their initial values
                    this._currentTransformParams.translation = this._initialTransformParams.translation;
                    this._currentTransformParams.rotation = this._initialTransformParams.rotation;
                    this._currentTransformParams.scale = this._initialTransformParams.scale;
                }
            },

            _applyMotion: function (pivot, translation, rotation, scaling) {
                /// <summary> 
                /// Applies the current motion to the manipulable object
                /// </summary>
                /// <param name="pivot" type="Object">
                /// X,Y values representing the pivot point of rotation and scaling
                /// </param>
                /// <param name="translation" type="Object">
                /// X,Y values representing the amount of translation in each corresponding direction
                /// </param>
                /// <param name="rotation" type="Number">
                /// Angle of rotation
                /// </param>
                /// <param name="scaling" type="Number">
                /// Factor to scale by
                /// </param>

                // Create the transform, apply parameters, and multiply by the current transform matrix
                var transform = new MSCSSMatrix().translate(pivot.x, pivot.y).
                    translate(translation.x, translation.y).
                    rotate(rotation).
                    scale(scaling).
                    translate(-pivot.x, -pivot.y).multiply(this._currentTransform);

                this._inputProcessor.element.style.transform = transform.toString();
                this._currentTransform = transform;
            },

            _updateTransformParams: function (delta) {
                /// <summary> 
                /// Updates the current transformation parameters given the new delta
                /// </summary>
                /// <param name="that" type="Object">
                /// Object holding the change in rotation, scaling, and translation
                /// </param>
                this._currentTransformParams.translation.x = this._currentTransformParams.translation.x + delta.translation.x;
                this._currentTransformParams.translation.y = this._currentTransformParams.translation.y + delta.translation.y;
                this._currentTransformParams.rotation = this._currentTransformParams.rotation + delta.rotation;
                this._currentTransformParams.scale = this._currentTransformParams.scale * delta.scale;
            }
        }, {
            _manipulationStarted: function (that, evt) {
                /// <summary> 
                /// Event handler for beginning manipulation
                /// </summary>
                /// <param name="that" type="Object">
                /// Manipulable object on which the event was performed
                /// </param>
                /// <param name="evt" type="Event">
                /// The event
                /// </param>

                ItemMovers.Manipulable._manipulationHelper(that, evt);
            },
            _manipulationUpdated: function (that, evt) {
                /// <summary> 
                /// Event handler for beginning manipulation
                /// </summary>
                /// <param name="that" type="Object">
                /// Manipulable object on which the event was performed
                /// </param>
                /// <param name="evt" type="Event">
                /// The event
                /// </param>

                ItemMovers.Manipulable._manipulationHelper(that, evt);
            },
            _manipulationEnded: function (that, evt) {
                /// <summary> 
                /// Event handler for ending manipulation
                /// </summary>
                /// <param name="that" type="Object">
                /// Manipulable object on which the event was performed
                /// </param>
                /// <param name="evt" type="Event">
                /// The event
                /// </param>

                // Pass event to manipulation helper function
                ItemMovers.Manipulable._manipulationHelper(that, evt);

                // if a finished handler function was registered, call it now
                if (that._finishedHandler) {
                    that._finishedHandler();
                }
            },
            _manipulationHelper: function (that, evt) {
                /// <summary> 
                /// Calculates the transformation parameters for this instance of manipulation
                /// </summary>
                /// <param name="that" type="Object">
                /// Manipulable object on which the event was performed
                /// </param>
                /// <param name="evt" type="Event">
                /// The event
                /// </param>

                if (evt.delta) {
                    // pivot point for rotation/scaling
                    var pivot = { x: evt.position.x, y: evt.position.y };

                    // x,y values for movement
                    var translation = { x: evt.delta.translation.x, y: evt.delta.translation.y };

                    // rotation angle
                    var rotation = evt.delta.rotation;

                    // scaling factor
                    var scale = evt.delta.scale;

                    // group the transformation parameters
                    var delta = {
                        pivot: pivot,
                        translation: translation,
                        rotation: rotation,
                        scale: scale
                    };

                    // if a move handler was registered, call it now
                    // (this is useful for things like boundary checking)
                    if (that._moveHandler) {
                        delta = that._moveHandler(that._moveHandlerArg, delta, that._currentTransformParams, that._currentTransform);
                    }

                    that._updateTransformParams(delta);

                    // apply the transformation
                    that._applyMotion(delta.pivot, delta.translation, delta.rotation, delta.scale);
                }
            },

        })
    });
})();