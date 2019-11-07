
// Variable that will hold the SharePoint ClientContext object
var context;

// Variable that will hold the SharePoint App Web object
var web;

// Variable that will hold the SharePoint user
var user;

// Variable that will hold various SharePoint List objects 
var list;
var positionList;
var interviewList;

// Variable that will hold interviewer name
var interviewerName;

// Variables that will hold number values
var jobsOffered;
var totalVacancies;

// Variable that will hold various SharePoint ListItem objects
var currentItem;
var interviewItem;

// Variables that will hold various SharePoint ListItem IDs
var vacancyID;
var interviewID;

// This code runs when the DOM is ready and creates a context object which is needed to use the SharePoint object model
$(document).ready(function () {
    // Turn various text inputs into calendars by using jQuery UI methods
        $('#newInterviewDate').datetimepicker({
            showOn: "both",
            buttonImage: "../images/calendar.gif",
            buttonImageOnly: true,
            nextText: "",
            prevText: "",
            changeMonth: true,
            changeYear: true,
            dateFormat: "MM dd, yy",
            timeFormat: "hh:mm"
        });
    
        $('#editInterviewDate').datetimepicker({
            showOn: "both",
            buttonImage: "../images/calendar.gif",
            buttonImageOnly: true,
            nextText: "",
            prevText: "",
            changeMonth: true,
            changeYear: true,
            dateFormat: "MM dd, yy",
            timeFormat: "hh:mm"
        });

    var errArea = document.getElementById("errAllVacancies");

    // Remove all nodes from the errAllVacancies <DIV> so we have a clean space to write to in case of errors
    while (errArea.hasChildNodes()) {
        errArea.removeChild(errArea.lastChild);
    }

    // Reference and load the basic SharePoint objects needed to start with
    context = SP.ClientContext.get_current();
    web = context.get_web();
    user = web.get_currentUser();
    context.load(web);
    context.load(user);
    context.executeQueryAsync(function () {

        // Success returned from executeQueryAsync
        initializePeoplePicker('peoplePickerDiv');
        hideAllPanels();
        $('#AllVacancies').fadeIn(500, null);
        showVacancies();
    },
    function (sender, args) {

        // Failure returned from executeQueryAsync
        var divMessage = document.createElement("DIV");
        divMessage.setAttribute("style", "padding:5px;");
        divMessage.appendChild(document.createTextNode("Failed to get started. Error: " + args.get_message()));
        errArea.appendChild(divMessage);
        $('#AllVacancies').fadeIn(500, null);
    });
});


// This function creates job description document
function createJobDescription() {
    var printWindow = window.open("", "", "width=520");
    var pageHead = "<html><head><link rel='Stylesheet' type='text/css' href='../Content/App.css' /><script type='text/javascript'>function printMe(){window.print();}</script></head><body>";
    var printHead = "<div class='printBar' onclick='printMe();'>[Print]</div>";
    var docHead = "<div id='printArea'><div class='clear'>&nbsp;</div><div class='printLogo'>Point8020 Limited</div><div class='clear'>&nbsp;</div><div class='printTitle'>Job Description</div><div class='clear'>&nbsp;</div>";
    var docContent = "";
    if (currentItem.get_fieldValues()["Location"] != "") {
        docContent = "<div class='printSubtitle'>" + currentItem.get_fieldValues()["Title"] + " (" + currentItem.get_fieldValues()["Location"] + ")</div><div class='printBody'>";
    }
    else {
         docContent = "<div class='printSubtitle'>" + currentItem.get_fieldValues()["Title"] + "</div><div class='printBody'>";
    }

    if (currentItem.get_fieldValues()["JobDescription"] != "") {
        docContent += "<div class='printSubtitle'>Role</div><div class='printBody'>"
            + currentItem.get_fieldValues()["JobDescription"].replace(/\n/g, '<br/>')
        + "</div>";
    }
    if (currentItem.get_fieldValues()["Prerequisites"] != "") {
        docContent += "<div class='printSubtitle'>Skills & Experience</div><div class='printBody'>To be considered for this role, you will need:<br/><ul><li>"
            + currentItem.get_fieldValues()["Prerequisites"].replace(/\n/g, '</li><li>')
        + "</li></ul></div>";
    }
    docContent += "</div>";
    var docFoot = "<div class='clear'>&nbsp;</div><div class='printBody'>For more about this job, please contact " + user.get_email() + "</div></div></body></html>";
    printWindow.document.write(pageHead + printHead + docHead + docContent + docFoot);
    printWindow.document.close();
    printWindow.focus();
}


