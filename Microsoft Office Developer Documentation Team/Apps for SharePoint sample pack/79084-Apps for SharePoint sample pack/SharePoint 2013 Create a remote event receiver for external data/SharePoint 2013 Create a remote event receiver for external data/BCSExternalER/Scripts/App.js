var context;
var web;
var user;

// This function is executed after the DOM is ready and SharePoint scripts are loaded
// Place any code you want to run when Default.aspx is loaded in this function
// The code creates a context object which is needed to use the SharePoint object model
function sharePointReady() {
    context = new SP.ClientContext.get_current();
    web = context.get_web();

    getUserName();
}

// This function prepares, loads, and then executes a SharePoint query to get the current users information
function getUserName() {
    user = web.get_currentUser();
    context.load(user);
    context.executeQueryAsync(onGetUserNameSuccess, onGetUserNameFail);
}

// This function is executed if the above OM call is successful
// It replaces the content of the 'welcome' element with the user name
function onGetUserNameSuccess() {
    $('#message').text('Hello ' + user.get_title());
}

// This function is executed if the above OM call fails
function onGetUserNameFail(sender, args) {
    alert('Failed to get user name. Error:' + args.get_message());
}

// Custom functions
function Subscribe() {
    context = new SP.ClientContext();
    var myweb = context.get_web();
    var employeeList = myweb.get_lists().getByTitle("Employee");
    var eventReceiver = new SP.EventReceiverDefinitionCreationInformation();
    eventReceiver.set_receiverName("ItemAddedEventReceiver");
    // Modify the set_receiverUrl to your local server URL
    eventReceiver.set_receiverUrl("http://sphvm-22376");
    eventReceiver.set_eventType(10001);
    eventReceiver.set_sequenceNumber(10000);
    employeeList.get_eventReceivers().add(eventReceiver);
    context.executeQueryAsync(OnEventReceiverAdded, failmethod);
}

function OnEventReceiverAdded() {
    alert("Event Receiver added");
}

function failmethod() {
    alert("Failed to add event receiver");
}
