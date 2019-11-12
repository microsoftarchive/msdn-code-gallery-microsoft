//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var dataArray = getSystemFonts();

    var dataList = new WinJS.Binding.List(dataArray);

    dataList.sort();

    // Create a namespace to make the data publically accessible.
    var publicMembers =
        {
            itemList: dataList
        };
    WinJS.Namespace.define("FontList", publicMembers);
})();
