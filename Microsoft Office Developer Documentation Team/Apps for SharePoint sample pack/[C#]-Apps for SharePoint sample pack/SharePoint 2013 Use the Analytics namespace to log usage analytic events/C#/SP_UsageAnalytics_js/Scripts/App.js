var context;

// This code runs when the DOM is ready and creates a context object which is needed to 
// use the SharePoint object model
$(document).ready(function () {
    context = SP.ClientContext.get_current();

    // Default.aspx has a paragraph with the ID of Status, which we will write to after
    // attempting to log a usage event. So we'll obtain a reference here and set its 
    // innerText property in the success or failure callbacks below
    var status = document.getElementById("Status");

    // You will need to follow the steps in the ReadMe to determine your GUID
    // which you will need to insert in the following line between the quotes
    var eventGuid = new SP.Guid("ENTER YOUR GUID HERE");

    // When you have entered a valid GUID, you can then call the logAnalyticsAppEvent
    // as follows:
    SP.Analytics.AnalyticsUsageEntry.logAnalyticsAppEvent(context, eventGuid, "Test App Page");
    context.executeQueryAsync(

        // This is the success callback:
        function () {
            status.innerText = "Success! The event for 'Test App Page' has been logged.";
        },

        // This is the failure callback:
        function (sender, e) {
            status.innerText = "Failed to log event for 'Test App Page': " + e.get_message();
        });

});

