//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

function createScaleMatrix(scaleWidth, scaleHeight, centerX, centerY) {
    var scale = new Object;
    scale._11 = scaleWidth;
    scale._12 = 0.0;
    scale._21 = 0.0;
    scale._22 = scaleHeight;
    scale._31 = centerX - scaleWidth * centerX;
    scale._32 = centerY - scaleHeight * centerY;
    return scale;
};

function createTranslateMatrix(xOffset, yOffset) {
    var translate = new Object;
    translate._11 = 1.0;
    translate._12 = 0.0;
    translate._21 = 1.0;
    translate._22 = 0.0;
    translate._31 = xOffset;
    translate._32 = yOffset;
    return translate;
};

function invertMatrix(matrix) {
    var det = (matrix._11 * matrix._22) - (matrix._12 * matrix._21);

    var retVal = new Object;

    retVal._11 = matrix._22 / det;
    retVal._12 = -matrix._12 / det;
    retVal._21 = -matrix._21 / det;
    retVal._22 = matrix._11 / det;
    retVal._31 = (matrix._21 * matrix._32 - matrix._31 * matrix._22) / det;
    retVal._32 = (matrix._31 * matrix._12 - matrix._11 * matrix._32) / det;

    return retVal;
};

function transformPoint(matrix, x, y) {
    var retVal = new Object;

    retVal.x = x * matrix._11 + y * matrix._21 + matrix._31;
    retVal.y = x * matrix._12 + y * matrix._22 + matrix._32;

    return retVal;
};

function parseMatrix(matrixString) {
    var retVal = new Object;
    var startIndex = matrixString.indexOf("(", 0) + 1;
    var endIndex = matrixString.indexOf(")", 0);

    var xOffset, yOffset, xScale, yScale;

    var matrixValues = matrixString.substring(startIndex, endIndex).split(", ");
    for (var i = 0; i < matrixValues.length; i++) {
        switch (i) {
            case 0:
                retVal._11 = parseFloat(matrixValues[i]);
                break;
            case 1:
                retVal._12 = parseFloat(matrixValues[i]);
                break;
            case 2:
                retVal._21 = parseFloat(matrixValues[i]);
                break;
            case 3:
                retVal._22 = parseFloat(matrixValues[i]);
                break;
            case 4:
                retVal._31 = parseFloat(matrixValues[i]);
                break;
            case 5:
                retVal._32 = parseFloat(matrixValues[i]);
                break;

            default:
                break;
        }
    }
    return retVal;
};
