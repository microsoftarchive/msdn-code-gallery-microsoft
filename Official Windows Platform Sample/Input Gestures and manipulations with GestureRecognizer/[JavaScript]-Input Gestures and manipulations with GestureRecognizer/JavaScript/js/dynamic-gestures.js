//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/dynamic-gestures.html", {
        ready: function (element, options) {
            document.getElementById("startButton").addEventListener("click", game.startGame, false);
        }
    });
    WinJS.Namespace.define("Primitives", {
        point: function (x, y) { return { X: x, Y: y }; },
        line: function (a, b) { return { A: a, B: b }; },
        matrix: function (a, b, c, d, e, f) { return { A: a, B: b, C: c, D: d, E: e, F: f }; }
    });

    WinJS.Namespace.define("Operators", {
        multiMatMult: function () {
            var i, result = arguments[arguments.length - 1];
            for (i = arguments.length - 2; i >= 0; --i) {
                // multiply arguments[i] with result so far
                var l = arguments[i];
                var r = result;
                var t = Primitives.matrix();
                t.A = l.A * r.A + l.B * r.D;
                t.B = l.A * r.B + l.B * r.E;
                t.C = l.A * r.C + l.B * r.F + l.C;
                t.D = l.D * r.A + l.E * r.D;
                t.E = l.D * r.B + l.E * r.E;
                t.F = l.D * r.C + l.E * r.F + l.F;
                result = t;
            }

            return result;
        },
        transformPoint: function (t, p) {
            return Primitives.point(t.A * p.X + t.B * p.Y + t.C, t.D * p.X + t.E * p.Y + t.F);
        },
        getTranslateMatrix: function (p) {
            return Primitives.matrix(1.0, 0.0, p.X, 0.0, 1.0, p.Y);
        },
        getScaleMatrix: function (s) {
            return Primitives.matrix(s, 0.0, 0.0, 0.0, s, 0.0);
        },
        getRotateMatrix: function (r) { // takes an angle in radians
            return Primitives.matrix(Math.cos(r), -Math.sin(r), 0.0, Math.sin(r), Math.cos(r), 0.0);
        },
        getVectorLengthSquared: function (v) {
            return v.X * v.X + v.Y * v.Y;
        },
        getResizedVector: function (v, l) {
            var magnitude = Math.sqrt(v.X * v.X + v.Y * v.Y);
            if (magnitude !== 0) {
                return Primitives.point(v.X * l / magnitude, v.Y * l / magnitude);
            } else {
                return Primitives.point(0, 0);
            }
        },
        subtractVectors: function (v1, v2) {
            return Primitives.point(v1.X - v2.X, v1.Y - v2.Y);
        },
        addVectors: function (v1, v2) {
            return Primitives.point(v1.X + v2.X, v1.Y + v2.Y);
        },
        dot: function (v1, v2) {
            return v1.X * v2.X + v1.Y * v2.Y;
        },
    });

    // Utility that helps keep track of transforms and updating transforms

    var transformable = function () {
        var that = {};
        var t = Primitives.matrix(1.0, 0.0, 0.0,
            0.0, 1.0, 0.0);

        var initialPosition;

        that.initialize = function (position) {
            initialPosition = position;
        };

        that.getPosition = function () {
            return Operators.transformPoint(t, initialPosition);
        };

        that.getInitialPosition = function () {
            return Primitives.point(initialPosition.X, initialPosition.Y);
        };

        that.transform = function (pivot, scale, rotation, translation) {
            var pivotT = Operators.getTranslateMatrix(pivot);
            var scaleT = Operators.getScaleMatrix(scale);
            var rotateT = Operators.getRotateMatrix(rotation);
            var translateT = Operators.getTranslateMatrix(translation);
            var pivotTinv = Operators.getTranslateMatrix(Primitives.point(-pivot.X, -pivot.Y));

            var currentScale = Math.sqrt(t.A * t.A + t.D * t.D);
            if (currentScale < 0.1 && scale < 1) {
                scaleT = Operators.getScaleMatrix(1); // enforce minimum scale
            }

            t = Operators.multiMatMult(pivotT, translateT, rotateT, scaleT, pivotTinv, that.getTransform());
            // This function sets a pivot point, then applies scaling, rotation, then translation.
        };

        that.getTransform = function () {
            return t;
        };

        that.setTransform = function (transform) {
            t = transform;
        };

        return that;
    };

    // A gameObject is a transformable that is Drawable on screen

    var gameObject = function (spec) {
        var that = transformable();
        var domElement = spec.element || '';

        // Updates the transform on the DOM element
        that.draw = function (viewT) {
            var t = Operators.multiMatMult(viewT, that.getTransform());
            if (domElement) {
                domElement.setAttributeNS(null, "transform", "matrix(" + t.A + " " + t.D + " " + t.B + " " +
                t.E + " " + t.C + " " + t.F + ")");
            }
        };

        that.getRootElement = function () {
            return domElement;
        };

        return that;
    };

    // entities contains all the movable elements of the game

    var entities = (function () {
        var that = {};
        var maze, ball, globalTransform;

        that.initialize = function () {
            maze = gameObject({ element: document.getElementById("Maze") });
            ball = gameObject({ element: document.getElementById("Ball") });
            globalTransform = transformable();
        };

        that.getMaze = function () { return maze; };
        that.getBall = function () { return ball; };
        that.getGlobalTransform = function () { return globalTransform; };

        return that;
    })();

    // Updates the physics simulation of the game

    var simulator = (function () {
        var that = {};

        var mazeEntity;
        var ballEntity;

        var ball = {};
        var walls = [];

        var g = transformable();

        var maxSpeed = 0.8;
        var maxSpeedSquared = maxSpeed * maxSpeed;

        var steadyState = false;

        function translateBall(translation) {
            ballEntity.transform(ball.position, 1.0, 0.0, translation);
            ball.position = ballEntity.getPosition();
        };

        that.initialize = function () {
            mazeEntity = entities.getMaze();
            ballEntity = entities.getBall();

            g.initialize(Primitives.point(0, 0.001));
            var ballElement = ballEntity.getRootElement();
            ball.radius = parseFloat(ballElement.getAttribute("r"));
            ballEntity.initialize(Primitives.point(parseFloat(ballElement.getAttribute("cx")), parseFloat(ballElement.getAttribute("cy"))));
            ball.position = ballEntity.getPosition();
            ball.initialPosition = Primitives.point(ball.position.X, ball.position.Y);
            ball.lastPosition = Primitives.point(ball.position.X, ball.position.Y);
            ball.velocity = Primitives.point(0, 0);

            var lineElements = mazeEntity.getRootElement().getElementsByTagName("line");
            for (var i = 0; i < lineElements.length; ++i) {
                var lineElement = lineElements[i];
                var A = Primitives.point(parseFloat(lineElement.getAttribute("x1")), parseFloat(lineElement.getAttribute("y1")));
                var B = Primitives.point(parseFloat(lineElement.getAttribute("x2")), parseFloat(lineElement.getAttribute("y2")));
                walls.push(Primitives.line(A, B));
            }
        };

        that.dehydratedState = function () {
            return { ball: ball, gravity: g.getTransform() };
        };

        that.rehydrateFromState = function (state) {
            ball = state.ball;
            g.setTransform(state.gravity);
        };

        that.transformGravity = function (pivot, rotation) {
            g.transform(pivot, 1, rotation, Primitives.point(0, 0));
        };

        that.getUp = function () {
            var down = g.getPosition();
            return Operators.getResizedVector(Primitives.point(-down.X, -down.Y), 1);
        };

        that.getBallPosition = function () {
            return ball.position;
        };

        that.step = function (deltaT) {
            var newVelocity = Primitives.point(0, 0);

            var collision = false;
            for (var i = 0; i < walls.length; ++i) {
                // has there been a collision?
                var b = Operators.subtractVectors(ball.position, walls[i].A);
                var w = Operators.subtractVectors(walls[i].B, walls[i].A);
                var wUnit = Operators.getResizedVector(w, 1);
                var n = Operators.getResizedVector(Primitives.point(-w.Y, w.X), 1);

                var distanceSq = 0;
                var bdotw = Operators.dot(b, wUnit);
                var corner = true;
                if (bdotw < 0) {
                    distanceSq = Operators.getVectorLengthSquared(b);
                } else if (bdotw * bdotw > Operators.getVectorLengthSquared(w)) {
                    distanceSq = Operators.getVectorLengthSquared(Operators.subtractVectors(ball.position, walls[i].B));
                } else {
                    var distance = Operators.dot(b, n);
                    distanceSq = distance * distance;
                    corner = false;
                }

                if (distanceSq < ball.radius * ball.radius) {
                    if (Operators.dot(n, ball.velocity) < 0) { n = Primitives.point(-n.X, -n.Y); }
                    var bPrime = Operators.addVectors(b, Operators.getResizedVector(n, ball.radius));
                    collision = true;
                    var reverseVelocity = Primitives.point(-ball.velocity.X, -ball.velocity.Y);
                    // component of new velocity parallel to the wall
                    var parallel = Operators.getResizedVector(wUnit, -Operators.dot(reverseVelocity, wUnit));

                    // component of new velocity perpendicular to the wall
                    var perpendicular = Operators.getResizedVector(n, Operators.dot(reverseVelocity, n));

                    // update velocity
                    newVelocity = Operators.addVectors(newVelocity, Operators.addVectors(parallel, perpendicular));
                }
            }

            if (collision) {
                // update velocity for the collision
                var speed = Math.sqrt(Operators.getVectorLengthSquared(ball.velocity));
                if (Operators.getVectorLengthSquared(newVelocity) > 0) {
                    ball.velocity = Operators.getResizedVector(newVelocity, speed);
                } else {
                    ball.velocity = Primitives.point(-ball.velocity.X, -ball.velocity.Y);
                }

            } else {
                // update velocity for gravity
                ball.velocity = Primitives.point(ball.velocity.X + deltaT * g.getPosition().X, ball.velocity.Y + deltaT * g.getPosition().Y);
                if (Operators.getVectorLengthSquared(ball.velocity) > maxSpeedSquared) {
                    ball.velocity = Operators.getResizedVector(ball.velocity, maxSpeed);
                }
            }

            ball.lastPosition.X = ball.position.X;
            ball.lastPosition.Y = ball.position.Y;
            translateBall(Primitives.point(deltaT * ball.velocity.X, deltaT * ball.velocity.Y));
        };

        return that;
    })();

    // The inputManager Singleton handles all Gesture Recognition for this Sample

    var inputManager = (function () {
        var that = {};
        var gr;
        var svg;
        var ball;
        var manipulating = false;


        // Helper functions
        function getViewBoxCoordinates(p) {
            var svgBoundingRect = svg.getBoundingClientRect();
            var scale = 1100 / (svgBoundingRect.right - svgBoundingRect.left);
            var t = Operators.getScaleMatrix(scale);
            return Operators.transformPoint(t, p);
        };

        that.manipulationHandler = function (evt) {
            if (evt.delta) {
                // pivot and translation need to be transformed into viewbox coordinates
                var pivot = getViewBoxCoordinates(Primitives.point(evt.position.x, evt.position.y));
                var rotation = evt.delta.rotation / 180 * Math.PI;
                var translation = getViewBoxCoordinates(Primitives.point(evt.delta.translation.x, evt.delta.translation.y));
                entities.getGlobalTransform().transform(pivot, evt.delta.scale, rotation, translation);

                // update simulation gravity
                simulator.transformGravity(Primitives.point(0, 0), -rotation);
            }
        };

        that.isManipulating = function () {
            return manipulating;
        };

        // The following functions are registered to handle DOM pointer events.

        that.processDown = function (evt) {
            // Get the current PointerPoint
            var pp = evt.getCurrentPoint(svg);

            // Feed the PointerPoint to GestureRecognizer
            gr.processDownEvent(pp);
        };

        that.processMove = function (evt) {
            // Get intermediate PointerPoints
            var pps = evt.getIntermediatePoints(svg);

            // processMoveEvents takes an array of intermediate PointerPoints
            gr.processMoveEvents(pps);
        };

        that.processUp = function (evt) {
            // Get the current PointerPoint
            var pp = evt.getCurrentPoint(svg);

            // Feed GestureRecognizer
            gr.processUpEvent(pp);

        };

        that.processMouse = function (evt) {
            var pp = evt.getCurrentPoint(svg);

            gr.processMouseWheelEvent(pp, evt.shiftKey, evt.ctrlKey);
        };

        // The following functions are registered to handle GestureRecognizer gesture events

        that.manipulationStartedHandler = function (evt) {
            manipulating = true;
            that.manipulationHandler(evt);
        };

        that.manipulationDeltaHandler = function (evt) {
            that.manipulationHandler(evt);
        };

        that.manipulationEndHandler = function (evt) {
            manipulating = false;
            that.manipulationHandler(evt);
        };

        that.tappedHandler = function (evt) {
            if (ball.getAttribute("fill") === "#33ccff") {
                ball.setAttributeNS(null, "fill", "yellow");
            } else {
                ball.setAttributeNS(null, "fill", "#33ccff");
            }
        };

        that.rightTappedHandler = function (evt) {
            if (ball.getAttribute("stroke") === "#33ccff") {
                ball.setAttributeNS(null, "stroke", "red");
            } else {
                ball.setAttributeNS(null, "stroke", "#33ccff");
            }
        };

        // Initialize the inputManager object


        that.initialize = function () {
            // Initialize gesture recognizer
            gr = new Windows.UI.Input.GestureRecognizer();

            // Configuring GestureRecognizer to detect manipulation rotation, translation, scaling,
            // + inertia for those three components of manipulation + the tap and press-and-hold gestures
            gr.gestureSettings =
                Windows.UI.Input.GestureSettings.manipulationRotate |
                Windows.UI.Input.GestureSettings.manipulationTranslateX |
                Windows.UI.Input.GestureSettings.manipulationTranslateY |
                Windows.UI.Input.GestureSettings.manipulationScale |
                Windows.UI.Input.GestureSettings.manipulationRotateInertia |
                Windows.UI.Input.GestureSettings.manipulationScaleInertia |
                Windows.UI.Input.GestureSettings.manipulationTranslateInertia |
                Windows.UI.Input.GestureSettings.hold | // Hold must be set in order to recognize press-and-hold
                Windows.UI.Input.GestureSettings.rightTap |
                Windows.UI.Input.GestureSettings.tap;

            // Turn off UI feedback for gestures (we'll still see UI feedback for PointerPoints)
            gr.showGestureFeedback = false;

            // Register event listeners for the gestures that we just configured
            gr.addEventListener('manipulationstarted', that.manipulationStartedHandler);
            gr.addEventListener('manipulationupdated', that.manipulationDeltaHandler);
            gr.addEventListener('manipulationcompleted', that.manipulationEndHandler);
            gr.addEventListener('tapped', that.tappedHandler);
            gr.addEventListener('righttapped', that.rightTappedHandler);

            ball = document.getElementById("Ball");
            svg = document.getElementById("SVGWindow");


            // Register event listeners for DOM pointer events, these are the
            // raw touch events we will be using to feed the gestureRecognizer
            svg.addEventListener('pointerdown', that.processDown, false);
            svg.addEventListener('pointermove', that.processMove, false);
            svg.addEventListener('pointerup', that.processUp, false);
            svg.addEventListener('pointercancel', that.processUp, false);
            svg.addEventListener('wheel', that.processMouse, false);

        };


        return that;
    })();

    var lifetimeManager = (function () {
        var that = {};
        var app = WinJS.Application;

        that.initialize = function () {
            app.addEventListener("checkpoint", that.checkpoint, false);
            app.addEventListener("activated", that.activated, false);
            app.start();
        };

        that.checkpoint = function (evt) {
            var state = {
                ball: entities.getBall().dehydratedState(),
                simulator: simulator.dehydratedState(),
                globalTransform: entities.getGlobalTransform().dehydratedState()
            };

            app.sessionState.state = state;
        };

        that.activated = function (evt) {
            if (evt.detail.kind === Windows.ApplicationModel.Activation.ActivationKind.launch) {
                if (evt.detail.previousExecutionState === Windows.ApplicationModel.Activation.ApplicationExecutionState.terminated) {
                    if (app.sessionState.state) {
                        var state = app.sessionState.state;
                        entities.getBall().rehydrateFromState(state.ball);
                        simulator.rehydrateFromState(state.simulator);
                        entities.getGlobalTransform().rehydrateFromState(state.globalTransform);
                    }
                }
            }

        };

        return that;

    })();

    // game contains the loop that runs updates of the game and sets up a timer that calls the loop
    // every 16 ms
    var game = (function () {

        var that = {};
        var lastTick;
        var gestureRecognizer;
        var wasManipulating = false;

        that.startGame = function () {
            entities.initialize();
            inputManager.initialize();
            simulator.initialize();
            lifetimeManager.initialize();

            var now = new Date();
            lastTick = now.getTime();
            var gameLoopId = window.setInterval(that.gameLoop, 16);
        };

        that.gameLoop = function () {
            // handle input

            var now = new Date();
            var ticks = now.getTime();
            while (ticks - lastTick >= 16) {
                // fixed timestep
                simulator.step(16);
                lastTick += 16;
            }

            entities.getMaze().draw(entities.getGlobalTransform().getTransform());
            entities.getBall().draw(entities.getGlobalTransform().getTransform());
        };

        return that;
    })();
})();
