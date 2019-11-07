// For an introduction to the Blank template, see the following documentation:
// http://go.microsoft.com/fwlink/?LinkID=392286
(function () {
    "use strict";

    var app = WinJS.Application;
    var activation = Windows.ApplicationModel.Activation;

    app.onactivated = function (args) {
        if (args.detail.kind === activation.ActivationKind.launch) {
            if (args.detail.previousExecutionState !== activation.ApplicationExecutionState.terminated) {
                // TODO: This application has been newly launched. Initialize
                // your application here.
            } else {
                // TODO: This application has been reactivated from suspension.
                // Restore application state here.
            }
            args.setPromise(WinJS.UI.processAll().then(function completed() {

                // Retrieve button
                var loadBtn = document.getElementById("loadbtn");
                var urlTextbox = document.getElementById("urltxb");
                var webviewControl = document.getElementById("webview");

                // Register Click Event of load button
                loadbtn.addEventListener("click", loadButtonClick, false);
                urlTextbox.addEventListener("keydown", urlTextKeydown, false);
                webviewControl.addEventListener("MSWebViewNavigationCompleted", webviewNavigationCompleted);

                document.getElementById("loadingProcessProgressRing").style.visibility = "collapse";
            }));
        }
    };

    app.oncheckpoint = function (args) {
        // TODO: This application is about to be suspended. Save any state
        // that needs to persist across suspensions here. You might use the
        // WinJS.Application.sessionState object, which is automatically
        // saved and restored across suspension. If you need to complete an
        // asynchronous operation before your application is suspended, call
        // args.setPromise().
    };

    // When the user presses "Enter" key, call LoadButton_Click method to load the content.
    function urlTextKeydown(key)
    {
        if (key.char == '\n')
        {
            loadButtonClick();
            return;
        }
        
        document.getElementById("loadbtn").removeAttribute("disabled");
    }
      

    // Load Button Click event handler
    function loadButtonClick() {
        // Retrieve the DOM element 
        var loadButton = document.getElementById("loadbtn");
        if (loadButton.hasAttribute("disabled"))
        {
            return;
        }
        loadButton.disabled = "disabled";
        
        try
        {            
            document.getElementById("webview").navigate(document.getElementById("urltxb").value);
        }
        catch(e)
        {
            var error = document.getElementById("urlerror");
            error.textContent = e.message;
            error.style.visibility = "visible";
            return;
        }      
        

        document.getElementById("urlerror").style.visibility = "collapse";

        document.getElementById("loadingProcessProgressRing").style.visibility = "visible";

    }

    // WebView navigation completed event hanlder.
    function webviewNavigationCompleted() {
        
        document.getElementById("loadingProcessProgressRing").style.visibility = "collapse";
    }

    app.start();
})();