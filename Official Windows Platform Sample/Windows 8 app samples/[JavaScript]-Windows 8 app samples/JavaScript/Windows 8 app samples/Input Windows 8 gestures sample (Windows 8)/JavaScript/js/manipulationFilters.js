//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF 
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO 
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//// PARTICULAR PURPOSE. 
//// 
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    WinJS.Namespace.define("ManipulationFilters", {
        /// <summary>PictureInFrame prevents the zoomable picture from leaving its container </summary>
        PictureInFrame: WinJS.Class.define(function () {
        }, {
            objectWidth: {
                /// <summary>
                /// The width of the manipulable object
                /// </summary>
                get: function () {
                    if (!this._objectWidth) {
                        return null;
                    }
                    return this._objectWidth;
                },
                set: function (value) {
                    this._objectWidth = value;
                }
            },
            objectHeight: {
                /// <summary>
                /// The height of the manipulable object
                /// </summary>
                get: function () {
                    if (!this._objectHeight) {
                        return null;
                    }
                    return this._objectHeight;
                },
                set: function (value) {
                    this._objectHeight = value;
                }
            },
            containerWidth: {
                /// <summary>
                /// The width of the frame in which the manipulable object should remain
                /// </summary>
                get: function () {
                    if (!this._frameWidth) {
                        return null;
                    }
                    return this._frameWidth;
                },
                set: function (value) {
                    this._frameWidth = value;
                }
            },
            containerHeight: {
                /// <summary>
                /// The height of the frame in which the manipulable object should remain
                /// </summary>
                get: function () {
                    if (!this._frameHeight) {
                        return null;
                    }
                    return this._frameHeight;
                },
                set: function (value) {
                    this._frameHeight = value;
                }
            },
            objectCorners: {
                /// <summary>
                /// An array holding the four corners of the object
                /// </summary>
                get: function () {
                    if (!this._objectCorners) {
                        this._objectCorners = [{ x: 0, y: 0 }, { x: this.objectWidth, y: 0 }, { x: 0, y: this.objectHeight }, { x: this.objectWidth, y: this.objectHeight }];
                    }
                    return this._objectCorners;
                }
            },
                
            calculateTransformedPosition: function (currentTransformParams, transform) {
                /// <summary>
                /// Calculates the position of the transformed manipulable object.
                /// </summary>
                /// <param name="currentTransformsParams" type="Object">
                /// Holds the current rotation and scaling values of the manipulable object
                /// </param>
                /// <param name="transform" type="Object">
                /// The MSCSSMatrix object transform that is pending application to the object
                /// </param>
                /// <returns type="Object">
                /// The new position of the object in the form:
                /// { left: 0, top: 0, right: 0, bottom: 0}
                /// </returns>

                // Get the position of the original top left corner from the transform matrix
                var left = transform.e;
                var thisTop = transform.f;

                // Get the scaled width of the object
                var w = this._objectWidth * currentTransformParams.scale;
                var h = this._objectHeight * currentTransformParams.scale;
                var right = left + w;
                var bottom = thisTop + h;

                // If there is no rotation, then everything is calculated
                // But with rotation, we need to determine the position of the box bounding the rotated element
                if (currentTransformParams.rotation !== 0) {
                    
                    // set the max/min corners
                    left = this.containerWidth;
                    thisTop = this.containerHeight;
                    right = 0;
                    bottom = 0;
                    
                    // iterate through all corners of the object
                    for (var point in this.objectCorners) {

                        // generate the vector for this corner
                        var pointVector = [this.objectCorners[point].x, this.objectCorners[point].y, 1];

                        // Construct a new matrix with this corner
                        var newMatrix = new MSCSSMatrix();
                        newMatrix.m11 = this.objectCorners[point].x;
                        newMatrix.m12 = this.objectCorners[point].y;
                        newMatrix.m13 = 0;
                        newMatrix.m14 = 1;

                        // Transform this corner with the new transform
                        newMatrix = transform.multiply(newMatrix);

                        // Recalculate the new points
                        left = Math.min(left, newMatrix.m11);
                        thisTop = Math.min(thisTop, newMatrix.m12);
                        right = Math.max(right, newMatrix.m11);
                        bottom = Math.max(bottom, newMatrix.m12);
                    }
                }

                return { left: left, top: thisTop, right: right, bottom: bottom };
            },
            
        }, {
            MoveHandler: function (pif, delta, currentTransformParams, currentTransform) {
                /// <summary>
                /// The function that will be called by the ItemMover before applying the transformation
                /// to the manipulable element
                /// </summary>
                /// <param name="pif" type="Object">
                /// Arguments passed through the ItemMover when the MoveHandler was registered.
                /// </param>
                /// <param name="delta" type="Object">
                /// Contains all of the transformation params:
                ///     {pivot, delta, rotation, scale}
                /// </param>
                /// <param name="currentTransformsParams" type="Object">
                /// Holds the current rotation and scaling values of the manipulable object
                /// </param>
                /// <param name="currentTransform" type="Object">
                /// The MSCSSmatrix transform that is pending application to the object
                /// </param>
                /// <returns type="Object">
                /// The updated change in transform parameters
                /// </returns>

                // create the new transform by applying 
                // the proposed delta to the current transform
                var transform = new MSCSSMatrix().translate(delta.pivot.x, delta.pivot.y).
                    translate(delta.translation.x, delta.translation.y).
                    rotate(delta.rotation).
                    scale(delta.scale).
                    translate(-delta.pivot.x, -delta.pivot.y).multiply(currentTransform);

                var transformParams = {
                    rotation: currentTransformParams.rotation + delta.rotation,
                    scale: currentTransformParams.scale * delta.scale
                };
                

                var position = pif.calculateTransformedPosition(transformParams, transform);

                // include a padding so that the object can't be moved completely to the edge
                var padding = 80;
                var locked = false;

                // make sure that the new position of the object is not outside it's bounding box
                // if it is outside, then update the delta translation so that it isn't moved
                if (position.right < padding) {
                    delta.translation.x = 0;
                    locked = true;
                }
                else if (position.left > pif.containerWidth - padding) {
                    delta.translation.x = 0;
                    locked = true;
                }
                if (position.bottom < padding) {
                    delta.translation.y = 0;
                    locked = true;
                }
                else if (position.top> pif.containerHeight - padding) {
                    delta.translation.y = 0;
                    locked = true;
                }

                // If the object will be moved outside of the bounds, lock rotation and scale as well
                if (locked) {
                    delta.rotation = 0;
                    delta.scale = 1;
                }

                return (delta);
            }
        }),
        
        CreateAndApplyManipulationFilter: function (elmParent, elm, moveable, initialScale, initialTranslation) {
            /// <summary>
            /// Static helper function to create a manipulation filter given the parent and element.
            /// Then applies it to the given Manipulable object
            /// </summary>
            /// <param name="elmParent" type="Object">
            /// The parent html element for the manipulation filter
            /// </param>
            /// <param name="elm" type="Object">
            /// The html element which the manipulation filter will be applied to
            /// </param>
            /// <param name="moveable" type="Object">
            /// The manipulable object to which the filter will be applied
            /// </param>
            /// <param name="initialScale" type="Number">
            /// The value of the scale parameter that was applied on creation of the object
            /// </param>
            /// <param name="initialTranslation" type="Object">
            /// The value of the translation parameter that was applied on creation of the object
            /// Is in the form: {x: 0, y: 0}
            /// </param>

            // create a new filter and set params
            var pictureInFrame = new ManipulationFilters.PictureInFrame();

            // The dimensions that will constrain the moveable object
            pictureInFrame.containerHeight = elmParent.offsetHeight;
            pictureInFrame.containerWidth = elmParent.offsetWidth;

            // The dimensions of the moveable object
            pictureInFrame.objectHeight = elm.offsetHeight;
            pictureInFrame.objectWidth = elm.offsetWidth;

            // apply the filter to the given Manipulable object
            moveable.registerMoveHandler(pictureInFrame, ManipulationFilters.PictureInFrame.MoveHandler);
        },

        /// <summary>
        /// FixPivot forces the center of manipulation to be a set coordinate (instead of
        /// being determined by the centroid of the fingers in the manipulation.
        /// </summary>
        FixPivot: WinJS.Class.define(function() {
        }, {
        }, {
            MoveHandler: function (pivot, delta) {
                /// <summary>
                /// The function that will be called by the ItemMover before applying the transformation
                /// to the manipulable element
                /// </summary>
                /// <param name="pivot" type="Object">
                /// The constant pivot angle that should be applied to the Manipulable object
                /// during every manipulation
                /// </param>
                /// <param name="delta" type="Object">
                /// Contains all of the transformation params:
                ///     pivot, delta, rotation, scale
                /// </param>
                delta.pivot = pivot;
                return delta;
            }
        }),
    });
})();