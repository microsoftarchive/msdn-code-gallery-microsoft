//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************
(function () {
    "use strict";

    var dataRequest, jobOwnerToken;
    var scheduler = WinJS.Utilities.Scheduler;

    WinJS.UI.Pages.define("/pages/scheduler/scheduler.html", {
        ready: function (element, options) {
            performance.mark("navigated to scheduler");

            dataRequest = Data.featuredCollections.
                then(function (collections) {
                    performance.mark("got collection");

                    var hub = element.querySelector("#featuredHub");
                    if (!hub) { return; }

                    var hubSections = hub.winControl.items;
                    var hubSection;
                    var collection;
                    var priority;

                    jobOwnerToken = scheduler.createOwnerToken();

                    for (var i = 0; i < hubSections.length; i++) {
                        hubSection = hubSections.getItem(i);
                        collection = collections.getItem(i);
 
                        priority == (i < 2) ? scheduler.Priority.normal : scheduler.Priority.idle;

                        scheduler.schedule(function () {
                            populateSection(this.section, this.collection)
                            },
                            priority,
                            { section: hubSection, collection: collection },
                            "adding hub section").
                        owner = jobOwnerToken;
                    }
                });
            },

            unload: function () {
                dataRequest && dataRequest.cancel();
                jobOwnerToken && jobOwnerToken.cancelAll();
            }

            // Other PageControl members …
        });

    function populateSection(section, collection) {
        performance.mark("creating a hub section");
        section.data.header = collection.data.title;

        var contentElement = section.data.contentElement;
        contentElement.innerHTML = "";

        var pictures = collection.data.pictures;
        for (var i = 0; i < 2; i++) {

            $(contentElement).append("<img src='" + pictures[i].pictureThumb + "' />");
            (i % 2) && $(contentElement).append("<br/>")
        }
    }
})();
