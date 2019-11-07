//// Copyright (c) Microsoft Corporation. All rights reserved

(

function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario1.html",

        {
            //on ready - set button states and add event listeners to the buttons.
            ready: function (element, options) {
                document.getElementById("ScenarioStartScanButton").addEventListener("click", startReceivingData, false);
                document.getElementById("ScenarioEndScanButton").addEventListener("click", endReceivingData, false);
                document.getElementById("ScenarioStartScanButton").disabled = false;
                document.getElementById("ScenarioEndScanButton").disabled = true;
            },

            //On unload - clean up if claimedscanner object is not null.
            unload: function () {
                if (document.getElementById("ScenarioStartScanButton").disabled) {
                    if (_claimedScanner !== null) {
                        //remove event listeners that were added before and close.
                        _claimedScanner.removeEventListener("datareceived", onDataReceived);
                        _claimedScanner.removeEventListener("releasedevicerequested", onReleasedeviceRequested);
                        _claimedScanner.close();
                        _claimedScanner = null;
                    }
                    _scanner = null;
                }
            }
        });
    var _scanner = null;
    var _claimedScanner = null;
  
    // Creates the default barcode scanner and claims it. Once, claimed, it adds event listeners for onDataReceived and releaseDeviceRequested events.
    // On error, it logs the error message.
    function startReceivingData() {
        // create the barcode scanner. 
        Windows.Devices.PointOfService.BarcodeScanner.getDefaultAsync().then(function (scanner) {
            if (scanner !== null) {
                _scanner = scanner;
                WinJS.logAppend("Default Barcode Scanner created..",  "status");
                WinJS.logAppend("Device Id is:" + scanner.deviceId, "status");
                // after successful creation, claim the scanner for exclusive use and enable it so that data reveived events are received.
                scanner.claimScannerAsync().done(function (claimedScanner) {
                    if (claimedScanner !== null) {
                        _claimedScanner = claimedScanner;
                        // Ask the API to decode the data by default. By setting this, API will decode the raw data from the barcode scanner and 
                        // send the ScanDataLabel and ScanDataType in the DataReceived event
                        claimedScanner.isDecodeDataEnabled = true;
                        WinJS.logAppend("Claim Barcode Scanner succeeded..", "status");                        
                        // after successfully claiming, attach the datareceived event handler.
                        claimedScanner.addEventListener("datareceived", onDataReceived);
                        // It is always a good idea to have a release device requested event handler. If this event is not handled, there are chances of another app can 
                        // claim ownsership of the barcode scanner.
                        claimedScanner.addEventListener("releasedevicerequested", onReleasedeviceRequested);
                        // enable the scanner.
                        // Note: If the scanner is not enabled (i.e. EnableAsync not called), attaching the event handler will not be any useful because the API will not fire the event 
                        // if the claimedScanner has not beed Enabled
                        claimedScanner.enableAsync().done(function () {
                            WinJS.logAppend("Enable Barcode Scanner succeeded..",  "status");
                            WinJS.logAppend("Ready to Scan...",  "status");
                            document.getElementById("ScenarioStartScanButton").disabled = true;
                            document.getElementById("ScenarioEndScanButton").disabled = false;
                        }, function error(e) {
                            WinJS.logAppend("Error enabling scanner..." + e.message,  "error");
                        });

                    }else {
                        WinJS.logAppend("Could not claim the scanner.",  "error");
                    }
                }, function error(e) {
                    WinJS.logAppend("Could not claim the scanner." + e.message,  "error");
                });

            }else {
                WinJS.logAppend("Barcode Scanner not found. Please connect a Barcode Scanner..",  "error");
            }

        }, function error(e) {
            WinJS.logAppend("Scanner GetDefault Async Unsuccessful" + e.message,  "error");
        });
    }

    // Event handler for the Release Device Requested event fired when barcode scanner receives Claim request from another application
    function onReleasedeviceRequested(args) {
        _claimedScanner.retainDevice();
    }

    function getDataString(data) {
        var result = "";

        if (data === null) {
            result = "No data";
        }
        else {
            // Just to show that we have the raw data, we'll print the value of the bytes.
            // Arbitrarily limit the number of bytes printed to 20 so the UI isn't overloaded.
            var MAX_BYTES_TO_PRINT = 20;
            var bytesToPrint = (data.length < MAX_BYTES_TO_PRINT) ? data.length : MAX_BYTES_TO_PRINT;

            var reader = Windows.Storage.Streams.DataReader.fromBuffer(data);

            for (var byteIndex = 0; byteIndex < bytesToPrint; ++byteIndex)
            {
                result += reader.readByte().toString(16) + " ";
            }

            if (bytesToPrint < data.length)
            {
                result += "...";
            }
        }

        return result;
    }

    function getDataLabelString(data, scanDataType)
    {
        var result = null;

        // Only certain data types contain encoded text.
        //   To keep this simple, we'll just decode a few of them.
        if (data === null) {
            result = "No data";
        }
        else {
            switch (Windows.Devices.PointOfService.BarcodeSymbologies.getName(scanDataType)) {
                case "Upca":
                case "UpcaAdd2":
                case "UpcaAdd5":
                case "Upce":
                case "UpceAdd2":
                case "UpceAdd5":
                case "Ean8":
                case "TfStd":
                    // The UPC, EAN8, and 2 of 5 families encode the digits 0..9
                    // which are then sent to the app in a UTF8 string (like "01234")

                    // This is not an exhaustive list of symbologies that can be converted to a string

                    var reader = Windows.Storage.Streams.DataReader.fromBuffer(data);
                    result = reader.readString(data.length);
                    break;
                default:
                    // Some other symbologies (typically 2-D symbologies) contain binary data that
                    //  should not be converted to text.
                    result = "Decoded data unavailable. Raw label data: " + getDataString(data);
                    break;
            }
        }

        return result;
    }

    // Event handler for the DataReceived event fired when a barcode is scanned by the barcode scanner 
    function onDataReceived(args) {
        var tempScanType = Windows.Devices.PointOfService.BarcodeSymbologies.getName(args.report.scanDataType);
       
        document.getElementById("ScenarioOutputScanDataType").textContent = tempScanType;
        document.getElementById("ScenarioOutputScanData").textContent = getDataString(args.report.scanData);
        document.getElementById("ScenarioOutputScanDataLabel").textContent = getDataLabelString(args.report.scanDataLabel, args.report.scanDataType);
    }
    
    //This method removes the event listeners from the claimed scanner object if it is not null and then closes the claimed scanner so that someone else can use it.
    function endReceivingData() {
        if (_claimedScanner !== null) {
            _claimedScanner.removeEventListener("datareceived", onDataReceived);
            _claimedScanner.removeEventListener("releasedevicerequested", onReleasedeviceRequested);            
            _claimedScanner.close();
            _claimedScanner = null;
        }
        _scanner = null;
        // reset button states
        WinJS.logAppend("Click the Start Scanning Button.", "status");
        document.getElementById("ScenarioStartScanButton").disabled = false;
        document.getElementById("ScenarioEndScanButton").disabled = true;
        document.getElementById("ScenarioOutputScanDataType").textContent = "No Data";
        document.getElementById("ScenarioOutputScanData").textContent = "No Data";
        document.getElementById("ScenarioOutputScanDataLabel").textContent = "No Data";       
    }


})();
