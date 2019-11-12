//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

var myData;

function resetMyData() {
    var myArray = [];
    for (var i = 0; i < 30; i++) {
        myArray.push({ title: "Item title " + (i + 1), text: "Item text " + (i + 1) });
    }
    myData = new WinJS.Binding.List(myArray);
}
resetMyData();

function rand(min, max) {
    var n = Math.random();
    return Math.floor((n * (max - min)) + min);
}

function getNItemsAsync(N) {
    // Simulate a slow data source where we have to wait for a while to get  more data
    var wait = rand(500, 3000);
    return WinJS.Promise.timeout(wait).then(getNItems.bind(null, N));
}

function getNItems(N) {
    // Simulate a limit on the data in order to test the end state.
    if (myData.length > 99) {
        return;
    }
    for (var i = 0; i < N; i++) {
        var number = myData.length + 1;
        myData.push({
            title: "New item title: " + number,
            text: "New item text: " + number
        });
    }
    WinJS.log && WinJS.log("Loaded " + N + " more items, new data length = " + myData.length, "sample", "status");
}
