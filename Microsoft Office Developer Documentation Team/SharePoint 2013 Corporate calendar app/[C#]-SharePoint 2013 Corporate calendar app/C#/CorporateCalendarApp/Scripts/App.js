"use strict";

var WingtipToys = window.WingtipToys || {};

$(document).ready(function () {

    //Get any included paths
    WingtipToys.Paths.read_all().then(

        //Get any included calendar paths
        function (data) {
            var results = data.d.results;
            var paths = [];
            for (var i = 0; i < results.length; i++) {
                paths.push(results[i].URL.Url);
            }

            //Load the calendar
            WingtipToys.Events.load(90, 365, paths).then(
                function (data) {
                    $("#corporateCalendar").fullCalendar({
                        events: WingtipToys.Events.get_formattedEvents()
                    });
                },
                function (err) {
                    alert(JSON.stringify(err));
                }
            );

        },
        function (err) {
            alert(JSON.stringify(err));
        }
    );

});


