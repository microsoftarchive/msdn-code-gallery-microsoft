//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

/// <reference path="//Microsoft.WinJS.2.0/js/base.js" />

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/1_ApplicationModel_IO.html", {
        ready: function (element, options) {
            var that = this;

            this.textToWrite = element.querySelector("#textToWrite");
            this.textToRead = element.querySelector("#textToRead");
            this.filename = element.querySelector("#fileName");
            this.folderType = element.querySelector("#folderType");

            element.querySelector("#writeButton").addEventListener("click",
                this.writeContent.bind(this), false);
            element.querySelector("#readButton").addEventListener("click",
                this.readContent.bind(this), false);
            element.querySelector("#textToWrite").addEventListener("focus",
                function () { that.textToWrite.value = ""; }, false);
            element.querySelector("#folderType").addEventListener("focus",
                this.selectionChanged.bind(this), false);
        },

        // Click handler for the "Write Content" button. Write out the
        // contents to the requested file.

        writeContent: function () {
            var that = this;

            if (this.inputsAreValid()) {
                if (this.textToWrite.value !== "") {
                    var folderClass = this.getSelectedStorageArea();

                    // Write the file. This is async, promise completes when the write is done.
                    folderClass.writeText(this.filename.value, this.textToWrite.value)
                        .done(function () {
                            that.textToRead.innerText = "Data is written to the file";
                        });
                } else {
                    that.textToWrite.value = "Please enter some text to be written to the file";
                }
            }
        },

        // Event handler for the "Read Content" button.
        // Based on the inputs from the user, load the
        // text from the given file and display it in the
        // UI.
        readContent: function () {
            var that = this;
            if (this.inputsAreValid()) {
                var folderClass = this.getSelectedStorageArea();

                // Check for existence of the file...
                folderClass.exists(this.filename.value)
                    .then(function (exist) {
                        if (!exist) {
                            return "File does not exist";
                        } else {
                            // Go out to read the text. This is async...
                            return folderClass.readText(that.filename.value);
                        }
                    }).done(function (str) {
                        if (str) {
                            that.textToRead.innerText = str;
                        }
                    });
            }
        },

        getSelectedStorageArea: function () {
            var selectedStorageArea = this.folderType.options[this.folderType.selectedIndex].value;

            // The application object has three properties that represent the
            // three different storage areas: local, roaming, and temp. These
            // strings are stored as the value property on the select options
            // (see the HTML) so we can just use this to look up the property
            // on the application object.
            return WinJS.Application[selectedStorageArea];
        },

        // Event handler for when the storage area select changes.
        // Clear out the various fields in the UI.
        selectionChanged: function () {
            this.textToRead.innerText = "";
            this.filename.value = "";
            this.textToWrite.value = "Write your text here";
        },

        // Input validation helpers
        inputsAreValid: function () {
            return this.fileNameIsValid() && this.folderSelectionIsValid();
        },

        fileNameIsValid: function () {
            if (this.filename.value === "") {
                this.markError("#errorSpan");
                return false;
            }
            this.clearError("#errorSpan");
            return true;
        },

        folderSelectionIsValid: function () {
            if (this.folderType.selectedIndex === 0) {
                this.markError("#errorSpan2");
                return false;
            }
            this.clearError("#errorSpan2");
            return true;
        },

        markError: function (errorSpanSelector) {
            this.element.querySelector(errorSpanSelector).style.display = "inline";
        },

        clearError: function (errorSpanSelector) {
            this.element.querySelector(errorSpanSelector).style.display = "none";
        }
    });
})();
