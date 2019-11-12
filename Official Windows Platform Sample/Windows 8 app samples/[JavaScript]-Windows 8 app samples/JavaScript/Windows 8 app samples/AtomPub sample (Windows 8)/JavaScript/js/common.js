//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

// The default values for the WordPress site.
var defaultBaseUri = "http://<Your Wordpress Site>.wordpress.com/";
var defaultUser = "";
var defaultPassword = "";

// The default Service Document and Edit 'URIs' for WordPress.
var defaultEditUri = "./wp-app.php/posts";
var defaultServiceDocUri = "./wp-app.php/service";
var defaultFeedUri = "./?feed=atom";

// Common variables to all pages.
var client = null;
var currentFeed = null;
var currentItemIndex = 0;

var displayCurrentItem = null;

// Initialize common input fields and current feed.
function initializeCommon() {
    var serviceAddress = WinJS.Application.sessionState.serviceAddress;
    document.getElementById("serviceAddressField").value = serviceAddress || defaultBaseUri;
    var userName = WinJS.Application.sessionState.userName;
    document.getElementById("userNameField").value = userName || defaultUser;
    var password = WinJS.Application.sessionState.password;
    document.getElementById("passwordField").value = password || defaultPassword;

    currentFeed = null;
}

// Called when an async function generates an error.
function onError(err) {
    displayError(err);

    // Match error number with a WebErrorStatus value, in order to deal
    // with a specific error.
    var errorStatus = Windows.Web.WebError.getStatus(err.number);
    if (errorStatus === Windows.Web.WebErrorStatus.unauthorized) {
        displayLog("Wrong username or password!");
    }
}

// Get syndication item from current feed at current position.
function getCurrentItem() {
    if (currentFeed) {
        return currentFeed.items[currentItemIndex];
    }
    return null;
}

// Display syndication item for retrieve and delete scenarios.
function displayCurrentItemGet() {
    var currentItem = getCurrentItem();
    if (currentItem) {
        var title = "(no title)";
        if (currentItem.title) {
            title = currentItem.title.text;
        }
        document.getElementById("titleField").innerHTML = title;
        var value = "(no value)";
        if (currentItem.content) {
            value = currentItem.content.text;
        }
        else if (currentItem.summary) {
            value = currentItem.summary.text;
        }
        document.getElementById("webView").innerHTML = window.toStaticHTML(value);
    }
}

// Callback for previousButton click event.
function previousItemButtonClick() {
    if (currentFeed && currentItemIndex > 0) {
        currentItemIndex--;
        displayCurrentItem();
    }
}

// Callback for nextButton click event.
function nextItemButtonClick() {
    if (currentFeed && currentItemIndex < currentFeed.items.size - 1) {
        currentItemIndex++;
        displayCurrentItem();
    }
}

// Get current credential and create atompub client
function createClient() {
    client = new Windows.Web.AtomPub.AtomPubClient();
    client.bypassCacheOnRetrieve = true;

    if ((document.getElementById("userNameField").value !== "") && (document.getElementById("passwordField").value !== "")) {
        var credential = new Windows.Security.Credentials.PasswordCredential();
        credential.userName = document.getElementById("userNameField").value;
        credential.password = document.getElementById("passwordField").value;
        client.serverCredential = credential;
    }
    else {
        client.serverCredential = null;
    }
}

function clearLog() {
    var outputField = document.getElementById("outputField");
    outputField.innerHTML = "";
}

function displayLog(message) {
    var outputField = document.getElementById("outputField");
    outputField.innerHTML += message + "<br/>";
}

function displayStatus(message) {
    WinJS.log && WinJS.log(message, "sample", "status");
}

function displayError(message) {
    WinJS.log && WinJS.log(message, "sample", "error");
}
