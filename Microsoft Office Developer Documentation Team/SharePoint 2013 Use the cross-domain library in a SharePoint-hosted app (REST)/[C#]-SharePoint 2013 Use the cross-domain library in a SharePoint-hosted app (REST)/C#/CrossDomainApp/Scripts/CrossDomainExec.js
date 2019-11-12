var hostweburl;

// Load the required SharePoint libraries
$(document).ready(function () {
  //Get the URI decoded URLs.
  hostweburl =
      decodeURIComponent(
          getQueryStringParameter("SPHostUrl")
  );

  // Load the js file and continue to the 
  //   success event handler
  $.getScript("../_layouts/15/SP.RequestExecutor.js", execCrossDomainRequest);
});

// Function to prepare and issue the request to get
//  SharePoint data
function execCrossDomainRequest() {
  var executor;

  // Initialize the RequestExecutor with the app web URL.
  executor = new SP.RequestExecutor("/");

  // Issue the call against the host web.
  // To get the title using REST we can hit the endpoint:
  //   app_web_url/_api/SP.AppContextSite(@target)/web/title?@target='siteUrl'
  // The response formats the data in the JSON format.
  // The functions successHandler and errorHandler attend the
  //      success and error events respectively.
  executor.executeAsync(
      {
        url:
            "../_api/SP.AppContextSite(@target)/web/title?@target='" +
            hostweburl + "'",
        method: "GET",
        headers: { "Accept": "application/json; odata=verbose" },
        success: successHandler,
        error: errorHandler
      }
  );
}

// Function to handle the success event.
// Prints the host web's title to the page.
function successHandler(data) {
  var jsonObject = JSON.parse(data.body);

  document.getElementById("HostwebTitle").innerHTML =
      "<b>" + jsonObject.d.Title + "</b>";
}

// Function to handle the error event.
// Prints the error message to the page.
function errorHandler(data, errorCode, errorMessage) {
  document.getElementById("HostwebTitle").innerText =
      "Could not complete cross-domain call: " + errorMessage;
}

// Function to retrieve a query string value.
// For production purposes you may want to use
//  a library to handle the query string.
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