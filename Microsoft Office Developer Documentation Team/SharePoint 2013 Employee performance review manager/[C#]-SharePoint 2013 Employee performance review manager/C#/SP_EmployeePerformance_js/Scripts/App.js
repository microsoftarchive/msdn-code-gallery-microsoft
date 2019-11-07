
// Variable that will hold the SharePoint ClientContext object
var context;

// Variable that will hold the SharePoint App Web object
var web;

//Variable that will hold the SharePoint user
var user;

// Variables that will hold various SharePoint List objects
var objList;
var empList;
var reviewList;
var oList;

//Variable that will hold the SharePoint user name
var userName;

//Variables that will hold specific SharePoint List IDs
var empID;
var revID;
var rID;

// Variables that will hold various SharePoint ListItem objects or data
var currentItem;
var employeeItem;
var objItem;
var oItem;
var reviewStatus;

// This function runs when the DOM is ready.
// It applies jQuery methods to turn various text input elements into date pickers.
// It also creates a context object which is needed to use the SharePoint object model,
// and determines whether the current user is a 'reviewer' or a 'reviewee'.
// Note: In this scenario, a person who has the SharePoint permissions for 'ManageWeb' is
// considered a 'reviewer', while a person who is a normal site member is a normal employee (reviewee).
$(document).ready(function () {
    context = SP.ClientContext.get_current();
    web = context.get_web();

    userName = web.get_currentUser();
    
    userName.retrieve();
    
    context.load(web, 'EffectiveBasePermissions');
    context.executeQueryAsync(
    function () {

        // We use the Client-Side PeoplePicker to add a nice feature to this app. It enables the 'reviewer' to
        // type user names and get matches from the current membership of the Web.
        if (web.get_effectiveBasePermissions().has(SP.PermissionKind.manageWeb)) {

            // User is a reviewer
            showEmployees();
            $('#peoplePicker').show();
            initializePeoplePicker('peoplePickerDiv');
        }
        else {

            // User is a normal site member
            matchUser(userName.get_title());
           
        }
       

    },
    function (sender, args) {
    });

    // Turn various text inputs into calendars by using jQuery UI methods
    $('#startDate').datepicker({
        showOn: "both",
        buttonImage: "../images/calendar.gif",
        buttonImageOnly: true,
        nextText: "",
        prevText: "",
        changeMonth: true,
        changeYear: true,
        dateFormat: "MM dd, yy"
    });
    $('#editStartDate').datepicker({
        showOn: "both",
        buttonImage: "../images/calendar.gif",
        buttonImageOnly: true,
        nextText: "",
        prevText: "",
        changeMonth: true,
        changeYear: true,
        dateFormat: "MM dd, yy"
    });
    $('#endDate').datepicker({
        showOn: "both",
        buttonImage: "../images/calendar.gif",
        buttonImageOnly: true,
        nextText: "",
        prevText: "",
        changeMonth: true,
        changeYear: true,
        dateFormat: "MM dd, yy"
    });
    $('#editEndDate').datepicker({
        showOn: "both",
        buttonImage: "../images/calendar.gif",
        buttonImageOnly: true,
        nextText: "",
        prevText: "",
        changeMonth: true,
        changeYear: true,
        dateFormat: "MM dd, yy"
    });
    $('editStatus').attr("disabled", true);
});


// Render and initialize the client-side People Picker.
function initializePeoplePicker(peoplePickerElementId) {

    // Create a schema to store picker properties, and set the properties.
    var schema = {};
    schema['PrincipalAccountType'] = 'User,DL,SecGroup,SPGroup';
    schema['SearchPrincipalSource'] = 15;
    schema['ResolvePrincipalSource'] = 15;
    schema['AllowMultipleValues'] = false;
    schema['MaximumEntitySuggestions'] = 50;
    schema['Width'] = '180px';

    // Render and initialize the picker. 
    // Pass the ID of the DOM element that contains the picker, an array of initial
    // PickerEntity objects to set the picker value, and a schema that defines
    // picker properties.
    this.SPClientPeoplePicker_InitStandaloneControlWrapper(peoplePickerElementId, null, schema);
}

// Query the picker for user information.
function getUserInfo() {

    // Get the people picker object from the page.
    var peoplePicker = this.SPClientPeoplePicker.SPClientPeoplePickerDict.peoplePickerDiv_TopSpan;
    var employeeName;
        // Get information about all users.
    empList = web.get_lists().getByTitle('Employee');
    users = peoplePicker.GetAllUserInfo();
    for (var i = 0; i < users.length; i++) {
        user = users[i];
        employeeName = user["DisplayText"];
        addEmployee(employeeName);
    }
    initializePeoplePicker('peoplePickerDiv');
}

// This function adds employee to the Employee list
function addEmployee(employeeName) {
    var camlQuery = SP.CamlQuery.createAllItemsQuery();
    var listItems = empList.getItems(camlQuery);
    var hasEmployee = false;
    context.load(listItems);
    context.executeQueryAsync(
       function () {
           var listItemEnumerator = listItems.getEnumerator();
           while (listItemEnumerator.moveNext()) {
               var listItem = listItemEnumerator.get_current();
  
               if (listItem.get_fieldValues()["Title"] == employeeName) {
                   hasEmployee = true;
               }
           }
           if (hasEmployee == false) {
               var itemCreateInfo = new SP.ListItemCreationInformation();
               var listItem = empList.addItem(itemCreateInfo);
               listItem.set_item("Title", employeeName);
               listItem.update();
               context.load(listItem);
               context.executeQueryAsync(function () {
                   showEmployees();
  
               },function(sender,args){alert(args.get_message());});
           }
         
          
       }, function () {
           alert("Error");
       });
   
}

// This function retrieves all employees who have been added by the reviewer
function showEmployees() {
    var errArea = document.getElementById("errAllEmployee");
    // Remove all nodes from the error <DIV> so we have a clean space to write to in case of errors
    while (errArea.hasChildNodes()) {
        errArea.removeChild(errArea.lastChild);
    }
    var hasEmployee = false;
    empList = web.get_lists().getByTitle('Employee');
    var camlQuery = SP.CamlQuery.createAllItemsQuery();
    var listItems = empList.getItems(camlQuery);

    context.load(listItems);
    context.executeQueryAsync(
        function () {

            // Success returned from executeQueryAsync
            var employeeTable = document.getElementById("EmployeeList");

            // Remove all nodes from the employee <DIV> so we have a clean space to write to
            while (employeeTable.hasChildNodes()) {
                employeeTable.removeChild(employeeTable.lastChild);
            }

            // Iterate through the Employee list
            var listItemEnumerator = listItems.getEnumerator();
            while (listItemEnumerator.moveNext()) {
                    var listItem = listItemEnumerator.get_current();
                    var employee = document.createElement("div");
                    var employeeLabel = document.createTextNode(listItem.get_fieldValues()["Title"]);
                    employee.appendChild(employeeLabel);

                    // Add an ID to the employee DIV
                    employee.id = listItem.get_id();

                    // Add an class to the employee DIV
                    employee.className = "item";

                    // Add an onclick event to show the employee details
                    $(employee).click(function (sender) {
                        showEmployeeDetails(sender.target.id);
                    });

                    // Add the employee div to the UI
                    employeeTable.appendChild(employee);
                    hasEmployee = true;
                }
                if (!hasEmployee) {
                var noEmployee = document.createElement("div");
                noEmployee.appendChild(document.createTextNode("There are currently no employees with reviews. Use the people picker above to add employees"));
                employeeTable.appendChild(noEmployee);
            }
            $('#AllEmployee').fadeIn(500, null);
        },
        function (sender, args) {

            // Failure returned from executeQueryAsync
            var divMessage = document.createElement("DIV");
            divMessage.setAttribute("style", "padding:5px;");
            divMessage.appendChild(document.createTextNode("Failed to get Employee. Error: " + args.get_message()));
            errArea.appendChild(divMessage);
            $('#EmployeeList').fadeIn(500, null);
        });

}

// This function hides all main DIV elements. The caller is then responsible 
// for re-showing the one that needs to be displayed.
function hideAllPanels() {
    $('#reviewForm').hide();
    $('#showReview').hide();
    $('#editReview').hide();
}

// This function shows the details for a specific Employee
function showEmployeeDetails(itemID) {
    hideAllPanels();
    clearNewReviewForm();
    clearEditReviewForm();
    var errArea = document.getElementById("errAllEmployee");
    // Remove all nodes from the error <DIV> so we have a clean space to write to
    while (errArea.hasChildNodes()) {
        errArea.removeChild(errArea.lastChild);
    }
   
    employeeItem = empList.getItemById(itemID);
    context.load(employeeItem);
    context.executeQueryAsync(
        function () {
            $('#employeeName').val(employeeItem.get_fieldValues()["Title"]);
            showReviews(itemID);
            $('#reviewForm').fadeIn(500, null);
        },
        function (sender, args) {
            var errArea = document.getElementById("errAllEmployee");
            // Remove all nodes from the error <DIV> so we have a clean space to write to
            while (errArea.hasChildNodes()) {
                errArea.removeChild(errArea.lastChild);
            }
            var divMessage = document.createElement("DIV");
            divMessage.setAttribute("style", "padding:5px;");
            divMessage.appendChild(document.createTextNode(args.get_message()));
            errArea.appendChild(divMessage);
        });
}
 
// This function retrieves the details of deferred objective for a specific employee and updates it in the new review
function getDeferredObj(revID) {
    revList = web.get_lists().getByTitle('Review');
    // Create a CAML query that retrieves the Employee Name for the particular employee
    var empCamlQuery = new SP.CamlQuery();
    empCamlQuery.set_viewXml("<View><Query><Where><Eq><FieldRef Name='EmployeeName' LookupId='TRUE' /><Value Type='Lookup'>"
        + empID
        + "</Value></Eq></Where></Query></View>");
    var listItems = revList.getItems(empCamlQuery);
    context.load(listItems);
    context.executeQueryAsync(function () {
        var listItemEnumerator = listItems.getEnumerator();
        while (listItemEnumerator.moveNext()) {
            var listItem = listItemEnumerator.get_current();
            var revLabel = listItem.get_fieldValues()["_Status"];
            if (revLabel == "Completed") {
                var getRevId = listItem.get_fieldValues()["ID"];
                if (getRevId != revID) {
                    // Create a CAML query that retrieves the Review ID for the particular review
                    var revCamlQuery = new SP.CamlQuery();
                    revCamlQuery.set_viewXml("<View><Query><Where><Eq><FieldRef Name='ReviewLookup' LookupId='TRUE' /><Value Type='Lookup'>"
                        + getRevId
                        + "</Value></Eq></Where></Query></View>");
                    objList = web.get_lists().getByTitle('Objective');
                    var objListItems = objList.getItems(revCamlQuery);
                    var objContext = SP.ClientContext.get_current();
                    objContext.load(objListItems);
                    objContext.executeQueryAsync(
                        function () {
                            var objItemEnumerator = objListItems.getEnumerator();
                            while (objItemEnumerator.moveNext()) {
                                var objListItem = objItemEnumerator.get_current();
                                var objstatus = objListItem.get_fieldValues()["_Status"];
                                if (objstatus == "Deferred") {
                                    var defObjName = objListItem.get_fieldValues()["ObjectiveName"];
                                    var defObjPriority = objListItem.get_fieldValues()["Priority"];
                                    objListItem.set_item("ReviewLookup", revID);
                                    objListItem.set_item("ObjectiveName", defObjName);
                                    objListItem.set_item("_Status", "Active");
                                    objListItem.set_item("Priority", defObjPriority);
                                    objListItem.set_item("EmployeeComments", "");
                                    objListItem.update();
                                    showObjectives(revID);
                                    $('#objectiveList').show();
                                }
                            }
                        },
                                    function () { alert("third async failed") });
                }
            }
        }
    },
        function () { alert("first async failed") });
}

// This function retrieves all reviews of an employee
function showReviews(itemID) {
    empID = itemID;
    var errArea = document.getElementById("errReview");
    
    // Remove all nodes from the error <DIV> so we have a clean space to write to in case of errors
    while (errArea.hasChildNodes()) {
        errArea.removeChild(errArea.lastChild);
    }
    hideAllPanels();
    clearNewReviewForm();
    clearEditReviewForm();
    var hasReviews = false;
    var activeRevCount = 0;
    reviewList = web.get_lists().getByTitle('Review');

    // Create a CAML query that retrieves the Employee Name for the particular employee
    var camlQuery = new SP.CamlQuery();
    camlQuery.set_viewXml("<View><Query><Where><Eq><FieldRef Name='EmployeeName' LookupId='TRUE' /><Value Type='Lookup'>"
        + empID
        + "</Value></Eq></Where></Query></View>");
    var listItems = reviewList.getItems(camlQuery);
    context.load(listItems);
    context.executeQueryAsync(
        function () {

            // Success returned from executeQueryAsync.
            var reviewTable = document.getElementById("reviewList");

            // Remove all nodes from the Review <DIV> so we have a clean space to write to
            while (reviewTable.hasChildNodes()) {
                reviewTable.removeChild(reviewTable.lastChild);
            }

      

            // Iterate through the Review list.
            var listItemEnumerator = listItems.getEnumerator();
            while (listItemEnumerator.moveNext()) {
                var listItem = listItemEnumerator.get_current();

                // Create a DIV to display the Review
                var rev = document.createElement("div");
                var revLabel = document.createTextNode(listItem.get_fieldValues()["StartDate"].format("MMMM dd, yyyy") + " to " + listItem.get_fieldValues()["_EndDate"].format("MMMM dd, yyyy"));

                var revStatus = listItem.get_fieldValues()["_Status"];
                if (revStatus == "Active") {
                    activeRevCount = activeRevCount + 1;

                }
                if (revStatus == "Completed") {
                    
                    document.getElementById("editStatus").disabled = true;
                }
                rev.appendChild(revLabel);
                // Add an ID to the Review DIV
                rev.id = listItem.get_id();
                // Add an class to the Review DIV
                rev.className = "itemReview";
                // Add an onclick event to show the Review details
                $(rev).click(function (sender) {
                    showReviewDetails(sender.target.id);                 
                });
                // Add the Review div to the UI
                reviewTable.appendChild(rev);
                hasReviews = true;
            }
            if (activeRevCount == 0) {
                $('#reviewForm').fadeIn(500, null);
                $('#newReview').show();
             
            }
            else {
                $('#newReview').hide();
            }
            if (!hasReviews) {
                var noRevs = document.createElement("div");
                noRevs.className = "errorReview";
                noRevs.appendChild(document.createTextNode("There are no reviews for this employee."));
                reviewTable.appendChild(noRevs);
            }
            $('#reviewForm').fadeIn(500, null);
        },
        function (sender, args) {

            // Failure returned from executeQueryAsync.
            var divMessage = document.createElement("DIV");
            divMessage.setAttribute("style", "padding:5px;");
            divMessage.appendChild(document.createTextNode("Failed to get reviews. Error: " + args.get_message()));
            errArea.appendChild(divMessage);
            $('#reviewForm').fadeIn(500, null);
        });
}

// This function shows the details for a specific Review
function showReviewDetails(itemID) {

    var errArea = document.getElementById("errReview");
    // Remove all nodes from the error <DIV> so we have a clean space to write to
    while (errArea.hasChildNodes()) {
        errArea.removeChild(errArea.lastChild);
    }
    revID = itemID;
    currentItem = reviewList.getItemById(itemID);
    context.load(currentItem);
    context.executeQueryAsync(
        function () {
            $('#editComments').val(currentItem.get_fieldValues()["_Comments"]);
            reviewStatus = currentItem.get_fieldValues()["_Status"];
            $('#editStatus').val(reviewStatus);
            $('#editStartDate').val(new Date(currentItem.get_fieldValues()["StartDate"]).format("MMMM dd, yyyy"));
            $('#editEndDate').val(new Date(currentItem.get_fieldValues()["_EndDate"]).format("MMMM dd, yyyy"));
            $('#editReview').slideDown(500,null);
            $('#showReview').hide();
            getDeferredObj(revID);
            if (reviewStatus == "Completed") {
                var disableStatus = document.getElementById("editStatus");
                disableStatus.disabled = true;
                showObjectives(revID);
            }
            else {
                showObjectives(revID);
            }
        },
        function (sender, args) {
            var errArea = document.getElementById("errReview");
            // Remove all nodes from the error <DIV> so we have a clean space to write to
            while (errArea.hasChildNodes()) {
                errArea.removeChild(errArea.lastChild);
            }
            var divMessage = document.createElement("DIV");
            divMessage.setAttribute("style", "padding:5px;");
            divMessage.appendChild(document.createTextNode(args.get_message()));
            errArea.appendChild(divMessage);
        });
}

// This function shows the form for adding a new review
function addNewReview() {
    $('#comment').val("");
    $('#startDate').val("");
    $('#endDate').val("");
    $('#showReview').slideDown(500, null);
    $('#objDetails').hide();
    $('#editReview').hide();
}

// This function saves the new review
function saveNewReview() {
        var itemCreateInfo = new SP.ListItemCreationInformation();
        var revList = web.get_lists().getByTitle('Review');
        var revListItem = revList.addItem(itemCreateInfo);
        revListItem.set_item("EmployeeName", empID);
        revListItem.set_item("_Comments", $('#comment').val());
        revListItem.set_item("_Status", $('#newReviewStatus').val());
        revListItem.set_item("StartDate", $('#startDate').val());
        revListItem.set_item("_EndDate", $('#endDate').val());
        revListItem.update();
        context.load(revListItem);
        context.executeQueryAsync(function () {
            showReviews(empID);
            $('#reviewList').show();
            $('#newReview').hide();
            $('#objDetails').show();
            clearNewReviewForm();          
        },
            function (sender, args) {
                var errArea = document.getElementById("errReview");
                // Remove all nodes from the error <DIV> so we have a clean space to write to
                while (errArea.hasChildNodes()) {
                    errArea.removeChild(errArea.lastChild);
                }
                var divMessage = document.createElement("DIV");
                divMessage.setAttribute("style", "padding:5px;");
                divMessage.appendChild(document.createTextNode(args.get_message()));
                errArea.appendChild(divMessage);
            });
    
}

// This function clears the inputs on the new review form
function clearNewReviewForm() {
    var errArea = document.getElementById("errReview");
    // Remove all nodes from the error <DIV> so we have a clean space to write to
    while (errArea.hasChildNodes()) {
        errArea.removeChild(errArea.lastChild);
    }
    $('#showReview').fadeOut(500, function () {
        $('#showReview').hide();
        $('#comment').val("");
        $('#startDate').val("");
        $('#endDate').val("");
    });
}

// This function clears the inputs on the edit review form
function clearEditReviewForm() {
    var errArea = document.getElementById("errReview");
    // Remove all nodes from the error <DIV> so we have a clean space to write to
    while (errArea.hasChildNodes()) {
        errArea.removeChild(errArea.lastChild);
    }
    $('#editReview').fadeOut(500, function () {
        $('#editReview').hide();
        $('#editComments').val("");
        $('#editStatus').val("");
        $('#editStartDate').val("");
        $('#editEndDate').val("");
    });
}

// This function cancels the review
function cancelReview() {
    clearNewReviewForm();
    clearEditReviewForm();
}

// This function shows the form for adding a new objective
function addNewObjective() {
    clearNewObjForm();
    $('#newObjectiveDialog').dialog(
        {
            height: "auto",
            width: "auto",
            
            modal: true,
           
            show: {
                effect: "Scale",
                duration: 1000
            },
            hide: {
                effect: "Explode",
                duration: 1000
            }
            
        });
}

// This function cancels the objective
function cancelObjective() {
    $('#newObjectiveDialog').dialog("close");  
}

// This function cancels the editing of an existing objective
function cancelEditObjective() {
    $('#editObjDialog').dialog("close");
}

// This function clears the inputs on the edit objective form
function clearEditObjForm() {
    var errArea = document.getElementById("errObj");
    // Remove all nodes from the error <DIV> so we have a clean space to write to
    while (errArea.hasChildNodes()) {
        errArea.removeChild(errArea.lastChild);
    }
    $('#editObjName').val("");
    $('#editObjPriority').val("High");
    $('#editObjStatus').val("Active");
    $('#editEmpComments').val("");

}

// This function saves the new objective
function saveNewObjective() {
    var itemCreateInfo = new SP.ListItemCreationInformation();
    var objList = web.get_lists().getByTitle('Objective');
    var objListItem = objList.addItem(itemCreateInfo);
    objListItem.set_item("ReviewLookup", revID);
    objListItem.set_item("ObjectiveName", $('#newObjName').val());
    objListItem.set_item("_Status", $('#newObjStatus').val());
    objListItem.set_item("Priority", $('#newObjPriority').val());
    objListItem.update();
    context.load(objListItem);
    context.executeQueryAsync(function () {
        showObjectives(revID);
        $('#objectiveList').show();
        $('#newObjective').hide();
        clearNewObjForm();
        cancelObjective();
    },
        function (sender, args) {
            var errArea = document.getElementById("errObj");
            // Remove all nodes from the error <DIV> so we have a clean space to write to
            while (errArea.hasChildNodes()) {
                errArea.removeChild(errArea.lastChild);
            }
            var divMessage = document.createElement("DIV");
            divMessage.setAttribute("style", "padding:5px;");
            divMessage.appendChild(document.createTextNode(args.get_message()));
            errArea.appendChild(divMessage);
        });
}

// This function clears the inputs on the new objective form
function clearNewObjForm() {
    var errArea = document.getElementById("errObj");
    // Remove all nodes from the error <DIV> so we have a clean space to write to
    while (errArea.hasChildNodes()) {
        errArea.removeChild(errArea.lastChild);
    }
    $('#newObjName').val("");
    $('#newObjPriority').val("High");
    $('#newObjStatus').val("Active");
    $('#newEmpComments').val("");

}

// This function retrieves all objectives for particular review of an employee
function showObjectives(revID) {
    objID = revID;
    var activeObjCount=0;
    var errArea = document.getElementById("errObj");
    // Remove all nodes from the error <DIV> so we have a clean space to write to in case of errors
    while (errArea.hasChildNodes()) {
        errArea.removeChild(errArea.lastChild);
    }
    var hasObjs = false;
    objList = web.get_lists().getByTitle('Objective');

    // Create a CAML query that retrieves the objectives for the review in question
    var camlQuery = new SP.CamlQuery();
    camlQuery.set_viewXml("<View><Query><Where><Eq><FieldRef Name='ReviewLookup' LookupId='TRUE' /><Value Type='Lookup'>"
        + objID
        + "</Value></Eq></Where></Query></View>");
    var listItems = objList.getItems(camlQuery);
    context.load(listItems);
    context.executeQueryAsync(
        function () {
            // Success returned from executeQueryAsync.
            var objTable = document.getElementById("objectiveList");
            // Remove all nodes from the objectives <DIV> so we have a clean space to write to
            while (objTable.hasChildNodes()) {
                objTable.removeChild(objTable.lastChild);
            }
            // Iterate through the objectives list.
            var listItemEnumerator = listItems.getEnumerator();
            while (listItemEnumerator.moveNext()) {
                var listItem = listItemEnumerator.get_current();

                // Create a DIV to display the objective name
                var obj = document.createElement("div");
                var objLabel = document.createTextNode(listItem.get_fieldValues()["ObjectiveName"]);

                var objStatus = listItem.get_fieldValues()["_Status"];
                if (objStatus == "Active") {
                  activeObjCount = activeObjCount + 1;
                }

                obj.appendChild(objLabel);

                // Add an ID to the objective DIV
                obj.id = listItem.get_id();

                // Add an class to the objective DIV   
                obj.className = "itemObj";



                // Add an onclick event to show the objective details
                $(obj).click(function (sender) {
                    showObjDetails(sender.target.id);

                });

                // Add the objective div to the UI
                objTable.appendChild(obj);
                hasObjs = true;
            }
            if (activeObjCount == 0) {               
                document.getElementById("editStatus").disabled = false;
            }
            else {
                document.getElementById("editStatus").disabled = true;
            }
            if (!hasObjs) {
                var noObjs = document.createElement("div");
                noObjs.className = "errorObj";
                noObjs.appendChild(document.createTextNode("There are no objectives for this review."));
                objTable.appendChild(noObjs);
                document.getElementById("editStatus").disabled = true;
            }
            $('#objDetails').fadeIn(500, null);
            if (reviewStatus == "Completed") {
                $('#newObjective').hide();
            }
            else {
                $('#newObjective').show();
            }
           

        },
        function (sender, args) {

            // Failure returned from executeQueryAsync.
            var divMessage = document.createElement("DIV");
            divMessage.setAttribute("style", "padding:5px;");
            divMessage.appendChild(document.createTextNode("Failed to get objectives. Error: " + args.get_message()));
            errArea.appendChild(divMessage);
            $('#objDetails').fadeIn(500, null);
        });
}

// This function shows the details for a specific Objective
function showObjDetails(objID) {
    var errArea = document.getElementById("errObj");
    // Remove all nodes from the error <DIV> so we have a clean space to write to
    while (errArea.hasChildNodes()) {
        errArea.removeChild(errArea.lastChild);
    }
    
    objItem = objList.getItemById(objID);
    context.load(objItem);
    context.executeQueryAsync(
        function () {
            $('#editObjName').val(objItem.get_fieldValues()["ObjectiveName"]);
            $('#editObjPriority').val(objItem.get_fieldValues()["Priority"]);
            $('#editObjStatus').val(objItem.get_fieldValues()["_Status"]);
            $('#editEmpComments').val(objItem.get_fieldValues()["EmployeeComments"]);
            // Close button is hidden using ".ui-dialog-titlebar-close" class in the jquery-ui-1.10.2.custom.css  
            $('#editObjDialog').dialog(
         {
             height: "auto",
             width: "auto",
             modal: true,
            
             show: {
                 effect: "Scale",
                 duration: 1000
             },
             hide: {
                 effect: "Explode",
                 duration: 1000
             }

         });
        },
        function (sender, args) {
            var errArea = document.getElementById("errObj");
            // Remove all nodes from the error <DIV> so we have a clean space to write to
            while (errArea.hasChildNodes()) {
                errArea.removeChild(errArea.lastChild);
            }
            var divMessage = document.createElement("DIV");
            divMessage.setAttribute("style", "padding:5px;");
            divMessage.appendChild(document.createTextNode(args.get_message()));
            errArea.appendChild(divMessage);
        });
}

// This function updates an existing review's details
function saveEditReview() {
    $('#selectedStatus').val("");
    currentItem.set_item("_Comments", $('#editComments').val());
    currentItem.set_item("_Status", $('#editStatus').val());
    currentItem.set_item("StartDate", $('#editStartDate').val());
    currentItem.set_item("_EndDate", $('#editEndDate').val());
 
    currentItem.update();
    context.load(currentItem);
    context.executeQueryAsync(function () {
        clearEditReviewForm();
        showEmployeeDetails(empID);
    },
        function (sender, args) {
            var errArea = document.getElementById("errReview");
            // Remove all nodes from the error <DIV> so we have a clean space to write to
            while (errArea.hasChildNodes()) {
                errArea.removeChild(errArea.lastChild);
            }
            var divMessage = document.createElement("DIV");
            divMessage.setAttribute("style", "padding:5px;");
            divMessage.appendChild(document.createTextNode(args.get_message()));
            errArea.appendChild(divMessage);
        });
}

// This function updates an existing objective's details
function saveEditObj() {
    objItem.set_item("ObjectiveName", $('#editObjName').val());
    objItem.set_item("Priority", $('#editObjPriority').val());
    objItem.set_item("_Status", $('#editObjStatus').val());
    objItem.update();
    context.load(objItem);
    context.executeQueryAsync(function () {
        clearNewObjForm();
        cancelEditObjective();
        showObjectives(revID);
    },
        function (sender, args) {
            var errArea = document.getElementById("errObj");
            // Remove all nodes from the error <DIV> so we have a clean space to write to
            while (errArea.hasChildNodes()) {
                errArea.removeChild(errArea.lastChild);
            }
            var divMessage = document.createElement("DIV");
            divMessage.setAttribute("style", "padding:5px;");
            divMessage.appendChild(document.createTextNode(args.get_message()));
            errArea.appendChild(divMessage);
        });
}

//This function retrives the employee ID of the logged in employee
function matchUser(userName) {
    empList = web.get_lists().getByTitle('Employee');
    var camlQuery = SP.CamlQuery.createAllItemsQuery();
    var listItems = empList.getItems(camlQuery);
    var employeeID;
    context.load(listItems);
    context.executeQueryAsync(
        function () {
            var listItemEnumerator = listItems.getEnumerator();
            while (listItemEnumerator.moveNext()) {
                var listItem = listItemEnumerator.get_current();
                var employeeLabel = listItem.get_fieldValues()["Title"];
                if (employeeLabel == userName) {
                    employeeID = listItem.get_fieldValues()["ID"];
                    matchReviews(employeeID);
                }
            }
        },
        function (sender, args) {
            var errArea = document.getElementById("errAllObjectives");
            // Remove all nodes from the error <DIV> so we have a clean space to write to
            while (errArea.hasChildNodes()) {
                errArea.removeChild(errArea.lastChild);
            }
            var divMessage = document.createElement("DIV");
            divMessage.setAttribute("style", "padding:5px;");
            divMessage.appendChild(document.createTextNode(args.get_message()));
            errArea.appendChild(divMessage);
        });
}

//This function retrieves the active review ID from Review List
function matchReviews(employeeID) {
    var hasRev = false;
    var rList = web.get_lists().getByTitle('Review');
    var camlQuery = new SP.CamlQuery();
    camlQuery.set_viewXml("<View><Query><Where><Eq><FieldRef Name='EmployeeName' LookupId='TRUE' /><Value Type='Lookup'>"
        + employeeID
        + "</Value></Eq></Where></Query></View>");
    var listItems = rList.getItems(camlQuery);
    context.load(listItems);
    context.executeQueryAsync(
        function () {
            var revTable = document.getElementById("EmpRevList");

            // Remove all nodes from the Objective <DIV> so we have a clean space to write to
            while (revTable.hasChildNodes()) {
                revTable.removeChild(objTable.lastChild);
            }
            var listItemEnumerator = listItems.getEnumerator();
            while (listItemEnumerator.moveNext()) {
                var listItem = listItemEnumerator.get_current();
                var revStatus = listItem.get_fieldValues()["_Status"];
                if (revStatus == "Active") {
                    var rID = listItem.get_fieldValues()["ID"];
                    matchObjs(rID);
                    hasRev = true;
                }
                
                
            }
            if (!hasRev) {
                var noObj = document.createElement("div");
                noObj.appendChild(document.createTextNode("There are no Reviews."));
                revTable.appendChild(noObj);
            }
            $('#AllObjectives').fadeIn(500, null);
        },function (sender, args) {

            // Failure returned from executeQueryAsync
            var divMessage = document.createElement("DIV");
            divMessage.setAttribute("style", "padding:5px;");
            divMessage.appendChild(document.createTextNode("Failed to get Reviews. Error: " + args.get_message()));
            errArea.appendChild(divMessage);
            $('#ObjectiveList').fadeIn(500, null);
        });
    
}

//This function shows the objectives for the specific review 
function matchObjs(rID) {
    var hasObjs = false;
    oList = web.get_lists().getByTitle('Objective');
    var camlQuery = new SP.CamlQuery();
    camlQuery.set_viewXml("<View><Query><Where><Eq><FieldRef Name='ReviewLookup' LookupId='TRUE' /><Value Type='Lookup'>"
        + rID
        + "</Value></Eq></Where></Query></View>");
    var listItems = oList.getItems(camlQuery);
    context.load(listItems);
    context.executeQueryAsync(
        function () {
            var objTable = document.getElementById("EmpObjList");

            // Remove all nodes from the Objective <DIV> so we have a clean space to write to
            while (objTable.hasChildNodes()) {
                objTable.removeChild(objTable.lastChild);
            }
            // Iterate through the objective list.
            var listItemEnumerator = listItems.getEnumerator();
            while (listItemEnumerator.moveNext()) {
                var listItem = listItemEnumerator.get_current();
                // Create a DIV to display the Objective Name
                var obj = document.createElement("div");
                var objLabel = document.createTextNode(listItem.get_fieldValues()["ObjectiveName"]);

                var objStatus = listItem.get_fieldValues()["_Status"];

                obj.appendChild(objLabel);
                // Add an ID to the objective DIV
                obj.id = listItem.get_id();
                // Add an class to the objective DIV   
                obj.className = "item";
                // Add an onclick event to show the objective details
                $(obj).click(function (sender) {
                    matchObjDetails(sender.target.id);

                });

                // Add the objective div to the UI
                objTable.appendChild(obj);
                hasObjs = true;
            }
            if (!hasObjs) {
                var noObj = document.createElement("div");
                noObj.appendChild(document.createTextNode("There are no objectives."));
                objTable.appendChild(noObj);
            }


            $('#AllObjectives').fadeIn(500, null);
        },
        function (sender, args) {

            // Failure returned from executeQueryAsync
            var divMessage = document.createElement("DIV");
            divMessage.setAttribute("style", "padding:5px;");
            divMessage.appendChild(document.createTextNode("Failed to get Objectives. Error: " + args.get_message()));
            errArea.appendChild(divMessage);
            $('#ObjectiveList').fadeIn(500, null);
        });
}

//This function displays the objective details
function matchObjDetails(oID) {

    oItem = oList.getItemById(oID);
    context.load(oItem);
    context.executeQueryAsync(
        function () {
            $('#objName').val(oItem.get_fieldValues()["ObjectiveName"]);
            $('#objPriority').val(oItem.get_fieldValues()["Priority"]);
            $('#objStatus').val(oItem.get_fieldValues()["_Status"]);
            $('#empComments').val(oItem.get_fieldValues()["EmployeeComments"]);
            $('#ObjectiveDetails').slideDown(500,null);
        },
        function (sender, args) {
            var errArea = document.getElementById("errAllObjective");
            // Remove all nodes from the error <DIV> so we have a clean space to write to
            while (errArea.hasChildNodes()) {
                errArea.removeChild(errArea.lastChild);
            }
            var divMessage = document.createElement("DIV");
            divMessage.setAttribute("style", "padding:5px;");
            divMessage.appendChild(document.createTextNode(args.get_message()));
            errArea.appendChild(divMessage);
        });
}

// This function saves the employee comments
function saveEmpComments() {
    oItem.set_item("EmployeeComments", $('#empComments').val());
    oItem.update();
    context.load(oItem);
    context.executeQueryAsync(function () {
          clearEmpObjForm();
    },
        function (sender, args) {
            var errArea = document.getElementById("errAllObjectives");
            // Remove all nodes from the error <DIV> so we have a clean space to write to
            while (errArea.hasChildNodes()) {
                errArea.removeChild(errArea.lastChild);
            }
            var divMessage = document.createElement("DIV");
            divMessage.setAttribute("style", "padding:5px;");
            divMessage.appendChild(document.createTextNode(args.get_message()));
            errArea.appendChild(divMessage);
        });
}

// This function clears the inputs on the objective form for particular employee
function clearEmpObjForm() {
    var errArea = document.getElementById("errAllObjectives");
    // Remove all nodes from the error <DIV> so we have a clean space to write to
    while (errArea.hasChildNodes()) {
        errArea.removeChild(errArea.lastChild);
    }
    $('#ObjectiveDetails').fadeOut(500, function () {
        $('#ObjectiveDetails').hide();
        $('#empComments').val("");
    });
}

// This function cancels the editing of an existing objective's details for particular employee
function cancelEmpObj() {
    clearEmpObjForm();
}

// This function deletes the selected objective
function deleteEditObj() {
    var errArea = document.getElementById("errObj");
    // Remove all nodes from the error <DIV> so we have a clean space to write to
    while (errArea.hasChildNodes()) {
        errArea.removeChild(errArea.lastChild);
    }
    objItem.deleteObject();
    context.executeQueryAsync(
    function () {
        clearEditObjForm();
        cancelEditObjective();
        showObjectives(revID);
    },
    function (sender, args) {
        var divMessage = document.createElement("DIV");
        divMessage.setAttribute("style", "padding:5px;");
        divMessage.appendChild(document.createTextNode(args.get_message()));
        errArea.appendChild(divMessage);
    });
}