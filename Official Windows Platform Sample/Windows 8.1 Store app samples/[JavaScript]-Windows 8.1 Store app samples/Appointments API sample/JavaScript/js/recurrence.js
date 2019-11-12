//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/Recurrence.html", {
        ready: function (element, options) {
            document.getElementById("createButton").addEventListener("click", createRecurrence, false);
        }
    });

    function createRecurrence() {
        var isRecurrenceValid = true;
        var recurrence = new Windows.ApplicationModel.Appointments.AppointmentRecurrence();

        // Unit
        switch (document.querySelector('#unitSelect').selectedIndex) {
            case 0:
                recurrence.unit = Windows.ApplicationModel.Appointments.AppointmentRecurrenceUnit.daily;
                break;
            case 1:
                recurrence.unit = Windows.ApplicationModel.Appointments.AppointmentRecurrenceUnit.weekly;
                break;
            case 2:
                recurrence.unit = Windows.ApplicationModel.Appointments.AppointmentRecurrenceUnit.monthly;
                break;
            case 3:
                recurrence.unit = Windows.ApplicationModel.Appointments.AppointmentRecurrenceUnit.monthlyOnDay;
                break;
            case 4:
                recurrence.unit = Windows.ApplicationModel.Appointments.AppointmentRecurrenceUnit.yearly;
                break;
            case 5:
                recurrence.unit = Windows.ApplicationModel.Appointments.AppointmentRecurrenceUnit.yearlyOnDay;
                break;
        }

        // Occurrences
        // Note: Occurrences and Until properties are mutually exclusive.
        if (document.querySelector('#occurrencesRadioButton').checked) {
            recurrence.occurrences = document.querySelector('#occurrencesRange').valueAsNumber;
        }

        // Until
        // Note: Until and Occurrences properties are mutually exclusive.
        if (document.querySelector('#untilRadioButton').checked) {
            recurrence.until = document.querySelector('#untilDatePicker').winControl.current;
        }

        // Interval
        recurrence.interval = document.querySelector('#intervalRange').valueAsNumber;

        // Week of the month
        switch (document.querySelector('#weekOfMonthSelect').selectedIndex) {
            case 0:
                recurrence.weekOfMonth = Windows.ApplicationModel.Appointments.AppointmentWeekOfMonth.first;
                break;
            case 1:
                recurrence.weekOfMonth = Windows.ApplicationModel.Appointments.AppointmentWeekOfMonth.second;
                break;
            case 2:
                recurrence.weekOfMonth = Windows.ApplicationModel.Appointments.AppointmentWeekOfMonth.third;
                break;
            case 3:
                recurrence.weekOfMonth = Windows.ApplicationModel.Appointments.AppointmentWeekOfMonth.fourth;
                break;
            case 4:
                recurrence.weekOfMonth = Windows.ApplicationModel.Appointments.AppointmentWeekOfMonth.last;
                break;
        }

        // Days of the Week
        // Note: For Weekly, MonthlyOnDay or YearlyOnDay recurrence unit values, at least one day must be specified.
        if (document.querySelector('#sundayCheckBox').checked) { recurrence.daysOfWeek |= Windows.ApplicationModel.Appointments.AppointmentDaysOfWeek.sunday; }
        if (document.querySelector('#mondayCheckBox').checked) { recurrence.daysOfWeek |= Windows.ApplicationModel.Appointments.AppointmentDaysOfWeek.monday; }
        if (document.querySelector('#tuesdayCheckBox').checked) { recurrence.daysOfWeek |= Windows.ApplicationModel.Appointments.AppointmentDaysOfWeek.tuesday; }
        if (document.querySelector('#wednesdayCheckBox').checked) { recurrence.daysOfWeek |= Windows.ApplicationModel.Appointments.AppointmentDaysOfWeek.wednesday; }
        if (document.querySelector('#thursdayCheckBox').checked) { recurrence.daysOfWeek |= Windows.ApplicationModel.Appointments.AppointmentDaysOfWeek.thursday; }
        if (document.querySelector('#fridayCheckBox').checked) { recurrence.daysOfWeek |= Windows.ApplicationModel.Appointments.AppointmentDaysOfWeek.friday; }
        if (document.querySelector('#saturdayCheckBox').checked) { recurrence.daysOfWeek |= Windows.ApplicationModel.Appointments.AppointmentDaysOfWeek.saturday; }

        if (((recurrence.unit === Windows.ApplicationModel.Appointments.AppointmentRecurrenceUnit.weekly)       ||
             (recurrence.unit === Windows.ApplicationModel.Appointments.AppointmentRecurrenceUnit.monthlyOnDay) ||
             (recurrence.unit === Windows.ApplicationModel.Appointments.AppointmentRecurrenceUnit.yearlyOnDay)) &&
            (recurrence.daysOfWeek === Windows.ApplicationModel.Appointments.AppointmentDaysOfWeek.none)) {
            isRecurrenceValid = false;
            document.querySelector('#result').innerText = "The recurrence specified is invalid. For Weekly, MonthlyOnDay or YearlyOnDay recurrence unit values, at least one day must be specified.";
        }

        // Month of the year
        recurrence.month = document.querySelector('#monthOfYearRange').valueAsNumber;

        // Day of the month
        recurrence.day = document.querySelector('#dayOfMonthRange').valueAsNumber;

        if (isRecurrenceValid)
        {
            document.querySelector('#result').innerText = "The recurrence specified was created successfully and is valid.";
        }
    }
})();