// This function creates job offer letter document
function createOfferLetter() {
    var printWindow = window.open("", "", "width=520");
    var pageHead = "<html><head><link rel='Stylesheet' type='text/css' href='../Content/App.css' /><script type='text/javascript'>function printMe(){window.print();}</script></head><body>";
    var printHead = "<div class='printBar' onclick='printMe();'>[Print]</div>";
    var docHead = "<div id='printArea'><div class='clear'>&nbsp;</div><div class='printLogo'>Point8020 Limited</div><div class='clear'>&nbsp;</div><div class='printTitle'>Job Offer</div><div class='clear'>&nbsp;</div>";
    var docContent = "<div class='printSubtitle'>RE: " + currentItem.get_fieldValues()["Title"] + "</div><div class='printBody'>";
    docContent += "<div class='printBody'>Dear " + interviewItem.get_fieldValues()["Interviewee"] + ",</div>";
    docContent += "<div class='printBody'>Thank you for attending an interview for the position of " + currentItem.get_fieldValues()["Title"] + " in " + interviewItem.get_fieldValues()["InterviewLocation"] + " on " + new Date(interviewItem.get_fieldValues()["DateTime"]).toDateString() + ".</div>";
    docContent += "<div class='printBody'>I am happy to inform you that  " + interviewItem.get_fieldValues()["Interviewer"] + " has decided you would be a good fit for the " + currentItem.get_fieldValues()["Title"] + " role, so please accept this letter as confirmation that we would like to offer you the position, subject to resume checks.</div>";
    if (currentItem.get_fieldValues()["Salary"] != "") {
        docContent += "<div class='printBody'>I would like to confirm that the starting annual salary for the role is $" + currentItem.get_fieldValues()["Salary"] + " (USD)."
        + "</div>";
    }
    if (currentItem.get_fieldValues()["Location"] != "") {
        docContent += "<div class='printBody'>I would also like to confirm that the position is based in our " + currentItem.get_fieldValues()["Location"] + " offices."
        + "</div>";
    }
    var docFoot = "<div class='printBody'>I would be grateful if you can confirm in writing whether you accept this position. If you have any questions please fee free to contact me at " + user.get_email() + "!</div><div class='printBody'>Yours faithfully,</div><div class='clear'>&nbsp;</div><div class='printBody'>" + user.get_title() + "</div></div></div></body></html>";
    printWindow.document.write(pageHead + printHead + docHead + docContent + docFoot);
    printWindow.document.close();
    printWindow.focus();
}

// This function hides all main DIV elements. The caller is then responsible 
// for re-showing the one that needs to be displayed.
function hideAllPanels() {
    $('#vacancyDetails').hide();
    $('#AddNewVacancy').hide();
}

// Render and initialize the client-side People Picker.
function initializePeoplePicker(peoplePickerElementId) {

    // Create a schema to store picker properties, and set the properties.
    var schema = {};
    schema['PrincipalAccountType'] = 'User,DL,SecGroup,SPGroup';
    schema['SearchPrincipalSource'] = 15;
    schema['ResolvePrincipalSource'] = 15;
    schema['AllowMultipleValues'] = false;
    schema['MaximumEntitySuggestions'] = 50;
    schema['Width'] = '300px';

    // Render and initialize the picker. 
    // Pass the ID of the DOM element that contains the picker, an array of initial
    // PickerEntity objects to set the picker value, and a schema that defines
    // picker properties.
    this.SPClientPeoplePicker_InitStandaloneControlWrapper(peoplePickerElementId, null, schema);
}

// This function shows new job vacancy form
function addNewVacancy() {
    hideAllPanels();
    $('#AddNewVacancy').fadeIn(500, null);
}

