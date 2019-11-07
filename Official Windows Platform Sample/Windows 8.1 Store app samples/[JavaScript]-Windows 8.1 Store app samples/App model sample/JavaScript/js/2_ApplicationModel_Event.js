//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

/// <reference path="//Microsoft.WinJS.2.0/js/base.js" />

(function () {
    "use strict";

    var counter = 1;

    var page = WinJS.UI.Pages.define("/html/2_ApplicationModel_Event.html", {
        ready: function (element, options) {
            // Connect up buttons
            element.querySelector("#addEventsButton").addEventListener("click",
                this.addEvents.bind(this));
            element.querySelector("#fireEventsButton").addEventListener("click",
                this.fireEvents.bind(this));
            element.querySelector("#removeEventsButton").addEventListener("click",
                this.removeEvents.bind(this));

            // Store away child elements for later
            this.eventHandlersOutput = element.querySelector("#eventHandlersOutput");
            this.checkboxes = WinJS.Utilities.query("[name=eventsCheckGroup]", element);

            this.clearLog();

            //
            // Bind our event handler to this page control instance. This saves
            // us time & confusion later on around getting the correct this pointer,
            // since this handler is added and removed from the event queue
            // multiple times.
            //

            this.eventHandler = this.eventHandler.bind(this);

            //
            // The event list. This list keeps track of which events have been signed up for
            // by the user. This is purely sample specific code, not an API demonstration.
            //
            this.eventList = new EventList();
        },

        //
        // Event handler for the "Add Events" button. Go through
        // the checked event type checkboxes, and if that event hasn't
        // already been registered, sign up for it and add it to the
        // event list
        //
        addEvents: function () {
            var that = this;

            // Go through our checkboxes and sign up for events that are checked.
            this.checkboxes.forEach(function (checkbox) {
                var eventType;

                if (checkbox.checked) {
                    eventType = checkbox.id;

                    // Hook up to the event queue for event type
                    WinJS.Application.addEventListener(eventType, that.eventHandler);

                    // Sock away event signup for sample display
                    that.eventList.add(eventType);
                }
            });

            // Update the display
            this.updateRegisteredEvents();
        },

        //
        // Event handler for the "Fire Events" button.
        // Go through the list of checked events and post
        // them to the queue.
        //
        fireEvents: function () {
            this.checkboxes.forEach(function (checkbox) {
                var eventType;
                if (checkbox.checked) {
                    eventType = checkbox.id;
                    //
                    // Post an event to the application object. You can pass
                    // arbitrary data here. The only requirement is that the
                    // posted object has a 'type' property containing the event
                    // type. The type is used to match up handlers to the corresponding
                    // calls to addEventListener. By convention, additional
                    // data is passed in a detail field.
                    //

                    WinJS.Application.queueEvent({
                        type: eventType,
                        detail: {
                            count: counter++
                        }
                    });
                }
            }, this);
        },

        //
        // Event handler for the "Remove Events" button. Go through the
        // checked boxes and unregister any event handlers for that
        // particular event.
        //
        removeEvents: function () {
            var that = this;

            this.clearLog();

            this.checkboxes.forEach(function (checkbox) {
                var eventType;

                if (checkbox.checked) {
                    eventType = checkbox.id;

                    //
                    // Unregister the event listener for this event type
                    //
                    WinJS.Application.removeEventListener(eventType, that.eventHandler);

                    that.eventList.remove(eventType);
                }
            });
            this.updateRegisteredEvents();
        },

        //
        // The events are routed to this method.
        //
        eventHandler: function (e) {
            // Retrieve event information from the event args. e.type is the
            // type of the event.
            var str = this.eventList.getEventNameFromType(e.type);
            var count = e.detail.count;

            // and act on the event. In this case, we just log it.
            str = str + " is fired with count " + count;

            this.logEvent(str);
        },

        //
        // Helper function for displaying events currently registered
        //
        updateRegisteredEvents: function () {
            var messages = this.eventList.registeredEventNames.join("\n");
            this.eventHandlersOutput.innerText = messages;
        },

        //
        // Helper functions for logging results
        //
        logEvent: function (message) {
            this.log = this.log || this.element.querySelector("#applicationModelEventsOutput");
            if (this.log.innerText !== "") {
                message = "\n" + message;
            }
            this.log.innerText += message;
        },
        clearLog: function () {
            this.log = this.log || this.element.querySelector("#applicationModelEventsOutput");
            this.log.innerText = "";
        },

        //
        // Navigating away from this page, unregister any
        // event listeners.
        //

        unload: function () {
            var that = this;
            this.eventList.forEach(function (eventType) {
                WinJS.Application.removeEventListener(eventType, that.eventHandler);
            });
        }
    });

    //
    // Helper object to manage the list of events page has registered for.
    // This is purely code specific to this sample, not part of the feature
    // being demonstrated.
    //
    var EventList = WinJS.Class.define(
        function () {
            this.eventList = [];
        },
        {
            // add the given event to the list if not already there
            add: function (eventTypeToAdd) {
                if (!this.eventList.some(
                    function (eventType) { return eventType === eventTypeToAdd; })
                ) {
                    this.eventList.push(eventTypeToAdd);
                }
            },

            // Remove event type from list if present
            remove: function (eventTypeToRemove) {
                this.eventList =
                    this.eventList.filter(function (eventType) {
                        return eventType !== eventTypeToRemove;
                    });
            },

            contains: function (eventType) {
                return this.eventList.some(function (e) { return e === eventType; });
            },

            // Convert event types into human readable names
            getEventNameFromType: function (eventType) {
                return {
                    firstEvent: "First event",
                    secondEvent: "Second event",
                    thirdEvent: "Third event"
                }[eventType] || "Unknown event";
            },

            registeredEventNames: {
                get: function () {
                    var that = this;
                    return this.eventList.map(
                        function (eventType) { return that.getEventNameFromType(eventType); }
                    );
                }
            },

            forEach: function (callback) {
                this.eventList.forEach(callback);
            }
        });

})();
