(function () {
    var executor;
    var hostweburl;
    var remotedomain;

    remotedomain = "http://localhost:17548/";

    //Get the URI decoded host web URL.
    hostweburl =
        decodeURIComponent(
            getQueryStringParameter("SPHostUrl")
    );

    // Initialize the RequestExecutor with the custom proxy URL.
    executor = new SP.RequestExecutor(remotedomain);
    executor.iFrameSourceUrl = "CustomProxy.aspx?SPHostUrl=" + hostweburl;

    // Issue the call against the remote endpoint.
    // The response formats the data in plain text.
    // The functions successHandler and errorHandler attend the
    //      sucess and error events respectively.
    executor.executeAsync(
        {
            url:
                remotedomain + "SimpleContent.aspx",
            method: "GET",
            headers: { "Accept": "text/plain" },
            success: successHandler,
            error: errorHandler
        }
    );
})();

// Function to handle the success event.
// Prints the data to the placeholder.
function successHandler(data) {
    document.getElementById("TextData").innerText =
        data.body;
}

// Function to handle the error event.
// Prints the error message to the page.
function errorHandler(data, errorCode, errorMessage) {
    document.getElementById("TextData").innerText =
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