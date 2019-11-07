//// Copyright (c) Microsoft Corporation. All rights reserved


(function () {

    var data = [];
    for (var i = 0; i < 100; i++) {
        data.push({ author: " Owen Lindstrom", titleColor: "Highlight", title: "Missed conversation", previewText: "1:53 PM: Thanks for taking the time...", time: "1:55p" });
        data.push({ author: "Luis Wallen", titleColor: "Highlight", title: "Need help later", previewText: "I was hoping you could help me with...", time: "1:50p" });
        data.push({ author: "Vicente Rhoden", titleColor: "Highlight", title: "Lunch with you", previewText: "Any chance you want to do lunch on...", time: "1:20p" });
        data.push({ author: "Kris Eck", titleColor: "rgb(153,153,153)", title: "QuickStart's and How To's", previewText: "More information on how to use WinJS controls", time: "1:17p" });
        data.push({ author: "Moises Phillip", titleColor: "Highlight", title: "Going out saturday", previewText: "Devon and I are leaving...", time: "12:28p" });
        data.push({ author: "Emery Jarman", titleColor: "rgb(153,153,153)", title: "Happened every time", previewText: "I always end up going to the ...", time: "7:12a" });
    }
    window.All = { dataSource: new WinJS.Binding.List(data).dataSource };


    data = [];
    data.push({ author: "Devon Loftus", titleColor: "rgb(153,153,153)", title: "QuickStart's and How To's", previewText: "More information on how to use WinJS controls", time: "1:17p" });
    data.push({ author: "Emery Jarman", titleColor: "Highlight", title: "Going out saturday", previewText: "Jean and I are leaving...", time: "12:28p" });
    data.push({ author: "Kris Eck", titleColor: "rgb(153,153,153)", title: "Happend every time", previewText: "I always end up going to the ...", time: "7:12a" });
    data.push({ author: "Stan Mallard", titleColor: "rgb(153,153,153)", title: "QuickStart's and How To's", previewText: "More information on how to use...", time: "1:17p" });
    window.Unread = { dataSource: new WinJS.Binding.List(data).dataSource };

    data = [];
    data.push({ author: "Alphonso Bettencourt", titleColor: "rgb(153,153,153)", title: "QuickStart's and How To's", previewText: "More information on how to use...", time: "1:17p" });
    data.push({ author: "Kris Eck", titleColor: "rgb(153,153,153)", title: "AppBar and ListView integration", previewText: "Contextual commands in the AppBar", time: "7:12a" });
    window.Flagged = { dataSource: new WinJS.Binding.List(data).dataSource };

    data = [];
    data.push({ author: "Stan Mallard", titleColor: "rgb(153,153,153)", title: "QuickStart's and How To's", previewText: "More information on how to use WinJS controls", time: "1:17p" });
    data.push({ author: "Moises Phillip", titleColor: "Highlight", title: "Going out saturday", previewText: "Emery and I are leaving...", time: "12:28p" });
    data.push({ author: "Luis Wallen", titleColor: "rgb(153,153,153)", title: "Happend every time", previewText: "I always end up going to the ...", time: "7:12a" });
    window.Urgent = { dataSource: new WinJS.Binding.List(data).dataSource };

})();

