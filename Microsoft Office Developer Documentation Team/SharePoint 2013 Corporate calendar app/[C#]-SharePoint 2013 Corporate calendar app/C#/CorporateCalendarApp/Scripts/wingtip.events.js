"use strict";

var WingtipToys = window.WingtipToys || {};

WingtipToys.Paths = function () {

    var readAll = function () {
        var deferred = $.ajax(
                {
                    url: _spPageContextInfo.webServerRelativeUrl +
                        "/_api/web/lists/getByTitle('Included Paths')/items",
                    type: "GET",
                    headers: {
                        "accept": "application/json;odata=verbose",
                    }
                }
            );

        return deferred.promise();
    };

    return {
        read_all: readAll
    };

}();

WingtipToys.Event = function (title, start, end, allDay) {

    var eventTitle = title,
        eventStart = start,
        eventEnd = end,
        eventAllDay = allDay,
        getTitle = function () { return eventTitle; },
        setTitle = function (v) { eventTitle = v; },
        getStart = function () { return eventStart; },
        getFormattedStart = function () {
            return eventStart.getFullYear() + "-" +
                   formatLeadingZeroes((eventStart.getMonth() + 1)) + "-" +
                   formatLeadingZeroes(eventStart.getDate()) + " " +
                   formatLeadingZeroes(eventStart.getHours()) + ":" +
                   formatLeadingZeroes(eventStart.getMinutes()) + ":" +
                   formatLeadingZeroes(eventStart.getSeconds());
        },
        setStart = function (v) { eventStart = v; },
        getEnd = function () { return eventEnd; },
        getFormattedEnd = function () {
            return eventEnd.getFullYear() + "-" +
                   formatLeadingZeroes((eventEnd.getMonth() + 1)) + "-" +
                   formatLeadingZeroes(eventEnd.getDate()) + " " +
                   formatLeadingZeroes(eventEnd.getHours()) + ":" +
                   formatLeadingZeroes(eventEnd.getMinutes()) + ":" +
                   formatLeadingZeroes(eventEnd.getSeconds());
        },
        setEnd = function (v) { eventEnd = v; },
        getAllDay = function () { return eventAllDay; },
        setAllDay = function (v) { eventAllDay = v; },
        getFormattedAllDay = function () {
            if (eventAllDay === 'true')
                return true;
            else
                return false;
        },

        formatLeadingZeroes = function (val) {
            if (val<10) {
                return "0" + val;
            }
            else {
                return val;
            }
        };

    return {
        get_title: getTitle,
        set_title: setTitle,
        get_start: getStart,
        get_formattedStart: getFormattedStart,
        set_start: setStart,
        get_end: getEnd,
        get_formattedEnd: getFormattedEnd,
        set_end: setEnd,
        get_allDay: getAllDay,
        set_allDay: setAllDay,
        get_formattedAllDay: getFormattedAllDay
    };

};

WingtipToys.Events = function () {

    var events = [],

        getEvents = function () { return events; },

        /* The calendar plugin expects the event information
        /* formatted in a specific way. */
        getFormattedEvents = function () {

            var json = [];
            for (var i = 0; i < events.length; i++) {
                json.push({
                    title: events[i].get_title(),
                    start: events[i].get_formattedStart(),
                    end: events[i].get_formattedEnd(),
                    allDay: events[i].get_formattedAllDay()
                });
            }

            return json;

        },

        load = function (daysPast, daysFuture, paths) {

            //Setup the date range for the search
            var now = new Date();
            var startDate = new Date();
            var endDate = new Date();
            startDate.setDate(startDate.getDate() - daysPast);
            endDate.setDate(endDate.getDate() + daysFuture);

            var startDateString = (startDate.getMonth() + 1) + "/" + startDate.getDate() + "/" + startDate.getFullYear();
            var endDateString = (endDate.getMonth() + 1) + "/" + endDate.getDate() + "/" + endDate.getFullYear();

            //Build the base RESTful url with the date range
            var url = _spPageContextInfo.webAbsoluteUrl +
            "/_api/search/query?querytext='ContentClass:STS_ListItem_Events" +
            " EventDate>" + startDateString +
            " EndDate<" + endDateString;
            
            //Add the included path property restrictions
            if (paths.length > 0) {
                url += " (";
                for (var i = 0; i < paths.length; i++) {
                    if (i > 0) url += " OR ";
                    url += "Path:" + paths[i];
                }
                url += ")"
            }

            //Add the selected properties
            url += "'&selectproperties='Title,EventDate,EndDate,AllDayEvent,Path'";

            //Execute the RESTful search call
            var deferred = $.ajax({
                url: url,
                method: "GET",
                headers: {
                    "accept": "application/xml",
                },
                success: function (data) {

                    //Fill event array with results
                    events = [];

                    $(data).find("d\\:Rows").children("d\\:element").each(function () {

                        $(this).find("d\\:Cells").each(function () {

                            var event = new WingtipToys.Event;

                            $(this).find("d\\:element").each(function () {

                                if ($(this).children("d\\:Key").first().text() == "Title")
                                    event.set_title($(this).children("d\\:Value").first().text());
                                if ($(this).children("d\\:Key").first().text() == "AllDayEvent")
                                    event.set_allDay($(this).children("d\\:Value").first().text());
                                if ($(this).children("d\\:Key").first().text() == "EventDate") {

                                    var val = $(this).children("d\\:Value").first().text();

                                    event.set_start(
                                        new Date(Date.UTC(
                                            parseInt(val.substring(0, 4)),
                                            parseInt(val.substring(5, 7)) - 1,
                                            parseInt(val.substring(8, 10)),
                                            parseInt(val.substring(11, 13)),
                                            parseInt(val.substring(14, 16)),
                                            parseInt(val.substring(17, 19))
                                        ))
                                    );
                                }
                                if ($(this).children("d\\:Key").first().text() == "EndDate") {

                                    var val = $(this).children("d\\:Value").first().text();

                                    event.set_end(
                                        new Date(Date.UTC(
                                            parseInt(val.substring(0, 4)),
                                            parseInt(val.substring(5, 7)) - 1,
                                            parseInt(val.substring(8, 10)),
                                            parseInt(val.substring(11, 13)),
                                            parseInt(val.substring(14, 16)),
                                            parseInt(val.substring(17, 19))
                                        ))
                                    );
                                }
                            });

                            events.push(event);

                        });
                    });
                },
                error: function (err) {
                    events = [];
                    alert(JSON.stringify(err));
                }
            });

            return deferred.promise();
        };

    return {
        load: load,
        get_events: getEvents,
        get_formattedEvents: getFormattedEvents
    };

}();

