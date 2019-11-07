//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    // This is an array that will be used to drive the FlipView in several
    // scenarios. The array contains objects with the following attributes:
    //
    //      type - There are two types that are used:
    //
    //              item -
    //                     The type for simple items.  It informs the custom
    //                     renderer that their is a title and picture that needs
    //                     to be rendered.
    //
    //              contentsArray -
    //                     This is used for creating a table of contents.  It
    //                     informs the renderer that an array of data is present
    //                     for use in constructing the Table of Contents.
    //
    //      title - The title of a photo to be displayed.
    //
    //      picture - The location of the photo to be displayed.
    var array = [
        { type: "item", title: "Cliff", picture: "images/Cliff.jpg" },
        { type: "item", title: "Grapes", picture: "images/Grapes.jpg" },
        { type: "item", title: "Rainier", picture: "images/Rainier.jpg" },
        { type: "item", title: "Sunset", picture: "images/Sunset.jpg" },
        { type: "item", title: "Valley", picture: "images/Valley.jpg" }
    ];
    var bindingList = new WinJS.Binding.List(array);

    WinJS.Namespace.define("DefaultData", {
        bindingList: bindingList,
        array: array
    });

    var e = DefaultData.bindingList.dataSource;
})();