// This function saves the newly-entered vacancy
function saveNewVacancy() {
    if (($('#newJobTitle').val() == "")||($('#newNoofVacancies').val()=="")) {
        var errArea = document.getElementById("errAllVacancies");
        // Remove all nodes from the errAllVacancies <DIV> so we have a clean space to write to
        while (errArea.hasChildNodes()) {
            errArea.removeChild(errArea.lastChild);
        }
        var divMessage = document.createElement("DIV");
        divMessage.setAttribute("style", "padding:5px;");
        divMessage.appendChild(document.createTextNode("'Job Title' or 'Number of Vacancy' field is required."));
        errArea.appendChild(divMessage);
    }
    
    else {
        positionList = web.get_lists().getByTitle('Position');
        var itemCreateInfo = new SP.ListItemCreationInformation();       
        var positionItem = positionList.addItem(itemCreateInfo);

        positionItem.set_item("Title", $('#newJobTitle').val());
        positionItem.set_item("NumberofVacancies", $('#newNoofVacancies').val());
        positionItem.set_item("JobDescription", $('#newJobDescription').text());
        positionItem.set_item("Prerequisites", $('#newPrerequisites').text());
        positionItem.set_item("Location", $('#newLocation').val());
        positionItem.set_item("WorkHours", $('#newWorkHours').val());
        positionItem.set_item("Salary", $('#newSalary').val());

        positionItem.update();
        context.load(positionItem);
        context.executeQueryAsync(function () {
            // Success returned from executeQueryAsync
            cancelNewVacancy();
            showVacancies();
        },
            function (sender, args) {
                // Failure returned from executeQueryAsync
                var errArea = document.getElementById("errAllVacancies");
                // Remove all nodes from the errAllVacancies <DIV> so we have a clean space to write to
                while (errArea.hasChildNodes()) {
                    errArea.removeChild(errArea.lastChild);
                }
                var clear = document.createElement("DIV");
                clear.className = "clear";
                var divMessage = document.createElement("DIV");
                divMessage.setAttribute("style", "padding:5px;");
                if ($('#newNoofVacancies').val() != "") {
                    var noOfVacancies = parseInt($('#newNoofVacancies').val());
                    if (isNaN(noOfVacancies)) {
                        divMessage.appendChild(document.createTextNode("Invalid 'Number of Vacancies' field. "));
                        divMessage.appendChild(clear);
                        errArea.appendChild(divMessage);
                    }
                }
                if ($('#newWorkHours').val() != "") {
                    var workHours = parseInt($('#newWorkHours').val());
                    if (isNaN(workHours)) {
                        divMessage.appendChild(document.createTextNode("Invalid 'Work Hours' field. "));
                        divMessage.appendChild(clear);
                        errArea.appendChild(divMessage);
                    }
                }
                if ($('#newSalary').val() != "") {
                    var salary = parseInt($('#newSalary').val());
                    if (isNaN(salary)) {
                        divMessage.appendChild(document.createTextNode("Invalid 'Salary' field. "));
                        divMessage.appendChild(clear);
                        errArea.appendChild(divMessage);
                    }
                }
            });
    }
}

// This function cancels the creation of a vacancy
function cancelNewVacancy() {
    $('#AddNewVacancy').hide();
    $('#newJobTitle').val("");
    $('#newNoofVacancies').val("");
    $('#newJobDescription').text("");
    $('#newPrerequisites').text("");
    $('#newLocation').val("");
    $('#newWorkHours').val("");
    $('#newSalary').val("");
    showVacancies();
}

// This function retrieves all vacancies
function showVacancies() {
    var errArea = document.getElementById("errAllVacancies");

    // Remove all nodes from the errAllVacancies <DIV> so we have a clean space to write to in case of errors
    while (errArea.hasChildNodes()) {
        errArea.removeChild(errArea.lastChild);
    }
    var hasVacancies = false;
    hideAllPanels();
    positionList = web.get_lists().getByTitle('Position');
    var camlQuery = SP.CamlQuery.createAllItemsQuery();
    var listItems = positionList.getItems(camlQuery);
    context.load(listItems);
    context.executeQueryAsync(
        function () {
            // Success returned from executeQueryAsync
            var positionTable = document.getElementById("vacancyList");

            // Remove all nodes from the vacancyList <DIV> so we have a clean space to write to
            while (positionTable.hasChildNodes()) {
                positionTable.removeChild(positionTable.lastChild);
            }

            // Iterate through the Position list
            var listItemEnumerator = listItems.getEnumerator();
            while (listItemEnumerator.moveNext()) {
                var listItem = listItemEnumerator.get_current();

                // Create a DIV to display the Position name
                var position = document.createElement("div");
                var positionLabel = document.createTextNode(listItem.get_fieldValues()["Title"]);
                position.appendChild(positionLabel);

                // Add an ID to the Position DIV
                position.id = listItem.get_id();

                // Add an class to the Position DIV
                position.className = "item";

                // Add an onclick event to show the Position details
                $(position).click(function (sender) {
                    showVacancyDetails(sender.target.id);
                });

                // Add the Position div to the UI
                positionTable.appendChild(position);
                hasVacancies = true;
            }
            if (!hasVacancies) {
                var noVacancies = document.createElement("div");
                noVacancies.appendChild(document.createTextNode("There are no job vacancies."));
                positionTable.appendChild(noVacancies);
            }
            $('#AllVacancies').fadeIn(500, null);
        },
        function (sender, args) {

            // Failure returned from executeQueryAsync
            var divMessage = document.createElement("DIV");
            divMessage.setAttribute("style", "padding:5px;");
            divMessage.appendChild(document.createTextNode("Failed to get job vacancies. Error: " + args.get_message()));
            errArea.appendChild(divMessage);
            $('#AllVacancies').fadeIn(500, null);
        });
}

