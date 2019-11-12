// Variables that we will use in callbacks
var context;
var peopleMgr;
var profileProperties;


// This code runs when the DOM is ready and creates a context object 
// which is needed to use the SharePoint object model. We also wire
// up the click event handler of the listProfiles button in default.aspx.
$(document).ready(function () {
    context = SP.ClientContext.get_current();
    $('#listProfiles').click(function () { listProfilesClick();});
});

// This function handles the click event of the listProfiles button in default.aspx
function listProfilesClick() {
   
    // Our way into the current user's profile is through the PeopleManager class 
    // so we'll instantiate a new object and pass in the current context,
    peopleMgr = new SP.UserProfiles.PeopleManager(context);

    // We'll then load the object...
    context.load(peopleMgr);

    // ... and we'll get the user profile properties and load them as well.
    profileProperties = peopleMgr.getMyProperties();
    context.load(profileProperties);

    // Next we ask SharePoint to run all of our previously batched commands...
    context.executeQueryAsync(

        // ... and if we're successful, the following success callback will run
        function () {
            // Clear the div in default.aspx of all current content
            // so we have a clean place to write to.
            $('#profileList').children().remove();

            // The first thing we'll tell the user is whether the folks they follow 
            // (and the ones that follow them)
            // are visible to people who view the user's public persona.
            // NOTE: The user can control this setting --- see the ReadMe document for how to
            // do this.
            var publicDiv = document.createElement("div");
            publicDiv.setAttribute("style", "float:none;padding:5px;");
            if (peopleMgr.get_isMyPeopleListPublic()) {
                publicDiv.appendChild(document.createTextNode("Your followers and those you follow are publicly visible"));
            }
            else {
                publicDiv.appendChild(document.createTextNode("Your followers and those you follow are not publicly visible"));
            }
            $('#profileList').append(publicDiv);
            
            // Then we'll add a few simple properties from the profile.
            // The first one is the account name
            var accountName = profileProperties.get_accountName();
            var accountDiv = document.createElement("div");
            accountDiv.setAttribute("style", "float:none;padding:5px;");
            accountDiv.appendChild(document.createTextNode("Your account: " + accountName));
            $('#profileList').append(accountDiv);

            // The second one is the display name.
            // Note that we'll check for null in case this property hasn't been set.
            var displayName = profileProperties.get_displayName();
            var displayNameDiv = document.createElement("div");
            displayNameDiv.setAttribute("style", "float:none;padding:5px;");
            if (displayName != null) {
                displayNameDiv.appendChild(document.createTextNode("Your display name: " + displayName));
            }
            else {
                displayNameDiv.appendChild(document.createTextNode("Your display name has not been set"));
            }
            $('#profileList').append(displayNameDiv);

            // The third thing to show the user is their profile picture.
            // Note that we'll check for null in case this property hasn't been set.
            // If it's not null, we'll render the actual picture.
            var myPicture = profileProperties.get_pictureUrl();
            var pictureDiv = document.createElement("div");
            pictureDiv.setAttribute("style", "float:none;padding:5px;");
            if (myPicture == null) {
                pictureDiv.appendChild(document.createTextNode("You have not set your picture"));
            }
            else {
                var myImage = document.createElement("img");
                myImage.setAttribute("src", myPicture);
                myImage.setAttribute("title", "You are a rare beauty!");
                myImage.setAttribute("alt", "Me, me, and me, me, me!");
                pictureDiv.appendChild(myImage);
            }
            $('#profileList').append(pictureDiv);

            // Finally, we'll add three links:
            // One for editing the profile...
            var myProfileLink = peopleMgr.get_editProfileLink();
            var editProfileLink = document.createElement("a");
            editProfileLink.setAttribute("href", myProfileLink);
            editProfileLink.setAttribute("title", "Edit your profile");
            editProfileLink.setAttribute("target", "_blank");
            editProfileLink.appendChild(document.createTextNode("[Your Profile ] "));
            $('#profileList').append(editProfileLink);
           
            // One for going to the user's personal site...
            var myPersonalSite = profileProperties.get_personalUrl();
            var personalLink = document.createElement("a");
            personalLink.setAttribute("href", myPersonalSite);
            personalLink.setAttribute("title", "Go to your personal site");
            personalLink.setAttribute("target", "_blank");
            personalLink.appendChild(document.createTextNode("[Your Personal Site] "));
            $('#profileList').append(personalLink);
            
            // And one that shows the user what their page looks like when 
            // viewed by other users.
            var myPublicPersona = profileProperties.get_userUrl();
            var publicLink = document.createElement("a");
            publicLink.setAttribute("href", myPublicPersona);
            publicLink.setAttribute("title", "View your public persona");
            publicLink.setAttribute("target", "_blank");
            publicLink.appendChild(document.createTextNode("[Your Public Persona]"));
            $('#profileList').append(publicLink);





        },

        // If we haven't been successful, the following failure callback
        // will run, so we'll clear the profile list div, and then use it
        // to tell the user what went wrong
        function (sender, e) {
            $('#profileList').children().remove();
            $('#profileList').append(docume.createTextNode(e.get_message()));
        });
}