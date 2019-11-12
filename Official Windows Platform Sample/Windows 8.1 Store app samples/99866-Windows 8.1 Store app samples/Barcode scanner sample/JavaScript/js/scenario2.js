//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario2.html", {
        //on ready - set button states and add event listeners to the buttons.
        ready: function (element, options) {
            document.getElementById("ScenarioStartScanningInstance1").addEventListener("click", buttonStartScanningInstance1, false);
            document.getElementById("ScenarioEndScanningInstance1").addEventListener("click", buttonEndScanningInstance1, false);
            document.getElementById("ScenarioStartScanningInstance1").disabled = false;
            document.getElementById("ScenarioEndScanningInstance1").disabled = true;

            document.getElementById("ScenarioStartScanningInstance2").addEventListener("click", buttonStartScanningInstance2, false);
            document.getElementById("ScenarioEndScanningInstance2").addEventListener("click", buttonEndScanningInstance2, false);
            document.getElementById("ScenarioStartScanningInstance2").disabled = false;
            document.getElementById("ScenarioEndScanningInstance2").disabled = true;
        },

        //On unload - clean up if claimedscanner objects are not null.
        unload: function () {
            if (_claimedScanner1 !== null) {
                //remove event listeners that were added before and close.
                _claimedScanner1.removeEventListener("datareceived", onDataReceived);
                _claimedScanner1.removeEventListener("releasedevicerequested", onReleasedeviceRequested1);
                _claimedScanner1.close();
                _claimedScanner1 = null;
            }

            if (_claimedScanner2 !== null) {
                //remove event listeners that were added before and close.
                _claimedScanner2.removeEventListener("datareceived", onDataReceived);
                _claimedScanner2.removeEventListener("releasedevicerequested", onReleasedeviceRequested2);
                _claimedScanner2.close();
                _claimedScanner2 = null;
            }
            _scanner1 = null;
            _scanner2 = null;
        }
    });

    var _activeBarcodeScannerInstance = { instance1: 0, instance2: 1 };
    var _scanner1 = null;
    var _claimedScanner1 = null;
    var _scanner2 = null;
    var _claimedScanner2 = null;

    var _currentInstance;
    var _retain1 = true;
    var _retain2 = true;

    //create scanner instance 1 and claim and enable it.
    function buttonStartScanningInstance1() {        
        // create the barcode scanner. 
        createScanner(_activeBarcodeScannerInstance.instance1).then(function (success) {
            if (success === true) {
                //Claim the scanner and enable
                claimAndEnableScanner(_activeBarcodeScannerInstance.instance1).then(function (bAsyncCallStatus) {
                    if (bAsyncCallStatus === true) {
                        resetUI();
                        document.getElementById("ScenarioStartScanningInstance1").disabled = true;
                        document.getElementById("ScenarioStartScanningInstance2").disabled = false;
                        document.getElementById("ScenarioEndScanningInstance1").disabled = false;
                        document.getElementById("ScenarioEndScanningInstance2").disabled = true;
                        _currentInstance = _activeBarcodeScannerInstance.instance1;
                    }
                });
            }
        });
    }


    //create scanner instance 2 and claim and enable it.
    function buttonStartScanningInstance2() {
        // create the barcode scanner. 
        createScanner(_activeBarcodeScannerInstance.instance2).then(function (success) {
            if (success === true) {
                //Claim the scanner and enable
                claimAndEnableScanner(_activeBarcodeScannerInstance.instance2).then(function (bAsyncCallStatus) {
                    if (bAsyncCallStatus === true) {
                        resetUI();
                        document.getElementById("ScenarioStartScanningInstance1").disabled = false;
                        document.getElementById("ScenarioStartScanningInstance2").disabled = true;
                        document.getElementById("ScenarioEndScanningInstance1").disabled = true;
                        document.getElementById("ScenarioEndScanningInstance2").disabled = false;
                        _currentInstance = _activeBarcodeScannerInstance.instance2;
                    }
                });
            }
        });
    }
    //Reset UI after ending instance 1
    function buttonEndScanningInstance1() {
        disableScannerAsync1().done(function (success) {

            if (success === true) {
                resetUI();
                document.getElementById("ScenarioStartScanningInstance2").disabled = false;
                document.getElementById("ScenarioStartScanningInstance1").disabled = false;
                document.getElementById("ScenarioEndScanningInstance1").disabled = true;
                document.getElementById("ScenarioEndScanningInstance2").disabled = true;
            }
        });
    }

    //Reset UI after ending instance 2
    function buttonEndScanningInstance2() {

        disableScannerAsync2().done(function (success) {

            if (success === true) {
                resetUI();
                document.getElementById("ScenarioStartScanningInstance2").disabled = false;
                document.getElementById("ScenarioStartScanningInstance1").disabled = false;
                document.getElementById("ScenarioEndScanningInstance1").disabled = true;
                document.getElementById("ScenarioEndScanningInstance2").disabled = true;
            }
        });
    }

    //Resets the textContent in the scanned data values.
    function resetUI() {

        switch (_currentInstance) {

            case _activeBarcodeScannerInstance.instance1:

                document.getElementById("ScanDataType1").textContent = "No Data";
                document.getElementById("ScanData1").textContent = "No Data";
                document.getElementById("DataLabel1").textContent = "No Data";
                break;

            case _activeBarcodeScannerInstance.instance2:

                document.getElementById("ScanDataType2").textContent = "No Data";
                document.getElementById("ScanData2").textContent = "No Data";
                document.getElementById("DataLabel2").textContent = "No Data";
                break;
        }
    }

    //This method is called upon when a claim request is made on instance 1. If a retain request was placed on the device it rejects the new claim.
    function onReleasedeviceRequested1(args) {
        WinJS.logAppend("ReleaseDeviceRequested ("+ _claimedScanner1.deviceId +")", "status");
        var retain = document.getElementById("Retain1");
        //check if the instance wants to retain the device
        if (retain.checked === true) {
            try {
                //Retain the device
                _claimedScanner1.retainDevice();
                WinJS.logAppend("(Scanner Retained)", "status");
            }
            catch (error) {
                WinJS.logAppend("(retain failed) (" + error.message +")", "error");
            }
        }      
    }

    //This method is called upon when a claim request is made on instance 2. If a retain request was placed on the device it rejects the new claim.
    function onReleasedeviceRequested2(args) {
        WinJS.logAppend("ReleaseDeviceRequested (" + _claimedScanner2.deviceId + ")", "status");        
        var retain = document.getElementById("Retain2");
        //check if the instance wants to retain the device
        if (retain.checked === true) {
            try {
                //Retain the device
                _claimedScanner2.retainDevice();
                WinJS.logAppend("(Scanner Retained)", "status");
            }
            catch (error) {
                WinJS.logAppend("(retain failed) (" + error.message + ")", "error");
            }
        }
    }

    //Claims and enables the scanner for the given scanner instance and returns true if success.
    function claimAndEnableScanner(instance) {
        return new WinJS.Promise(function (complete) {
            switch (instance) {
                case _activeBarcodeScannerInstance.instance1:

                    if (_scanner1 !== null) {
                        //Claim the scanner
                        _scanner1.claimScannerAsync().done(function (claimedScanner) {
                            if (claimedScanner !== null) {
                                _claimedScanner1 = claimedScanner;
                                claimedScanner.isDecodeDataEnabled = true;
                                WinJS.logAppend("Instance1 Claim Barcode Scanner succeeded.", "status");
                                //add the event handlers
                                claimedScanner.addEventListener("datareceived", onDataReceived);
                                claimedScanner.addEventListener("releasedevicerequested", onReleasedeviceRequested1);
                                //Enable the Scanner
                                claimedScanner.enableAsync().done(function () {
                                    WinJS.logAppend("Instance1 Enable Barcode Scanner succeeded.", "status");
                                    WinJS.logAppend("Ready to Scan.", "status");
                                    complete(true);
                                }, function error(e) {
                                    WinJS.logAppend("Instance1 Claim Barcode Scanner failed." + e.message, "error");
                                    complete(false);
                                });
                            } else {
                                WinJS.logAppend("Instance1 Claim Barcode Scanner failed.", "error");
                                complete(false);
                            }
                        }, function error(e) {
                            WinJS.logAppend("Instance1 Claim Barcode Scanner failed." + e.message, "error");
                            complete(false);
                        });
                    }
                    break;
                case _activeBarcodeScannerInstance.instance2:
                    if (_scanner2 !== null) {
                        //Claim the scanner
                        _scanner2.claimScannerAsync().done(function (claimedScanner) {
                            if (claimedScanner !== null) {
                                _claimedScanner2 = claimedScanner;
                                claimedScanner.isDecodeDataEnabled = true;
                                WinJS.logAppend("Instance2 Claim Barcode Scanner succeeded.", "status");
                                //add the event handlers
                                claimedScanner.addEventListener("datareceived", onDataReceived);
                                claimedScanner.addEventListener("releasedevicerequested", onReleasedeviceRequested2);
                                //Enable the Scanner
                                claimedScanner.enableAsync().done(function () {
                                    WinJS.logAppend("Instance2 Enable Barcode Scanner succeeded.", "status");
                                    WinJS.logAppend("Ready to Scan.", "status");
                                    complete(true);
                                }, function error(e) {
                                    WinJS.logAppend("Instance2 Claim Barcode Scanner failed." + e.message, "error");
                                    complete(false);
                                });
                            } else {
                                WinJS.logAppend("Instance2 Claim Barcode Scanner failed.", "error");
                                complete(false);
                            }
                        }, function error(e) {
                            WinJS.logAppend("Instance2 Claim Barcode Scanner failed." + e.message, "error");
                            complete(false);
                        });
                    }
                    break;
            }
        });
    }

    function createScanner(instance) {

        return new WinJS.Promise(function (complete) {
            //Get the handle to the default scanner 
            Windows.Devices.PointOfService.BarcodeScanner.getDefaultAsync().done(function (scanner) {
                if (scanner !== null) {

                    switch (instance) {
                        case _activeBarcodeScannerInstance.instance1:
                            _scanner1 = scanner;
                            WinJS.logAppend("Instance1 Default Barcode Scanner created..", "status");
                            break;
                        case _activeBarcodeScannerInstance.instance2:
                            _scanner2 = scanner;
                            WinJS.logAppend("Instance2 Default Barcode Scanner created..", "status");
                            break;
                    }
                    complete(true);

                } else {
                    WinJS.logAppend("Scanner not found. Please connect a Barcode Scanner.", "error");
                    complete(false);
                }

            }, function error(e) {
                WinJS.logAppend("Scanner GetDefault Async Unsuccessful" + e.message, "error");
                complete(false);
            });

        });
    }

  

    // Disables the scanner Instance1.
    function disableScannerAsync1() {
        return new WinJS.Promise(function (complete) {
            if (_claimedScanner1 !== null) {
                _claimedScanner1.disableAsync().then(function (success) {
                    //remove event listeners and close.
                    _claimedScanner1.removeEventListener("datareceived", onDataReceived);
                    _claimedScanner1.removeEventListener("releasedevicerequested", onReleasedeviceRequested1);
                    _claimedScanner1.close();
                    _claimedScanner1 = null;
                    _scanner1 = null;
                    resetUI();
                    WinJS.logAppend("Scanner Instance 1 Destroyed", "status");
                    complete(true);
                });
            }
        });
    }

    // Disables the scanner Instance2.
    function disableScannerAsync2() {
        return new WinJS.Promise(function (complete) {
            if (_claimedScanner2 !== null) {
                _claimedScanner2.disableAsync().then(function (success) {
                    //remove event listeners and close handle
                    _claimedScanner2.removeEventListener("datareceived", onDataReceived);
                    _claimedScanner2.removeEventListener("releasedevicerequested", onReleasedeviceRequested2);
                    _claimedScanner2.close();
                    _claimedScanner2 = null;
                    _scanner2 = null;
                    resetUI();
                    WinJS.logAppend("Scanner Instance 2 Destroyed", "status");
                    complete(true);
                });
            }
        });
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

            for (var byteIndex = 0; byteIndex < bytesToPrint; ++byteIndex) {
                result += reader.readByte().toString(16) + " ";
            }

            if (bytesToPrint < data.length) {
                result += "...";
            }
        }

        return result;
    }

    function getDataLabelString(data, scanDataType) {
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

    // This is an event handler for the claimed scanner Instance when it scans and recieves data
    function onDataReceived(args) {
        var tempScanLabel = "";
        var tempScanData = "";

        if (args.report.scanData !== null) {
            tempScanData = getDataString(args.report.scanData);
        }
        if (args.report.scanDataLabel !== null) {
            tempScanLabel = getDataLabelString(args.report.scanDataLabel, args.report.scanDataType);
        }

        // Now populate the UI
        switch (_currentInstance) {
            case _activeBarcodeScannerInstance.instance1:
                if (args.report.scanDataType === 501) {
                    document.getElementById("ScanDataType1").textContent = "OEM - " + Windows.Devices.PointOfService.BarcodeSymbologies.getName(args.report.scanDataType);
                } else {
                    document.getElementById("ScanDataType1").textContent = Windows.Devices.PointOfService.BarcodeSymbologies.getName(args.report.scanDataType);
                }
                // DataLabel
                document.getElementById("DataLabel1").textContent = tempScanLabel;
                // Data
                document.getElementById("ScanData1").textContent = tempScanData;

                break;

            case _activeBarcodeScannerInstance.instance2:
                // BarcodeSymbologies.ExtendedBase
                if (args.report.scanDataType === 501) {
                    document.getElementById("ScanDataType2").textContent = "OEM - " + Windows.Devices.PointOfService.BarcodeSymbologies.getName(args.report.scanDataType);
                } else {
                    document.getElementById("ScanDataType2").textContent = Windows.Devices.PointOfService.BarcodeSymbologies.getName(args.report.scanDataType);
                }
                // DataLabel
                document.getElementById("DataLabel2").textContent = tempScanLabel;
                // Data
                document.getElementById("ScanData2").textContent = tempScanData;
                break;
        }
    }

 
})();
