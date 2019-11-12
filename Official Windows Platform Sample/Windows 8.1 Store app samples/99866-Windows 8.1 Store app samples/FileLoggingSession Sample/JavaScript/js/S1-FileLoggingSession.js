//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/S1-FileLoggingSession.html", {
        ready: function (element, options) {
            document.getElementById("doScenarioButton").addEventListener("click", doScenario, false);
            document.getElementById("enableDisableLoggingButton").addEventListener("click", enableDisableLogging, false);

            var loggingScenario = LoggingScenario.LoggingScenario.loggingScenarioSingleton;
            // This sample UI is interested in events from
            // the LoggingScenario class so the UI can be updated. 
            loggingScenario.addEventListener("statusChanged", onStatusChanged);
            loggingScenario.resumeLoggingIfApplicable();
            updateControls();
        }
    });

    function resizeTextAreaWidth(ta) {
        var lines = ta.value.split("\n");
        var widestLine = 0;
        for (var i = 0; i < lines.length; i++) {
            if (lines[i].length > widestLine) {
                widestLine = lines[i].length;
            }
        }
        ta.cols = widestLine;
    }

    function getJustFileName(path) {
        var extractJustFileNamePattern = new RegExp("[\\\\\\/]([^\\\\\\/]+)$", "i");
        var matches = extractJustFileNamePattern.exec(path);
        if (matches == null) {
            return "";
        }
        return matches[1];
    }

    function getJustDirectoryName(path) {
        var extractJustFileNamePattern = new RegExp("(.*)[\\\\\\/]([^\\\\\\/]+)$", "i");
        var matches = extractJustFileNamePattern.exec(path);
        if (matches == null) {
            return "";
        }
        return matches[1];
    }

    // Add a message to the UI control which displays status while the sample is running.
    function addMessage(message) {
        statusMessageList.value += message + "\n";
        statusMessageList.scrollTop = statusMessageList.scrollHeight;
    }

    // Updates the UI with status information when a new log file is created. 
    function addLogFileMessage(message, logFilePath) {
        var finalMessage;
        if (logFilePath !== null && logFilePath.length > 0) {

            finalMessage = message + ": " + getJustFileName(logFilePath);

            appLogFolder.hidden = false;
            appLogFolder.value = "App log folder:" + getJustDirectoryName(logFilePath);
            resizeTextAreaWidth(appLogFolder);

            viewLogInfo.hidden = false;
            viewLogInfo.value =
                "To view the contents of the ETL files:\r\n" +
                "Using tracerpt to create an XML file: tracerpt.exe \"" +
                logFilePath +
                "\" -of XML -o LogFile.xml\r\n" +
                "Using the Windows Performance Toolkit (WPT): wpa.exe \"" +
                logFilePath + "\"";
            resizeTextAreaWidth(viewLogInfo);
        } else {
            finalMessage = message + ": none, nothing logged since saving the last file.";
        }
        addMessage(finalMessage);
    }

    // For this sample, the logging sample code is in LoggingScenario.
    // The following method handles status events from LoggingScenario as 
    // it runs the scenario. 
    function onStatusChanged(args) {
        var eventData = args.detail;
        switch (eventData.type) {
            case "BusyStatusChanged":
                updateControls();
                break;
            case "LogFileGenerated":
                addLogFileMessage("LogFileGenerated", eventData.logFilePath);
                break;
            case "LogFileGeneratedAtSuspend":
                addLogFileMessage("LogFileGeneratedAtSuspend", eventData.logFilePath);
                break;
            case "LogFileGeneratedAtDisable":
                addLogFileMessage("LogFileGeneratedAtDisable", eventData.logFilePath);
                break;
            case "LoggingEnabledDisabled":
                addMessage("Logging has been " + (eventData.enabled ? "enabled" : "disabled") + ".");
                break;
        }
    }

    // Adjust UI controls based on what the sample is doing.
    function updateControls() {

        var loggingScenario = LoggingScenario.LoggingScenario.loggingScenarioSingleton;

        if (loggingScenario.isLoggingEnabled) {

            inputText.innerText = "Logging is enabled. Click 'Disable Logging' to disable logging. With logging enabled, you can click 'Log Messages' to use the logging API to generate log files.";
            enableDisableLoggingButton.innerText = "Disable Logging";
            if (loggingScenario.getBusyStatus()) {
                doScenarioButton.disabled = true;
                enableDisableLoggingButton.disabled = true;
            } else {
                doScenarioButton.disabled = false;
                enableDisableLoggingButton.disabled = false;
            }
        } else {

            inputText.innerText = "Logging is disabled. Click 'Enable Logging' to enable logging. After you enable logging you can click 'Log Messages' to use the logging API to generate log files.";
            enableDisableLoggingButton.innerText = "Enable Logging";
            doScenarioButton.disabled = true;
            if (loggingScenario.getBusyStatus()) {
                enableDisableLoggingButton.disabled = true;
            } else {
                enableDisableLoggingButton.disabled = false;
            }
        }
    }

    // Enabled/disabled logging.
    function enableDisableLogging() {

        var loggingScenario = LoggingScenario.LoggingScenario.loggingScenarioSingleton;

        if (loggingScenario.isLoggingEnabled) {
            WinJS.log && WinJS.log("Disabling logging...", "sample", "status");
        } else {
            WinJS.log && WinJS.log("Enabling logging...", "sample", "status");
        }

        LoggingScenario.LoggingScenario.loggingScenarioSingleton.toggleLoggingEnabledDisabledAsync().then(function () {

            if (loggingScenario.isLoggingEnabled) {
                WinJS.log && WinJS.log("Logging enabled.", "sample", "status");
            } else {
                WinJS.log && WinJS.log("Logging disabled.", "sample", "status");
            }

            updateControls();
        });
    }

    // Run a sample scenario which logs lots of messages to produce several log files. 
    function doScenario() {
        WinJS.log && WinJS.log("The scenario is running.", "sample", "status");
        LoggingScenario.LoggingScenario.loggingScenarioSingleton.doScenarioAsync().then(function () {
            WinJS.log && WinJS.log("The scenario is finished.", "sample", "status");
            updateControls();
        });
    }

})();