// This function shows the details for a specific Vacancy
function showVacancyDetails(itemID) {
    hideAllPanels();
    totalVacancies = 0;
    var errArea = document.getElementById("errAllVacancies");

    // Remove all nodes from the errAllVacancies <DIV> so we have a clean space to write to
    while (errArea.hasChildNodes()) {
        errArea.removeChild(errArea.lastChild);
    }

    currentItem = positionList.getItemById(itemID);
    context.load(currentItem);
    context.executeQueryAsync(
        function () {
            // Success returned from executeQueryAsync
            $('#editJobTitle').val(currentItem.get_fieldValues()["Title"]);
            $('#editNoofVacancies').val(currentItem.get_fieldValues()["NumberofVacancies"]);
            $('#editJobDescription').val(currentItem.get_fieldValues()["JobDescription"]);
            $('#editPrerequisites').val(currentItem.get_fieldValues()["Prerequisites"]);
            $('#editLocation').val(currentItem.get_fieldValues()["Location"]);
            $('#editWorkHours').val(currentItem.get_fieldValues()["WorkHours"]);
            $('#editSalary').val(currentItem.get_fieldValues()["Salary"]);
            $('#vacancyDetails').fadeIn(500, null);
            totalVacancies = currentItem.get_fieldValues()["NumberofVacancies"];
            vacancyID = currentItem.get_id();
            showInterviews();
        },
        function (sender, args) {
            //Failure returned from executeQueryAsync
            var divMessage = document.createElement("DIV");
            divMessage.setAttribute("style", "padding:5px;");
            divMessage.appendChild(document.createTextNode(args.get_message()));
            errArea.appendChild(divMessage);
        });
}

// This function cancels the editing of an existing vacancy's details
function cancelEditVacancy() {
    $('#vacancyDetails').hide();
    $('#editJobTitle').val("");
    $('#editNoofVacancies').val("");
    $('#editJobDescription').text("");
    $('#editPrerequisites').text("");
    $('#editLocation').val("");
    $('#editWorkHours').val("");
    $('#editSalary').val("");
    $('#vacancyDetails').val("");
    showVacancies();
}

