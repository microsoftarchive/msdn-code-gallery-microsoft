//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {

    function getTimeStamp() {
        var d = new Date();
        var ts = String("0" + d.getYear()).slice(-2) +
                 String("0" + d.getMonth()).slice(-2) +
                 String("0" + d.getDay()).slice(-2) +
                 "-" +
                 String("0" + d.getHours()).slice(-2) +
                 String("0" + d.getMinutes()).slice(-2) +
                 String("0" + d.getSeconds()).slice(-2) +
                 String("00" + d.getMilliseconds()).slice(-3);
        return ts;
    }

    var LoggingScenario = WinJS.Class.define(
        function () {
            this._channel = new Windows.Foundation.Diagnostics.LoggingChannel("AppChannel1");
            this._channel.addEventListener("loggingenabled", this._onLoggingEnabled);

            Windows.UI.WebUI.WebUIApplication.addEventListener("suspending", this._onAppSuspension);
            Windows.UI.WebUI.WebUIApplication.addEventListener("resuming", this._onAppResume);
        },
        {
            doScenarioAsync: function () {
                this._setBusyStatus(true);
                try {

                    var data = { loggingScenario: this, messageIndex: 0, startFileCount: this._logFileGeneratedCount, countDownToError: 5000 };

                    function logMessagesAndPauseAsync(data) {

                        var totalFilesCreatedSoFar = data.loggingScenario._logFileGeneratedCount - data.startFileCount;
                        if (totalFilesCreatedSoFar >= 3) {
                            // At least 3 files created, stop.
                            return new WinJS.Promise(function (completionFunction, errorFunction, progressFunction) {
                                data.loggingScenario._setBusyStatus(false);
                                completionFunction();
                            });
                        } else {

                            var channel = data.loggingScenario._channel;

                            for (var index = 0; index < 50; index++) {

                                try {
                                    // Since the channel is added to the session at level Warning,
                                    // the following is logged because it is logged at level LoggingLevel.Critical.
                                    messageToLog =
                                        "Message=" +
                                        (++data.messageIndex) +
                                        ": Lorem ipsum dolor sit amet, consectetur adipiscing elit.In ligula nisi, vehicula nec eleifend vel, rutrum non dolor.Vestibulum ante ipsum " +
                                        "primis in faucibus orci luctus et ultrices posuere cubilia Curae; Curabitur elementum scelerisque accumsan. In hac habitasse platea dictumst.";
                                    channel.logMessage(messageToLog, Windows.Foundation.Diagnostics.LoggingLevel.critical);

                                    // Since the channel is added to the session at level Warning,
                                    // the following is *not* logged because it is logged at LoggingLevel.Information.
                                    messageToLog =
                                        "Message=" +
                                        (++data.messageIndex) +
                                        ": Lorem ipsum dolor sit amet, consectetur adipiscing elit.In ligula nisi, vehicula nec eleifend vel, rutrum non dolor.Vestibulum ante ipsum " +
                                        "primis in faucibus orci luctus et ultrices posuere cubilia Curae; Curabitur elementum scelerisque accumsan. In hac habitasse platea dictumst.";
                                    channel.logMessage(messageToLog, Windows.Foundation.Diagnostics.LoggingLevel.information);

                                    var value = 1000000; // one million, 7 digits, 4-bytes as an int, 14 bytes as a wide character string.
                                    channel.logMessage("Value #" + (++data.messageIndex).toString() + "  " + value, Windows.Foundation.Diagnostics.LoggingLevel.critical); // value is logged as 14 byte wide character string.
                                    channel.logValuePair("Value #" + (++data.messageIndex).toString(), value, Windows.Foundation.Diagnostics.LoggingLevel.critical); // value is logged as a 4-byte integer.

                                    //
                                    // Every once in a while, simulate an application error
                                    // which causes the app to save the current snapshot
                                    // of logging events in memory to a disk ETL file.
                                    //

                                    data.countDownToError--;
                                    if (data.countDownToError <= 0) {

                                        data.countDownToError = 5000;

                                        // Before simulating an application error, demonstrate LoggingActivity.
                                        // A LoggingActivity outputs a pair of begin and end messages to the channel.
                                        var loggingActivity = new Windows.Foundation.Diagnostics.LoggingActivity("Add two numbers.", channel, Windows.Foundation.Diagnostics.LoggingLevel.critical);
                                        try {
                                            var oneNumber = 100;
                                            var anotherNumber = 200;
                                            var total = oneNumber + anotherNumber;
                                            channel.logMessage("Message=" + (++data.messageIndex) + ": The result of adding two numbers: " + total, Windows.Foundation.Diagnostics.LoggingLevel.critical);
                                        }
                                        finally {
                                            loggingActivity.close(); // The LoggingActivity instance will log the second message when close() is called. 
                                        }
                           
                                        // Simulate an application error.
                                        throw "Some bad app error occurred.";
                                    }
                                } catch (e) {

                                    // Log the exception string.
                                    channel.logMessage("Exception occurrred: " + e.toString(), Windows.Foundation.Diagnostics.LoggingLevel.error);
                                    return data.loggingScenario._saveLogInMemoryToFileAsync(data.loggingScenario._session).then(function (logFile) {
                                        // The log containinng the error has been saved.
                                        // Update the UI of the sample.
                                        data.loggingScenario._logFileGeneratedCount++;
                                        data.loggingScenario._dispatchStatusChanged({ type: "LogFileGenerated", logFilePath: logFile.path });
                                        // Continue with the scenario. 
                                        return logMessagesAndPauseAsync(data);
                                    });
                                }
                            }

                            // Pause every once in a while to simulate application 
                            // activity outside of logging.
                            return WinJS.Promise.timeout(25).then(function () {

                                return logMessagesAndPauseAsync(data);
                            });
                        }
                    }

                    // Log messages until at least 3 log files are created. 
                    return logMessagesAndPauseAsync(data);

                } catch (e) {
                    this._setBusyStatus(false);
                    throw e;
                }
            },
            toggleLoggingEnabledDisabled: function () {
                this._setBusyStatus(true);
                try {
                    var loggingScenario = this;
                    if (this._session !== null) {
                        this._session.close();
                        this._session = null;
                        Windows.Storage.ApplicationData.current.localSettings.values["LoggingEnabled"] = false;
                        loggingScenario._dispatchStatusChanged({ type: "LoggingEnabledDisabled", enabled: false });
                    } else {
                        this._startLogging();
                        Windows.Storage.ApplicationData.current.localSettings.values["LoggingEnabled"] = true;
                        loggingScenario._dispatchStatusChanged({ type: "LoggingEnabledDisabled", enabled: true });
                    }
                    loggingScenario._setBusyStatus(false);
                } catch (e) {
                    this._setBusyStatus(false);
                    throw e;
                }
            },
            isLoggingEnabled: {
                get: function () {
                    return this._session !== null;
                }
            },
            _session: null,
            _channel: null,
            _isChannelEnabled: false,
            _channelLoggingLevel: Windows.Foundation.Diagnostics.LoggingLevel.verbose,
            _logFileGeneratedCount: 0,
            _isBusy: false,
            _setBusyStatus: function (busy) {
                if (this._isBusy === busy) {
                    return;
                }
                this._isBusy = busy;
                if (busy) {
                    this._dispatchStatusChanged({ type: "BusyStatusChanged" });
                }
            },
            getBusyStatus: function () {
                return this._isBusy;
            },            
            _onAppSuspension: function (suspendingEventArgs) {
                var loggingScenario = LoggingScenario.loggingScenarioSingleton;

                // Get a deferral before performing any async operations
                // to avoid suspension prior to LoggingScenario completing 
                // PrepareToSuspendAsync().
                var deferral = suspendingEventArgs.suspendingOperation.getDeferral();
                loggingScenario._prepareToSuspend();
                // From LoggingScenario's perspective, it's now okay to 
                // suspend, so release the deferral. 
                deferral.complete();
            },
            _onAppResume: function (resmingEventArgs) {
                var loggingScenario = LoggingScenario.loggingScenarioSingleton;
                loggingScenario.resumeLoggingIfApplicable();
            },
            _startLogging: function () {

                if (this._session === null)
                {
                    this._session = new Windows.Foundation.Diagnostics.LoggingSession("AppSession1");
                }

                // This sample adds the channel at level "warning" to 
                // demonstrated how messages logged at more verbose levels
                // are ignored by the session. 
                this._session.addLoggingChannel(this._channel, Windows.Foundation.Diagnostics.LoggingLevel.warning);
            },
            _onLoggingEnabled: function (args) {

                var loggingScenario = LoggingScenario.loggingScenarioSingleton;
                loggingScenario._isChannelEnabled = args.target.enabled;
                loggingScenario._channelLoggingLevel = args.target.level;
            },
            resumeLoggingIfApplicable: function () {

                // If logging is already enabled, nothing to do.
                if (this.isLoggingEnabled) {
                    return true;
                }

                var loggingEnabled;
                if (Windows.Storage.ApplicationData.current.localSettings.values.hasKey("LoggingEnabled")) {
                    loggingEnabled = Windows.Storage.ApplicationData.current.localSettings.values.hasKey("LoggingEnabled");
                } else {
                    Windows.Storage.ApplicationData.current.localSettings.values["LoggingEnabled"] = true;
                    loggingEnabled = true;
                }

                if (loggingEnabled) {
                    this._startLogging();
                }
            },
            _saveLogInMemoryToFileAsync: function () {

                // The loggingScenario it shared across async continuations.
                var loggingScenario = this;

                // Create the app-defined folder under the app's local folder.
                // An app defines where/how it wants to store log files. 
                // This sample uses OUR_SAMPLE_APP_LOG_FILE_FOLDER_NAME under
                // the app's local folder. 
                return Windows.Storage.ApplicationData.current.localFolder.createFolderAsync(
                           "OurAppLogFiles",
                           Windows.Storage.CreationCollisionOption.openIfExists)
                .then(function (ourAppLogFolder) {
                    // Create a new log file name.
                    var newLogFileName = "OurAppLog-" + getTimeStamp() + ".etl";
                    // Asynchronously save the in-memory log file to a log file in our app's log folder.
                    return loggingScenario._session.saveToFileAsync(ourAppLogFolder, newLogFileName);
                });
            },
            _prepareToSuspend: function () {
                if (this._session !== null) {
                    Windows.Storage.ApplicationData.current.localSettings.values["LoggingEnabled"] = true;
                } else {
                    Windows.Storage.ApplicationData.current.localSettings.values["LoggingEnabled"] = false;
                }
            },
            _dispatchStatusChanged: function (statusArgs) {
                this.dispatchEvent("statusChanged", statusArgs);
            }
        },
        {
            // LoggingScenario static members:

            _loggingScenarioSingleton: null,
            loggingScenarioSingleton: {
                get: function () {
                    if (!this._loggingScenarioSingleton) {
                        this._loggingScenarioSingleton = new LoggingScenario()
                    }
                    return this._loggingScenarioSingleton;
                }
            }
        });
    
    WinJS.Class.mix(LoggingScenario, WinJS.Utilities.eventMixin);

    // Export public methods & controls
    WinJS.Namespace.define("LoggingScenario", {
        LoggingScenario: LoggingScenario
    });

})();