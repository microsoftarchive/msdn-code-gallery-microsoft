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


// Place custom JavaScript below

$(document).ready(function () {

    // Namespace
    window.AppLevelECT = window.AppLevelECT || {};

    // Constructor
    AppLevelECT.Grid = function (colContainerName, numCols, surlWeb) {
        var _colContainerName = colContainerName;
        var _numCols = numCols;
        if (surlWeb.length > 0 &&
            surlWeb.substring(surlWeb.length - 1, surlWeb.length) != "/")
            surlWeb += "/";
        var _surlWeb = surlWeb;

        this.init = function () {
            var listURL = _surlWeb +
                    "_api/Lists/GetByTitle('Employee')/items?" +
                    "$select=BdcIdentity,EmployeeId,EmployeeFirstName,EmployeeLastName,Telephone";
             $.ajax({
                url: listURL,
                headers: {
                    "accept": "application/json",
                    "X-RequestDigest": $("#__REQUESTDIGEST").val()
                },
                success: this.showItems,
                error: this.failMethod
            });
        }

        this.showItems = function (data) {

            // Remove any contact cards loaded onto the page earlier
            // E.g. in case refreshing the page after deleting a contact card
            $("#Container").children().remove();

            $.each(data.d.results, function (key, val) {
                // Clone the template
                var newEl = $("#ContactCardTemplate").clone()
                                .attr("id", val.BdcIdentity)
                                .fadeIn("slow");

                // *** Now fill in the data
                newEl.find(".ContactCardFirstName").text("FIRST NAME: " + val.EmployeeFirstName);
                newEl.find(".ContactCardLastName").text("LAST NAME: " + val.EmployeeLastName);
                newEl.find(".ContactCardID").text("ID: " + val.EmployeeId);
                newEl.find(".ContactCardPhone").text("PHONE: " + val.Telephone);

                // *** Append item to the list view container
                newEl.appendTo("#Container");
            });
        }

        this.failMethod = function (jqXHR, textStatus, errorThrown) {
            alert('failed: ' + errorThrown);
        }

        this.deleteItem = function (BDCId) {
            var listURL = _surlWeb +
                    "_api/Lists/GetByTitle('Employee')/GetItemByStringId('" + BDCId + "')";
            $.ajax({
                url: listURL,
                type: "DELETE",
                headers: {
                    "accept": "application/json",
                    "X-RequestDigest": $("#__REQUESTDIGEST").val()
                },
                success: function () {
                    var grid = new AppLevelECT.Grid("ColumnContainer", 3, _spPageContextInfo.webServerRelativeUrl);
                    grid.init();
                },
                error: this.failMethod
            });
        }

    }
    ExecuteOrDelayUntilScriptLoaded(getEmployees, "sp.js");
});


function getEmployees() {
    var grid = new AppLevelECT.Grid("ColumnContainer", 3, _spPageContextInfo.webServerRelativeUrl);
    grid.init();
}

function ShowEditWindow(item) {
    var BDCId = $(item).attr("id");
    var page = window.location;
    window.location = "../Lists/Employee/EditForm.aspx?ID=" + BDCId + "&Source=" + escape(page.toString());
}

function DeleteItem(item) {
    var BDCId = $(item).attr("id");
    var grid = new AppLevelECT.Grid("ColumnContainer", 3, _spPageContextInfo.webServerRelativeUrl);
    grid.deleteItem(BDCId);
}