// This function updates an existing vacancy's details
function saveEditVacancy() {
    if (($('#editJobTitle').val() == "") || $('#editNoofVacancies').val()=="") {
        var errArea = document.getElementById("errAllVacancies");
        // Remove all nodes from the errAllVacancies <DIV> so we have a clean space to write to
        while (errArea.hasChildNodes()) {
            errArea.removeChild(errArea.lastChild);
        }
        var divMessage = document.createElement("DIV");
        divMessage.setAttribute("style", "padding:5px;");
        divMessage.appendChild(document.createTextNode("'Job Title' or 'Number of Vacancy' field is required."));
        errArea.appendChild(divMessage);
    }
    else {
        currentItem.set_item("Title", $('#editJobTitle').val());
        currentItem.set_item("NumberofVacancies", $('#editNoofVacancies').val());
        currentItem.set_item("JobDescription", $('#editJobDescription').text());
        currentItem.set_item("Prerequisites", $('#editPrerequisites').text());
        currentItem.set_item("Location", $('#editLocation').val());
        currentItem.set_item("WorkHours", $('#editWorkHours').val());
        currentItem.set_item("Salary", $('#editSalary').val());
        currentItem.update();
        context.load(currentItem);
        context.executeQueryAsync(function () {
            //Success returned from executeQueryAsync
            cancelEditVacancy();
            showVacancies();
        },
            function (sender, args) {
                //Failure returned from executeQueryAsync
                var errArea = document.getElementById("errAllVacancies");
                // Remove all nodes from the errAllVacancies <DIV> so we have a clean space to write to
                while (errArea.hasChildNodes()) {
                    errArea.removeChild(errArea.lastChild);
                }
                var clear = document.createElement("DIV");
                clear.className = "clear";
                var divMessage = document.createElement("DIV");
                divMessage.setAttribute("style", "padding:5px;");
                if ($('#editNoofVacancies').val() != "") {
                    var noOfVacancies = parseInt($('#editNoofVacancies').val());
                    if (isNaN(noOfVacancies)) {
                        divMessage.appendChild(document.createTextNode("Invalid 'Number of Vacancies' field. "));
                        divMessage.appendChild(clear);
                        errArea.appendChild(divMessage);
                    }
                }
                if ($('#editWorkHours').val() != "") {
                    var workHours = parseInt($('#editWorkHours').val());
                    if (isNaN(workHours)) {
                        divMessage.appendChild(document.createTextNode("Invalid 'Work Hours' field. "));
                        divMessage.appendChild(clear);
                        errArea.appendChild(divMessage);
                    }
                }
                if ($('#editSalary').val() != "") {
                    var salary = parseInt($('#editSalary').val());
                    if (isNaN(salary)) {
                        divMessage.appendChild(document.createTextNode("Invalid 'Salary' field. "));
                        divMessage.appendChild(clear);
                        errArea.appendChild(divMessage);
                    }
                }
            });
    }
}

// This function deletes the selected vacancy
function deleteVacancy() {
    var errArea = document.getElementById("errAllVacancies");
    // Remove all nodes from the errAllVacancies <DIV> so we have a clean space to write to
    while (errArea.hasChildNodes()) {
        errArea.removeChild(errArea.lastChild);
    }
    context.executeQueryAsync(
    function () {
        // Success returned from executeQueryAsync
        getVacancyInterviews(currentItem.get_id());
        currentItem.deleteObject();
        cancelEditVacancy();
        showVacancies();
    },
    function (sender, args) {
        // Failure returned from executeQueryAsync
        var divMessage = document.createElement("DIV");
        divMessage.setAttribute("style", "padding:5px;");
        divMessage.appendChild(document.createTextNode(args.get_message()));
        errArea.appendChild(divMessage);
    });
}

// This function retrives all interviews respective to deleted vacancy
function getVacancyInterviews(vacID) {
    interviewList = web.get_lists().getByTitle('InterviewDetails');
    var interviewQuery = new SP.CamlQuery();
    interviewQuery.set_viewXml("<View><Query><Where><Eq><FieldRef Name='PositionLookup' LookupId='TRUE' /><Value Type='Lookup'>"
        + vacID
        + "</Value></Eq></Where></Query></View>");
    var listItems = interviewList.getItems(interviewQuery);
    context.load(listItems);
    context.executeQueryAsync(
        function () {
            // Success returned from executeQueryAsync

            // Iterate through the InterviewDetails list
            var listItemEnumerator = listItems.getEnumerator();
            while (listItemEnumerator.moveNext()) {
                var listItem = listItemEnumerator.get_current();
                deleteVacancyInterview(listItem.get_id());
            }
        },
        function (sender, args) {
            // Failure returned from executeQueryAsync
            alert("Error in retrieving interviews for deletion:" + args.get_message());
        });
}

// This function deletes an interview respective to deleted vacancy 
function deleteVacancyInterview(itemID) {
    var deleteInterviewitem = interviewList.getItemById(itemID);
    deleteInterviewitem.deleteObject();
    context.executeQueryAsync(
        function () {
            // Success returned from executeQueryAsync
        },
        function (sender, args) {
            // Failure returned from executeQueryAsync
            alert("Error in deleting interviews :" + args.get_message());
        });
}

