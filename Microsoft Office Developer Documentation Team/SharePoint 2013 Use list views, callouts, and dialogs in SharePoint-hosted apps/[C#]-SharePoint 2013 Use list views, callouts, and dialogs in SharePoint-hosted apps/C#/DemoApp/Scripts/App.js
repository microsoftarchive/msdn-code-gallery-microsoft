var keywords = "sports+apps+news";
var postTitle;
var postLink;
var postText;

var context;
var web;

var person;
var itemId;
var title;

// This code runs when the DOM is ready. It ensures the SharePoint
// script file sp.js is loaded and then executes sharePointReady()
$(document).ready(function () {
    SP.SOD.executeFunc('sp.js', 'SP.ClientContext', sharePointReady);
});

// This function creates a context object which is needed 
// to use the SharePoint object model
function sharePointReady() {
    //This code replaces the icon for the appweb
    document.getElementById("ctl00_onetidHeadbnnr2").src = "../Images/siteIcon.png";
    document.getElementById("ctl00_onetidHeadbnnr2").className = "ms-emphasis";

    //Get client context
    this.context = new SP.ClientContext.get_current();
    this.web = context.get_web();

    if (window.location.pathname.indexOf("Default.aspx") != -1) {
        calloutScripts();
        getKeywords();
    };
    if (window.location.pathname.indexOf("Post.aspx") != -1) {

        postTitle = getQueryStringParameter("title");
        postLink = getQueryStringParameter("link");

        loadPeoplePicker();
        
        $('#box').show();
    };
}

// This function executes a SharePoint query to get the titles from the PinList
function getKeywords() {
    this.web = context.get_web();
    var selectedList = web.get_lists().getByTitle("PinList");

    var camlQuery = new SP.CamlQuery.createAllItemsQuery();
    var listItems = selectedList.getItems(camlQuery);

    context.load(listItems);
    context.executeQueryAsync(onGetItemSuccess, onFail);

    function onGetItemSuccess(sender, args) {
        var listItemEnumerator = listItems.getEnumerator();
        var allKeywords = 'news';
        while (listItemEnumerator.moveNext()) {
            allKeywords = allKeywords + "+"
                + listItemEnumerator.get_current().get_item('Title');
        }
        getData(allKeywords);
    }
}

// This function executes a SharePoint query to get external data using webproxy
function getData(keywords) {
    var url = "http://www.bing.com/news?q=" + keywords + "&format=RSS";
    var request = new SP.WebRequestInfo();
    request.set_url(url);
    request.set_method("GET");
    var response = SP.WebProxy.invoke(context, request);

    context.executeQueryAsync(onGetDataSuccess, onFail);

    function onGetDataSuccess() {
        //body parsing
        var newsList = document.getElementById("articles");
        var xml = $.parseXML(response.get_body());
        newsList.innerHTML = '';

        $(xml).find("item").each(function () {
            var newsListing = document.createElement("div");
            var link = $(this).find("link").text();
            var title = $(this).find("title").text();
            var pubDate = $(this).find("pubDate").text();
            var description = $(this).find("description").text().substr(0, 265);

            newsListing.innerHTML = "<div><h1><a href='"
            + link + "'>" + title + "</a></h1></div>"
            + "<div class=\"ms-soften\">" + pubDate + "</div>"
            + "<div>" + description + "</div>";

            newsList.appendChild(newsListing);

            //Adding a callout to each article retrieved
            var calloutLink = document.createElement("div");
            calloutLink.id = link;
            calloutLink.onmouseover = function () {
                curListUrl = this.id;
            }

            calloutLink.innerHTML = "<div class=\"ms-commandLink\" style=\"text-align: right;\">"
               + "Details" + "</div>";

            newsList.appendChild(calloutLink);

            var listCallout = CalloutManager.createNew({
                launchPoint: calloutLink,
                beakOrientation: "leftRight",
                openOptions: {
                    event: "click",
                    showCloseButton: true
                },
                ID: title,
                title: title,
                content: "<div class=\"ms-soften\" style=\"margin-top:13px;\">"
                        + "Pulication Date: " + pubDate + "</div>"
                        + "<div class=\"callout-section\" style=\"margin-top:13px;\">"
                        + "<div>Description:</div>" + description + "</div>",
            });

            var calloutAction1 = new CalloutAction({
                text: "Open ",
                isEnabledCallback: function () {
                    callBackListUrl = curListUrl;
                    return true;
                },
                onClickCallback: function () {
                    window.open(callBackListUrl);
                    listCallout.close();
                }
            });
            listCallout.addAction(calloutAction1);

            var calloutAction2 = new CalloutAction({
                text: "Post to Feed",
                isEnabledCallback: function () {
                    callBackListUrl = curListUrl;
                    return true;
                },
                onClickCallback: function () {
                    var appWebUrl = getQueryStringParameter("SPAppWebUrl");
                    var pageUrl = "Post.aspx?SPAppWebUrl=" + appWebUrl + "&title=" + title + "&link=" + link;
                    dialogOpen(pageUrl);
                    listCallout.close();
                }
            });
            listCallout.addAction(calloutAction2);
        });
    }
}

//function to load the people picker
function loadPeoplePicker(peoplePickerElementId) {
    window.EnsurePeoplePickerRefinementInit = function (peoplePickerElementId) {
        //JSON dictionary to use as a schema that stores picker-specific properties
        var schema = new Array();
        schema["PrincipalAccountType"] = "User";
        schema["AllowMultipleValues"] = false;
        schema["Width"] = 100;
        schema["OnUserResolvedClientScript"] = function () {
            var pickerObj = SPClientPeoplePicker.SPClientPeoplePickerDict.peoplePicker_TopSpan;
            var users = pickerObj.GetAllUserInfo();
            person = users[0];
            var userInfo = '';

            // Get user information
            for (var userProperty in person) {
                userInfo += userProperty + ':  ' + person[userProperty] + '<br>';
            }

            $('#resolvedUser').html(userInfo);
            $('#userKey').html(person.Key);

            $("#postButton").click(function (event) {
                postText = document.getElementById("message").value;
                postToFeed();
            });
        };

        SP.SOD.executeFunc("clienttemplates.js", "SPClientTemplates", function () {
            SP.SOD.executeFunc("clientforms.js", "SPClientPeoplePicker_InitStandaloneControlWrapper", function () {
                SPClientPeoplePicker_InitStandaloneControlWrapper("peoplePicker", null, schema);
            });
        });
    }
    EnsurePeoplePickerRefinementInit("peoplePicker");
}

//function to post to the feed
function postToFeed() {
    var feedManager = new SP.Social.SocialFeedManager(this.context);

    var userDataItem = new SP.Social.SocialDataItem();
    userDataItem.set_itemType(SP.Social.SocialDataItemType.user);
    userDataItem.set_text(person.DisplayText);
    userDataItem.set_accountName(person.Key);

    var linkDataItem = new SP.Social.SocialDataItem();
    linkDataItem.set_itemType(SP.Social.SocialDataItemType.link);
    linkDataItem.set_text(postTitle);
    linkDataItem.set_uri(postLink);

    var imgAttachment = new SP.Social.SocialAttachment();
    imgAttachment.set_attachmentKind(SP.Social.SocialAttachmentKind.Image);
    var appWebUrl = getQueryStringParameter("SPAppWebUrl");
    imgAttachment.set_uri(appWebUrl + "/Images/postIcon.png");

    // Create the post content with the message and add the data item.
    var postData = new SP.Social.SocialPostCreationData();
    postData.set_contentText("Hey {0} check this out: {1} " + postText);
    postData.set_contentItems([userDataItem, linkDataItem]);
    postData.set_attachment(imgAttachment);

    // Publish the post. Pass null for the "targetId" parameter because this is a root post.
    var resultThread = feedManager.createPost(null, postData);

    this.context.executeQueryAsync(onPostToFeedSuccess, onFail);
    dialogClose();

    function onPostToFeedSuccess(sender, args) {
        alert("Sucess");
    }
}

// This function loads the callout scripts
function calloutScripts() {
    SP.SOD.executeFunc("callout.js", "Callout", function () {
        SP.SOD.executeFunc("mQuery.js", "mQuery");
    });
}

//function to display a dialog
function dialogOpen(pageUrl) {
    var options = { url: pageUrl, width: 400, height: 300 };//, dialogReturnValueCallback: demoCallback };
    SP.SOD.execute('sp.ui.dialog.js', 'SP.UI.ModalDialog.showModalDialog', options);
}

//Close dialog 
function dialogClose() {
    SP.SOD.execute('sp.ui.dialog.js', 'SP.UI.ModalDialog.commonModalDialogClose', 1);
}

// Function to retrieve a query string value.
function getQueryStringParameter(name) {
    name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
    var regexS = "[\\?&]" + name + "=([^&#]*)";
    var regex = new RegExp(regexS);
    var results = regex.exec(window.location.search);
    if (results == null)
        return "";
    else
        return decodeURIComponent(results[1].replace(/\+/g, " "));
}

//funtion to catch all the query failues
function onFail(sender, args) {
    alert('Failed. Error:' + args.get_message() + '\n' + args.get_stackTrace());
    logAppErrorToSharePoint(args.get_message());
}

//This funtion to log custom app error mesagges
function logAppErrorToSharePoint(message) {
    SP.Utilities.Utility.logCustomAppError(context, message);
    this.context.executeQueryAsync(alert('Logging error message to SharePoint: \n' + message),
        alert('Failed to log a message in SharePoint'));
}