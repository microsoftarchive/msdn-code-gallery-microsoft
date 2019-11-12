//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved



(function () {
    platform.HotspotAuthenticationSamplePlatformSpecific = {

        //
        // Determine if task with given name requires background access.
        //
        taskRequiresBackgroundAccess: function () {
            return false;
        },

        isNativeWISPrSample: function() {
            return true;
        }
    };
})();
