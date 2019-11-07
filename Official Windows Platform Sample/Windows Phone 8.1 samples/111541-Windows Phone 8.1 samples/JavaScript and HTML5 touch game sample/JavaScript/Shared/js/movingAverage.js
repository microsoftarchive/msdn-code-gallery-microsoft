//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

var MovingAverage = WinJS.Class.define(
    null,
{
    values: [],
    valueCountMax: 20,

    initialize: function (valueCountMax) {
        this.valueCountMax = valueCountMax;
    },

    add: function (newValue) {
        if (this.values.length > this.valueCountMax) {
            this.values.shift();
        }
        this.values.push(newValue);
    },

    average: function () {
        if (this.values.length > 0) {
            return this.sum() / this.values.length;
        } else {
            return 0;
        }
    },

    sum: function () {
        var runningSum = 0;
        for (var i = 0; i < this.values.length; i++) {
            runningSum += this.values[i];
        }
        return runningSum;
    },

});
