//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved
/// <reference path="//Microsoft.WinJS.2.0/js/base.js" />

(function () {
    "use strict";

    // Our 'base' class - this is a classic inheritance example

    var DrawableShape = WinJS.Class.define(
        function (x, y) {
            this.x = x;
            this.y = y;
        },
        {
            stroke: "rgb(0,0,0)",
            fill: "rgb(255, 255, 255)",
            draw: function (context) {
                context.fillStyle = this.fill;
                context.strokeStyle = this.stroke;
                this.drawFill(context);
                this.drawStroke(context);
            },

            drawFill: function (context) {
                // To be implemented by derived classes
            },

            drawStroke: function (context) {
                // to be implmented by derived classes
            }
        });

    // First derived class

    var Line = WinJS.Class.derive(DrawableShape,
        // Constructor
        function (x1, y1, x2, y2) {
            DrawableShape.call(this, x1, y1);
            this.x2 = x2;
            this.y2 = y2;
        },
        // Instance members to add to the class
        {
            // this overrides the one from the base class
            drawStroke: function (context) {
                context.beginPath();
                context.moveTo(this.x, this.y);
                context.lineTo(this.x2, this.y2);
                context.stroke();
            }
        }
        // And static members, if any go here
        );

    // Another "derived" class to draw a rectangle outline
    var Rect = WinJS.Class.derive(DrawableShape,
        function (x, y, width, height) {
            // invoke the "base class" constructor
            DrawableShape.call(this, x, y);
            this.width = width;
            this.height = height;
        },
        {
            drawStroke: function (context) {
                context.strokeRect(this.x, this.y, this.width, this.height);
            }
        });

    // And a second derived class, deriving from Rect.
    // If you examine the prototype for this object,
    // you'll see that the prototypes are not built up in a
    // chain but instead the methods are aggregated into a single
    // level of prototypes.
    var FilledRect = WinJS.Class.derive(Rect,
        function (x, y, width, height, fill) {
            // Invoke the "base class" constructor
            Rect.call(this, x, y, width, height);
            this.fill = fill;
        },
        {
            drawFill: function (context) {
                context.fillRect(this.x, this.y, this.width, this.height);
            }
        });

    var page = WinJS.UI.Pages.define("/html/3_PrototypalInheritance.html", {
        ready: function (element, options) {
            var canvas = element.querySelector(".prototypalInheritanceCanvas");
            var ctx = canvas.getContext("2d");

            var shapes = [
                new Rect(75, 0, 50, 150),
                new Line(25, 75, 50, 125),
                new Line(150, 25, 175, 75),
                new FilledRect(50, 50, 100, 50, "green")
            ];

            for (var i = 0, len = shapes.length; i < len; ++i) {
                shapes[i].draw(ctx);
            }
        }
    });
})();
