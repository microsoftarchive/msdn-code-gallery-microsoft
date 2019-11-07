var context;
var web;
var site;
var session;
var termStore;
var groups;

// This code runs when the DOM is ready and creates a context object 
// which is needed to use the SharePoint object model.
// It also wires up the click handlers for the two HTML buttons in Default.aspx.
$(document).ready(function () {
    context = SP.ClientContext.get_current();
    site = context.get_site();
    web = context.get_web();
    $('#listExisting').click(function () { listGroups(); });
    $('#createTerms').click(function () { createTerms(); });
});

// When the listExisting button is clicked, start by loading
// a TaxonomySession for the current context. Also get and load
// the associated term store.
function listGroups() {
    session = SP.Taxonomy.TaxonomySession.getTaxonomySession(context);
    termStore = session.getDefaultSiteCollectionTermStore();
    context.load(session);
    context.load(termStore);
    context.executeQueryAsync(onListTaxonomySession, onFailListTaxonomySession);
}

// Runs when the executeQueryAsync method in the listGroups function has succeeded.
// In this case, get and load the groups associated with the term store that we 
// know we now have a reference to.
function onListTaxonomySession() {
    groups = termStore.get_groups();
    context.load(groups);
    context.executeQueryAsync(onRetrieveGroups, onFailRetrieveGroups);
}

// Runs when the executeQueryAsync method in the onListTaxonomySession function has succeeded.
// In this case, loop through all the groups and add a clickable div element to the report area
// for each group.
// NOTE: We clear the report area first to ensure we have a clean place to write to.
// Also note how we create a click event handler for each div on-the-fly, and that we pass in the 
// current group ID to that function. So when the user clicks one of these divs, we will know which
// one was clicked.
function onRetrieveGroups() {
    $('#report').children().remove();
    
    var groupEnum = groups.getEnumerator();

    // For each group, we'll build a clickable div.
    while (groupEnum.moveNext()) {
        var currentGroup = groupEnum.get_current();
        var groupName = document.createElement("div");
        groupName.setAttribute("style", "float:none;cursor:pointer");
        var groupID = currentGroup.get_id();
        groupName.setAttribute("id", groupID);
        groupName.setAttribute("onclick", "showTermSets('"+ groupID + "');");
        groupName.appendChild(document.createTextNode(currentGroup.get_name()));
        $('#report').append(groupName);
    }
}

// This is the function that runs when the user clicks one of the divs
// that we created in the onRetrieveGroups function. We can know which
// div was clicked by interrogating the groupID parameter. So what we'll
// do is retrieve a reference to the group with the same ID as the div, and
// then add the term sets that belong to that group under the div that was clicked.
function showTermSets(groupID) {

    // First thing is to remnove the divs under the group DIV to ensure we have a clean place to write to.
    // The reason we don't clear them all is becuase we want to retain the text node of the
    // group div. I.E. that's why we use "parentDiv.childNodes.length>1" as our loop
    // controller.
    var parentDiv = document.getElementById(groupID);
    while (parentDiv.childNodes.length>1) {
        parentDiv.removeChild(parentDiv.lastChild);
    }
    var groupEnum = groups.getEnumerator();

    // For each term set, we'll build a clickable div
    while (groupEnum.moveNext()) {
        var currentGroup = groupEnum.get_current();
        if (currentGroup.get_id() == groupID) {

            // We need to load and populate the matching group first, or the
            // term sets that it contains will be inaccessible to our code.
            context.load(currentGroup);
            context.executeQueryAsync(
                function () {

                    // The group is now available becuase this is the 
                    // success callback. So now we'll load and populate the
                    // term set collection. We have to do this before we can 
                    // iterate through the collection, so we can do this
                    // with the following nested executeQueryAsync method call.
                    var termSets = currentGroup.get_termSets();
                    context.load(termSets);
                    context.executeQueryAsync(
                    function () {

                        // The term sets are now available becuase this is the 
                        // success callback. So now we'll iterate through the collection
                        // and create the clickable div. Also note how we create a 
                        // click event handler for each div on-the-fly, and that we pass in the 
                        // current group ID and term set ID to that function. So when the user 
                        // clicks one of these divs, we will know which
                        // one was clicked by its term set ID, and to which group it belongs by its
                        // group ID. We also pass in the event object, so that we can cancel the bubble
                        // because this clickable div will be inside a parent clickable div and we
                        // don't want the parent's event to fire.
                        var termSetEnum = termSets.getEnumerator();
                        while (termSetEnum.moveNext()) {
                            var currentTermSet = termSetEnum.get_current();
                            var termSetName = document.createElement("div");
                            termSetName.appendChild(document.createTextNode(" + " + currentTermSet.get_name()));
                            termSetName.setAttribute("style", "float:none;cursor:pointer;");
                            var termSetID = currentTermSet.get_id();
                            termSetName.setAttribute("id", termSetID);
                            termSetName.setAttribute("onclick", "showTerms(event,'" + groupID + "','" + termSetID + "');");
                            termSetName.setAttribute("id", termSetID);
                            parentDiv.appendChild(termSetName);
                        }

                    },
                        function () {
                            //Failure to load term set
                            parentDiv.appendChild(document.createTextNode("An error occurred in loading the term sets for this group"));
                        });

                },
                 function () {
                     //Failure to load current group
                     parentDiv.appendChild(document.createTextNode("An error occurred in loading the term sets for this group"));
                 });

            break;
        }

    }

}


