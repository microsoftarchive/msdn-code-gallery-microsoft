//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    var data = [];
    for (var i = 0; i < 100; i++) {
        data.push({ author: "Alonzo Rhoden", titleColor: "Highlight", title: "Missed conversation with Michael Brown", previewText: "Michael Brown [1:53 PM]: WinJS AppBar Sample", time: "1:55p" });
        data.push({ author: "Donnell Bowler", titleColor: "Highlight", title: "WinJS AppBar control sample", previewText: "AppBar ListView integration. From: Thomas Lee", time: "1:50p" });
        data.push({ author: "Adam Cowles", titleColor: "Highlight", title: "MSDN SDK samples", previewText: "Check out other MSDN SDK samples", time: "1:20p" });
        data.push({ author: "Lester Carrico", titleColor: "rgb(153,153,153)", title: "QuickStart's and How To's", previewText: "More information on how to use WinJS controls", time: "1:17p" });
        data.push({ author: "Donald Nall", titleColor: "Highlight", title: "WinJS ListView control sample", previewText: "Learn how to use ListView", time: "12:28p" });
        data.push({ author: "Clifford Coppola", titleColor: "rgb(153,153,153)", title: "AppBar and ListView integration", previewText: "Contextual commands in the AppBar", time: "7:12a" });
    }
    window.All = { dataSource: new WinJS.Binding.List(data).dataSource };
})();