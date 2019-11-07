// Variables used in various callbacks
var context;
var web;
var pubWeb;
var pageInfo;
var newPage;
var listItem;

// This code runs when the DOM is ready and creates a 
// context object which is needed to use the SharePoint object model
$(document).ready(function () {
    context = SP.ClientContext.get_current();
    web = context.get_web();
    $('#CreatePage').click(function () { createPage(); });   
});

function createPage() {
    context.load(web);
    context.executeQueryAsync(

        // Success callback after loading the App Web.
        // We first want to get the host web, and cast it as a PublishingWeb.
        function () {
            var hostUrl = decodeURIComponent(getQueryStringParameter("SPHostUrl"));
            var hostcontext = new SP.AppContextSite(context, hostUrl);
            web = hostcontext.get_web();
            pubWeb = SP.Publishing.PublishingWeb.getPublishingWeb(context, web);
            context.load(web);
            context.load(pubWeb);
            context.executeQueryAsync(

                // Success callback after getting the host Web as a PublishingWeb.
                // We now want to add a new Publishing Page.
                function () {
                    pageInfo = new SP.Publishing.PublishingPageInformation();
                    newPage = pubWeb.addPublishingPage(pageInfo);
                    context.load(newPage);
                    context.executeQueryAsync(
                        function () {

                            // Success callback after adding a new Publishing Page.
                            // We want to get the actual list item that is represented by the Publishing Page.
                            listItem = newPage.get_listItem();
                            context.load(listItem);
                            context.executeQueryAsync(

                                // Success callback after getting the actual list item that is 
                                // represented by the Publishing Page.
                                // We can now get its FieldValues, one of which is its FileLeafRef value.
                                // We can then use that value to build the Url to the new page
                                // and set the href or our link to that Url.
                                function () {
                                    var link = document.getElementById("linkToPage");
                                    link.setAttribute("href", web.get_url() + "/Pages/" + listItem.get_fieldValues().FileLeafRef);
                                    link.innerText = "Go to new page!";
                                },

                                // Failure callback after getting the actual list item that is 
                                // represented by the Publishing Page.
                                function (sender, args) {
                                    alert('Failed to get new page: ' + args.get_message());
                                }
                                );
                        },
                        // Failure callback after trying to add a new Publishing Page.
                        function (sender, args) {
                            alert('Failed to Add Page: ' + args.get_message());
                        }
                        );
                },
                // Failure callback after trying to get the host Web as a PublishingWeb.
                function (sender, args) {
                    alert('Failed to get the PublishingWeb: ' + args.get_message());
                }
                );
        },
        // Failure callback after trying to load the App Web.
        function (sender, args) {
            alert('Failed to get the hosting Web: ' + args.get_message());
        });
}

// Helper function for getting the hostUrl.
function getQueryStringParameter(paramToRetrieve) {
    var params =
    document.URL.split("?")[1].split("&");
    var strParams = "";
    for (var i = 0; i < params.length; i = i + 1) {
        var singleParam = params[i].split("=");
        if (singleParam[0] == paramToRetrieve)
            return singleParam[1];
    }
}