// This is the function that runs when the user clicks one of the divs
// that we created in the showTermSets function. We can know which
// div was clicked by interrogating the termSetID parameter. So what we'll
// do is retrieve a reference to the term set with the same ID as the div, and
// then add the term  that belong to that term set under the div that was clicked.

function showTerms(event, groupID, termSetID) {

    // First, cancel the bubble so that the group div click handler does not also fire
    // because that removes all term set divs and we don't want that here.
    event.cancelBubble = true;

    // Get a reference to the term set div that was click and
    // remove its children (apart from the TextNode that is currently
    // showing the term set name.
    var parentDiv = document.getElementById(termSetID);
    while (parentDiv.childNodes.length > 1) {
        parentDiv.removeChild(parentDiv.lastChild);
    }

    // We need to load and populate the matching group first, or the
    // term sets that it contains will be inaccessible to our code.
    var groupEnum = groups.getEnumerator();
    while (groupEnum.moveNext()) {
        var currentGroup = groupEnum.get_current();
        if (currentGroup.get_id() == groupID) {
            context.load(currentGroup);
            context.executeQueryAsync(

                // The group is now available becuase this is the 
                // success callback. So now we'll load and populate the
                // term set collection. We have to do this before we can 
                // iterate through the collection, so we can do this
                // with the following nested executeQueryAsync method call.
                function () {
                    var termSets = currentGroup.get_termSets();
                    context.load(termSets);
                    context.executeQueryAsync(
                    function () {

                        // The term sets are now available becuase this is the 
                        // success callback. So now we'll iterate through the collection
                        // and get a reference to the specific term set that was represented 
                        // by the clicked div.
                        var termSetEnum = termSets.getEnumerator();
                        while (termSetEnum.moveNext()) {
                            var currentTermSet = termSetEnum.get_current();
                            if (currentTermSet.get_id() == termSetID) {

                                // We need to load and populate the term set, so that we can 
                                // access the terms in it.
                                context.load(currentTermSet);
                                context.executeQueryAsync(
                                    function () {

                                        // Now we have access to the term set because this is the 
                                        // success callback, so we can now create and populate a collection
                                        // object to hold the actual terms.
                                        // Note that we need to do one final load and populate before we
                                        // can iterate over the collection object.
                                        var terms = currentTermSet.get_terms();
                                        context.load(terms);
                                        context.executeQueryAsync(
                                            function () {

                                                // Now we can iterate over the terms because this is the 
                                                // success callback. So we'll build an indented list of terms
                                                var termsEnum = terms.getEnumerator();
                                                while (termsEnum.moveNext()) {
                                                    var currentTerm = termsEnum.get_current();

                                                    var term = document.createElement("div");
                                                    term.appendChild(document.createTextNode("    - " + currentTerm.get_name()));
                                                    term.setAttribute("style", "float:none;margin-left:10px;");
                                                    parentDiv.appendChild(term);
                                                }
                                            },
                                            function () {
                                                //Failure to load terms
                                                parentDiv.appendChild(document.createTextNode("An error occurred when trying to retrieve terms in this term set"));
                                            });
                                    },
                                    function () {
                                        //Failure to load the current term set
                                        parentDiv.appendChild(document.createTextNode("An error occurred when trying to retrieve terms in this term set"));
                                    });
                                break;
                            }
                        }
                    },
                        function () {
                            //Failure to load term sets
                            parentDiv.appendChild(document.createTextNode("An error occurred when trying to retrieve terms in this term set"));
                        });

                },
                 function () {
                     //Failure to load current group
                     parentDiv.appendChild(document.createTextNode("An error occurred when trying to retrieve terms in this term set"));
                 });
            break;
        }
    }
}