// This function shows add interview dialog
function sheduleInterview() {
    var errArea = document.getElementById("errorAddInterview");
    // Remove all nodes from the errEditInterView <DIV> so we have a clean space to write to
    while (errArea.hasChildNodes()) {
        errArea.removeChild(errArea.lastChild);
    }
    if (totalVacancies > jobsOffered) {
        $('#addInterView').dialog(
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
    else {
        alert("Vacancies full");
    }
}

// This function cancels the creation of a interview
function cancelNewInterview() {
    $('#addInterView').dialog("close");
    $('#newInterviewDate').val("");
    $('#newInterviewLoc').val("");
    initializePeoplePicker('peoplePickerDiv');
    $('#newInterviewee').val("");
}

// This function saves the newly-entered interview
function saveNewInterview() {
    interviewerName = "";
    if ($('#newInterviewee').val() == "") {
        var errArea = document.getElementById("errorAddInterview");
        // Remove all nodes from the errorAddInterview <DIV> so we have a clean space to write to
        while (errArea.hasChildNodes()) {
            errArea.removeChild(errArea.lastChild);
        }
        var divMessage = document.createElement("DIV");
        divMessage.setAttribute("style", "padding:5px;");
        divMessage.appendChild(document.createTextNode("'Interviewee name' field is required."));
        errArea.appendChild(divMessage);
    }
    else {
        interviewList = web.get_lists().getByTitle('InterviewDetails');

        var itemCreateInfo = new SP.ListItemCreationInformation();
        var newInterviewItem = interviewList.addItem(itemCreateInfo);

        var peoplePicker = this.SPClientPeoplePicker.SPClientPeoplePickerDict.peoplePickerDiv_TopSpan;

        // Get information about interviewer
        var users = peoplePicker.GetAllUserInfo();
        if (users.length > 1) {
            alert("Only one interviewer is allowed.");
        }
        else if (users.length < 1) {
            alert("Interviewer field is required.");
        }
        else {
            for (var i = 0; i < 1; i++) {
                user = users[0];
                interviewerName = user["DisplayText"];
            }

            newInterviewItem.set_item("PositionLookup", vacancyID);
            newInterviewItem.set_item("DateTime", $('#newInterviewDate').val());
            newInterviewItem.set_item("InterviewLocation", $('#newInterviewLoc').val());
            newInterviewItem.set_item("Interviewer", interviewerName);
            newInterviewItem.set_item("Interviewee", $('#newInterviewee').val());
            newInterviewItem.set_item("JobOffered", "Job Not Offered");
            newInterviewItem.update();
            context.load(newInterviewItem);
            context.executeQueryAsync(function () {
                // Success returned from executeQueryAsync
                cancelNewInterview();
                showInterviews();
            },
                function (sender, args) {
                    // Failure returned from executeQueryAsync
                    var errArea = document.getElementById("errorAddInterview");
                    // Remove all nodes from the errorAddInterview <DIV> so we have a clean space to write to
                    while (errArea.hasChildNodes()) {
                        errArea.removeChild(errArea.lastChild);
                    }
                    var divMessage = document.createElement("DIV");
                    divMessage.setAttribute("style", "padding:5px;");
                    divMessage.appendChild(document.createTextNode(args.get_message()));
                    errArea.appendChild(divMessage);
                });
        }
    }
}

// This function retrieves all interviews
function showInterviews() {
    var errArea = document.getElementById("errorInterview");
    jobsOffered = 0;   
    // Remove all nodes from the errorInterview <DIV> so we have a clean space to write to in case of errors
    while (errArea.hasChildNodes()) {
        errArea.removeChild(errArea.lastChild);
    }

    var hasInterviews = false;
    var hasjobOffered = false;
    interviewList = web.get_lists().getByTitle('InterviewDetails');
    var camlQuery = SP.CamlQuery.createAllItemsQuery();
    var listItems = interviewList.getItems(camlQuery);
    context.load(listItems);
    context.executeQueryAsync(
        function () {
            // Success returned from executeQueryAsync
            var interviewTable = document.getElementById("interviewList");
            var jobOfferedTable = document.getElementById("jobOfferedList");

            // Remove all nodes from the interviewList <DIV> so we have a clean space to write to
            while (interviewTable.hasChildNodes()) {
                interviewTable.removeChild(interviewTable.lastChild);
            }
            // Remove all nodes from the jobOfferedList <DIV> so we have a clean space to write to
            while (jobOfferedTable.hasChildNodes()) {
                jobOfferedTable.removeChild(jobOfferedTable.lastChild);
            }

            // Iterate through the InterviewDetails list
            var listItemEnumerator = listItems.getEnumerator();
            while (listItemEnumerator.moveNext()) {
                var listItem = listItemEnumerator.get_current();
                var positionLookup = listItem.get_fieldValues()["PositionLookup"].get_lookupValue();
                
                if (positionLookup == vacancyID) {
                   
                    
                    // Create a DIV to display the Interviewee name
                    var interview = document.createElement("div");
                    var interviewLabel = document.createTextNode(listItem.get_fieldValues()["Interviewee"]);
                    interview.appendChild(interviewLabel);

                    // Add an ID to the interview DIV
                    interview.id = listItem.get_id();

                    // Add an class to the interview DIV
                    interview.className = "interviewItem";

                    // Add an onclick event to show the interview details
                    $(interview).click(function (sender) {
                        showInterviewDetails(sender.target.id);
                    });

                    // Add the interview div to the UI
                    if (listItem.get_fieldValues()["JobOffered"] == "Job Offered") {
                        jobsOffered = jobsOffered + 1;
                        jobOfferedTable.appendChild(interview);
                         hasjobOffered= true;
                    }
                    else {
                        
                        interviewTable.appendChild(interview);
                        hasInterviews = true;
                    }
                    
                }
            }
            if (!hasInterviews) {
                var noInterviews = document.createElement("div");
                noInterviews.className = "formLabel";
                noInterviews.appendChild(document.createTextNode("There are no interviews scheduled."));
                interviewTable.appendChild(noInterviews);
            }
            if (!hasjobOffered) {
                var nojobOffered = document.createElement("div");
                nojobOffered.className = "formLabel";
                nojobOffered.appendChild(document.createTextNode("No job offers have been made."));
                jobOfferedTable.appendChild(nojobOffered);
            }

        },
        function (sender, args) {
            // Failure returned from executeQueryAsync
            var divMessage = document.createElement("DIV");
            divMessage.setAttribute("style", "padding:5px;");
            divMessage.appendChild(document.createTextNode("Failed to get interviews. Error: " + args.get_message()));
            errArea.appendChild(divMessage);
        });
}

// This function shows the details for a specific interview
function showInterviewDetails(itemID) {
    $('#editInterView').dialog(
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
    initializePeoplePicker('editPeoplePickerDiv');
    var errArea = document.getElementById("errEditInterView");
    // Remove all nodes from the errEditInterView <DIV> so we have a clean space to write to
    while (errArea.hasChildNodes()) {
        errArea.removeChild(errArea.lastChild);
    }
    interviewItem = interviewList.getItemById(itemID);
    context.load(interviewItem);
    context.executeQueryAsync(
        function () {
            // Success returned from executeQueryAsync
            interviewID = itemID;
            $('#editInterviewLoc').val(interviewItem.get_fieldValues()["InterviewLocation"]);
            $('#editInterviewDate').val(new Date(interviewItem.get_fieldValues()["DateTime"]).format("MM dd, yy hh:mm"));
            $('#editInterviewee').val(interviewItem.get_fieldValues()["Interviewee"]);
            $('#editComments').val(interviewItem.get_fieldValues()["_Comments"]);
            
            $('#showInterviewers').text(interviewItem.get_fieldValues()["Interviewer"]);
            if (interviewItem.get_fieldValues()["JobOffered"] == "Job Offered") {
                document.getElementById('jobOffered').checked = true;
                $('#createOfferLetter').show();
                $('#saveEditInterview').hide();
                $('#deleteInterview').show();
                $('#changeInterviewer').hide();
            }
            else {
                document.getElementById('jobOffered').checked = false;
                $('#createOfferLetter').hide();
                $('#saveEditInterview').show();
                $('#deleteInterview').show();
                $('#changeInterviewer').show();
            }
        },
        function (sender, args) {
            //Failure returned from executeQueryAsync
            var divMessage = document.createElement("DIV");
            divMessage.setAttribute("style", "padding:5px;");
            divMessage.appendChild(document.createTextNode(args.get_message()));
            errArea.appendChild(divMessage);
        });
}

// This function updates an existing interview's details
function saveEditInterview() {
    if ($('#editInterviewee').val() == "") {
        var errArea = document.getElementById("errEditInterView");
        // Remove all nodes from the errEditInterView <DIV> so we have a clean space to write to
        while (errArea.hasChildNodes()) {
            errArea.removeChild(errArea.lastChild);
        }
        var divMessage = document.createElement("DIV");
        divMessage.setAttribute("style", "padding:5px;");
        divMessage.appendChild(document.createTextNode("'Interviewee Name' field is required."));
        errArea.appendChild(divMessage);
    }
    else {
        var vacancyList = web.get_lists().getByTitle('Position');
        
        interviewItem.set_item("InterviewLocation", $('#editInterviewLoc').val());
        interviewItem.set_item("DateTime", $('#editInterviewDate').val());
        interviewItem.set_item("Interviewee", $('#editInterviewee').val());
        interviewItem.set_item("_Comments", $('#editComments').text());
        
        if ($('#jobOffered').is(':checked')) {
            interviewItem.set_item("JobOffered", "Job Offered");
        }
        else {
            interviewItem.set_item("JobOffered", "Job Not Offered");
        }
        interviewItem.update();
        context.load(interviewItem);
        context.executeQueryAsync(function () {
            //Success returned from executeQueryAsync
            cancelEditInterview();
            showInterviews();
        },
            function (sender, args) {
                //Failure returned from executeQueryAsync
                var errArea = document.getElementById("errEditInterView");
                // Remove all nodes from the errEditInterView <DIV> so we have a clean space to write to
                while (errArea.hasChildNodes()) {
                    errArea.removeChild(errArea.lastChild);
                }
                var divMessage = document.createElement("DIV");
                divMessage.setAttribute("style", "padding:5px;");
                divMessage.appendChild(document.createTextNode(args.get_message()));
                errArea.appendChild(divMessage);
            });
    }
}

//This function shows the new Attendee Dialog Box
function changeInterviewer() {
    $('#editInterviewer').dialog({
        height: "auto",
        width: "600px",
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
    initializePeoplePicker('editPeoplePickerDiv');
}

// This function Saves the Attendee in edit offsite form
function saveInterviewer() {
    var peoplePicker = this.SPClientPeoplePicker.SPClientPeoplePickerDict.editPeoplePickerDiv_TopSpan;
    // Get information about all users.
    var users = peoplePicker.GetAllUserInfo();
    if (users.length > 1) {
        alert("Enter only one Interviewer name.");
    }
    else if (users.length < 1) {
        alert("Interviewer name is mandatory.");
    }
    else {
        for (var i = 0; i < 1; i++) {
            user = users[i];
            interviewerName = user["DisplayText"];
        }
        interviewItem.set_item("Interviewer", interviewerName);
        interviewItem.update();
        context.load(interviewItem);
        context.executeQueryAsync(
            function () {
                // Success returned from executeQueryAsync
            $('#showInterviewers').text(interviewItem.get_fieldValues()["Interviewer"]);
            cancelInterviewer();
            },
        function (sender, args) {
            // Failure returned from executeQueryAsync
            alert("Failed to save interviewer. Error: " + args.get_message());
        });
    }
    
}

//This function closes the new Interviewer Dialog box
function cancelInterviewer() {
    $('#editInterviewer').dialog("close");
    $('#editPeoplePickerDiv').val("");
}

// This function cancels the editing of an existing interview's details
function cancelEditInterview() {
    $('#editInterView').dialog("close");
    $('#editInterviewLoc').val("");
    $('#editInterviewDate').val("");
    $('#editInterviewee').val("");
    initializePeoplePicker('editPeoplePickerDiv');
    $('#editComments').text("");
}

// This function deletes the selected interview
function deleteInterview() {
    var errArea = document.getElementById("errEditInterView");
    // Remove all nodes from the errEditInterView <DIV> so we have a clean space to write to
    while (errArea.hasChildNodes()) {
        errArea.removeChild(errArea.lastChild);
    }
    interviewItem.deleteObject();
    context.executeQueryAsync(
    function () {
        // Success returned from executeQueryAsync
        cancelEditInterview();
        showInterviews();
    },
    function (sender, args) {
        // Failure returned from executeQueryAsync
        var divMessage = document.createElement("DIV");
        divMessage.setAttribute("style", "padding:5px;");
        divMessage.appendChild(document.createTextNode(args.get_message()));
        errArea.appendChild(divMessage);
    });
}

