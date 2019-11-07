//// Copyright (c) Microsoft Corporation. All rights reserved


(function () {
    var NameListEntry = WinJS.Class.define(
       function (fileName) {
           this.fileName = fileName;
       },
       {
           name: {
               get: function () {
                   return this.fileName;
               }
           },
       }
   );

    //
    // Create a public namespace to for utility fucntions.
    //
    WinJS.Namespace.define("Utils", {
        id: function (elementId) {
            return document.getElementById(elementId);
        },
        /// <summary>
        /// Display the image of the first scanned file and output the corresponding message
        /// </summary>
        /// <param name="fileStorageList">List of storage files create by the Scan Runtime API for all the scanned images given by the scanner</param>
        /// <param name="image">image for which the source needs to be set to first of the scanned storage files</param>
        displayImageAndScanCompleteMessage: function (fileStorageList, image) {
            var file = fileStorageList.getAt(0);
            
            image.src = window.URL.createObjectURL(file, { oneTimeOnly: true });
            image.alt = file.name;
            if (fileStorageList.length > 1) {
                WinJS.log && WinJS.log("Scanning is complete. Below is the first of the scanned images. \n" +
                        "All the scanned files have been saved to local My Pictures folder.", "sample", "status");
            } else {
                WinJS.log && WinJS.log("Scanning is complete. Below is the scanned image.\n" +
                        "Scanned file has been saved to local My Pictures folder with file name: " + file.name, "sample", "status");
            }
        },
        /// <summary>
        /// Add the file names of the scanned files to the list of file names that will be displayed to the user
        /// </summary>
        /// <param name="fileStorageList">List of storage files create by the Scan Runtime API for all the scanned images given by the scanner</param>
        /// <param name="fileNameList">List contianing all the file names that are scanned</param>
        updateFileListData: function(fileStorageList, fileNameList){
            // populate list with the names of the files that are scanned			
            for (var i = 0; i < fileStorageList.length; i++) {
                var file = fileStorageList.getAt(i);
                var entry = new NameListEntry(file.name);
                fileNameList.push(entry);
            }
        },
        /// <summary>
        /// Displays an error message when an exception is thrown during the scenario
        /// </summary>
        displayErrorMessage: function(error){
            WinJS.log && WinJS.log("An error occurred while executing the scenario. error number: 0x" + (error.number + 0xFFFFFFFF + 1).toString(16), "sample", "status");
        },
        /// <summary>
        /// Displays cancellation message when user cancels scanning
        /// </summary>
        displayScanCancelationMessage: function(){
            WinJS.log && WinJS.log("Scanning has been cancelled. Files that have been scanned so far would be saved to the local My Pictures folder.", "sample", "status");
        },
        /// <summary>
        /// Updates start and cancel scenario 
        /// </summary>
        updateStartAndEndScenarioButtons: function (startButton,
                                            cancelButton,
                                            currentScannerId,
                                            scenarioRunning) {
            if (!currentScannerId) {
                startButton.disabled = true;
                cancelButton.disabled = true;
            } else {
                if (scenarioRunning) {
                    startButton.disabled = true;
                    cancelButton.disabled = false;
                } else {
                    startButton.disabled = false;
                    cancelButton.disabled = true;
                }
            }
        }
    });
})();