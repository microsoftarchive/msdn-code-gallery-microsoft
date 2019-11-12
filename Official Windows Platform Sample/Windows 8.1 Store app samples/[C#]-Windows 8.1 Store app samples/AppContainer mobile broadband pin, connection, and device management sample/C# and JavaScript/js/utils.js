//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

var mbComApiUtil = {};

// This function parses the common error codes
(function () {
    "use strict";

    function parseExceptionCodeAndPrint(error) {
        var returnStr = "";

        switch (error.number) {
            case -2147023728: // 0x80070490: ERROR_NOT_FOUND
                returnStr += "\"The mobile broadband interface is not present\", error code: 0x80070490";
                returnStr += "\nPlease plug in required mobile broadband device and restart sample.";
                break;
            case -2147023834: // 0x80070426: ERROR_SERVICE_NOT_ACTIVE
                returnStr += "\"The service has not been started\", error code: 0x80070426";
                returnStr += "\nPlease start wwansvc and restart sample.";
                break;
            case -2147024875: // 0x80070015: ERROR_NOT_READY
                returnStr += "\"The device is not ready\", error code: 0x80070015";
                break;
            case -2147024841: // 0x80070037: ERROR_DEV_NOT_EXIST
                returnStr += " \"The specified mobile broadband device is no longer available\", error code: 0x80070037";
                break;
            default:
                returnStr += "Unexpected exception occured: " + error;
                break;
        }

        WinJS.log && WinJS.log(returnStr, "sample", "error");
    }

    mbComApiUtil.parseExceptionCodeAndPrint = parseExceptionCodeAndPrint;
})();
