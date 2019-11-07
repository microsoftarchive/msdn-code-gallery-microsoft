//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    WinJS.Namespace.define("Microsoft.Sample.WorkspaceBrokerApi", {
        
        WorkspaceActiveX: {

            createInstance: function () {
                var wkspObj = document.createElement("object");
                var wkspRegion = document.getElementById("wkspAxControlRegion");                

                // Register for standard ActiveX events
                wkspObj.addEventListener("readystatechange", function (e) {
                    if (wkspObj.readyState !== 4) {
                        WinJS.log && WinJS.log("Error: the ActiveX control readyStateChange fired, but readyState != 4. ReadyState: " + wkspObj.readyState, "sample", "error");
                    }
                }, false);
                wkspObj.addEventListener("error", function (e) {
                    WinJS.log && WinJS.log("Error loading the ActiveX control", "sample", "error");
                }, false);

                // Hook up the ActiveX control
                wkspObj.id = document.uniqueID;
                wkspObj.className = "ActiveX";
                wkspObj.classid = "CLSID:CD70A734-B6DB-4588-9813-FF2E37A4661F";
                wkspRegion.appendChild(wkspObj);

                return wkspObj;
            },

        }
   
    });
})();
