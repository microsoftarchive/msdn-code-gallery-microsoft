// For an introduction to the Blank template, see the following documentation:
// http://go.microsoft.com/fwlink/?LinkId=232509
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
                // retrieve button element
                var getdataButton = document.getElementById('getdatabtn');         
                window.addEventListener("resize", handleResize, false);          
               
                // register click event handle
                getdataButton.addEventListener("click", getDatabuttonclick, false);
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

    // event handler of get data button 
    function getDatabuttonclick()
    {
        // clear error message 
        document.getElementById('error').innerText = "";
        document.getElementById('getdatabtn').style.setAttribute("disabled", "disabled");
        var baseURI = "http://localhost:42920/Service.svc/querySql";
        var xmlDoc;
        WinJS.xhr({
            type:"get",
            url: baseURI
        }).then(function (response) {
            if (eval('(' + response.responseText + ')').queryParam == true) {
                var items = [];
                var resulttxt = eval('(' + response.responseText + ')').querySqlResult;               
                if (window.DOMParser) {
                    var parser = new DOMParser();
                    xmlDoc = parser.parseFromString(resulttxt, "text/xml");
                   
                }
                else {// Internet Explorer
                    xmlDoc = new ActiveXObject("Microsoft.XMLDOM");
                    xmlDoc.async = false;
                    xmlDoc.loadXML(resulttxt);
                }
                var nodes = xmlDoc.querySelectorAll("Table");
                
                for (var i = 0; i < nodes.length; i++)
                {
                    var item =new Object();
                    item.Title=nodes[i].childNodes[0].textContent;
                    item.Text =nodes[i].childNodes[1].textContent;
                    items.push(item);
                }

                var list = new WinJS.Binding.List(items);
                document.getElementById('listView').winControl.itemDataSource = list.dataSource;
                document.getElementById('getdatabtn').removeAttribute("disabled");
            }
            else {
                writeError("Error occurs. Please make sure the database has been attached to SQL Server!");
            }
        });
    }

    // handle window resize event
    function handleResize() {
        // Get window size
        var screenWidth =window.outerWidth;
        var screenHeight = window.outerHeight;

        if (screenWidth <= 500) {
            document.getElementById("title").style.display = "none";
            document.getElementById("link").style.display = "none";
        }
        else if (screenWidth < screenHeight)
        {
            document.getElementById("rootGrid").style.msGridColumns = "20px 1fr 20px";
        }
        else {
            document.getElementById("rootGrid").style.msGridColumns = "100px 1fr 100px";
            document.getElementById("title").style.removeAttribute("display");
            document.getElementById("link").style.removeAttribute("display");
        }
    }

    // Print error message
    function writeError(text) {
        document.getElementById("error").innerText = text;
    }
    app.start();
})();
