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
    // A variable that will be used to hold the text to be translated
    var documentContent;
    
    // A <div> that we can write to if the user hasn't selected any text.,
    // We'll clear its contents here so we have a clean place to write to.
    var status = document.getElementById("Status");
    while (status.hasChildNodes()) {
        status.removeChild(status.lastChild);
    }

    // Get the text selected by the user for translation
    Office.context.document.getSelectedDataAsync(Office.BindingType.Text,
        function (asyncResult) {
            if (asyncResult.status == Office.AsyncResultStatus.Failed) {

                //Write a message to the page if the operation has failed.
                status.appendChild(document.createTextNode("Cannot get document text"));
            }
            else {

                // Store the selected text in our variable
                documentContent = asyncResult.value;
                if (documentContent == "") {

                    // Tell the user they need to select some text
                    status.appendChild(document.createTextNode("Please select some text before continuing."));
                }
                else {
                    // Store the selected text in local storage so that it is accessible
                    // after the OAuth flow has completed.
                    localStorage.setItem("contentToTranslate", documentContent);

                    // Get the selected language and also store that in local storage 
                    // so that it is accessible after the OAuth flow has completed.
                    var language = document.getElementById("targetLanguage");
                    localStorage.setItem("targetLanguage", language.value);
                    
                    // Start the OAuth Flow
                    initAuthFlow(siteUrl, inSameWindow);
                }

            }
        });
}

function translateNow() {
    // Client-side click event retrieves the data from local storage
    // and pops it into the hidden textarea and hidden form field on the page,
    // which will then get posted to the Translation.aspx page.
   
    var documentContent = localStorage.getItem("contentToTranslate");
    var targetLanguage = localStorage.getItem("targetLanguage");
    $("#documentContent").val(documentContent);
    $("#documentLanguage").val(targetLanguage);
    $("#translationForm").submit();
    
}

// This function initiaties the OAuth flow
function initAuthFlow(siteUrl, inSameWindow) {
    if (!siteUrl.endsWith("/")) {
        siteUrl += "/";
    }
    authorizeUrl = siteUrl + "_layouts/15/OAuthAuthorize.aspx?IsDlg=1";
    authorizeUrl += "&client_id=" + $("#clientId").val();
    authorizeUrl += "&scope=Site.Manage";
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




