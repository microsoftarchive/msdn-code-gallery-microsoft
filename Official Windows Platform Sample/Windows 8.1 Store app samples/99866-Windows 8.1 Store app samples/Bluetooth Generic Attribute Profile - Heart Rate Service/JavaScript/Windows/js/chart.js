//// Copyright (c) Microsoft Corporation. All rights reserved


//// The Chart control is used as an example, for illustration purposes only, as a way to present relevant data to the
//// user. It is not meant as a suggestion or Microsoft recommended best practice for rendering data associated with 
//// devices. For more information on planning, developing or differentiating your Windows Store app please visit:
//// http://dev.windows.com/
(function () {
    'use strict';

    function createOptions(data, options) {
        /// <summary>
        /// Retrieve important information from the data to be displayed for rendering purposes
        /// </summary>

        // Extract the dataset extreme values
        options.minValue = Number.MAX_VALUE;
        options.maxValue = Number.MIN_VALUE;

        if ((data !== null) && (data !== undefined)) {
            for (var i in data) {
                options.minValue = Math.min(data[i].value, options.minValue);
                options.maxValue = Math.max(data[i].value, options.maxValue);
            }
        }
    };


    // Define the class actually responsible for rendering the chart
    var renderer = WinJS.Class.define(function () { }, {
        chartId: undefined,
        canvasElement: undefined,
        canvasContext: undefined,
        options: {},
        offsetMap: [],
        yaxisCount: 5,      // Number of guidance lines for the chart background
        nPoints:    30,     // Number of data points to draw on the chart at a time

        makeVisible: function (canvasId) {
            this.chartId = canvasId;
            this.canvasElement = document.getElementById(this.chartId);
            if (this.canvasElement) {
                this.canvasElement.style.display = "";
            }
        },

        plot: function (data) {
            /// <summary>
            /// The function responsible for rendering the chart.
            /// </summary>
            /// <param name="canvasId" type="String">
            /// The id for the HTML5 canvas element to which the chart will get rendered for a particular device.
            /// </param>
            /// <param name="data" type="Object[]">
            /// The set of data points that will be displayed using the chart.
            /// </param>
            this.canvasElement = document.getElementById(this.chartId);
            if (this.canvasElement) {

                // Set the appropriate dimensions for the canvasElement
                this.canvasElement.width = this.canvasElement.offsetWidth;
                this.canvasElement.height = this.canvasElement.offsetHeight;

                // Retrieve the HTML5 canvas' rendering context
                this.canvasContext = this.canvasElement.getContext('2d');

                // Extract useful rendering options from the data
                this.options = {};
                createOptions(data, this.options);

                // Process the data for rendering
                this.fillOffsetMap(data);

                // Render the actual chart

                this.drawBackground();

                this.drawYaxis();

                this.draw('#DFD');
            }
        },

        fillOffsetMap: function (data) {

            // Clear the offsetMap before adding the new data
            this.offsetMap = [];

            if ((data !== null) && (data !== undefined) && (data.length > 0)) {

                var valueDiff = this.options.maxValue - this.options.minValue;
                // Add a 10% buffer to the extreme values on the chart, for better visibility
                var diffBuffer = (valueDiff > 0) ? (valueDiff * 0.1) : 2;
                this.options.maxValueBuffered = this.options.maxValue + diffBuffer;
                this.options.minValueBuffered = this.options.minValue - diffBuffer;
                this.options.minValueBuffered = (this.options.minValueBuffered > 0) ?
                    this.options.minValueBuffered : 0;

                valueDiff = this.options.maxValueBuffered - this.options.minValueBuffered;

                // Restrict the number of points displayed to nPoints to achieve the chart data scrolling
                var pointsDisplayed = (data.length > this.nPoints) ? this.nPoints : data.length;

                var bufferWidth = this.canvasElement.width * 0.05;
                var tickOffset = (this.canvasElement.width - (bufferWidth * 2)) / pointsDisplayed;
                var currentOffset = bufferWidth;

                for (var i = data.length - pointsDisplayed, len = data.length; i < len; i++) {
                    var currentDiff = this.options.maxValueBuffered - data[i].value;
                    var dataObj = {
                        offset_x: currentOffset,
                        offset_y: (currentDiff / valueDiff) * this.canvasElement.height,
                        value: data[i].value
                    };

                    this.offsetMap.push(dataObj);
                    currentOffset += tickOffset;
                }
            }
        },

        draw: function (strokeStyle) {
            /// <summary>
            /// Function that implements chart rendering
            /// </summary>

            if (this.offsetMap.length > 0) {
                this.canvasContext.lineWidth = 10;
                this.canvasContext.lineCap = 'round';
                this.canvasContext.lineJoin = 'round';
                this.canvasContext.shadowOffsetX = 1.5;
                this.canvasContext.shadowOffsetY = 1.5;
                this.canvasContext.shadowBlur = 0;
                this.canvasContext.shadowColor = 'rgba(0, 0, 0, .6)';
                this.canvasContext.strokeStyle = strokeStyle;

                this.canvasContext.beginPath();
                this.canvasContext.moveTo(this.offsetMap[0].offset_x, this.offsetMap[0].offset_y);
                for (var i in this.offsetMap) {
                    this.canvasContext.lineTo(this.offsetMap[i].offset_x, this.offsetMap[i].offset_y);
                }
                this.canvasContext.stroke();
            }
        },

        drawBackground: function () {
            /// <summary>
            /// Function that refreshes the chart background
            /// </summary>
            var colors = ['#9FF', '#8EF', '#7DF', '#6CF', '#5BF'];
            var tickOffset_y = this.canvasElement.height / this.yaxisCount;
            var offset_y = 0;
            for (var i = 0; i <= this.yaxisCount; i++) {
                this.canvasContext.fillStyle = colors[i];
                this.canvasContext.fillRect(0, offset_y, this.canvasElement.width, tickOffset_y);
                offset_y += tickOffset_y;
            }
        },

        drawYaxis: function () {
            var rightTextMargin = 9;
            var bottomTextMargin = 22;
            var noOfStripes = this.yaxisCount - 1;
            
            if ((this.canvasContext !== null) && (this.canvasContext !== undefined)) {
                this.canvasContext.font = '18px "Segoe UI"';
                this.canvasContext.fillStyle = 'rgba(255, 255, 255, 0.9)';
                this.canvasContext.textBaseline = 'top';
                this.canvasContext.textAlign = 'right';

                if ((this.canvasElement === null) || (this.canvasElement === undefined)) {
                    return;
                }
                if ((this.options.maxValueBuffered === null) || (this.options.maxValueBuffered === undefined)) {
                    return;
                }

                this.canvasContext.fillText(this.options.maxValueBuffered.toFixed(1), this.canvasElement.width - 2, 0);

                var percent = (this.options.maxValueBuffered - this.options.minValueBuffered) * 0.25;
                for (var i = 1; i < 4; i++) {
                    var percentVal = this.options.maxValueBuffered - (percent * i);
                    this.canvasContext.fillText(percentVal.toFixed(1), this.canvasElement.width - 2,
                        (i * (this.canvasElement.height / noOfStripes)) - rightTextMargin);
                }

                this.canvasContext.fillText(this.options.minValueBuffered.toFixed(1), this.canvasElement.width - 2,
                    this.canvasElement.height - bottomTextMargin);
            }
        }
    });

    WinJS.Namespace.define('Chart', {
        renderer: renderer,
    });
})();
