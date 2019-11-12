var hostWebUrl;
var hostLayoutsUrl;

// Load the SharePoint resources.
$(function() {

  // Get the URI decoded app web URL.
  hostWebUrl = decodeURIComponent(getQueryStringParameter("SPHostUrl"));
  $("#linkHostWeb").attr("href", hostWebUrl);

});

function getQueryStringParameter(paramToRetrieve) {
  var params = document.URL.split("?")[1].split("&");
  var strParams = "";
  for (var i = 0; i < params.length; i = i + 1) {
    var singleParam = params[i].split("=");
    if (singleParam[0] == paramToRetrieve)
      return singleParam[1];
  }
}
