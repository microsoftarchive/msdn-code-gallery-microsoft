Office.initialize = function (reason) {

};

var authWindow = null;
var listDataString = "";
$(document).ready(function () {
    var code = getQueryStringParam("code");
    var state = getQueryStringParam("state")
    if (code != undefined) {
        //We are redirected and got the code
        $("#oAuthCode").val(code);
        $("#state").val(state);
        if (state != undefined) {
            // Redirect task pane to the app code with the authCode token
            window.location = "OAuth.aspx?code=" + code + "&siteUrl=" + state; 
        }
    }
});

function startFlow(siteUrl, inSameWindow) {
    var listTitle;
    var listDescription;
    var listColumns;
    var status;
    Office.context.document.bindings.addFromNamedItemAsync("ListTitle",
        Office.BindingType.Text,
        { id: 'listTitle' },
        function (asyncResult) {
            if (asyncResult.status == Office.AsyncResultStatus.Failed) {
                //Write a message to the page if the binding has failed
                status = document.getElementById("Status");
                status.appendChild(document.createTextNode("Cannot bind to list definition: ListTitle"));
            }
            else {
                Office.select("bindings#listTitle").getDataAsync(function (asyncResult) {
                    if (asyncResult.status == Office.AsyncResultStatus.Failed) {
                        //Write a message to the page if the binding has failed
                        status = document.getElementById("Status");
                        status.appendChild(document.createTextNode("Cannot bind to list definition: ListTitle"));
                    } else {
                        // If the ListTitle has been read successfully, 
                        // hold that data in our variable and then get the list description.
                        listTitle = asyncResult.value;

                        Office.context.document.bindings.addFromNamedItemAsync("ListDescription",
                            Office.BindingType.Text,
                            { id: 'listDescription' },
                            function (asyncResult) {
                                if (asyncResult.status == Office.AsyncResultStatus.Failed) {
                                    //Write a message to the page if the binding has failed
                                    status = document.getElementById("Status");
                                    status.appendChild(document.createTextNode("Cannot bind to list definition: ListDescription"));
                                }
                                else {
                                    Office.select("bindings#listDescription").getDataAsync(function (asyncResult) {
                                        if (asyncResult.status == Office.AsyncResultStatus.Failed) {
                                            //Write a message to the page if the binding has failed
                                            status = document.getElementById("Status");
                                            status.appendChild(document.createTextNode("Cannot bind to list definition: ListDescription"));
                                        } else {
                                            // If the ListDescription has been read successfully, 
                                            // hold that data in our variable and then get the list columns.
                                            listDescription = asyncResult.value;
                                            Office.context.document.bindings.addFromNamedItemAsync("ListColumns",
                                                Office.BindingType.Text,
                                                { id: 'listColumns' },
                                                function (asyncResult) {
                                                    if (asyncResult.status == Office.AsyncResultStatus.Failed) {
                                                        //Write a message to the page if the binding has failed
                                                        status = document.getElementById("Status");
                                                        status.appendChild(document.createTextNode("Cannot bind to list definition: ListColumns"));
                                                    }
                                                    else {
                                                        Office.select("bindings#listColumns").getDataAsync(function (asyncResult) {
                                                            if (asyncResult.status == Office.AsyncResultStatus.Failed) {
                                                                //Write a message to the page if the binding has failed
                                                                status = document.getElementById("Status");
                                                                status.appendChild(document.createTextNode("Cannot bind to list definition: ListColumns"));
                                                            } else {
                                                                // If the ListColumns value has been read successfully, 
                                                                // hold that data in our variable validate the data.
                                                                listColumns = asyncResult.value;
                                                                // validate the data
                                                                status = document.getElementById("Status");
                                                                while (status.hasChildNodes()) {
                                                                    status.removeChild(status.lastChild);
                                                                }
                                                                if ((listTitle == "Click here to enter text.")
                                                                    || (listDescription == "Click here to enter text.")
                                                                    || (listColumns == "Click here to enter text.")
                                                                    || (siteUrl=="")
                                                                    ) {
                                                                    
                                                                    status.appendChild(document.createTextNode("Please provide all data before continuing."));
                                                                }
                                                                else {
                                                                    //var listColumnData = listColumns.split(";");
                                                                    //var listFields = "[[";
                                                                    //for (var iColumn = 0; iColumn<listColumnData.length; iColumn++)
                                                                    //{
                                                                    //    if (iColumn < listColumnData.length - 1) {
                                                                    //        listFields += listColumnData[iColumn] + ",";
                                                                    //    }
                                                                    //    else {
                                                                    //        listFields += listColumnData[iColumn]
                                                                    //    }
                                                                    //}
                                                                    //listFields+="]]";
                                                                    localStorage.setItem("listTitle", listTitle);
                                                                    localStorage.setItem("listDescription", listDescription);
                                                                    localStorage.setItem("listColumns", listColumns);
                                                                    status.appendChild(document.createTextNode("OK"));
                                                                    initAuthFlow(siteUrl, inSameWindow);
                                                                }
                                                                return;
                                                            }
                                                        });
                                                    }
                                                });
                                        }
                                    });
                                }
                            });
                    }
                });
            }
        });
}

function createList() {
    // Client-side click event retrieves the data from local storage
    // and pops it into the hidden textarea form field on the page,
    // which will then get posted to the CreateList.aspx page.
   
    var listTitle = localStorage.getItem("listTitle");
    var listDescription = localStorage.getItem("listDescription");
    var listColumns = localStorage.getItem("listColumns");
    $("#listName").val(listTitle);
    $("#listDescription").val(listDescription);
    $("#listData").val(JSON.stringify(listColumns));
    $("#listForm").submit();
}

// This function initiaties the OAuth flow
function initAuthFlow(siteUrl, inSameWindow) {
    if (!siteUrl.endsWith("/")) {
        siteUrl += "/";
    }
    authorizeUrl = siteUrl + "_layouts/15/OAuthAuthorize.aspx?IsDlg=1";
    authorizeUrl += "&client_id=" + $("#clientId").val();
    authorizeUrl += "&scope=Web.Manage";
    authorizeUrl += "&response_type=code";
    authorizeUrl += "&redirect_uri=" + $("#redirectUrl").val();
    authorizeUrl += "&state=" + siteUrl;
    if (inSameWindow) {

        //Perform authorization flow in this window
        window.location = authorizeUrl;
    }
    else {
        //Open a new window to perform the authorization flow
        authWindow = window.open(authorizeUrl, "authWindow");
        codeListener(); //start listening for the auth code
    }
}


function codeListener() {
    setTimeout(function () { readCode(); }, 500); //check for the auth code every one second
}

function readCode() {
    try {
        //if we can actually reach the authCode field on RedirectAccept.aspx
        // we'll get the authCode value and the state value
        var authCode = authWindow.document.getElementById("oAuthCode").value;
        var state = authWindow.document.getElementById("state").value;
        if (authCode != "") {
            // Redirect task pane to the app code with the authCode token
            window.location = "OAuth.aspx?code="
                + authCode
                + "&siteUrl="
                + state; 
            authWindow.close();
        }
        else {
            //Wait another second and try again
            codeListener();
        }
    }
    catch (e) {

        //Wait another second and try again
        codeListener();
    }
}