// Runs when the executeQueryAsync method in the onListTaxonomySession function has failed.
// In this case, clear the report area in the page and tell the user what went wrong.
function onFailRetrieveGroups(sender, args) {
    $('#report').children().remove();
    $('#report').append("Failed to retrieve groups. Error:" + args.get_message());
}

// Runs when the executeQueryAsync method in the listGroups function has failed.
// In this case, clear the report area in the page and tell the user what went wrong.
function onFailListTaxonomySession(sender, args) {
    $('#report').children().remove();
    $('#report').append("Failed to get session. Error: " + args.get_message());
}


// When the createTerms button is clicked, start by loading
// a TaxonomySession for the current context. Also get and load
// the associated term store.
function createTerms() {
    session = SP.Taxonomy.TaxonomySession.getTaxonomySession(context);
    termStore = session.getDefaultSiteCollectionTermStore();
    context.load(session);
    context.load(termStore);
    context.executeQueryAsync(onGetTaxonomySession, onFailTaxonomySession);
}


// This function is the success callback for loading the session and store from the createTerms function
function onGetTaxonomySession() {
    // Create six GUIDs that we will need when we create a new group, term set, and associated terms
    var guidGroupValue = new SP.Guid.newGuid();
    var guidTermSetValue = new SP.Guid.newGuid();
    var guidTerm1 = new SP.Guid.newGuid();
    var guidTerm2 = new SP.Guid.newGuid();
    var guidTerm3 = new SP.Guid.newGuid();
    var guidTerm4 = new SP.Guid.newGuid();

    // Create a new group
    var myGroup = termStore.createGroup("CustomTerms", guidGroupValue);

    // Create a new term set in the newly-created group
    var myTermSet = myGroup.createTermSet("Privacy", guidTermSetValue, 1033);

    // Create four new terms in the newly-created  term set
    myTermSet.createTerm("Top Secret", 1033, guidTerm1);
    myTermSet.createTerm("Company Confidential", 1033, guidTerm2);
    myTermSet.createTerm("Partners Only", 1033, guidTerm3);
    myTermSet.createTerm("Public", 1033, guidTerm4);
   
    // Ensure the groups variable has been set, because when this all succeeds we will
    // effectively run the same code as if the user had clicked the listGroups button
    groups = termStore.get_groups();
    context.load(groups);

    // Execute all the preceeding statements in this function
    context.executeQueryAsync(onAddTerms, onFailAddTerms);
    
}

// If all is well with creating the terms, then this function will run.
// Effectively this runs the same code as if the user had clicked the listGroups button
// so the user will see their newly-created group
function onAddTerms() {
    listGroups();
}

// Runs when the executeQueryAsync method in the onGetTaxonomySession function has failed.
// In this case, clear the report area in the page and tell the user what went wrong.
function onFailAddTerms(sender, args) {
    $('#report').children().remove();
    $('#report').append("Failed to add terms. Error: " + args.get_message());
}

// Runs when the executeQueryAsync method in the createTerms function has failed.
// In this case, clear the report area in the page and tell the user what went wrong.
function onFailTaxonomySession(sender, args) {
    $('#report').children().remove();
    $('#report').append("Failed to get session. Error: " + args.get_message());
}

