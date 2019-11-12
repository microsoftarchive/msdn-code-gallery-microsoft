//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

// Array passed as the message data in the ListView
var myMessageData = new WinJS.Binding.List([
        { title: "New Flavors out this week!", text: "Adam Barr", picture: "images/60Mail01.png" },
        { title: "Check out this Topping!", text: "David Alexander", picture: "images/60Mail01.png" },
        { title: "Ice Cream Party!!!", text: "Josh Bailey", picture: "images/60Mail01.png" },
        { title: "I Love Ice Cream", text: "Chris Berry", picture: "images/60Mail01.png" },
        { title: "What's Your Favorite?", text: "Sean Bentley", picture: "images/60Mail01.png" },
        { title: "Where is the Coupon?", text: "Adrian Lannin", picture: "images/60Mail01.png" },
        { title: "Your Invited!", text: "Gary Schare", picture: "images/60Mail01.png" },
        { title: "Make a Custom Carton!", text: "Garth Fort", picture: "images/60Mail01.png" },
        { title: "Check this out", text: "Raymond Fong", picture: "images/60Mail01.png" },
        { title: "When Are You Available", text: "Derek Brown", picture: "images/60Mail01.png" },
        { title: "Peanut Butter!", text: "Maria Cameron", picture: "images/60Mail01.png" },
        { title: "Caramel Topping Coming", text: "Judy Lew", picture: "images/60Mail01.png" },
        { title: "Candy Cane Flavor?", text: "Chris Mayo", picture: "images/60Mail01.png" },
        { title: "Spinkles Galor!", text: "Randy", picture: "images/60Mail01.png" },
        { title: "Tell Me More", text: "Mike Nash", picture: "images/60Mail01.png" }
    ]);

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario2.html", {
        ready: function (element, options) {
            var messageTitle = element.querySelector("#inboxMessage_Title");
            var messageText = element.querySelector("#inboxMessage_Text");
            var messageBody = element.querySelector("#inboxMessage_Body");
            var listView = element.querySelector("#listView2").winControl;
            listView.forceLayout();

            function selectionChangedHandler() {

                // Check for selection
                var selectionCount = listView.selection.count();
                if (selectionCount === 1) {

                    // Only one item is selected, show the message
                    // information for the item
                    listView.selection.getItems().done(function (items) {

                        // Print item data to the relevant message pane locations
                        inboxMessage_Title.innerText = "From: " + items[0].data.text;

                        inboxMessage_Text.innerText = items[0].data.title;

                        inboxMessage_Body.style.color = "black";
                        inboxMessage_Body.innerText = "\n\nMessage Body";
                    });

                } else {

                    // If none or multiple items are selected, clear the view
                    inboxMessage_Title.innerText = "";
                    inboxMessage_Text.innerText = "";
                    inboxMessage_Body.style.color = "green";

                    if (selectionCount > 1) {

                        //multiple messages are selected
                        inboxMessage_Body.innerText = "Multiple messages are selected!";

                    } else {

                        //no items are selected
                        inboxMessage_Body.innerText = "No message selected!";
                    }
                }
            }

            // Register the selection changed event
            listView.addEventListener("selectionchanged", selectionChangedHandler, false);

            listView.itemDataSource.getCount().done(function (itemCount) {

                // Set the initial selection in the ListView
                var initialSelection = (itemCount > 0) ? 0 : [];
                listView.selection.set(initialSelection);
            });
        }
    });
})();
